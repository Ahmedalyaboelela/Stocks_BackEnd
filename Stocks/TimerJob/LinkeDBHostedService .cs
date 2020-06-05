using BAL.Repositories;
using DAL.Context;
using DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Stocks.TimerJob
{
    public class LinkeDBHostedService : IHostedService, IDisposable
    {
        private readonly ILogger<LinkeDBHostedService> _logger;
        private Timer _timer;
        private readonly IServiceScopeFactory scopeFactory;

        public LinkeDBHostedService(IServiceScopeFactory scopeFactory,ILogger<LinkeDBHostedService> logger)
        {
            _logger = logger;
            this.scopeFactory = scopeFactory;
        }
        public Task StartAsync(CancellationToken stoppingToken)
        {
            double timehour=0, timeminutes = 0;
            using (var scope = scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<StocksContext>();
                var checkKlioconnection = dbContext.SettingKiloConnections.SingleOrDefault();
                if (checkKlioconnection != null)
                {
                    var timestart = checkKlioconnection.TimerJobStartTime;
                    timehour = double.Parse(timestart.Substring(0,2));
                    timeminutes = double.Parse(timestart.Substring(3,2));
                }

            }
                _logger.LogInformation("Timed Hosted Service running.");
            TimeSpan interval = TimeSpan.FromHours(24);
            //calculate time to run the first time & delay to set the timer
            //DateTime.Today gives time of midnight 00.00
            var nextRunTime = DateTime.Today.AddDays(1).AddHours(timehour).AddMinutes(timeminutes);
            var curTime = DateTime.Now;
            var firstInterval = nextRunTime.Subtract(curTime);
            Action action = () =>
            {
                var t1 = Task.Delay(firstInterval);
                t1.Wait();
                //remove inactive accounts at expected time
                DoWork(null);
                //now schedule it to be called every 24 hours for future
                // timer repeates call to RemoveScheduledAccounts every 24 hours.
                _timer = new Timer(
                    DoWork,
                    null,
                    TimeSpan.Zero,
                    interval
                );
            };

            // no need to await this call here because this task is scheduled to run much much later.
            Task.Run(action);
            return Task.CompletedTask;

            //_timer = new Timer(DoWork, null,TimeSpan.Zero
            //    TimeSpan.FromDays(1));

            //return Task.CompletedTask;
        }
        private void DoWork(object state)
        {
              var LinkedAccName = "";
             int linkedparentacc = 0;
            string connectionString = "";
            using (var scope = scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<StocksContext>();
                var checkKlioconnection = dbContext.SettingKiloConnections.SingleOrDefault();
                if (checkKlioconnection != null)
                {
                    List<Account> AccountList = new List<Account>();
                    if(checkKlioconnection.UserId !=null && checkKlioconnection.Password !=null)
                    {
                         connectionString = "Server='"+ checkKlioconnection.ServerName  + "';Database='"+ checkKlioconnection.DatabaseName + "';" +
                            "User Id= '"+ checkKlioconnection.UserId + "';password='"+ checkKlioconnection.Password + "';MultipleActiveResultSets=true;trusted_connection=true";
                    }
                    else
                    {
                        connectionString = "Server='" + checkKlioconnection.ServerName + "';Database='" + checkKlioconnection.DatabaseName + "';" +
                            "Integrated Security=true;MultipleActiveResultSets=true;trusted_connection=true";
                    }
                    try
                    {
                        using (SqlConnection connection = new SqlConnection(connectionString))
                        {
                            //SqlDataReader
                            connection.Open();
                            string sql = "Select * From ACCOUNTS"; SqlCommand command = new SqlCommand(sql, connection);
                            using (SqlDataReader dataReader = command.ExecuteReader())
                            {
                                while (dataReader.Read())
                                {
                                    Account account = new Account();
                                    account.Code = dataReader["ACC_CODE"].ToString();
                                    account.NameAR = dataReader["ACC_AR_NAME"].ToString();
                                    account.NameEN = dataReader["ACC_EN_NAME"].ToString();
                                    if (Convert.ToInt16(dataReader["ACC_STATE"]) == 1)
                                    {
                                        account.AccountType = true;
                                    }
                                    else
                                    {
                                        account.AccountType = false;
                                    }
                                    if (dataReader["ACC_MAX_DEBIT"].ToString() != "")
                                        account.DebitLimit = Convert.ToDecimal(dataReader["ACC_MAX_DEBIT"]);
                                    if (dataReader["ACC_MAX_CREDIT"].ToString() != "")
                                        account.CreditLimit = Convert.ToDecimal(dataReader["ACC_MAX_CREDIT"]);
                                    if (dataReader["ACC_CREDIT"].ToString() != "")
                                        account.Credit = Convert.ToDecimal(dataReader["ACC_CREDIT"]);
                                    if (dataReader["ACC_DEBIT"].ToString() != "")
                                        account.Debit = Convert.ToDecimal(dataReader["ACC_DEBIT"]);
                                    if (dataReader["CreditOpeningAccount"].ToString() != "")
                                        account.CreditOpenningBalance = Convert.ToDecimal(dataReader["CreditOpeningAccount"]);
                                    if (dataReader["DepitOpeningAccount"].ToString() != "")
                                        account.DebitOpenningBalance = Convert.ToDecimal(dataReader["DepitOpeningAccount"]);
                                    if (dataReader["ACC_ID"].ToString() != "")
                                        account.LinkedDBAccID = (int)dataReader["ACC_ID"];
                                    AccountList.Add(account);
                                }
                            }
                            connection.Close();
                        }
                        var Check = dbContext.Accounts.ToList();
                        foreach (var item in AccountList)
                        {
                            if (!Check.Any(m => m.NameAR == item.NameAR))
                            {
                                dbContext.Accounts.Add(item);
                                dbContext.SaveChanges();
                            }
                        }
                        var afteradded = dbContext.Accounts.ToList();
                        foreach (var item in afteradded)
                        {

                            using (SqlConnection connection = new SqlConnection(connectionString))
                            {
                                //SqlDataReader
                                connection.Open();
                                string sql = "Select * From ACCOUNTS where ACC_AR_NAME='" + item.NameAR + "'"; SqlCommand command = new SqlCommand(sql, connection);
                                using (SqlDataReader dataReader = command.ExecuteReader())
                                {
                                    while (dataReader.Read())
                                    {

                                        if (dataReader["PARENT_ACC_ID"].ToString() != "")
                                        {
                                            linkedparentacc = int.Parse(dataReader["PARENT_ACC_ID"].ToString());
                                        }

                                    }
                                }
                                connection.Close();
                                connection.Open();
                                string sql2 = "Select * From ACCOUNTS where ACC_ID='" + linkedparentacc + "'"; SqlCommand command2 = new SqlCommand(sql2, connection);
                                using (SqlDataReader dataReader = command2.ExecuteReader())
                                {
                                    while (dataReader.Read())
                                    {
                                        LinkedAccName = dataReader["ACC_AR_NAME"].ToString();
                                    }
                                }

                                connection.Close();

                            }
                            //  var parentrow = unitOfWork.AccountRepository.Get(filter: m => m.NameAR == LinkedAccName).SingleOrDefault();
                            if (LinkedAccName != "")
                            {
                                var parentrow = dbContext.Accounts.Where(m => m.NameAR == LinkedAccName).FirstOrDefault();
                                if (linkedparentacc == 0)
                                {
                                    item.AccoutnParentID = null;
                                }
                                else
                                {
                                    item.AccoutnParentID = parentrow.AccountID;
                                }

                                dbContext.Accounts.Attach(item);
                                dbContext.Entry(item).State = EntityState.Modified;
                                dbContext.SaveChanges();
                            }


                        }
                    }

                    catch(Exception ex)
                    {

                    }

                }

            }

           // var checkKlioconnection = unitOfWork.SettingKiloRepository.Get().SingleOrDefault();
        


        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Timed Hosted Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}

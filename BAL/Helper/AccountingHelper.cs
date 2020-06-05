using AutoMapper;
using BAL.Model;
using BAL.Repositories;
using DAL.Context;
using DAL.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;

namespace BAL.Helper
{
    public class AccountingHelper : IAccountingHelper
    {

        private UnitOfWork unitOfWork;
        private readonly IMapper _mapper;

        public AccountingHelper(StocksContext context, IMapper mapper)
        {
            this.unitOfWork = new UnitOfWork(context);
            this._mapper = mapper;
        }

        //Transfer Entry To Accounts
        public void TransferToAccounts(List<EntryDetail> EntryList)
        {
            foreach (var item in EntryList)
            {
                var account = unitOfWork.AccountRepository.GetByID(item.AccountID);
                if(item.Credit!=null)
                {
                    if(account.Credit==null)
                    {
                        account.Credit = 0;
                    }
                    account.Credit += item.Credit;
                    if(item.StocksCredit!=null)
                    {
                        if (account.StocksCredit == null)
                        {
                            account.StocksCredit = 0;
                        }
                        account.StocksCredit += item.StocksCredit;
                    }
                }
                if(item.Debit !=null)
                {
                    if (account.Debit == null)
                    {
                        account.Debit = 0;
                    }
                    account.Debit += item.Debit;
                    if (item.StocksDebit != null)
                    {
                        if (account.StocksDebit == null)
                        {
                            account.StocksDebit = 0;
                        }
                        account.StocksDebit += item.StocksDebit;
                    }

                }
                unitOfWork.AccountRepository.Update(account);
            }
        }

        //Cancel Transfer Entry From Accounts
        public void CancelTransferToAccounts(List<EntryDetail> EntryList)
        {
            foreach (var item in EntryList)
            {
                var account = unitOfWork.AccountRepository.GetByID(item.AccountID);
                if (item.Credit != 0)
                {
                    if (account.Credit == null)
                    {
                        account.Credit = 0;
                    }
                    account.Credit -= item.Credit;
                }
                if (item.Debit != 0)
                {
                    if (account.Debit == null)
                    {
                        account.Debit = 0;
                    }
                    account.Debit -= item.Debit;
                }
                unitOfWork.AccountRepository.Update(account);
            }
        }

        public int AddEntryToLinkedDB(EntryModel entryModel)
        {
            var connectionString = "";
            Int64 entryid = 0;
            int rownum = 0;
            bool checkaccount = false;
            var entrydetails = entryModel.EntryDetailModel;
            DateTime entrydate = DateTime.ParseExact(entryModel.Date, "d/M/yyyy", CultureInfo.InvariantCulture);
            var checkKlioconnection = unitOfWork.SettingKiloRepository.Get().SingleOrDefault();
            if (checkKlioconnection != null)
            {
                foreach (var item in entrydetails)
                {
                    int? accid = unitOfWork.AccountRepository.Get(filter: m => m.AccountID == item.AccountID).Select(m => m.LinkedDBAccID).SingleOrDefault();
                    if (accid == null)
                        checkaccount = true;
                }
                if(checkaccount !=true)
                {   
                    if (checkKlioconnection.UserId != null && checkKlioconnection.Password != null)
                    {
                        connectionString = "Server='" + checkKlioconnection.ServerName + "';Database='" + checkKlioconnection.DatabaseName + "';" +
                           "User Id= '" + checkKlioconnection.UserId + "';password='" + checkKlioconnection.Password + "';MultipleActiveResultSets=true;trusted_connection=true";
                    }
                    else
                    {
                        connectionString = "Server='" + checkKlioconnection.ServerName + "';Database='" + checkKlioconnection.DatabaseName + "';" +
                            "Integrated Security=true;MultipleActiveResultSets=true;trusted_connection=true";
                    }
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        string sql = $"Insert Into ENTRY_MASTER (ENTRY_SETTING_ID, ENTRY_NUMBER, ENTRY_DATE, ENTRY_CREDIT," +
                        $"ENTRY_DEBIT,ENTRY_GOLD_CREDIT,ENTRY_GOLD_DEBIT,CURRENCY_ID,CURRENCY_RATE,IS_POSTED) " +
                        $"Values (130, '1','{entrydate}',0,0,0,0,2,1,0)";
                        using (SqlCommand command = new SqlCommand(sql, connection))
                        {
                            command.CommandType = CommandType.Text;
                            connection.Open();
                            command.ExecuteNonQuery();
                            connection.Close();
                        }
                        connection.Open();
                        string sql2 = "SELECT IDENT_CURRENT('ENTRY_MASTER')";
                        SqlCommand command1 = new SqlCommand(sql2, connection);
                        using (SqlDataReader dataReader = command1.ExecuteReader())
                        {
                            while (dataReader.Read())
                            {
                                entryid = Convert.ToInt64(dataReader[0]);
                            }
                        }
                        connection.Close();
                        foreach (var item in entrydetails)
                        {
                            if (item.Debit == null)
                                item.Debit = 0;
                            if (item.Credit == null)
                                item.Credit = 0;
                            int? accid = unitOfWork.AccountRepository.Get(filter: m => m.AccountID == item.AccountID).Select(m => m.LinkedDBAccID).SingleOrDefault();
                            if (accid != null)
                            {
                                string sql3 = $"Insert Into ENTRY_DETAILS (ENTRY_ID, ENTRY_ROW_NUMBER, ACC_ID, ENTRY_CREDIT," +
                                             $"ENTRY_DEBIT,ENTRY_GOLD24_CREDIT,ENTRY_GOLD24_DEBIT) " +
                                             $"Values ({entryid}, '{rownum}','{accid}','{item.Credit}','{item.Debit}',0,0)";
                                using (SqlCommand command = new SqlCommand(sql3, connection))
                                {
                                    command.CommandType = CommandType.Text;
                                    connection.Open();
                                    command.ExecuteNonQuery();
                                    connection.Close();
                                }
                                rownum++;

                            }

                        }

                    }
                }
     
            }
            return Convert.ToInt32( entryid);
        }

        public void DeleteEntryToLinkedDB(Entry entry,IEnumerable<EntryDetail> entryDetail)
        {
            var connectionString = "";
            bool checkaccount = false;
            var checkKlioconnection = unitOfWork.SettingKiloRepository.Get().SingleOrDefault();
            if (checkKlioconnection != null)
            {
                foreach (var item in entryDetail)
                {
                    int? accid = unitOfWork.AccountRepository.Get(filter: m => m.AccountID == item.AccountID).Select(m => m.LinkedDBAccID).SingleOrDefault();
                    if (accid == null)
                        checkaccount = true;
                }
                if (checkaccount != true)
                {
                    if (checkKlioconnection.UserId != null && checkKlioconnection.Password != null)
                    {
                        connectionString = "Server='" + checkKlioconnection.ServerName + "';Database='" + checkKlioconnection.DatabaseName + "';" +
                           "User Id= '" + checkKlioconnection.UserId + "';password='" + checkKlioconnection.Password + "';MultipleActiveResultSets=true;trusted_connection=true";
                    }
                    else
                    {
                        connectionString = "Server='" + checkKlioconnection.ServerName + "';Database='" + checkKlioconnection.DatabaseName + "';" +
                            "Integrated Security=true;MultipleActiveResultSets=true;trusted_connection=true";
                    }

                    if(entry.RefrenceEntryId !=null)
                    {
                        using (SqlConnection connection = new SqlConnection(connectionString))
                        {
                            string sql = $"Delete From ENTRY_DETAILS Where ENTRY_ID='{entry.RefrenceEntryId}'";
                            using (SqlCommand command = new SqlCommand(sql, connection))
                            {
                                connection.Open();
                                try
                                {
                                    command.ExecuteNonQuery();
                                }
                                catch (SqlException ex)
                                {

                                }
                                connection.Close();
                            }
                        }

                        using (SqlConnection connection = new SqlConnection(connectionString))
                        {
                            string sql = $"Delete From ENTRY_MASTER Where ENTRY_ID='{entry.RefrenceEntryId}'";
                            using (SqlCommand command = new SqlCommand(sql, connection))
                            {
                                connection.Open();
                                try
                                {
                                    command.ExecuteNonQuery();
                                }
                                catch (SqlException ex)
                                {

                                }
                                connection.Close();
                            }
                        }
                    }
         



                }
            }
        }

    }
}

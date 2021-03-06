﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BAL.Model;
using BAL.Repositories;
using DAL.Context;
using DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Stocks.Controllers
{
    [Authorize(Roles = "SuperAdmin,Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class SettingController : ControllerBase
    {

        #region CTOR & Definitions
        private UnitOfWork unitOfWork;
        private readonly IMapper _mapper;

        public SettingController(StocksContext context, IMapper mapper)
        {
            this.unitOfWork = new UnitOfWork(context);
            this._mapper = mapper;
        }
        #endregion

        #region Get Method
        [HttpGet]
        [Route("~/api/Setting/GetSelected/{type}")]
        public SettingModel GetSpecificSetting(int type)
        {
            var setting = unitOfWork.SettingRepository.Get(filter: a => a.VoucherType == type).FirstOrDefault();
            var model = _mapper.Map<SettingModel>(setting);

            if (model == null)
            {
                return null;
            }

            #region Assign details to it's setting

            var accounts = setting.SettingAccounts.ToList();
            var Accmodel = _mapper.Map<IEnumerable<SettingAccountModel>>(accounts).ToList();


            // assign setting accounts model to setting model

            if (Accmodel.Count() > 0)
            {

                var j = 0;
                if (Accmodel[0].SettingID == model.SettingID)
                {
                    foreach (var acc in Accmodel)
                    {
                        acc.AccCode = accounts[j].Account.Code;
                        acc.AccNameAR = accounts[j].Account.NameAR;
                        acc.AccNameEN = accounts[j].Account.NameEN;

                        j++;
                    }

                    model.SettingAccs = Accmodel;

                }
            }


            #endregion

            return model;
        }

        [HttpGet]
        [Route("~/api/Setting/Get")]
        public IActionResult GetSetting()
        {
            var setting = unitOfWork.SettingRepository.Get();
            var model = _mapper.Map<IEnumerable<SettingModel>>(setting).ToList();

            if (model == null)
            {
                return Ok(0);
            }

            #region Assign details to it's setting
            var i = 0;

            foreach (var item in setting)
            {
                var accounts = item.SettingAccounts.ToList();
                var Accmodel = _mapper.Map<IEnumerable<SettingAccountModel>>(accounts).ToList();
                //// to bind account type to each account
                //foreach (var acc in Accmodel)
                //{
                //    acc.AccountType = i++;

                //}

                // assign setting accounts model to setting model

                if (Accmodel.Count() > 0)
                {

                    var j = 0;
                    if (Accmodel[0].SettingID == model[i].SettingID)
                    {
                        foreach (var acc in Accmodel)
                        {
                            acc.AccCode = accounts[j].Account.Code;
                            acc.AccNameAR = accounts[j].Account.NameAR;
                            acc.AccNameEN = accounts[j].Account.NameEN;

                            j++;
                        }

                        model[i].SettingAccs = Accmodel;

                    }
                }



                i++;

            }
            #endregion

            return Ok(model);
        }
        [HttpGet]
        [Route("~/api/Setting/GetSettingKilo")]
        public IActionResult GetSettingKilo()
        {
            var settingkilo = unitOfWork.SettingKiloRepository.Get().SingleOrDefault();
            var model = _mapper.Map<SettingKiloModel>(settingkilo);

            if (model == null)
            {
                return Ok(0);
            }

            return Ok(model);
        }
        #endregion


        #region Add or Edit Method
        [HttpPost]
        [Route("~/api/Setting/Save")]
        public IActionResult SaveSetting([FromBody] List<SettingModel> settingModel)
        {
            if (ModelState.IsValid)
            {
                var model = _mapper.Map<IEnumerable<Setting>>(settingModel).ToList();
                if (model == null)
                {
                    return Ok(0);
                }

                var Check = unitOfWork.SettingRepository.Get(NoTrack: "NoTrack");
                // check if there is already data
                // insert case
                #region Insert new setting
                if (Check.Count() == 0)
                {
                    var i = 0;

                    // add each setting
                    foreach (var item in settingModel)
                    {
                        if(item.VoucherType != 0)
                        {
                            var setting = _mapper.Map<Setting>(item);
                            setting.Code = "0";
                            unitOfWork.SettingRepository.Insert(setting);


                            // add accounts related to each setting 
                            #region account related to  setting
                            var accounts = _mapper.Map<IEnumerable<SettingAccount>>(item.SettingAccs);
                            if (accounts != null)
                            {
                                foreach (var acc in item.SettingAccs)
                                {
                                    var obj = _mapper.Map<SettingAccount>(acc);

                                    obj.SettingID = setting.SettingID;

                                    unitOfWork.SettingAccountRepository.Insert(obj);


                                }
                            }

                        }


                        #endregion

                    }


                    var reslt = unitOfWork.Save();
                    if (reslt == 200)
                    {

                        return Ok(4);
                    }
                    else if (reslt == 501)
                    {
                        return Ok(5);
                    }
                    else
                    {
                        return Ok(6);
                    }

                }
                #endregion
                #region Update exist data as remove old and add new setting
                else
                {
                    var oldSetting = unitOfWork.SettingRepository.Get(NoTrack: "NoTrack");
                    var oldAccounts = unitOfWork.SettingAccountRepository.Get(NoTrack: "NoTrack");


                    #region Remove old accounts related to setting

                    if (oldAccounts.Count() > 0 && oldAccounts != null)
                    {
                        unitOfWork.SettingAccountRepository.RemovRange(oldAccounts);

                    }
                    #endregion

                    #region Remove old setting and add new one
                    unitOfWork.SettingRepository.RemovRange(oldSetting);
                    #endregion
                    #region insert new setting
                    foreach (var item in settingModel)
                    {
                        if (item.VoucherType != 0)
                        {
                            item.Code = "0";
                            var setting = _mapper.Map<Setting>(item);
                            unitOfWork.SettingRepository.Insert(setting);


                            // add accounts related to each setting 
                            #region account related to  setting
                            var accounts = _mapper.Map<IEnumerable<SettingAccount>>(item.SettingAccs);
                            if (accounts != null)
                            {
                                foreach (var acc in item.SettingAccs)
                                {
                                    var obj = _mapper.Map<SettingAccount>(acc);

                                    obj.SettingID = setting.SettingID;

                                    unitOfWork.SettingAccountRepository.Insert(obj);


                                }
                            }

                        }
                        #endregion

                    }

                    #endregion
                    var reslt = unitOfWork.Save();
                    if (reslt == 200)
                    {

                        return Ok(4);
                    }
                    else if (reslt == 501)
                    {
                        return Ok(5);
                    }
                    else
                    {
                        return Ok(6);
                    }

                }


                #endregion

            }
            else
            {
                return Ok(3);
            }
        }

        [HttpPost]
        [Route("~/api/Setting/SaveSettingKilo")]
        public IActionResult SaveSettingKilo([FromBody] SettingKiloModel settingKiloModel)
        {
            if (ModelState.IsValid)
            {

                var model = _mapper.Map<SettingKiloConnection> (settingKiloModel);
                if (model == null)
                {
                    return Ok(0);
                }

                var Check = unitOfWork.SettingKiloRepository.Get(NoTrack: "NoTrack");
                // check if there is already data
                // insert case
                #region Insert new setting
                if (Check.Count() == 0)
                {

                    unitOfWork.SettingKiloRepository.Insert(model);

                    var reslt = unitOfWork.Save();
                    if (reslt == 200)
                    {

                        return Ok(4);
                    }
                    else if (reslt == 501)
                    {
                        return Ok(5);
                    }
                    else
                    {
                        return Ok(6);
                    }

                }
                #endregion
                #region Update exist data as remove old and add new setting
                else
                {
                    var oldSettingKilo = unitOfWork.SettingKiloRepository.Get(NoTrack: "NoTrack").SingleOrDefault();


                    unitOfWork.SettingKiloRepository.Delete(oldSettingKilo);
                    unitOfWork.SettingKiloRepository.Insert(model);


                    #endregion
                    var reslt = unitOfWork.Save();
                    if (reslt == 200)
                    {

                        return Ok(4);
                    }
                    else if (reslt == 501)
                    {
                        return Ok(5);
                    }
                    else
                    {
                        return Ok(6);
                    }

                }


                #endregion

            }
            else
            {
                return Ok(3);
            }
        }

        [HttpGet]
        [Route("~/api/Setting/SyncNow")]
        public IActionResult SyncNow()
        {
            var LinkedAccName = "";
            int linkedparentacc = 0;
            string connectionString = "";
    
              //  var dbContext = scope.ServiceProvider.GetRequiredService<StocksContext>();
                var checkKlioconnection = unitOfWork.SettingKiloRepository.Get().SingleOrDefault();
                if (checkKlioconnection != null)
                {
                    List<Account> AccountList = new List<Account>();
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
                        var Check = unitOfWork.AccountRepository.Get();
                        foreach (var item in AccountList)
                        {
                            if (!Check.Any(m => m.NameAR == item.NameAR))
                            {
                            unitOfWork.AccountRepository.Insert(item);
                            unitOfWork.Save();

                            }
                        }
                        var afteradded = unitOfWork.AccountRepository.Get();
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
                            var parentrow = unitOfWork.AccountRepository.Get(filter: m => m.NameAR == LinkedAccName).FirstOrDefault();
                                if (linkedparentacc == 0)
                                {
                                    item.AccoutnParentID = null;
                                }
                                else
                                {
                                    item.AccoutnParentID = parentrow.AccountID;
                                }
                            unitOfWork.AccountRepository.Update(item);
                            unitOfWork.Save();

                            }


                        }
                    return Ok(4);
                    }

                    catch (Exception ex)
                    {
                    return Ok(3);
                    }

                }
                else
            {
                return Ok(1);
            }

            
        }




    }
}
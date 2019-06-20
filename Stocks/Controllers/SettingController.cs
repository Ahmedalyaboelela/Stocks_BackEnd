﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BAL.Model;
using BAL.Repositories;
using DAL.Context;
using DAL.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Stocks.Controllers
{
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
        [Route("~/api/Setting/Get")]
        public IEnumerable<SettingModel> GetSetting()
        {
            var setting = unitOfWork.SettingRepository.Get();
            var model = _mapper.Map<IEnumerable<SettingModel>>(setting).ToList();

            if (model == null)
            {
                return model;
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
                    if (model[i].SettingID == Accmodel[0].SettingID)
                    {
                        foreach (var acc in Accmodel)
                        {
                            acc.AccCode = accounts[i].Account.Code;
                            acc.AccNameAR = accounts[i].Account.NameAR;
                            acc.AccNameEN = accounts[i].Account.NameEN;

                        }

                        model[i].SettingAccs = Accmodel;

                    }
                }



                i++;

            }
            #endregion

            return (model);
        }
        #endregion

        //#region Insert Method
        //public SettingModel Save(SettingModel settingModel,Setting model)
        //{
        //    var settingAccounts = settingModel.SettingAccs;
        //    var oldAccounts = unitOfWork.SettingAccountRepository

        //     .Get(NoTrack: "NoTrack", filter: m => m.SettingID == model.SettingID);
        //    unitOfWork.SettingAccountRepository.RemovRange(oldAccounts);


        //    unitOfWork.SettingRepository.Insert(model);

        //        var Result = unitOfWork.Save();
        //        if (Result == true)
        //            return settingModel;
        //        else
        //            return null;

        //}

        [HttpPost]
        [Route("~/api/Setting/Save")]
        public IActionResult SaveSetting([FromBody] List<SettingModel> settingModel)
        {
            if (ModelState.IsValid)
            {
                var model = _mapper.Map<IEnumerable<Setting>>(settingModel).ToList();
                if (model == null)
                {
                    return Ok(settingModel);
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


                        #endregion

                    }

                    bool CheckSave = unitOfWork.Save();



                    if (CheckSave == true)
                    {
                        return Ok(model);
                    }
                    else
                    {
                        return Ok("توجد مشكله ...");
                    }


                } 
                #endregion
                #region Update exist data
                else
                {
                    var oldSetting = unitOfWork.SettingRepository.Get(NoTrack: "NoTrack");
                    var oldAccounts = unitOfWork.SettingAccountRepository.Get(NoTrack: "NoTrack");


                    #region Remove old data

                    if (oldAccounts.Count() > 0 && oldAccounts != null)
                    {
                        unitOfWork.SettingAccountRepository.RemovRange(oldAccounts);

                    }
                    #endregion

                    #region Update data
                    for (int i = 0; i < oldSetting.Count(); i++)
                    {
                        for (int j = i; j == i; j++)
                        {
                            var set = _mapper.Map<Setting>(settingModel[i]);
                            var accs = settingModel[i].SettingAccs;
                            unitOfWork.SettingRepository.Update(set);

                            foreach (var item in accs)
                            {
                                item.SettingID = set.SettingID;
                                item.SettingAccountID = 0;
                                var newAcc = _mapper.Map<SettingAccount>(item);

                                unitOfWork.SettingAccountRepository.Insert(newAcc);

                            }
                        }
                    }
                    #endregion



                    bool CheckSave = unitOfWork.Save();



                    if (CheckSave == true)
                    {
                        return Ok(settingModel);
                    }
                    else
                    {
                        return Ok("توجد مشكله ...");
                    }
                }


                #endregion

            }
            else
            {
                return BadRequest("Bad request");
            }
        }
        
    }
}
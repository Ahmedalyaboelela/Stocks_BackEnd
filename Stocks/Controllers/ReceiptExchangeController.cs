using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BAL.Helper;
using BAL.Model;
using BAL.Repositories;
using DAL.Context;
using DAL.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Stocks.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReceiptExchangeController : ControllerBase
    {
        #region CTOR & Definitions
        private UnitOfWork unitOfWork;
        private readonly IMapper _mapper;

        private SettingController setting;
        private readonly IAccountingHelper accountingHelper;
        public ReceiptExchangeController(StocksContext context, IMapper mapper)
        {
            this.unitOfWork = new UnitOfWork(context);
            this._mapper = mapper;
            this.setting = new SettingController(context, mapper);
            accountingHelper = new AccountingHelper(context, mapper);
        }

        #endregion

        #region Generate Entry

        public IActionResult GenerateEntry()
        {
            var settingObj = setting.GetSpecificSetting(1);
            return Ok();
        }
        #endregion

        #region Default Setting
        public SettingModel DefaultSetting(bool type)
        {
            var screenNo = 3;
            if (type)
                screenNo = 4;

            var settingObj = setting.GetSpecificSetting(screenNo);

            return settingObj;
        }
        #endregion

        [HttpPost] // يولد قيد مع ترحيل تلقائي
        [Route("~/api/ReceiptExchange/Generateconstraint")]
        public IActionResult Generateconstraint(ReceiptExchangeModel receiptExchangeModel)
        {
            var lastEntry = unitOfWork.EntryRepository.Last();
            var lastEntryDitails = unitOfWork.EntryDetailRepository.Get(filter: x => x.EntryID == lastEntry.EntryID).Select(m => new EntryDetailModel
            {
                AccountID = m.AccountID,
                Credit = m.Credit,
                Debit = m.Debit,
                EntryID = m.EntryID,
                EntryDetailID = m.EntryDetailID,
                AccCode = m.Account.Code,
                AccNameAR = m.Account.NameAR,
                AccNameEN = m.Account.NameEN,



            });

            var EntryMODEL = EntriesHelper.InsertCalculatedEntries(null, null, receiptExchangeModel, null, lastEntry);
            var Entry = _mapper.Map<Entry>(EntryMODEL);


            var DetailEnt = EntryMODEL.EntryDetailModel;

            if (receiptExchangeModel.SettingModel.TransferToAccounts == true)
            {
                Entry.TransferedToAccounts = true;
                unitOfWork.EntryRepository.Insert(Entry);
                foreach (var item in DetailEnt)
                {
                    item.EntryID = Entry.EntryID;
                    item.EntryDetailID = 0;
                    var details = _mapper.Map<ReceiptExchangeDetail>(item);

                    unitOfWork.ReceiptExchangeDetailRepository.Insert(details);
                }
                accountingHelper.TransferToAccounts(DetailEnt.Select(a => new EntryDetail
                {

                    Credit = a.Credit,
                    Debit = a.Debit,
                    EntryDetailID = a.EntryDetailID,
                    EntryID = a.EntryID,
                    StocksCredit = a.StocksCredit,
                    StocksDebit = a.StocksDebit,
                    AccountID = a.AccountID

                }).ToList());
            }
            else
            {
                Entry.TransferedToAccounts = false;
                unitOfWork.EntryRepository.Insert(Entry);
                foreach (var item in DetailEnt)
                {
                    item.EntryID = Entry.EntryID;
                    item.EntryDetailID = 0;
                    var details = _mapper.Map<SellingOrderDetail>(item);

                    unitOfWork.SellingOrderDetailRepository.Insert(details);

                }
            }
            return Ok(receiptExchangeModel);
        }


        [HttpPost]// ترحيل يدوي للقيد اليدوي والتلقائي
        [Route("~/api/SellingOrder/Manualmigration")]
        public IActionResult Manualmigration(EntryModel EntryMODEL)
        {
            var Entry = unitOfWork.EntryRepository.GetByID(EntryMODEL.EntryID);
            Entry.TransferedToAccounts = true;
            unitOfWork.EntryRepository.Update(Entry);
            var Details = EntryMODEL.EntryDetailModel;
            foreach (var item in Details)
            {
                var detail = _mapper.Map<ReceiptExchangeDetail>(item);

                unitOfWork.ReceiptExchangeDetailRepository.Update(detail);
            }

            accountingHelper.TransferToAccounts(Details.Select(x => new EntryDetail
            {
                EntryDetailID = x.EntryDetailID,
                AccountID = x.AccountID,
                Credit = x.Credit,
                Debit = x.Debit,
                EntryID = x.EntryID


            }).ToList());




            unitOfWork.Save();



            return Ok("تم ترحيل القيد");

        }


        [HttpGet]//القيد
        [Route("~/api/SellingOrder/GetEntry")]
        public IActionResult GetEntry(ReceiptExchangeModel receiptExchangeModel)
        {
            var Entry = unitOfWork.EntryRepository.Get(x => x.ReceiptID == receiptExchangeModel.ReceiptID).SingleOrDefault();
            var EntryDetails = unitOfWork.EntryDetailRepository.Get(filter: a => a.EntryID == Entry.EntryID);
            EntryModel entryModel = new EntryModel();
            entryModel.EntryID = Entry.EntryID;
            entryModel.Code = Entry.Code;
            entryModel.Date = Entry.Date.ToString();
            entryModel.DateHijri = DateHelper.GetHijriDate(Entry.Date);
            entryModel.NoticeID = Entry.NoticeID;
            entryModel.PurchaseOrderID = Entry.PurchaseOrderID;
            entryModel.ReceiptID = Entry.ReceiptID;
            entryModel.SellingOrderID = Entry.SellingOrderID;
            entryModel.EntryDetailModel = EntryDetails.Select(m => new EntryDetailModel
            {
                AccCode = m.Account.Code,
                AccNameAR = m.Account.NameAR,
                AccNameEN = m.Account.NameEN,
                AccountID = m.AccountID,
                Credit = m.Credit,
                Debit = m.Debit,
                EntryDetailID = m.EntryDetailID,
                EntryID = m.EntryID,


            });

            return Ok(entryModel);
        }




        #region GET Methods

        public ReceiptExchangeModel GetReceiptExchange(ReceiptExchange RecExc,bool type)
        {
            var model = _mapper.Map<ReceiptExchangeModel>(RecExc);
            if (model == null)
            {
                return model;
            }

            #region Date part

            model.Date = RecExc.Date.Value.ToString("dd/MM/yyyy");
            model.DateHijri = DateHelper.GetHijriDate(RecExc.Date);

            model.ChiqueDate = RecExc.ChiqueDate.Value.ToString("dd/MM/yyyy");
            model.ChiqueDateHijri = DateHelper.GetHijriDate(RecExc.ChiqueDate);
            #endregion

            #region Details part
            var RecExcDetails = unitOfWork.ReceiptExchangeDetailRepository

                .Get(filter: m => m.ReceiptID == RecExc.ReceiptID)
                .Select(m => new ReceiptExchangeDetailModel
                {
                    ReceiptExchangeID = m.ReceiptExchangeID,
                  //  ReceiptExchangeAmount=m.ReceiptExchangeAmount,
                    ReceiptID = m.ReceiptID,
                    AccountID = m.AccountID,
                    AccNameAR = m.Account.Code,
                    AccNameEN=m.Account.NameEN,
                    ChiqueNumber = m.ChiqueNumber
                    //Type = m.Type
                });
            if (RecExcDetails != null)
                model.RecExcDetails = RecExcDetails;

            #endregion

            model.Count = unitOfWork.ReceiptExchangeRepository.Get(filter:m=>m.Type==type).Count();
            model.CurrencyNameAR = RecExc.Currency.NameAR;
            model.CurrencyNameEN= RecExc.Currency.NameEN;
            model.CurrencyCode = RecExc.Currency.Code;

            #region Setting part

            model.SettingModel = DefaultSetting(type);
            #endregion


            return model;
        }

        [HttpGet]
        [Route("~/api/ReceiptExchange/FirstOpen/{type}")]
        public IActionResult FirstOpen(bool type)
        {
            DefaultSettingModel model = new DefaultSettingModel();
            model.ScreenSetting = DefaultSetting(type);
            model.LastCode = unitOfWork.EntryRepository.Last().Code;
            return Ok(model);
        }

        [HttpGet]
        [Route("~/api/ReceiptExchange/GetLast/{type}")]
        public IActionResult GetLast(bool type)
        {
           
            // get last Receipt or Exchange
            var RecExc = unitOfWork.ReceiptExchangeRepository.Get(filter:m=>m.Type==type).Last();
            if(RecExc !=null)
                return Ok(GetReceiptExchange(RecExc,type));
            else
                return Ok(0);

        }


        [HttpGet]
        [Route("~/api/ReceiptExchange/Paging/{pageNumber}/{type}")]
        public IActionResult Pagination(int pageNumber,bool type)
        {
            if (pageNumber > 0)
            {

                var RecExc = unitOfWork.ReceiptExchangeRepository.Get(filter: m => m.Type == type, page: pageNumber).FirstOrDefault();
                if(RecExc != null)
                    return Ok(GetReceiptExchange(RecExc,type));
                else
                    return Ok(0);

            }
            else
                return Ok(1);
        }


        [HttpGet]
        [Route("~/api/ReceiptExchange/Get/{id}/{type}")]

        public IActionResult GetById(int id,bool type)
        {

            if (id > 0)
            {

                var RecExc = unitOfWork.ReceiptExchangeRepository.Get(filter: m => m.Type == type &&m.ReceiptID==id).FirstOrDefault();
                if (RecExc != null)
                    return Ok(GetReceiptExchange(RecExc, type));
                else
                    return Ok(0);


            }
            else
                return Ok(1);
        }


        [Route("~/api/ReceiptExchange/GetAll/{type}")]
        public IActionResult GetAll(bool type)
        {
            var RecExcs = unitOfWork.ReceiptExchangeRepository.Get(filter: m => m.Type == type).ToList();

            var model = _mapper.Map<IEnumerable<ReceiptExchangeModel>>(RecExcs).ToList();

            if (model == null)
            {
                return Ok(0);
            }

            for (int i = 0; i < RecExcs.Count(); i++)
            {
                for (int j = i; j < model.Count(); j++)
                {
                    if (model[j].ReceiptID == RecExcs[i].ReceiptID)
                    {
                        #region Date part

                        model[j].Date = RecExcs[i].Date.Value.ToString("dd/MM/yyyy");
                        model[j].DateHijri = DateHelper.GetHijriDate(RecExcs[i].Date);

                        model[j].ChiqueDate = RecExcs[i].ChiqueDate.Value.ToString("dd/MM/yyyy");
                        model[j].ChiqueDateHijri = DateHelper.GetHijriDate(RecExcs[i].ChiqueDate);
                        #endregion

                        model[j].Count= unitOfWork.ReceiptExchangeRepository.Get(filter: m => m.Type == type).Count();

                        #region Details part
                        var RecExcDetails = unitOfWork.ReceiptExchangeDetailRepository

                            .Get(filter: m => m.ReceiptID == RecExcs[i].ReceiptID)
                            .Select(m => new ReceiptExchangeDetailModel
                            {
                                ReceiptExchangeID = m.ReceiptExchangeID,
                                //ReceiptExchangeAmount=m.ReceiptExchangeAmount,
                                ReceiptID = m.ReceiptID,
                                AccountID = m.AccountID,
                                AccNameAR = m.Account.Code,
                                AccNameEN = m.Account.NameEN,
                                ChiqueNumber = m.ChiqueNumber
                              //  Type = m.Type

                            });
                        if (RecExcDetails != null)
                            model[j].RecExcDetails = RecExcDetails;

                        #endregion


                    }
                    else
                        continue;


                }

            }


            return Ok(model);
        }

        #endregion

        #region Insert Methods
        [HttpPost]
        [Route("~/api/ReceiptExchange/Add")]
        public IActionResult PostItem([FromBody] ReceiptExchangeModel recExcModel)
        {

            if (ModelState.IsValid)
            {

                var Check = unitOfWork.ReceiptExchangeRepository.Get();
                if (recExcModel == null)
                {
                    return Ok(0);
                }
                if (Check.Any(m => m.Code == recExcModel.Code))
                {
                    return Ok(2);
                }
                else
                {
                    // map model to entity
                    var model = _mapper.Map<ReceiptExchange>(recExcModel);


                    var recExcDetails = recExcModel.RecExcDetails;

                    var RecExcDetails = _mapper.Map<IEnumerable<ReceiptExchangeDetail>>(recExcModel.RecExcDetails);
                      // insert main data of receipt exchange 
                        unitOfWork.ReceiptExchangeRepository.Insert(model);

                        if (RecExcDetails != null)
                        {
                            foreach (var item in recExcDetails)
                            {
                                var obj = _mapper.Map<ReceiptExchangeDetail>(item);
                                obj.ReceiptID = model.ReceiptID;
                                unitOfWork.ReceiptExchangeDetailRepository.Insert(obj);
                            }
                        }


                        try
                        {


                        #region Generate entry
                        //===============================================================توليد قيد مع ترحيل تلقائي============================

                        var lastEntry = unitOfWork.EntryRepository.Last();
                        var lastEntryDitails = unitOfWork.EntryDetailRepository.Get(filter: x => x.EntryID == lastEntry.EntryID).Select(m => new EntryDetailModel
                        {
                            AccountID = m.AccountID,
                            Credit = m.Credit,
                            Debit = m.Debit,
                            EntryID = m.EntryID,
                            EntryDetailID = m.EntryDetailID,
                            AccCode = m.Account.Code,
                            AccNameAR = m.Account.NameAR,
                            AccNameEN = m.Account.NameEN,



                        });

                        var EntryMODEL = EntriesHelper.InsertCalculatedEntries(null, null, recExcModel, null, lastEntry);
                        var Entry = _mapper.Map<Entry>(EntryMODEL);


                        var DetailEnt = EntryMODEL.EntryDetailModel;

                        if (recExcModel.SettingModel.TransferToAccounts == true)
                        {
                            Entry.TransferedToAccounts = true;
                            unitOfWork.EntryRepository.Insert(Entry);
                            foreach (var item in DetailEnt)
                            {
                                item.EntryID = Entry.EntryID;
                                item.EntryDetailID = 0;
                                var details = _mapper.Map<ReceiptExchangeDetail>(item);

                                unitOfWork.ReceiptExchangeDetailRepository.Insert(details);
                            }
                            accountingHelper.TransferToAccounts(DetailEnt.Select(a => new EntryDetail
                            {

                                Credit = a.Credit,
                                Debit = a.Debit,
                                EntryDetailID = a.EntryDetailID,
                                EntryID = a.EntryID,
                                StocksCredit = a.StocksCredit,
                                StocksDebit = a.StocksDebit,
                                AccountID = a.AccountID

                            }).ToList());
                        }
                        else
                        {
                            Entry.TransferedToAccounts = false;
                            unitOfWork.EntryRepository.Insert(Entry);
                            foreach (var item in DetailEnt)
                            {
                                item.EntryID = Entry.EntryID;
                                item.EntryDetailID = 0;
                                var details = _mapper.Map<ReceiptExchangeDetail>(item);

                                unitOfWork.ReceiptExchangeDetailRepository.Insert(details);

                            }
                        }


                        unitOfWork.Save();


                        #endregion
                    }
                    catch (DbUpdateException ex)
                        {
                            var sqlException = ex.GetBaseException() as SqlException;

                            if (sqlException != null)
                            {
                                var number = sqlException.Number;
                                // data related
                                if (number == 547)
                                {
                                    return Ok(5);

                                }
                                else
                                    return Ok(6);
                            }
                        }
                        return Ok(model);

                    


                }



            }
            else
            {
                return Ok(3);
            }
        }
        #endregion


        #region Update Methods
        [HttpPut]
        [Route("~/api/ReceiptExchange/Update/{id}/{type}")]
        public IActionResult Update(int id,bool type, [FromBody] ReceiptExchangeModel Model)
        {
            if (id != Model.ReceiptID)
            {

                return Ok(1);
            }

            if (ModelState.IsValid)
            {
                // map main data to entity
                var model = _mapper.Map<ReceiptExchange>(Model);

                var recExcDetail = Model.RecExcDetails;
                //var EmpolyeeCard = _mapper.Map<IEnumerable<EmployeeCard>>(empolyeeCard);

                var Check = unitOfWork.ReceiptExchangeRepository.Get(NoTrack: "NoTrack", filter: m => m.Type == type);
                var oldDetail = unitOfWork.ReceiptExchangeDetailRepository.Get(NoTrack: "NoTrack", filter: m => m.ReceiptID == model.ReceiptID);
                var Entry = unitOfWork.EntryRepository.Get(filter: x => x.ReceiptID == model.ReceiptID).SingleOrDefault();
                var OldEntryDetails = unitOfWork.EntryDetailRepository.Get(filter: a => a.EntryID == Entry.EntryID);
                if (Entry.TransferedToAccounts == true)
                {
                    accountingHelper.CancelTransferToAccounts(OldEntryDetails.ToList());
                }

                if (oldDetail != null)
                {
                    unitOfWork.ReceiptExchangeDetailRepository.RemovRange(oldDetail);

                } 
               
                    unitOfWork.EntryDetailRepository.RemovRange(OldEntryDetails);
                

                if (Check.Any(m => m.Code != Model.Code))
                {
                    unitOfWork.ReceiptExchangeRepository.Update(model);


                    foreach (var item in recExcDetail)
                    {
                        item.ReceiptID = model.ReceiptID;
                        item.ReceiptExchangeID = 0;
                        var newDetail = _mapper.Map<ReceiptExchangeDetail>(item);

                        unitOfWork.ReceiptExchangeDetailRepository.Insert(newDetail);

                    }

                    try
                    {

                        #region Generate entry

                        var EntryDitails = EntriesHelper.UpdateCalculateEntries(Entry.EntryID, null, null, Model, null);

                        if (Model.SettingModel.TransferToAccounts == true)
                        {
                            Entry.TransferedToAccounts = true;
                            unitOfWork.EntryRepository.Update(Entry);
                            foreach (var item in EntryDitails)
                            {
                                //    item.EntryID = Entry.EntryID;
                                item.EntryDetailID = 0;
                                var details = _mapper.Map<ReceiptExchangeDetail>(item);

                                unitOfWork.ReceiptExchangeDetailRepository.Insert(details);

                            }
                            accountingHelper.TransferToAccounts(EntryDitails.Select(x => new EntryDetail
                            {


                                EntryDetailID = x.EntryDetailID,
                                AccountID = x.AccountID,
                                Credit = x.Credit,
                                Debit = x.Debit,
                                EntryID = x.EntryID


                            }).ToList());
                        }
                        else
                        {
                            Entry.TransferedToAccounts = false;
                            unitOfWork.EntryRepository.Update(Entry);
                            foreach (var item in EntryDitails)
                            {
                                //  item.EntryID = Entry.EntryID;
                                item.EntryDetailID = 0;
                                var details = _mapper.Map<PurchaseOrderDetail>(item);

                                unitOfWork.PurchaseOrderDetailRepository.Insert(details);

                            }
                        }
                        unitOfWork.Save();
                        #endregion

                    }
                    catch (DbUpdateException ex)
                    {
                        var sqlException = ex.GetBaseException() as SqlException;

                        if (sqlException != null)
                        {
                            var number = sqlException.Number;
                            // data related
                            if (number == 547)
                            {
                                return Ok(5);

                            }
                            else
                                return Ok(6);
                        }
                    }
                    return Ok(model);


                }
                else
                {
                    if (Check.Any(m => m.Code == Model.Code && m.ReceiptID == id))
                    {

                        unitOfWork.ReceiptExchangeRepository.Update(model);
                        
                        foreach (var item in recExcDetail)
                        {
                            item.ReceiptID = model.ReceiptID;
                            item.ReceiptExchangeID = 0;
                            var neweDetail = _mapper.Map<ReceiptExchangeDetail>(item);

                            unitOfWork.ReceiptExchangeDetailRepository.Insert(neweDetail);

                        }

                        try
                        {


                            #region Generate entry

                            var EntryDitails = EntriesHelper.UpdateCalculateEntries(Entry.EntryID,null,null,Model,null);

                            if (Model.SettingModel.TransferToAccounts == true)
                            {
                                Entry.TransferedToAccounts = true;
                                unitOfWork.EntryRepository.Update(Entry);
                                foreach (var item in EntryDitails)
                                {
                                //    item.EntryID = Entry.EntryID;
                                    item.EntryDetailID = 0;
                                    var details = _mapper.Map<ReceiptExchangeDetail>(item);

                                    unitOfWork.ReceiptExchangeDetailRepository.Insert(details);

                                }
                                accountingHelper.TransferToAccounts(EntryDitails.Select(x => new EntryDetail
                                {


                                    EntryDetailID = x.EntryDetailID,
                                    AccountID = x.AccountID,
                                    Credit = x.Credit,
                                    Debit = x.Debit,
                                    EntryID = x.EntryID


                                }).ToList());
                            }
                            else
                            {
                                Entry.TransferedToAccounts = false;
                                unitOfWork.EntryRepository.Update(Entry);
                                foreach (var item in EntryDitails)
                                {
                                  //  item.EntryID = Entry.EntryID;
                                    item.EntryDetailID = 0;
                                    var details = _mapper.Map<PurchaseOrderDetail>(item);

                                    unitOfWork.PurchaseOrderDetailRepository.Insert(details);

                                }
                            }
                            unitOfWork.Save();
                            #endregion

                        }
                        catch (DbUpdateException ex)
                        {
                            var sqlException = ex.GetBaseException() as SqlException;

                            if (sqlException != null)
                            {
                                var number = sqlException.Number;
                                // data related
                                if (number == 547)
                                {
                                    return Ok(5);

                                }
                                else
                                    return Ok(6);
                            }
                        }

                        return Ok(model);

                    }
                    else
                    {
                        return Ok(2);
                    }
                }

            }
            else
            {
                return Ok(3);
            }
        }
        #endregion


        #region Delete Methods

        [HttpDelete]
        [Route("~/api/ReceiptExchange/Delete/{id}")]
        public IActionResult Delete(int? id)
        {

            //var RecExc = unitOfWork.ReceiptExchangeRepository.Get(filter: m => m.Type == type && m.ReceiptID == id).FirstOrDefault();
            if (id>0)
            {
                var RecExc = unitOfWork.ReceiptExchangeRepository.GetByID(id);

                if (RecExc == null)
                {
                    return Ok(0);
                }
                var recDetails = unitOfWork.ReceiptExchangeDetailRepository.Get(filter: m => m.ReceiptID == id);



                unitOfWork.ReceiptExchangeDetailRepository.RemovRange(recDetails);
                var entry = unitOfWork.EntryRepository.Get(x=> x.ReceiptID==id).SingleOrDefault();
                var entryDitails = unitOfWork.EntryDetailRepository.Get(a=> a.EntryID==entry.EntryID);
                if (entry.TransferedToAccounts == true)
                {
                    accountingHelper.CancelTransferToAccounts(entryDitails.ToList());
                }
                unitOfWork.EntryDetailRepository.RemovRange(entryDitails);
                unitOfWork.EntryRepository.Delete(entry.EntryID);
                unitOfWork.ReceiptExchangeRepository.Delete(RecExc);
                try
                {
                    unitOfWork.Save();
                }
                catch (DbUpdateException ex)
                {
                    var sqlException = ex.GetBaseException() as SqlException;

                    if (sqlException != null)
                    {
                        var number = sqlException.Number;

                        if (number == 547)
                        {
                            return Ok(5);

                        }
                        else
                            return Ok(6);
                    }
                }
                return Ok(4);

                //var Result = unitOfWork.Save();
                //if (Result == true)
                //{
                //    return Ok(4);
                //}
                //else
                //{
                //    return NotFound();
                //} 
            }
            else
                return Ok(1);


        }

        #endregion
    }
}
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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Stocks.Controllers
{
    [Authorize(Roles = "SuperAdmin,Admin,Employee")]
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

        [Route("~/api/ReceiptExchange/GetSettingAccounts/{id}")]
        public IEnumerable<SettingAccountModel> SettingAccounts(int id)
        {

            var setAccounts = unitOfWork.SettingAccountRepository.Get(filter: x => x.SettingID == id).Select(a => new SettingAccountModel
            {

                SettingID = a.SettingID,
                AccNameEN = a.Account.NameEN,
                AccCode = a.Account.Code,
                AccNameAR = a.Account.NameAR,
                AccountID = a.AccountID,
                AccountType = a.AccountType,
                SettingAccountID = a.SettingAccountID,
                Code = a.Setting.Code,



            });



            return setAccounts;


        }

        [Route("~/api/ReceiptExchange/GetSetting")]
        public SettingModel GetSetting(int flag)
        {

            var setsetting = unitOfWork.SettingRepository.Get(filter: x => x.VoucherType == flag).Select(a => new SettingModel
            {

                SettingID = a.SettingID,
                VoucherType = 2,
                AutoGenerateEntry = a.AutoGenerateEntry,
                Code = a.Code,
                DoNotGenerateEntry = a.DoNotGenerateEntry,
                GenerateEntry = a.GenerateEntry,
                SettingAccs = SettingAccounts(a.SettingID),
                TransferToAccounts=a.TransferToAccounts

            }).SingleOrDefault();
            return setsetting;


        }

       

        

        //[HttpPost] // يولد قيد مع ترحيل تلقائي
        //[Route("~/api/ReceiptExchange/Generateconstraint")]
        //public IActionResult Generateconstraint(ReceiptExchangeModel receiptExchangeModel)
        //{
        //    var lastEntry = unitOfWork.EntryRepository.Last();
        //    var lastEntryDitails = unitOfWork.EntryDetailRepository.Get(filter: x => x.EntryID == lastEntry.EntryID).Select(m => new EntryDetailModel
        //    {
        //        AccountID = m.AccountID,
        //        Credit = m.Credit,
        //        Debit = m.Debit,
        //        EntryID = m.EntryID,
        //        EntryDetailID = m.EntryDetailID,
        //        AccCode = m.Account.Code,
        //        AccNameAR = m.Account.NameAR,
        //        AccNameEN = m.Account.NameEN,



        //    });
        //    int portofolioaccount = unitOfWork.PortfolioAccountRepository.Get(filter: m => m.PortfolioID == noticeModel.PortfolioID && m.Type == true).Select(m => m.PortfolioAccountID).SingleOrDefault();

        //    var EntryMODEL = EntriesHelper.InsertCalculatedEntries(null, null, receiptExchangeModel, null, lastEntry);
        //    var Entry = _mapper.Map<Entry>(EntryMODEL);


        //    var DetailEnt = EntryMODEL.EntryDetailModel;

        //    if (receiptExchangeModel.SettingModel.TransferToAccounts == true)
        //    {
        //        Entry.TransferedToAccounts = true;
        //        unitOfWork.EntryRepository.Insert(Entry);
        //        foreach (var item in DetailEnt)
        //        {
        //            item.EntryID = Entry.EntryID;
        //            item.EntryDetailID = 0;
        //            var details = _mapper.Map<ReceiptExchangeDetail>(item);

        //            unitOfWork.ReceiptExchangeDetailRepository.Insert(details);
        //        }
        //        accountingHelper.TransferToAccounts(DetailEnt.Select(a => new EntryDetail
        //        {

        //            Credit = a.Credit,
        //            Debit = a.Debit,
        //            EntryDetailID = a.EntryDetailID,
        //            EntryID = a.EntryID,
        //            StocksCredit = a.StocksCredit,
        //            StocksDebit = a.StocksDebit,
        //            AccountID = a.AccountID

        //        }).ToList());
        //    }
        //    else
        //    {
        //        Entry.TransferedToAccounts = false;
        //        unitOfWork.EntryRepository.Insert(Entry);
        //        foreach (var item in DetailEnt)
        //        {
        //            item.EntryID = Entry.EntryID;
        //            item.EntryDetailID = 0;
        //            var details = _mapper.Map<SellingOrderDetail>(item);

        //            unitOfWork.SellingOrderDetailRepository.Insert(details);

        //        }
        //    }
        //    return Ok(receiptExchangeModel);
        //}


        [HttpPost]// ترحيل يدوي للقيد اليدوي والتلقائي
        [Route("~/api/ReceiptExchange/Manualmigration")]
        public IActionResult Manualmigration(EntryModel EntryMODEL)
        {
            var Entry = unitOfWork.EntryRepository.GetByID(EntryMODEL.EntryID);
            Entry.TransferedToAccounts = true;
            unitOfWork.EntryRepository.Update(Entry);
            var Details = EntryMODEL.EntryDetailModel;
            //foreach (var item in Details)
            //{
            //    var detail = _mapper.Map<EntryDetail>(item);

            //    unitOfWork.ReceiptExchangeDetailRepository.Update(detail);
            //}

            accountingHelper.TransferToAccounts(Details.Select(x => new EntryDetail
            {
                EntryDetailID = x.EntryDetailID,
                AccountID = x.AccountID,
                Credit = x.Credit,
                Debit = x.Debit,
                EntryID = x.EntryID


            }).ToList());




            var Result = unitOfWork.Save();
            if (Result == 200)
            {

                return Ok(4);
            }
            else if (Result == 501)
            {
                return Ok(5);

            }
            else
            {
                return Ok(6);
            }

        }


        [HttpGet]//القيد
        [Route("~/api/ReceiptExchange/GetEntry")]
        public EntryModel GetEntry(int receiptID)
        {
            var Entry = unitOfWork.EntryRepository.Get(x => x.ReceiptID == receiptID).FirstOrDefault();
            EntryModel entryModel = new EntryModel();
            if(Entry != null)
            {
                entryModel.EntryID = Entry.EntryID;
                entryModel.Code = Entry.Code;
                entryModel.Date = Entry.Date != null ? Entry.Date.Value.ToString("d/M/yyyy") : null;
                entryModel.DateHijri = Entry.Date != null ? DateHelper.GetHijriDate(Entry.Date) : null;
                entryModel.NoticeID = Entry.NoticeID;
                entryModel.PurchaseInvoiceID = Entry.PurchaseInvoiceID;
                entryModel.ReceiptID = Entry.ReceiptID;
                entryModel.SellingInvoiceID = Entry.SellingInvoiceID;
                entryModel.EntryDetailModel = unitOfWork.EntryDetailRepository.Get(filter: a => a.EntryID == Entry.EntryID) != null ? unitOfWork.EntryDetailRepository.Get(filter: a => a.EntryID == Entry.EntryID).Select(m => new EntryDetailModel
                {
                    AccCode = m.Account.Code,
                    AccNameAR = m.Account.NameAR,
                    AccNameEN = m.Account.NameEN,
                    AccountID = m.AccountID,
                    Credit = m.Credit,
                    Debit = m.Debit,
                    EntryDetailID = m.EntryDetailID,
                    EntryID = m.EntryID,


                }) : null;
                entryModel.TransferedToAccounts = Entry.TransferedToAccounts;

            }

            return entryModel;
        }




        #region GET Methods

        public ReceiptExchangeModel GetReceiptExchange(ReceiptExchange RecExc, bool ReceiptExchangeType, bool type, int num)
       {
            var model = _mapper.Map<ReceiptExchangeModel>(RecExc);
            if (model == null)
            {
                return model;
            }
            model.BankName = RecExc.BankName;
            model.ChiqueDate = RecExc.ChiqueDate != null ? RecExc.ChiqueDate.Value.ToString("d/M/yyyy") : null;
            model.ChiqueDateHijri = RecExc.ChiqueDate != null ? DateHelper.GetHijriDate(RecExc.ChiqueDate) : null;
            model.ChiqueNumber = RecExc.ChiqueNumber;
            model.Code = RecExc.Code;
            model.Date = RecExc.Date != null ? RecExc.Date.Value.ToString("d/M/yyyy") : null;
            model.DateHijri = RecExc.Date != null ? DateHelper.GetHijriDate(RecExc.Date) : null;
            model.Description = RecExc.Description;
            model.Handling = RecExc.Handling;
            model.ReceiptExchangeType = RecExc.ReceiptExchangeType;
            model.ReceiptID = RecExc.ReceiptID;
            model.Type = RecExc.Type;
            model.RecieptValue = RecExc.RecieptValue;
            model.TaxNumber = RecExc.TaxNumber;
            model.Count = unitOfWork.ReceiptExchangeRepository.Get(filter: m => m.Type == type && m.ReceiptExchangeType == ReceiptExchangeType).Count();
            model.EntryModel = GetEntry(RecExc.ReceiptID);
            model.RecExcDetails = unitOfWork.ReceiptExchangeDetailRepository

                   .Get(filter: m => m.ReceiptID == RecExc.ReceiptID) != null ? unitOfWork.ReceiptExchangeDetailRepository.Get(filter: m => m.ReceiptID == RecExc.ReceiptID)
                   .Select(m => new ReceiptExchangeDetailModel
                   {
                       ReceiptExchangeID = m.ReceiptExchangeID,
                       ReceiptID = m.ReceiptID,
                       AccountID = m.AccountID,
                       AccNameAR = m.Account.NameAR,
                       AccNameEN = m.Account.NameEN,
                       AccCode = m.Account.Code,
                       DetailType = m.DetailType,
                       Debit = m.Debit,
                       Credit = m.Credit,


                   }) : null;
            model.LastCode = unitOfWork.ReceiptExchangeRepository.Get(filter: m => m.Type == type && m.ReceiptExchangeType == ReceiptExchangeType) != null ? unitOfWork.ReceiptExchangeRepository.Get(filter: m => m.Type == type && m.ReceiptExchangeType == ReceiptExchangeType).FirstOrDefault().Code : null;
            model.SettingModel = GetSetting(num);
                

           
            return model;
        }

        [HttpGet]
        [Route("~/api/ReceiptExchange/FirstOpen/{ReceiptExchangeType}/{type}")]
        public IActionResult FirstOpen(bool ReceiptExchangeType, bool type)

        {
            ReceiptExchangeModel model = new ReceiptExchangeModel();
            //RS
            if (ReceiptExchangeType==true)
            {
                // exchange
                if (type == true)
                {
                    if (unitOfWork.ReceiptExchangeRepository.Get(filter: x => x.Type == true && x.ReceiptExchangeType == true).Count() > 0)
                    {
                        if (unitOfWork.ReceiptExchangeRepository.Get(filter: x => x.Type == true && x.ReceiptExchangeType == true).Last() == null)
                        {
                            return Ok(0);
                        }
                        var noti = unitOfWork.ReceiptExchangeRepository.Get(filter: x => x.Type == true && x.ReceiptExchangeType == true).Last();


                        model.LastCode = noti.Code;
                    }
                    model.SettingModel = GetSetting(6);
                    model.Count = unitOfWork.ReceiptExchangeRepository.Get(filter: x => x.Type == true && x.ReceiptExchangeType == true).Count();

                }
                //receipt
                else
                {
                    
                    if (unitOfWork.ReceiptExchangeRepository.Get(filter: x => x.Type == false && x.ReceiptExchangeType == true).Count() > 0)
                    {
                        if (unitOfWork.ReceiptExchangeRepository.Get(filter: x => x.Type == false && x.ReceiptExchangeType == true).Last() == null)
                        {
                            return Ok(0);
                        }

                        var noti = unitOfWork.ReceiptExchangeRepository.Get(filter: x => x.Type == false && x.ReceiptExchangeType == true).Last();

                        model.LastCode = noti.Code;
                    }

                    model.SettingModel = GetSetting(5);
                    model.Count = unitOfWork.ReceiptExchangeRepository.Get(filter: x => x.Type == false && x.ReceiptExchangeType == true).Count();

                }

            }
            else
            {
                //شيك
                if (ReceiptExchangeType==false)
                {
                    //exchange
                    if (type == true)
                    {
                        if (unitOfWork.ReceiptExchangeRepository.Get(filter: x => x.Type == true &&x.ReceiptExchangeType == false).Count() > 0)
                        {
                            if (unitOfWork.ReceiptExchangeRepository.Get(filter: x => x.Type == true && x.ReceiptExchangeType == false).Last() == null)
                            {
                                return Ok(0);
                            }
                            var noti = unitOfWork.ReceiptExchangeRepository.Get(filter: x => x.Type == true && x.ReceiptExchangeType == false).Last();


                            model.LastCode = noti.Code;
                        }
                        model.SettingModel = GetSetting(6);
                        model.Count = unitOfWork.ReceiptExchangeRepository.Get(filter: x => x.Type == true && x.ReceiptExchangeType == false).Count();

                    }
                    //receipt
                    else
                    {
                       
                        if (unitOfWork.ReceiptExchangeRepository.Get(filter: x => x.Type == false && x.ReceiptExchangeType == false).Count() > 0)
                        {
                            if (unitOfWork.ReceiptExchangeRepository.Get(filter: x => x.Type == false && x.ReceiptExchangeType == false).Last() == null)
                            {
                                return Ok(0);
                            }

                            var noti = unitOfWork.ReceiptExchangeRepository.Get(filter: x => x.Type == false && x.ReceiptExchangeType == false).Last();

                            model.LastCode = noti.Code;
                        }

                        model.SettingModel = GetSetting(5);
                        model.Count = unitOfWork.ReceiptExchangeRepository.Get(filter: x => x.Type == false && x.ReceiptExchangeType == false).Count();

                    }

                }
            }



            return Ok(model);
        }

        [HttpGet]
        [Route("~/api/ReceiptExchange/GetLast/{ReceiptExchangeType}/{type}/{numSetting}")]
        public IActionResult GetLast(bool ReceiptExchangeType, bool type,int numSetting)
        {
           
            // get last Receipt or Exchange
            var RecExc = unitOfWork.ReceiptExchangeRepository.Get(filter:m=>m.Type==type && m.ReceiptExchangeType==ReceiptExchangeType).Last();
            if (RecExc != null) {
                var model = GetReceiptExchange(RecExc, ReceiptExchangeType, type, numSetting);
                return Ok(model);
            }
               
            else
                return Ok(0);

        }


        [HttpGet]
        [Route("~/api/ReceiptExchange/Paging/{pageNumber}/{ReceiptExchangeType}/{type}/{numSetting}")]
        public IActionResult Pagination(int pageNumber, bool ReceiptExchangeType, bool type, int numSetting)
        {
            if (pageNumber > 0)
            {

                var RecExc = unitOfWork.ReceiptExchangeRepository.Get(filter: m => m.Type == type&&m.ReceiptExchangeType==ReceiptExchangeType, page: pageNumber).FirstOrDefault();
                if(RecExc != null)
                    return Ok(GetReceiptExchange(RecExc,ReceiptExchangeType, type, numSetting));
                else
                    return Ok(0);

            }
            else
                return Ok(1);
        }


        [HttpGet]
        [Route("~/api/ReceiptExchange/Get/{id}/{ReceiptExchangeType}/{type}/{numSetting}")]

        public IActionResult GetById(int id, bool ReceiptExchangeType, bool type, int numSetting)
        {

            if (id > 0)
            {

                var RecExc = unitOfWork.ReceiptExchangeRepository.Get(filter: m => m.Type == type &&m.ReceiptID==id).FirstOrDefault();
                if (RecExc != null)
                    return Ok(GetReceiptExchange(RecExc,ReceiptExchangeType, type, numSetting));
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

                        model[j].Date = RecExcs[i].Date.Value.ToString("d/M/yyyy");
                        model[j].DateHijri = DateHelper.GetHijriDate(RecExcs[i].Date);

                        model[j].ChiqueDate = RecExcs[i].ChiqueDate.Value.ToString("d/M/yyyy");
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
                                AccNameEN = m.Account.NameEN
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
                if (Check.Any(m => m.Code == recExcModel.Code && m.Type==recExcModel.Type && m.ReceiptExchangeType==recExcModel.ReceiptExchangeType))
                {

                    return Ok(2);
                }
                else
                {

                    var receipt = _mapper.Map<ReceiptExchange>(recExcModel);


                    var Details = recExcModel.RecExcDetails;

                    unitOfWork.ReceiptExchangeRepository.Insert(receipt);
                    if (Details.Count() > 0)
                    {
                        foreach (var item in Details)
                        {
                            ReceiptExchangeDetailModel receiptExchangeDetailModel = new ReceiptExchangeDetailModel();
                            receiptExchangeDetailModel.ReceiptID = receipt.ReceiptID;
                            receiptExchangeDetailModel.AccountID = item.AccountID;
                            receiptExchangeDetailModel.ChiqueNumber = item.ChiqueNumber;
                            receiptExchangeDetailModel.Credit = item.Credit;
                            receiptExchangeDetailModel.Debit = item.Debit;
                            receiptExchangeDetailModel.DetailType = item.DetailType;
                            var details = _mapper.Map<ReceiptExchangeDetail>(receiptExchangeDetailModel);
                            unitOfWork.ReceiptExchangeDetailRepository.Insert(details);

                        }

                    }



                    //===============================================================توليد قيد مع ترحيل تلقائي============================



                        var lastEntry = unitOfWork.EntryRepository.Last();
                        var EntryMODEL = EntriesHelper.InsertCalculatedEntries(0, null, null, recExcModel, null, lastEntry);
                        var Entry = _mapper.Map<Entry>(EntryMODEL);
                        Entry.ReceiptID = receipt.ReceiptID;

                        var DetailEnt = EntryMODEL.EntryDetailModel;

                        if (recExcModel.SettingModel.TransferToAccounts == true)
                        {
                            Entry.TransferedToAccounts = true;
                            unitOfWork.EntryRepository.Insert(Entry);
                            foreach (var item in DetailEnt)
                            {
                                item.EntryID = Entry.EntryID;
                                item.EntryDetailID = 0;
                                var details = _mapper.Map<EntryDetail>(item);

                                unitOfWork.EntryDetailRepository.Insert(details);

                            }
                            accountingHelper.TransferToAccounts(DetailEnt.Select(x => new EntryDetail
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
                            unitOfWork.EntryRepository.Insert(Entry);
                            foreach (var item in DetailEnt)
                            {
                                item.EntryID = Entry.EntryID;
                                item.EntryDetailID = 0;
                                var details = _mapper.Map<EntryDetail>(item);

                                unitOfWork.EntryDetailRepository.Insert(details);
                            }
                        }



                    


                    var Res = unitOfWork.Save();
                    if (Res == 200)
                    {

                        return Ok(4);
                    }
                    else if (Res == 501)
                    {
                        return Ok(5);
                    }
                    else
                    {
                        return Ok(6);
                    }


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
        [Route("~/api/ReceiptExchange/Update/{id}")]
        public IActionResult Update(int id, [FromBody] ReceiptExchangeModel receiptExchangeModel)
        {
            if (id != receiptExchangeModel.ReceiptID)
            {

                return Ok(1);
            }

            if (ModelState.IsValid)
            {

                var Check = unitOfWork.ReceiptExchangeRepository.Get(NoTrack: "NoTrack");

                var ReceiptExchange = _mapper.Map<ReceiptExchange>(receiptExchangeModel);
                var NewdDetails = receiptExchangeModel.RecExcDetails;
                var Newdetails = _mapper.Map<IEnumerable<ReceiptExchangeDetail>>(NewdDetails);
                var OldDetails = unitOfWork.ReceiptExchangeDetailRepository.Get(filter: m => m.ReceiptID == ReceiptExchange.ReceiptID);
                var EntryCheck = unitOfWork.EntryRepository.Get(x => x.ReceiptID == ReceiptExchange.ReceiptID).SingleOrDefault();
                if (EntryCheck != null)
                {

                    var OldEntry = unitOfWork.EntryRepository.Get(filter: x => x.ReceiptID == ReceiptExchange.ReceiptID).SingleOrDefault();
                    var OldEntryDetails = unitOfWork.EntryDetailRepository.Get(filter: a => a.EntryID == OldEntry.EntryID);
                    if (OldEntry.TransferedToAccounts == true)
                    {
                        accountingHelper.CancelTransferToAccounts(OldEntryDetails.ToList());
                    }
                    // delete old entry
                    unitOfWork.EntryDetailRepository.RemovRange(OldEntryDetails);
                    unitOfWork.EntryRepository.Delete(OldEntry);

                    if (Check.Any(m => m.Code != ReceiptExchange.Code))
                    {
                        unitOfWork.ReceiptExchangeRepository.Update(ReceiptExchange);
                        if (OldDetails != null)
                        {
                            unitOfWork.ReceiptExchangeDetailRepository.RemovRange(OldDetails);
                           
                        }


                        if (Newdetails != null)
                        {
                            foreach (var item in Newdetails)
                            {
                                item.ReceiptID = ReceiptExchange.ReceiptID;
                                item.ReceiptExchangeID = 0;
                                var details = _mapper.Map<ReceiptExchangeDetail>(item);

                                unitOfWork.ReceiptExchangeDetailRepository.Insert(details);

                            }
                        }


                        //===================================توليد قيد مع ترحيل تلقائي===================================

                        //var EntryDitails = EntriesHelper.UpdateCalculateEntries(0,Entry.EntryID, null, null, receiptExchangeModel, null);
                        var lastEntry = unitOfWork.EntryRepository.Last();
                        var EntryMODEL = EntriesHelper.InsertCalculatedEntries(0, null, null, receiptExchangeModel, null, lastEntry, OldEntry);
                        var Entry = _mapper.Map<Entry>(EntryMODEL);
                        Entry.ReceiptID = ReceiptExchange.ReceiptID;
                        var EntryDitails = EntryMODEL.EntryDetailModel;

                        if (receiptExchangeModel.SettingModel.TransferToAccounts == true)
                            {
                                Entry.TransferedToAccounts = true;
                                unitOfWork.EntryRepository.Insert(Entry);
                                foreach (var item in EntryDitails)
                                {
                                    item.EntryID = Entry.EntryID;
                                    item.EntryDetailID = 0;
                                    var details = _mapper.Map<EntryDetail>(item);

                                    unitOfWork.EntryDetailRepository.Insert(details);

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
                                unitOfWork.EntryRepository.Insert(Entry);
                                foreach (var item in EntryDitails)
                                {
                                    item.EntryID = Entry.EntryID;
                                    item.EntryDetailID = 0;
                                    var details = _mapper.Map<EntryDetail>(item);

                                    unitOfWork.EntryDetailRepository.Insert(details);
                                }
                            }

                        


                        var Res = unitOfWork.Save();
                        if (Res == 200)
                        {

                            return Ok(4);
                        }
                        else if (Res == 501)
                        {
                            return Ok(5);
                        }
                        else
                        {
                            return Ok(6);
                        }



                    }


                    //==========================================Second Case OF Code Of Purchase=======================================

                    else
                    {
                        if (Check.Any(m => m.Code == ReceiptExchange.Code && m.ReceiptID == id && m.Type == receiptExchangeModel.Type && m.ReceiptExchangeType == receiptExchangeModel.ReceiptExchangeType))
                        {
                            unitOfWork.ReceiptExchangeRepository.Update(ReceiptExchange);
                            if (OldDetails != null)
                            {
                                unitOfWork.ReceiptExchangeDetailRepository.RemovRange(OldDetails);
                                //unitOfWork.Save();
                            }


                            if (Newdetails != null)
                            {
                                foreach (var item in Newdetails)
                                {
                                    item.ReceiptID = ReceiptExchange.ReceiptID;
                                    item.ReceiptExchangeID = 0;
                                    var details = _mapper.Map<ReceiptExchangeDetail>(item);

                                    unitOfWork.ReceiptExchangeDetailRepository.Insert(details);

                                }
                            }


                            //===================================توليد قيد مع ترحيل تلقائي===================================

                            //var EntryDitails = EntriesHelper.UpdateCalculateEntries(0,Entry.EntryID, null, null, receiptExchangeModel, null);
                            var lastEntry = unitOfWork.EntryRepository.Last();
                            var EntryMODEL = EntriesHelper.InsertCalculatedEntries(0, null, null, receiptExchangeModel, null, lastEntry, OldEntry);
                            var Entry = _mapper.Map<Entry>(EntryMODEL);
                            Entry.ReceiptID = ReceiptExchange.ReceiptID;
                            var EntryDitails = EntryMODEL.EntryDetailModel;
                            if (receiptExchangeModel.SettingModel.TransferToAccounts == true)
                                {
                                    Entry.TransferedToAccounts = true;
                                    unitOfWork.EntryRepository.Insert(Entry);
                                    foreach (var item in EntryDitails)
                                    {
                                        item.EntryID = Entry.EntryID;
                                        item.EntryDetailID = 0;
                                        var details = _mapper.Map<EntryDetail>(item);

                                        unitOfWork.EntryDetailRepository.Insert(details);

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
                                    unitOfWork.EntryRepository.Insert(Entry);
                                    foreach (var item in EntryDitails)
                                    {
                                        item.EntryID = Entry.EntryID;
                                        item.EntryDetailID = 0;
                                        var details = _mapper.Map<EntryDetail>(item);

                                        unitOfWork.EntryDetailRepository.Insert(details);
                                    }
                                }

                            
                            
                            }

                        var Res = unitOfWork.Save();
                        if (Res == 200)
                        {

                            return Ok(4);
                        }
                        else if (Res == 501)
                        {
                            return Ok(5);
                        }
                        else
                        {
                            return Ok(6);
                        }

                    }

                   

                }
                    
                

                // now We Will Create new Entry As Insert


                else
                {
                    if (Check.Any(m => m.Code != ReceiptExchange.Code && m.Type == receiptExchangeModel.Type && m.ReceiptExchangeType == receiptExchangeModel.ReceiptExchangeType))
                    {
                        unitOfWork.ReceiptExchangeRepository.Update(ReceiptExchange);
                        if (OldDetails != null)
                        {
                            unitOfWork.ReceiptExchangeDetailRepository.RemovRange(OldDetails);
                            //unitOfWork.Save();
                        }


                        if (Newdetails != null)
                        {
                            foreach (var item in Newdetails)
                            {
                                item.ReceiptID = ReceiptExchange.ReceiptID;
                                item.ReceiptExchangeID = 0;
                                var details = _mapper.Map<ReceiptExchangeDetail>(item);

                                unitOfWork.ReceiptExchangeDetailRepository.Insert(details);

                            }
                        }


                        //===============================================================توليد قيد مع ترحيل تلقائي============================



                       
                            var lastEntry = unitOfWork.EntryRepository.Last();
                            var EntryMODEL = EntriesHelper.InsertCalculatedEntries(0,null, null, receiptExchangeModel, null, lastEntry);
                            var Entry = _mapper.Map<Entry>(EntryMODEL);
                            Entry.ReceiptID = ReceiptExchange.ReceiptID;

                            var DetailEnt = EntryMODEL.EntryDetailModel;

                            if (receiptExchangeModel.SettingModel.TransferToAccounts == true)
                            {
                                Entry.TransferedToAccounts = true;
                                unitOfWork.EntryRepository.Insert(Entry);
                                foreach (var item in DetailEnt)
                                {
                                    item.EntryID = Entry.EntryID;
                                    item.EntryDetailID = 0;
                                    var details = _mapper.Map<EntryDetail>(item);

                                    unitOfWork.EntryDetailRepository.Insert(details);

                                }
                                accountingHelper.TransferToAccounts(DetailEnt.Select(x => new EntryDetail
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
                                unitOfWork.EntryRepository.Insert(Entry);
                                foreach (var item in DetailEnt)
                                {
                                    item.EntryID = Entry.EntryID;
                                    item.EntryDetailID = 0;
                                    var details = _mapper.Map<EntryDetail>(item);

                                    unitOfWork.EntryDetailRepository.Insert(details);
                                }
                            }
                        


                        var Res = unitOfWork.Save();
                        if (Res == 200)
                        {

                            return Ok(4);
                        }
                        else if (Res == 501)
                        {
                            return Ok(5);
                        }
                        else
                        {
                            return Ok(6);
                        }


                    }


                    //==========================================Second Case OF Code Of Purchase=======================================

                    else
                    {
                        if (Check.Any(m => m.Code == ReceiptExchange.Code && m.ReceiptID == id && m.Type == receiptExchangeModel.Type && m.ReceiptExchangeType == receiptExchangeModel.ReceiptExchangeType))
                        {
                            unitOfWork.ReceiptExchangeRepository.Update(ReceiptExchange);
                            if (OldDetails != null)
                            {
                                unitOfWork.ReceiptExchangeDetailRepository.RemovRange(OldDetails);
                                unitOfWork.Save();
                            }


                            if (Newdetails != null)
                            {
                                foreach (var item in Newdetails)
                                {
                                    item.ReceiptID = ReceiptExchange.ReceiptID;
                                    item.ReceiptExchangeID = 0;
                                    var details = _mapper.Map<ReceiptExchangeDetail>(item);

                                    unitOfWork.ReceiptExchangeDetailRepository.Insert(details);

                                }
                            }


                            //===============================================================توليد قيد مع ترحيل تلقائي============================



                            else if (receiptExchangeModel.SettingModel.AutoGenerateEntry == true)
                            {
                                var lastEntry = unitOfWork.EntryRepository.Last();
                                var EntryMODEL = EntriesHelper.InsertCalculatedEntries(0,null, null, receiptExchangeModel, null, lastEntry);
                                var Entry = _mapper.Map<Entry>(EntryMODEL);
                                Entry.ReceiptID = ReceiptExchange.ReceiptID;

                                var DetailEnt = EntryMODEL.EntryDetailModel;

                                if (receiptExchangeModel.SettingModel.TransferToAccounts == true)
                                {
                                    Entry.TransferedToAccounts = true;
                                    unitOfWork.EntryRepository.Insert(Entry);
                                    foreach (var item in DetailEnt)
                                    {
                                        item.EntryID = Entry.EntryID;
                                        item.EntryDetailID = 0;
                                        var details = _mapper.Map<EntryDetail>(item);

                                        unitOfWork.EntryDetailRepository.Insert(details);

                                    }
                                    accountingHelper.TransferToAccounts(DetailEnt.Select(x => new EntryDetail
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
                                    unitOfWork.EntryRepository.Insert(Entry);
                                    foreach (var item in DetailEnt)
                                    {
                                        item.EntryID = Entry.EntryID;
                                        item.EntryDetailID = 0;
                                        var details = _mapper.Map<EntryDetail>(item);

                                        unitOfWork.EntryDetailRepository.Insert(details);
                                    }
                                }
                            }


                            var Res = unitOfWork.Save();
                            if (Res == 200)
                            {

                                return Ok(4);
                            }
                            else if (Res == 501)
                            {
                                return Ok(5);
                            }
                            else
                            {
                                return Ok(6);
                            }

                        }


                    }

                    return Ok(4);

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
                var entry = unitOfWork.EntryRepository.Get(x=> x.ReceiptID==id).FirstOrDefault();
                if(entry != null)
                {
                    var entryDitails = unitOfWork.EntryDetailRepository.Get(a => a.EntryID == entry.EntryID);
                    if (entry.TransferedToAccounts == true)
                    {
                        accountingHelper.CancelTransferToAccounts(entryDitails.ToList());
                    }
                    unitOfWork.EntryDetailRepository.RemovRange(entryDitails);
                    unitOfWork.EntryRepository.Delete(entry.EntryID);
                }
                
                unitOfWork.ReceiptExchangeRepository.Delete(RecExc);
                var Result = unitOfWork.Save();
                if (Result == 200)
                {

                    return Ok(4);

                }
                else if (Result == 501)
                {
                    return Ok(5);
                }
                else
                {
                    return Ok(6);
                }

            }
            else
                return Ok(1);


        }

        #endregion
    }
}
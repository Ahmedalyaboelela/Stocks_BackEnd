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
        [Route("~/api/ReceiptExchange/GetEntry")]
        public EntryModel GetEntry(int receiptID)
        {
            var Entry = unitOfWork.EntryRepository.Get(x => x.ReceiptID == receiptID).SingleOrDefault();
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

            return entryModel;
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

            model.SettingModel = GetSetting(4);
            #endregion
            var check = unitOfWork.EntryRepository.Get(x => x.ReceiptID == RecExc.ReceiptID).SingleOrDefault();
            if (check != null)
            {
                model.EntryModel = GetEntry(model.ReceiptID);
            }
            return model;
        }

        [HttpGet]
        [Route("~/api/ReceiptExchange/FirstOpen/{type}")]
        public IActionResult FirstOpen(bool type)
        {
            DefaultSettingModel model = new DefaultSettingModel();
            model.ScreenSetting = GetSetting(4);
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
        //[HttpPost]
        //[Route("~/api/ReceiptExchange/Add")]
        //public IActionResult PostItem([FromBody] ReceiptExchangeModel recExcModel)
        //{

        //    if (ModelState.IsValid)
        //    {
        //        var Check = unitOfWork.ReceiptExchangeRepository.Get();
        //        if (Check.Any(m => m.Code == recExcModel.Code))
        //        {

        //            return Ok("كود امر بيع مكرر");
        //        }
        //        else
        //        {

        //            var receipt = _mapper.Map<ReceiptExchange>(recExcModel);


        //            var Details = recExcModel.RecExcDetails;

        //            unitOfWork.ReceiptExchangeRepository.Insert(receipt);

        //            foreach (var item in Details)
        //            {
        //                ReceiptExchangeDetailModel receiptExchangeDetailModel = new ReceiptExchangeDetailModel();
        //                receiptExchangeDetailModel.ReceiptID = receipt.ReceiptID;
        //                receiptExchangeDetailModel.AccountID = item.AccountID;
        //                receiptExchangeDetailModel.ChiqueNumber = item.ChiqueNumber;
        //                receiptExchangeDetailModel.Credit = item.Credit;
        //                receiptExchangeDetailModel.Debit = item.Debit;
        //                receiptExchangeDetailModel.Type = item.Type;
        //                var details = _mapper.Map<ReceiptExchangeDetail>(receiptExchangeDetailModel);
        //                unitOfWork.ReceiptExchangeDetailRepository.Insert(details);

        //            }


        //            //==================================================لا تولد قيد ===================================
        //            if (recExcModel.SettingModel.DoNotGenerateEntry == true)
        //            {
        //                unitOfWork.Save();

        //                return Ok(recExcModel);
        //            }

        //            //===============================================================توليد قيد مع ترحيل تلقائي============================



        //            else if (recExcModel.SettingModel.AutoGenerateEntry == true)
        //            {
        //                var lastEntry = unitOfWork.EntryRepository.Last();
        //                var EntryMODEL = EntriesHelper.InsertCalculatedEntries(null, null, recExcModel, null, lastEntry);
        //                var Entry = _mapper.Map<Entry>(EntryMODEL);
        //                Entry.ReceiptID = receipt.ReceiptID;

        //                var DetailEnt = EntryMODEL.EntryDetailModel;

        //                if (recExcModel.SettingModel.TransferToAccounts == true)
        //                {
        //                    Entry.TransferedToAccounts = true;
        //                    unitOfWork.EntryRepository.Insert(Entry);
        //                    foreach (var item in DetailEnt)
        //                    {
        //                        item.EntryID = Entry.EntryID;
        //                        item.EntryDetailID = 0;
        //                        var details = _mapper.Map<EntryDetail>(item);

        //                        unitOfWork.EntryDetailRepository.Insert(details);

        //                    }
        //                    accountingHelper.TransferToAccounts(DetailEnt.Select(x => new EntryDetail
        //                    {


        //                        EntryDetailID = x.EntryDetailID,
        //                        AccountID = x.AccountID,
        //                        Credit = x.Credit,
        //                        Debit = x.Debit,
        //                        EntryID = x.EntryID


        //                    }).ToList());
        //                }



        //            }
        //            //================================توليد قيد مع عدم الترحيل======================================
        //            if (recExcModel.SettingModel.GenerateEntry == true)

        //            {
        //                var lastEntry = unitOfWork.EntryRepository.Last();
        //                var EntryMODEL = EntriesHelper.InsertCalculatedEntries(null, null, recExcModel, null, lastEntry);
        //                var Entry = _mapper.Map<Entry>(EntryMODEL);
        //                Entry.ReceiptID = receipt.ReceiptID;

        //                var DetailEnt = EntryMODEL.EntryDetailModel;
        //                Entry.TransferedToAccounts = false;
        //                unitOfWork.EntryRepository.Insert(Entry);
        //                foreach (var item in DetailEnt)
        //                {
        //                    item.EntryID = Entry.EntryID;
        //                    item.EntryDetailID = 0;
        //                    var details = _mapper.Map<EntryDetail>(item);

        //                    unitOfWork.EntryDetailRepository.Insert(details);

        //                }
        //            }


        //            unitOfWork.Save();



        //            return Ok(recExcModel);



        //        }
        //    }
        //    else
        //    {
        //        return Ok(3);
        //    }
        //}

        #endregion


        #region Update Methods
        //[HttpPut]
        //[Route("~/api/ReceiptExchange/Update/{id}/{type}")]
        //public IActionResult Update(int id, bool type, [FromBody] ReceiptExchangeModel receiptExchangeModel)
        //{
        //    if (id != receiptExchangeModel.ReceiptID)
        //    {

        //        return Ok(1);
        //    }

        //    if (ModelState.IsValid)
        //    {

        //        var Check = unitOfWork.ReceiptExchangeRepository.Get(NoTrack: "NoTrack");

        //        var ReceiptExchange = _mapper.Map<ReceiptExchange>(receiptExchangeModel);
        //        var NewdDetails = receiptExchangeModel.RecExcDetails;
        //        var Newdetails = _mapper.Map<IEnumerable<ReceiptExchangeDetail>>(NewdDetails);
        //        var OldDetails = unitOfWork.ReceiptExchangeDetailRepository.Get(filter: m => m.ReceiptExchangeID == ReceiptExchange.ReceiptID);
        //        var EntryCheck = unitOfWork.EntryRepository.Get(x => x.ReceiptID == ReceiptExchange.ReceiptID).SingleOrDefault();
        //        if (EntryCheck != null)
        //        {

        //            var Entry = unitOfWork.EntryRepository.Get(filter: x => x.ReceiptID == ReceiptExchange.ReceiptID).SingleOrDefault();
        //            var OldEntryDetails = unitOfWork.EntryDetailRepository.Get(filter: a => a.EntryID == Entry.EntryID);
        //            if (Entry.TransferedToAccounts == true)
        //            {
        //                accountingHelper.CancelTransferToAccounts(OldEntryDetails.ToList());
        //            }
        //            unitOfWork.EntryDetailRepository.RemovRange(OldEntryDetails);

        //            if (Check.Any(m => m.Code != ReceiptExchange.Code))
        //            {
        //                unitOfWork.ReceiptExchangeRepository.Update(ReceiptExchange);
        //                if (OldDetails != null)
        //                {
        //                    unitOfWork.ReceiptExchangeDetailRepository.RemovRange(OldDetails);
        //                    unitOfWork.Save();
        //                }


        //                if (Newdetails != null)
        //                {
        //                    foreach (var item in Newdetails)
        //                    {
        //                        item.ReceiptID = ReceiptExchange.ReceiptID;
        //                        item.ReceiptExchangeID = 0;
        //                        var details = _mapper.Map<ReceiptExchangeDetail>(item);

        //                        unitOfWork.ReceiptExchangeDetailRepository.Insert(details);

        //                    }
        //                }


        //                //==================================================لا تولد قيد ===================================
        //                if (receiptExchangeModel.SettingModel.DoNotGenerateEntry == true)
        //                {
        //                    unitOfWork.EntryRepository.Delete(Entry.EntryID);
        //                    unitOfWork.Save();

        //                    return Ok(receiptExchangeModel);
        //                }
        //                //===================================توليد قيد مع ترحيل تلقائي===================================
        //                if (receiptExchangeModel.SettingModel.AutoGenerateEntry == true)
        //                {
        //                    var EntryDitails = EntriesHelper.UpdateCalculateEntries(Entry.EntryID, null, null, receiptExchangeModel, null);

        //                    if (receiptExchangeModel.SettingModel.TransferToAccounts == true)
        //                    {
        //                        Entry.TransferedToAccounts = true;
        //                        unitOfWork.EntryRepository.Update(Entry);
        //                        foreach (var item in EntryDitails)
        //                        {
        //                            item.EntryID = Entry.EntryID;
        //                            item.EntryDetailID = 0;
        //                            var details = _mapper.Map<EntryDetail>(item);

        //                            unitOfWork.EntryDetailRepository.Insert(details);

        //                        }
        //                        accountingHelper.TransferToAccounts(EntryDitails.Select(x => new EntryDetail
        //                        {


        //                            EntryDetailID = x.EntryDetailID,
        //                            AccountID = x.AccountID,
        //                            Credit = x.Credit,
        //                            Debit = x.Debit,
        //                            EntryID = x.EntryID


        //                        }).ToList());
        //                    }

        //                }
        //                //===================================توليد قيد مع  عدم ترحيل===================================
        //                if (receiptExchangeModel.SettingModel.GenerateEntry == true)

        //                {
        //                    var EntryDitails = EntriesHelper.UpdateCalculateEntries(Entry.EntryID, null, null, receiptExchangeModel, null);
        //                    Entry.TransferedToAccounts = false;
        //                    unitOfWork.EntryRepository.Update(Entry);
        //                    foreach (var item in EntryDitails)
        //                    {
        //                        item.EntryID = Entry.EntryID;
        //                        item.EntryDetailID = 0;
        //                        var details = _mapper.Map<EntryDetail>(item);

        //                        unitOfWork.EntryDetailRepository.Insert(details);

        //                    }
        //                }
        //                unitOfWork.Save();



        //                return Ok(receiptExchangeModel);


        //            }


        //            //==========================================Second Case OF Code Of Purchase=======================================

        //            else
        //            {
        //                if (Check.Any(m => m.Code == ReceiptExchange.Code && m.ReceiptID == id))
        //                {
        //                    unitOfWork.ReceiptExchangeRepository.Update(ReceiptExchange);
        //                    if (OldDetails != null)
        //                    {
        //                        unitOfWork.ReceiptExchangeDetailRepository.RemovRange(OldDetails);
        //                        unitOfWork.Save();
        //                    }


        //                    if (Newdetails != null)
        //                    {
        //                        foreach (var item in Newdetails)
        //                        {
        //                            item.ReceiptID = ReceiptExchange.ReceiptID;
        //                            item.ReceiptExchangeID = 0;
        //                            var details = _mapper.Map<ReceiptExchangeDetail>(item);

        //                            unitOfWork.ReceiptExchangeDetailRepository.Insert(details);

        //                        }
        //                    }


        //                    //==================================================لا تولد قيد ===================================
        //                    if (receiptExchangeModel.SettingModel.DoNotGenerateEntry == true)
        //                    {
        //                        unitOfWork.EntryRepository.Delete(Entry.EntryID);
        //                        unitOfWork.Save();

        //                        return Ok(receiptExchangeModel);
        //                    }
        //                    //===================================توليد قيد مع ترحيل تلقائي===================================
        //                    if (receiptExchangeModel.SettingModel.AutoGenerateEntry == true)
        //                    {
        //                        var EntryDitails = EntriesHelper.UpdateCalculateEntries(Entry.EntryID, null, null, receiptExchangeModel, null);

        //                        if (receiptExchangeModel.SettingModel.TransferToAccounts == true)
        //                        {
        //                            Entry.TransferedToAccounts = true;
        //                            unitOfWork.EntryRepository.Update(Entry);
        //                            foreach (var item in EntryDitails)
        //                            {
        //                                item.EntryID = Entry.EntryID;
        //                                item.EntryDetailID = 0;
        //                                var details = _mapper.Map<EntryDetail>(item);

        //                                unitOfWork.EntryDetailRepository.Insert(details);

        //                            }
        //                            accountingHelper.TransferToAccounts(EntryDitails.Select(x => new EntryDetail
        //                            {


        //                                EntryDetailID = x.EntryDetailID,
        //                                AccountID = x.AccountID,
        //                                Credit = x.Credit,
        //                                Debit = x.Debit,
        //                                EntryID = x.EntryID


        //                            }).ToList());
        //                        }

        //                    }
        //                    //===================================توليد قيد مع  عدم ترحيل===================================
        //                    if (receiptExchangeModel.SettingModel.GenerateEntry == true)

        //                    {
        //                        var EntryDitails = EntriesHelper.UpdateCalculateEntries(Entry.EntryID, null, null, receiptExchangeModel, null);
        //                        Entry.TransferedToAccounts = false;
        //                        unitOfWork.EntryRepository.Update(Entry);
        //                        foreach (var item in EntryDitails)
        //                        {
        //                            item.EntryID = Entry.EntryID;
        //                            item.EntryDetailID = 0;
        //                            var details = _mapper.Map<EntryDetail>(item);

        //                            unitOfWork.EntryDetailRepository.Insert(details);

        //                        }
        //                    }
        //                    unitOfWork.Save();



        //                    return Ok(receiptExchangeModel);

        //                }


        //            }
        //            return Ok(receiptExchangeModel);
        //        }

        //        // now We Will Create new Entry As Insert


        //        else
        //        {
        //            if (Check.Any(m => m.Code != ReceiptExchange.Code))
        //            {
        //                unitOfWork.ReceiptExchangeRepository.Update(ReceiptExchange);
        //                if (OldDetails != null)
        //                {
        //                    unitOfWork.ReceiptExchangeDetailRepository.RemovRange(OldDetails);
        //                    unitOfWork.Save();
        //                }


        //                if (Newdetails != null)
        //                {
        //                    foreach (var item in Newdetails)
        //                    {
        //                        item.ReceiptID = ReceiptExchange.ReceiptID;
        //                        item.ReceiptExchangeID = 0;
        //                        var details = _mapper.Map<ReceiptExchangeDetail>(item);

        //                        unitOfWork.ReceiptExchangeDetailRepository.Insert(details);

        //                    }
        //                }


        //                //==================================================لا تولد قيد ===================================
        //                if (receiptExchangeModel.SettingModel.DoNotGenerateEntry == true)
        //                {

        //                    unitOfWork.Save();

        //                    return Ok(receiptExchangeModel);
        //                }
        //                //===============================================================توليد قيد مع ترحيل تلقائي============================



        //                else if (receiptExchangeModel.SettingModel.AutoGenerateEntry == true)
        //                {
        //                    var lastEntry = unitOfWork.EntryRepository.Last();
        //                    var EntryMODEL = EntriesHelper.InsertCalculatedEntries(null, null, receiptExchangeModel, null, lastEntry);
        //                    var Entry = _mapper.Map<Entry>(EntryMODEL);
        //                    Entry.ReceiptID = ReceiptExchange.ReceiptID;

        //                    var DetailEnt = EntryMODEL.EntryDetailModel;

        //                    if (receiptExchangeModel.SettingModel.TransferToAccounts == true)
        //                    {
        //                        Entry.TransferedToAccounts = true;
        //                        unitOfWork.EntryRepository.Insert(Entry);
        //                        foreach (var item in DetailEnt)
        //                        {
        //                            item.EntryID = Entry.EntryID;
        //                            item.EntryDetailID = 0;
        //                            var details = _mapper.Map<EntryDetail>(item);

        //                            unitOfWork.EntryDetailRepository.Insert(details);

        //                        }
        //                        accountingHelper.TransferToAccounts(DetailEnt.Select(x => new EntryDetail
        //                        {


        //                            EntryDetailID = x.EntryDetailID,
        //                            AccountID = x.AccountID,
        //                            Credit = x.Credit,
        //                            Debit = x.Debit,
        //                            EntryID = x.EntryID


        //                        }).ToList());
        //                    }
        //                }
        //                //================================توليد قيد مع عدم الترحيل====================================== 
        //                if (receiptExchangeModel.SettingModel.GenerateEntry == true)

        //                {
        //                    var lastEntry = unitOfWork.EntryRepository.Last();
        //                    var EntryMODEL = EntriesHelper.InsertCalculatedEntries(null, null, receiptExchangeModel, null, lastEntry);
        //                    var Entry = _mapper.Map<Entry>(EntryMODEL);
        //                    Entry.ReceiptID = ReceiptExchange.ReceiptID;

        //                    var DetailEnt = EntryMODEL.EntryDetailModel;
        //                    Entry.TransferedToAccounts = false;
        //                    unitOfWork.EntryRepository.Insert(Entry);
        //                    foreach (var item in DetailEnt)
        //                    {
        //                        item.EntryID = Entry.EntryID;
        //                        item.EntryDetailID = 0;
        //                        var details = _mapper.Map<EntryDetail>(item);

        //                        unitOfWork.EntryDetailRepository.Insert(details);

        //                    }
        //                }


        //                unitOfWork.Save();



        //                return Ok(receiptExchangeModel);


        //            }


        //            //==========================================Second Case OF Code Of Purchase=======================================

        //            else
        //            {
        //                if (Check.Any(m => m.Code == ReceiptExchange.Code && m.ReceiptID == id))
        //                {
        //                    unitOfWork.ReceiptExchangeRepository.Update(ReceiptExchange);
        //                    if (OldDetails != null)
        //                    {
        //                        unitOfWork.ReceiptExchangeDetailRepository.RemovRange(OldDetails);
        //                        unitOfWork.Save();
        //                    }


        //                    if (Newdetails != null)
        //                    {
        //                        foreach (var item in Newdetails)
        //                        {
        //                            item.ReceiptID = ReceiptExchange.ReceiptID;
        //                            item.ReceiptExchangeID = 0;
        //                            var details = _mapper.Map<ReceiptExchangeDetail>(item);

        //                            unitOfWork.ReceiptExchangeDetailRepository.Insert(details);

        //                        }
        //                    }


        //                    //==================================================لا تولد قيد ===================================
        //                    if (receiptExchangeModel.SettingModel.DoNotGenerateEntry == true)
        //                    {

        //                        unitOfWork.Save();

        //                        return Ok(receiptExchangeModel);
        //                    }
        //                    //===============================================================توليد قيد مع ترحيل تلقائي============================



        //                    else if (receiptExchangeModel.SettingModel.AutoGenerateEntry == true)
        //                    {
        //                        var lastEntry = unitOfWork.EntryRepository.Last();
        //                        var EntryMODEL = EntriesHelper.InsertCalculatedEntries(null, null, receiptExchangeModel, null, lastEntry);
        //                        var Entry = _mapper.Map<Entry>(EntryMODEL);
        //                        Entry.ReceiptID = ReceiptExchange.ReceiptID;

        //                        var DetailEnt = EntryMODEL.EntryDetailModel;

        //                        if (receiptExchangeModel.SettingModel.TransferToAccounts == true)
        //                        {
        //                            Entry.TransferedToAccounts = true;
        //                            unitOfWork.EntryRepository.Insert(Entry);
        //                            foreach (var item in DetailEnt)
        //                            {
        //                                item.EntryID = Entry.EntryID;
        //                                item.EntryDetailID = 0;
        //                                var details = _mapper.Map<EntryDetail>(item);

        //                                unitOfWork.EntryDetailRepository.Insert(details);

        //                            }
        //                            accountingHelper.TransferToAccounts(DetailEnt.Select(x => new EntryDetail
        //                            {


        //                                EntryDetailID = x.EntryDetailID,
        //                                AccountID = x.AccountID,
        //                                Credit = x.Credit,
        //                                Debit = x.Debit,
        //                                EntryID = x.EntryID


        //                            }).ToList());
        //                        }
        //                    }
        //                    //================================توليد قيد مع عدم الترحيل====================================== 
        //                    if (receiptExchangeModel.SettingModel.GenerateEntry == true)

        //                    {
        //                        var lastEntry = unitOfWork.EntryRepository.Last();
        //                        var EntryMODEL = EntriesHelper.InsertCalculatedEntries(null, null, receiptExchangeModel, null, lastEntry);
        //                        var Entry = _mapper.Map<Entry>(EntryMODEL);
        //                        Entry.ReceiptID = ReceiptExchange.ReceiptID;

        //                        var DetailEnt = EntryMODEL.EntryDetailModel;
        //                        Entry.TransferedToAccounts = false;
        //                        unitOfWork.EntryRepository.Insert(Entry);
        //                        foreach (var item in DetailEnt)
        //                        {
        //                            item.EntryID = Entry.EntryID;
        //                            item.EntryDetailID = 0;
        //                            var details = _mapper.Map<EntryDetail>(item);

        //                            unitOfWork.EntryDetailRepository.Insert(details);

        //                        }
        //                    }


        //                    unitOfWork.Save();



        //                    return Ok(receiptExchangeModel);
        //                }


        //            }
        //            return Ok(receiptExchangeModel);
        //        }
        //    }
        //    else
        //    {
        //        return Ok(3);
        //    }
        //}

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
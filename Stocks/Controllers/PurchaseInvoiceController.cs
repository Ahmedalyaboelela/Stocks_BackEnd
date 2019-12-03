using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BAL.Model;
using BAL.Repositories;
using DAL.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DAL.Entities;
using BAL.Helper;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Authorization;

namespace Stocks.Controllers
{
    [Authorize(Roles = "SuperAdmin,Admin,Employee")]
    [Route("api/[controller]")]
    [ApiController]
    public class PurchaseInvoiceController : ControllerBase
    {
        private UnitOfWork unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAccountingHelper accountingHelper;
        private readonly IStocksHelper _stocksHelper;
        private LoggerHistory loggerHistory;
        public PurchaseInvoiceController(StocksContext context, IMapper mapper, IStocksHelper stocksHelper)
        {
            this.unitOfWork = new UnitOfWork(context);
            this._mapper = mapper;
            accountingHelper = new AccountingHelper(context, mapper);
            _stocksHelper = stocksHelper;
            loggerHistory = new LoggerHistory(context, mapper);
        }


        [HttpGet] //القيد 
        [Route("~/api/PurchaseInvoice/GetEntry")]
        public EntryModel GetEntryPurchaseInvoiceModel( int PurchaseInvoiceID)
        {
            var Entry = unitOfWork.EntryRepository.Get(x => x.PurchaseInvoiceID == PurchaseInvoiceID).SingleOrDefault();

            EntryModel entryModel = new EntryModel();

            if (Entry != null)
            {
                var EntryDetails = unitOfWork.EntryDetailRepository.Get(filter: a => a.EntryID == Entry.EntryID);
                entryModel.EntryID = Entry.EntryID;
                entryModel.Code = Entry.Code;
                entryModel.Date = Entry.Date.Value.ToString("d/M/yyyy");
                entryModel.DateHijri = DateHelper.GetHijriDate(Entry.Date);
                entryModel.NoticeID = Entry.NoticeID;
                entryModel.PurchaseInvoiceID = Entry.PurchaseInvoiceID;
                entryModel.ReceiptID = Entry.ReceiptID;
                entryModel.PurchaseInvoiceID = Entry.PurchaseInvoiceID;
                entryModel.EntryDetailModel = EntryDetails.Select(m => new EntryDetailModel
                {
                    AccCode = m.Account.Code,
                    AccNameAR = m.Account.NameAR,
                    AccNameEN = m.Account.NameEN,
                    AccountID = m.AccountID,
                    ParentAccountID = m.Account.AccoutnParentID,
                    ParentAccCode = unitOfWork.AccountRepository.Get(filter: a => a.AccountID == m.Account.AccoutnParentID).Select(s => s.Code).FirstOrDefault(),
                    ParentAccNameAR = unitOfWork.AccountRepository.Get(filter: a => a.AccountID == m.Account.AccoutnParentID).Select(s => s.NameAR).FirstOrDefault(),
                    Credit = m.Credit,
                    Debit = m.Debit,
                    EntryDetailID = m.EntryDetailID,
                    EntryID = m.EntryID,


                });
                entryModel.TransferedToAccounts = Entry.TransferedToAccounts;

            }


            return entryModel;
        }

        [Route("~/api/PurchaseInvoice/GetSettingAccounts/{id}")]
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
                ParentAccountID = a.Account.AccoutnParentID,
                ParentAccCode = unitOfWork.AccountRepository.GetEntity(s => s.AccountID == a.Account.AccoutnParentID).Code,
                ParentAccNameAR = unitOfWork.AccountRepository.GetEntity(s => s.AccountID == a.Account.AccoutnParentID).NameAR,
                ParentAccNameEN = unitOfWork.AccountRepository.GetEntity(s => s.AccountID == a.Account.AccoutnParentID).NameEN,


            });



            return setAccounts;


        }



        [Route("~/api/PurchaseInvoice/GetSetting")]
        public SettingModel GetSetting( int flag)
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


        [HttpGet]
        [Route("~/api/PurchaseInvoice/FirstOpen")]
        public IActionResult FirstOpen()
        {
            PurchaseInvoiceModel model = new PurchaseInvoiceModel();
            var count = unitOfWork.PurchaseInvoiceRepository.Count();
            if (count > 0)
            {
                model.LastCode = unitOfWork.PurchaseInvoiceRepository.Last().Code;
                model.Count = count;
            }
            else
                model.Count = 0;
            model.SettingModel = GetSetting(2);


            return Ok(model);
        }




        [HttpPost] // يولد قيد يدوي مع ترحيل تلقائي
        [Route("~/api/PurchaseInvoice/GenerateconstraintManual")]
        public IActionResult GenerateconstraintManual([FromBody]PurchaseInvoiceModel purchaseInvoiceModel)
        { 
            if (purchaseInvoiceModel.SettingModel.GenerateEntry==true)
            {
                var lastEntry = unitOfWork.EntryRepository.Last();
                int portofolioaccount = unitOfWork.PortfolioAccountRepository.Get(filter: m => m.PortfolioID == purchaseInvoiceModel.PortfolioID && m.Type == true).Select(m => m.AccountID).SingleOrDefault();


                var EntryMODEL = EntriesHelper.InsertCalculatedEntries(portofolioaccount,null, purchaseInvoiceModel, null, null, lastEntry);
                var Entry = _mapper.Map<Entry>(EntryMODEL);


                var DetailEnt = EntryMODEL.EntryDetailModel;

                if (purchaseInvoiceModel.SettingModel.TransferToAccounts == true)
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
                //purchaseInvoiceModel.EntryModel = GetEntryPurchaseInvoiceModel(purchaseInvoiceModel.PurchaseInvoiceID);

            }

            return Ok(purchaseInvoiceModel);

        }



        [HttpPost]// ترحيل يدوي للقيد اليدوي والتلقائي
        [Route("~/api/PurchaseInvoice/Manualmigration")]
        public IActionResult Manualmigration([FromBody]EntryModel entryModel)
        {
            //var Entry = unitOfWork.EntryRepository.GetByID(EntryMODEL.EntryID);
            //Entry.TransferedToAccounts = true;
            //unitOfWork.EntryRepository.Update(Entry);
            var Details = entryModel.EntryDetailModel;
            //foreach (var item in Details)
            //{
            //    var detail = _mapper.Map<EntryDetail>(item);

            //    unitOfWork.EntryDetailRepository.Update(detail);
            //}

            accountingHelper.TransferToAccounts(Details.Select(x => new EntryDetail
            {
                EntryDetailID = x.EntryDetailID,
                AccountID = x.AccountID,
                Credit = x.Credit,
                Debit = x.Debit,
                EntryID = x.EntryID


            }).ToList());

            var Entry = unitOfWork.EntryRepository.Get(filter: x => x.EntryID == entryModel.EntryID).SingleOrDefault();
            Entry.TransferedToAccounts = true;
            unitOfWork.EntryRepository.Update(Entry);


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

            //return Ok(GetEntry(Entry));

        }



        [HttpGet]
        [Route("~/api/PurchaseInvoice/GetLastPurchaseInvoice")]
        public IActionResult GetLastPurchaseInvoice()
        {
            var purchase = unitOfWork.PurchaseInvoiceRepository.Last();


            var model = _mapper.Map<PurchaseInvoiceModel>(purchase);
            model.PortfolioAccount = unitOfWork.PortfolioAccountRepository.GetEntity(x=> x.PortfolioID==purchase.PurchaseOrder.PortfolioID && x.Type==true).AccountID;
            if (model == null)
            {
                return Ok(0);

            }

            #region portfolio data
            var portfolio = unitOfWork.PortfolioAccountRepository.GetEntity(x => x.PortfolioID == purchase.PurchaseOrder.PortfolioID && x.Type == true);
            if (portfolio != null)
            {

                model.PortfolioAccount = portfolio.AccountID;

                // portfolio data
                model.PortfolioCode = portfolio.Portfolio.Code;
                model.PortfolioNameAR = portfolio.Portfolio.NameAR;
                model.PortfolioNameEN = portfolio.Portfolio.NameEN;
                model.PortfolioID = portfolio.Portfolio.PortfolioID;
            }

            #endregion

            // employee data
            #region employee part
            var employee = unitOfWork.EmployeeRepository.GetEntity(x => x.EmployeeID == purchase.EmployeeID);
            if (employee != null)
            {
                model.EmpCode = employee.Code;
                model.EmpNameAR = employee.NameAR;
                model.EmpNameEN = employee.NameEN;
                model.EmployeeID = employee.EmployeeID;

            }
            #endregion

            // date part
            #region Date part
            if (purchase.Date != null)
            {

                model.PurchaseDate = purchase.Date.Value.ToString("d/M/yyyy");
                model.PurchaseDateHijri = DateHelper.GetHijriDate(purchase.Date);
            }

            #endregion

            model.Count = unitOfWork.PurchaseInvoiceRepository.Count();

            if (model.Count == 0)
            {
                return Ok(model);
            }

          


            var Details = unitOfWork.PurchaseInvoiceDetailRepository.Get(filter: a => a.PurchaseInvoiceID == purchase.PurchaseInvoiceID)
                            .Select(m => new PurchaseInvoiceDetailModel
                            {

                                PurchaseInvoiceID = m.PurchaseInvoiceID,
                                BankCommission = m.BankCommission,
                                NetAmmount = m.NetAmmount,
                                PurchaseInvoiceDetailID = m.PurchaseInvoiceDetailID,
                                BankCommissionRate = m.BankCommissionRate,
                                PurchasePrice = m.PurchasePrice,
                                PurchaseValue = m.PurchaseValue,
                                StockCount = m.StockCount,
                                TaxOnCommission = m.TaxOnCommission,
                                TaxRateOnCommission = m.TaxRateOnCommission,
                                PartnerID = m.PartnerID,
                                PartnerCode = m.Partner.Code,
                                PartnerNameAR = m.Partner.NameAR,
                                PartnerNameEN = m.Partner.NameEN
                            });
            if (Details != null)
            {


                model.DetailsModels = Details;
            }


           model.SettingModel = GetSetting(2);

            var check = unitOfWork.EntryRepository.Get(x=> x.PurchaseInvoiceID== purchase.PurchaseInvoiceID).SingleOrDefault();
            if (check !=null)
            {
                model.EntryModel = GetEntryPurchaseInvoiceModel(purchase.PurchaseInvoiceID);
            }
          
           
            

            return Ok(model);

        }



        [HttpGet]
        [Route("~/api/PurchaseInvoice/GetPurchaseInvoicebyID/{id}")]
        public IActionResult GetPurchaseInvoiceByID(int id)
        {
            if (id > 0)
            {


                var purchase = unitOfWork.PurchaseInvoiceRepository.GetByID(id);

                var model = _mapper.Map<PurchaseInvoiceModel>(purchase);
                model.PortfolioAccount = unitOfWork.PortfolioAccountRepository.GetEntity(x => x.PortfolioID == purchase.PurchaseOrder.PortfolioID && x.Type == true).AccountID;
                if (model == null)
                {
                    return Ok(model);

                }


                #region portfolio data
                var portfolio = unitOfWork.PortfolioAccountRepository.GetEntity(x => x.PortfolioID == purchase.PurchaseOrder.PortfolioID && x.Type == true);
                if (portfolio != null)
                {

                    model.PortfolioAccount = portfolio.AccountID;

                    // portfolio data
                    model.PortfolioCode = portfolio.Portfolio.Code;
                    model.PortfolioNameAR = portfolio.Portfolio.NameAR;
                    model.PortfolioNameEN = portfolio.Portfolio.NameEN;
                    model.PortfolioID = portfolio.Portfolio.PortfolioID;
                }

                #endregion

                // employee data
                #region employee part
                var employee = unitOfWork.EmployeeRepository.GetEntity(x => x.EmployeeID == purchase.EmployeeID);
                if (employee != null)
                {
                    model.EmpCode = employee.Code;
                    model.EmpNameAR = employee.NameAR;
                    model.EmpNameEN = employee.NameEN;
                    model.EmployeeID = employee.EmployeeID;

                }
                #endregion

                // date part
                #region Date part
                if (purchase.Date != null)
                {

                    model.PurchaseDate = purchase.Date.Value.ToString("d/M/yyyy");
                    model.PurchaseDateHijri = DateHelper.GetHijriDate(purchase.Date);
                }

                #endregion


                model.Count = unitOfWork.PurchaseInvoiceRepository.Count();

                if (model.Count == 0)
                {
                    return Ok(model);
                }



                var Details = unitOfWork.PurchaseInvoiceDetailRepository.Get(filter: a => a.PurchaseInvoiceID == purchase.PurchaseInvoiceID)
                                .Select(m => new PurchaseInvoiceDetailModel
                                {

                                    PurchaseInvoiceID = m.PurchaseInvoiceID,
                                    BankCommission = m.BankCommission,
                                    NetAmmount = m.NetAmmount,
                                    PurchaseInvoiceDetailID = m.PurchaseInvoiceDetailID,
                                    BankCommissionRate = m.BankCommissionRate,
                                    PurchasePrice = m.PurchasePrice,
                                    PurchaseValue = m.PurchaseValue,
                                    StockCount = m.StockCount,
                                    TaxOnCommission = m.TaxOnCommission,
                                    TaxRateOnCommission = m.TaxRateOnCommission,
                                    PartnerID = m.PartnerID,
                                    PartnerCode = m.Partner.Code,
                                    PartnerNameAR = m.Partner.NameAR,
                                    PartnerNameEN = m.Partner.NameEN
                                });
                if (Details != null)
                {


                    model.DetailsModels = Details;
                }


                model.SettingModel = GetSetting(2);

                var check = unitOfWork.EntryRepository.Get(x => x.PurchaseInvoiceID == purchase.PurchaseInvoiceID).SingleOrDefault();
                if (check != null)
                {
                    model.EntryModel = GetEntryPurchaseInvoiceModel(purchase.PurchaseInvoiceID);
                }
                return Ok(model);
            }
            else
                return Ok(1);

        }


        [HttpGet]
        [Route("~/api/PurchaseInvoice/Paging/{pageNumber}")]
        public IActionResult PaginationPurchaseInvoice(int pageNumber)
        {
            if (pageNumber > 0)
            {

                var UserID = loggerHistory.getUserIdFromRequest(Request);
                var purchase = unitOfWork.PurchaseInvoiceRepository.Get(page: pageNumber).FirstOrDefault();
                var model = _mapper.Map<PurchaseInvoiceModel>(purchase);
                model.PortfolioAccount = unitOfWork.PortfolioAccountRepository.GetEntity(x => x.PortfolioID == purchase.PurchaseOrder.PortfolioID && x.Type == true).AccountID;


                #region portfolio 
                var portfolio = unitOfWork.PortfolioAccountRepository.GetEntity(x => x.PortfolioID == purchase.PurchaseOrder.PortfolioID && x.Type == true);
                if (portfolio != null)
                {

                    model.PortfolioAccount = portfolio.AccountID;

                    // portfolio data
                    model.PortfolioCode = portfolio.Portfolio.Code;
                    model.PortfolioNameAR = portfolio.Portfolio.NameAR;
                    model.PortfolioNameEN = portfolio.Portfolio.NameEN;
                    model.PortfolioID = portfolio.Portfolio.PortfolioID;
                }

                #endregion

                // employee data
                #region employee part
                var employee = unitOfWork.EmployeeRepository.GetEntity(x => x.EmployeeID == purchase.EmployeeID);
                if (employee != null)
                {
                    model.EmpCode = employee.Code;
                    model.EmpNameAR = employee.NameAR;
                    model.EmpNameEN = employee.NameEN;
                    model.EmployeeID = employee.EmployeeID;

                }
                #endregion

                // date part
                #region Date part
                if (purchase.Date != null)
                {

                    model.PurchaseDate = purchase.Date.Value.ToString("d/M/yyyy");
                    model.PurchaseDateHijri = DateHelper.GetHijriDate(purchase.Date);
                }

                #endregion


                if (model == null)
                {
                    return Ok(model);

                }


                model.Count = unitOfWork.PurchaseInvoiceRepository.Count();

                if (model.Count == 0)
                {
                    return Ok(model);
                }



                var Details = unitOfWork.PurchaseInvoiceDetailRepository.Get(filter: a => a.PurchaseInvoiceID == purchase.PurchaseInvoiceID)
                                .Select(m => new PurchaseInvoiceDetailModel
                                {

                                    PurchaseInvoiceID = m.PurchaseInvoiceID,
                                    BankCommission = m.BankCommission,
                                    NetAmmount = m.NetAmmount,
                                    PurchaseInvoiceDetailID = m.PurchaseInvoiceDetailID,
                                    BankCommissionRate = m.BankCommissionRate,
                                    PurchasePrice = m.PurchasePrice,
                                    PurchaseValue = m.PurchaseValue,
                                    StockCount = m.StockCount,
                                    TaxOnCommission = m.TaxOnCommission,
                                    TaxRateOnCommission = m.TaxRateOnCommission,
                                    PartnerID = m.PartnerID,
                                    PartnerCode = m.Partner.Code,
                                    PartnerNameAR = m.Partner.NameAR,
                                    PartnerNameEN = m.Partner.NameEN,
                                    StocksCount = unitOfWork.PortfolioTransactionsRepository.Get(filter: a => a.PartnerID == m.PartnerID).Select(p => p.CurrentStocksCount).FirstOrDefault(),
                                    StocksValue = unitOfWork.PortfolioTransactionsRepository.Get(filter: a => a.PartnerID == m.PartnerID).Select(p => p.CurrentStockValue).FirstOrDefault()

                                });
                if (Details != null)
                {


                    model.DetailsModels = Details;
                }


                model.SettingModel = GetSetting(2);
                var check = unitOfWork.EntryRepository.Get(x => x.PurchaseInvoiceID == purchase.PurchaseInvoiceID).SingleOrDefault();
                if (check != null)
                {
                    model.EntryModel = GetEntryPurchaseInvoiceModel(purchase.PurchaseInvoiceID);
                }
                decimal? oldNetAmmount = 0.0m;
                foreach (var item in Details)
                {
                   oldNetAmmount += item.NetAmmount;

                }
                model.newRialBalance = _stocksHelper.RialBalancUpdate(purchase.PurchaseOrder.PortfolioID, oldNetAmmount);
                return Ok(model);
            }
            else
                return Ok(1);

        }

          

        



        [HttpPost]
        [Route("~/api/PurchaseInvoice/PostPurchaseInvoice")]
        public IActionResult PostPurchaseInvoice([FromBody] PurchaseInvoiceModel purchaseInvoiceModel)
        {
            if (ModelState.IsValid)
            {
                var Check = unitOfWork.PurchaseInvoiceRepository.Get();
                if (Check.Any(m => m.Code == purchaseInvoiceModel.Code))
                {

                    return Ok(2);
                }
                else
                {

                    var purchaseInvoice = _mapper.Map<PurchaseInvoice>(purchaseInvoiceModel);
                    int portofolioaccount = unitOfWork.PortfolioAccountRepository.Get(filter: m => m.PortfolioID == purchaseInvoiceModel.PortfolioID && m.Type == true).Select(m => m.AccountID).SingleOrDefault();

                    #region Warehouse
                
                    // Add Purchase Invoice Stocks Count To Portofolio
                    _stocksHelper.TransferPurchaseToStocks(purchaseInvoiceModel);
                    #endregion

                    var Details = purchaseInvoiceModel.DetailsModels;

                    unitOfWork.PurchaseInvoiceRepository.Insert(purchaseInvoice);
                    if (Details != null && Details.Count() > 0)
                    {
                        foreach (var item in Details)
                        {
                            PurchaseInvoiceDetailModel purchaseInvoiceDetailModel = new PurchaseInvoiceDetailModel();
                            purchaseInvoiceDetailModel.PurchaseInvoiceID = purchaseInvoice.PurchaseInvoiceID;
                            purchaseInvoiceDetailModel.NetAmmount = item.NetAmmount;
                            purchaseInvoiceDetailModel.StockCount = item.StockCount;
                            purchaseInvoiceDetailModel.TaxOnCommission = item.TaxOnCommission;
                            purchaseInvoiceDetailModel.TaxRateOnCommission = item.TaxRateOnCommission;
                            purchaseInvoiceDetailModel.BankCommission = item.BankCommission;
                            purchaseInvoiceDetailModel.BankCommissionRate = item.BankCommissionRate;
                            purchaseInvoiceDetailModel.PurchaseValue = item.PurchaseValue;
                            purchaseInvoiceDetailModel.PurchasePrice = item.PurchasePrice;
                            purchaseInvoiceDetailModel.PartnerID = item.PartnerID;
                            var details = _mapper.Map<PurchaseInvoiceDetail>(purchaseInvoiceDetailModel);
                            unitOfWork.PurchaseInvoiceDetailRepository.Insert(details);

                        }
                    }


                  

                
                    //==================================================لا تولد قيد ===================================
                    if (purchaseInvoiceModel.SettingModel.DoNotGenerateEntry == true)
                    {
                        var Res = unitOfWork.Save();
                        if (Res == 200)
                        {
                            var UserID = loggerHistory.getUserIdFromRequest(Request);

                            loggerHistory.InsertUserLog(UserID, " فاتوره شراء", "اضافه فاتوره شراء", false);
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

                    //===============================================================توليد قيد مع ترحيل تلقائي============================



                    else  if (purchaseInvoiceModel.SettingModel.AutoGenerateEntry==true)
                    {
                        var lastEntry = unitOfWork.EntryRepository.Last();
                        var EntryMODEL = EntriesHelper.InsertCalculatedEntries(portofolioaccount,null, purchaseInvoiceModel, null, null, lastEntry);
                        EntryMODEL.PurchaseInvoiceID = purchaseInvoice.PurchaseInvoiceID;
                        var Entry = _mapper.Map<Entry>(EntryMODEL);
                        var DetailEnt = EntryMODEL.EntryDetailModel;

                        if (purchaseInvoiceModel.SettingModel.TransferToAccounts == true)
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
                                EntryID = x.EntryID,
                                StocksCredit = x.StocksCredit,
                                StocksDebit = x.StocksDebit,

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
                 


                    var Result = unitOfWork.Save();
                    if (Result == 200)
                    {
                        var UserID = loggerHistory.getUserIdFromRequest(Request);

                        loggerHistory.InsertUserLog(UserID, " فاتوره شراء", "اضافه فاتوره شراء", false);
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
            }
            else
            {
                return Ok(3);
            }
        }





        [HttpPut]
        [Route("~/api/PurchaseInvoice/PutPurchaseInvoice/{id}")]
        public IActionResult PutPurchaseInvoice(int id, [FromBody]  PurchaseInvoiceModel purchaseInvoiceModel )
        {
            if (purchaseInvoiceModel != null)
            {
                if (id != purchaseInvoiceModel.PurchaseInvoiceID)
                {

                    return Ok(1);
                }
            }

            if (ModelState.IsValid)
            {
                
                var Check = unitOfWork.PurchaseInvoiceRepository.Get(NoTrack: "NoTrack");
                int portofolioaccount = unitOfWork.PortfolioAccountRepository.Get(filter: m => m.PortfolioID == purchaseInvoiceModel.PortfolioID && m.Type == true)
                    .Select(m => m.AccountID).SingleOrDefault();

                var purchaseInvoice = _mapper.Map<PurchaseInvoice>(purchaseInvoiceModel);
                var NewdDetails = purchaseInvoiceModel.DetailsModels;
                var Newdetails = _mapper.Map<IEnumerable<PurchaseInvoiceDetail>>(NewdDetails);
                var OldDetails = unitOfWork.PurchaseInvoiceDetailRepository.Get(filter: m => m.PurchaseInvoiceID == purchaseInvoice.PurchaseInvoiceID);
                var EntryCheck = unitOfWork.EntryRepository.Get(x => x.PurchaseInvoiceID == purchaseInvoice.PurchaseInvoiceID).SingleOrDefault(); 
               

                #region Warehouse
                //Cancel Purchase Invoice From Portofolio Stocks
                _stocksHelper.CancelPurchaseFromStocks(purchaseInvoiceModel.PortfolioID, OldDetails);
                // Add Purchase Invoice Stocks Count To Portofolio
                _stocksHelper.TransferPurchaseToStocks(purchaseInvoiceModel);
                #endregion
                if (EntryCheck != null)
                {
                    // get old entry
                    var Entry = unitOfWork.EntryRepository.Get(filter: x => x.PurchaseInvoiceID == purchaseInvoice.PurchaseInvoiceID).SingleOrDefault();
                    var OldEntryDetails = unitOfWork.EntryDetailRepository.Get(filter: a => a.EntryID == Entry.EntryID);
                    if (Entry.TransferedToAccounts == true)
                    {
                        accountingHelper.CancelTransferToAccounts(OldEntryDetails.ToList());
                    }
                    // remove old entry data
                    unitOfWork.EntryDetailRepository.RemovRange(OldEntryDetails);
                    unitOfWork.EntryRepository.Delete(Entry.EntryID);

                    if (Check.Any(m => m.Code != purchaseInvoice.Code))
                    {
                        unitOfWork.PurchaseInvoiceRepository.Update(purchaseInvoice);
                        if (OldDetails != null)
                        {
                            unitOfWork.PurchaseInvoiceDetailRepository.RemovRange(OldDetails);
                            //unitOfWork.Save();
                        }


                        if (Newdetails != null)
                        {
                            foreach (var item in Newdetails)
                            {
                                item.PurchaseInvoiceID = purchaseInvoice.PurchaseInvoiceID;
                                item.PurchaseInvoiceDetailID = 0;
                                var details = _mapper.Map<PurchaseInvoiceDetail>(item);

                                unitOfWork.PurchaseInvoiceDetailRepository.Insert(details);

                            }
                        }


                        //==================================================لا تولد قيد ===================================
                        if (purchaseInvoiceModel.SettingModel.DoNotGenerateEntry == true)
                        {
                            //unitOfWork.EntryRepository.Delete(Entry.EntryID);
                            var Res = unitOfWork.Save();
                            if (Res == 200)
                            {
                                var UserID = loggerHistory.getUserIdFromRequest(Request);

                                loggerHistory.InsertUserLog(UserID, " فاتوره شراء", "تعديل فاتوره شراء", false);
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
                        //===================================توليد قيد مع ترحيل تلقائي===================================
                        if (purchaseInvoiceModel.SettingModel.AutoGenerateEntry == true)
                        {
                            //var EntryDitails = EntriesHelper.UpdateCalculateEntries(portofolioaccount,Entry.EntryID, null, purchaseInvoiceModel, null, null);

                            var lastEntry = unitOfWork.EntryRepository.Last();
                            var EntryMODEL = EntriesHelper.InsertCalculatedEntries(portofolioaccount, null, purchaseInvoiceModel, null, null, lastEntry,Entry);
                            EntryMODEL.PurchaseInvoiceID = purchaseInvoice.PurchaseInvoiceID;
                            var NewEntry = _mapper.Map<Entry>(EntryMODEL);
                            var EntryDitails = EntryMODEL.EntryDetailModel;

                            if (purchaseInvoiceModel.SettingModel.TransferToAccounts == true)
                            {
                                NewEntry.TransferedToAccounts = true;
                                unitOfWork.EntryRepository.Insert(NewEntry);
                                foreach (var item in EntryDitails)
                                {
                                    item.EntryID = NewEntry.EntryID;
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
                                    EntryID = x.EntryID,
                                    StocksCredit = x.StocksCredit,
                                    StocksDebit = x.StocksDebit

                                }).ToList());
                            }
                            else
                            {
                                NewEntry.TransferedToAccounts = false;
                                unitOfWork.EntryRepository.Insert(NewEntry);
                                foreach (var item in EntryDitails)
                                {
                                    item.EntryID = NewEntry.EntryID;
                                    item.EntryDetailID = 0;
                                    var details = _mapper.Map<EntryDetail>(item);

                                    unitOfWork.EntryDetailRepository.Insert(details);
                                }
                            }

                        }

                        var Result = unitOfWork.Save();
                        if (Result == 200)
                        {
                            var UserID = loggerHistory.getUserIdFromRequest(Request);

                            loggerHistory.InsertUserLog(UserID, " فاتوره شراء", "تعديل فاتوره شراء", false);
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


                    //==========================================Second Case OF Code Of Purchase=======================================

                    else
                    {
                        if (Check.Any(m => m.Code == purchaseInvoice.Code && m.PurchaseInvoiceID == id))
                        {
                            unitOfWork.PurchaseInvoiceRepository.Update(purchaseInvoice);
                            if (OldDetails != null)
                            {
                                unitOfWork.PurchaseInvoiceDetailRepository.RemovRange(OldDetails);
                         
                            }


                            if (Newdetails != null)
                            {
                                foreach (var item in Newdetails)
                                {
                                    item.PurchaseInvoiceID = purchaseInvoice.PurchaseInvoiceID;
                                    item.PurchaseInvoiceDetailID = 0;
                                    var details = _mapper.Map<PurchaseInvoiceDetail>(item);

                                    unitOfWork.PurchaseInvoiceDetailRepository.Insert(details);

                                }
                            }


                            //==================================================لا تولد قيد ===================================
                            if (purchaseInvoiceModel.SettingModel.DoNotGenerateEntry == true)
                            {
                                //unitOfWork.EntryRepository.Delete(Entry.EntryID);
                                var Res = unitOfWork.Save();
                                if (Res == 200)
                                {
                                    var UserID = loggerHistory.getUserIdFromRequest(Request);

                                    loggerHistory.InsertUserLog(UserID, " فاتوره شراء", "تعديل فاتوره شراء", false);
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
                            //===================================توليد قيد مع ترحيل تلقائي===================================
                            if (purchaseInvoiceModel.SettingModel.AutoGenerateEntry == true)
                            {
                                //var EntryDitails = EntriesHelper.UpdateCalculateEntries(portofolioaccount,Entry.EntryID, null, purchaseInvoiceModel, null, null);
                                var lastEntry = unitOfWork.EntryRepository.Last();
                                var EntryMODEL = EntriesHelper.InsertCalculatedEntries(portofolioaccount, null, purchaseInvoiceModel, null, null, lastEntry,Entry);
                                EntryMODEL.PurchaseInvoiceID = purchaseInvoice.PurchaseInvoiceID;
                                var NewEntry = _mapper.Map<Entry>(EntryMODEL);
                                var EntryDitails = EntryMODEL.EntryDetailModel;

                                if (purchaseInvoiceModel.SettingModel.TransferToAccounts == true)
                                {
                                    NewEntry.TransferedToAccounts = true;
                                    unitOfWork.EntryRepository.Insert(NewEntry);
                                    foreach (var item in EntryDitails)
                                    {
                                        item.EntryID = NewEntry.EntryID;
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
                                        EntryID = x.EntryID,
                                        StocksDebit = x.StocksDebit,
                                        StocksCredit = x.StocksCredit


                                    }).ToList());
                                }
                                
                            }
                        

                            var Result = unitOfWork.Save();
                            if (Result == 200)
                            {
                                var UserID = loggerHistory.getUserIdFromRequest(Request);

                                loggerHistory.InsertUserLog(UserID, " فاتوره شراء", "تعديل فاتوره شراء", false);
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


                    }
                    return Ok(4);
                }

                // now We Will Create new Entry As Insert


                else
                {
                    if (Check.Any(m => m.Code != purchaseInvoice.Code))
                    {
                        unitOfWork.PurchaseInvoiceRepository.Update(purchaseInvoice);
                        if (OldDetails != null && OldDetails.Count()>0)
                        {
                            unitOfWork.PurchaseInvoiceDetailRepository.RemovRange(OldDetails);
                    
                        }


                        if (Newdetails != null && Newdetails.Count()>0)
                        {
                            foreach (var item in Newdetails)
                            {
                                item.PurchaseInvoiceID = purchaseInvoice.PurchaseInvoiceID;
                                item.PurchaseInvoiceDetailID = 0;
                                var details = _mapper.Map<PurchaseInvoiceDetail>(item);

                                unitOfWork.PurchaseInvoiceDetailRepository.Insert(details);

                            }
                        }


                        //==================================================لا تولد قيد ===================================
                        if (purchaseInvoiceModel.SettingModel.DoNotGenerateEntry == true)
                        {


                            var Rest = unitOfWork.Save();
                            if (Rest == 200)
                            {
                                var UserID = loggerHistory.getUserIdFromRequest(Request);

                                loggerHistory.InsertUserLog(UserID, " فاتوره شراء", "تعديل فاتوره شراء", false);
                                return Ok(4);
                            }
                            else if (Rest == 501)
                            {
                                return Ok(5);
                            }
                            else
                            {
                                return Ok(6);
                            }

                        }
                        //===============================================================توليد قيد مع ترحيل تلقائي============================



                        else if (purchaseInvoiceModel.SettingModel.AutoGenerateEntry == true)
                        {
                            var lastEntry = unitOfWork.EntryRepository.Last();
                            var EntryMODEL = EntriesHelper.InsertCalculatedEntries(portofolioaccount,null, purchaseInvoiceModel, null, null, lastEntry);
                            var Entry = _mapper.Map<Entry>(EntryMODEL);
                            Entry.PurchaseInvoiceID = purchaseInvoice.PurchaseInvoiceID;

                            var DetailEnt = EntryMODEL.EntryDetailModel;

                            if (purchaseInvoiceModel.SettingModel.TransferToAccounts == true)
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
                                    EntryID = x.EntryID,
                                    StocksCredit = x.StocksCredit,
                                    StocksDebit = x.StocksDebit


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
                            var UserID = loggerHistory.getUserIdFromRequest(Request);

                            loggerHistory.InsertUserLog(UserID, " فاتوره شراء", "تعديل فاتوره شراء", false);
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
                        if (Check.Any(m => m.Code == purchaseInvoice.Code && m.PurchaseInvoiceID == id))
                        {
                            unitOfWork.PurchaseInvoiceRepository.Update(purchaseInvoice);
                            if (OldDetails != null && OldDetails.Count()>0)
                            {
                                unitOfWork.PurchaseInvoiceDetailRepository.RemovRange(OldDetails);

                            }


                            if (Newdetails != null && Newdetails.Count()>0)
                            {
                                foreach (var item in Newdetails)
                                {
                                    item.PurchaseInvoiceID = purchaseInvoice.PurchaseInvoiceID;
                                    item.PurchaseInvoiceDetailID = 0;
                                    var details = _mapper.Map<PurchaseInvoiceDetail>(item);

                                    unitOfWork.PurchaseInvoiceDetailRepository.Insert(details);

                                }
                            }


                            //==================================================لا تولد قيد ===================================
                            if (purchaseInvoiceModel.SettingModel.DoNotGenerateEntry == true)
                            {


                                var Rest = unitOfWork.Save();
                                if (Rest == 200)
                                {
                                    var UserID = loggerHistory.getUserIdFromRequest(Request);

                                    loggerHistory.InsertUserLog(UserID, " فاتوره شراء", "تعديل فاتوره شراء", false);
                                    return Ok(4);
                                }
                                else if (Rest == 501)
                                {
                                    return Ok(5);
                                }
                                else
                                {
                                    return Ok(6);
                                }

                            }
                            //===============================================================توليد قيد مع ترحيل تلقائي============================



                            else if (purchaseInvoiceModel.SettingModel.AutoGenerateEntry == true)
                            {
                                var lastEntry = unitOfWork.EntryRepository.Last();
                                var EntryMODEL = EntriesHelper.InsertCalculatedEntries(portofolioaccount,null, purchaseInvoiceModel, null, null, lastEntry);
                                purchaseInvoice.PurchaseInvoiceID = purchaseInvoice.PurchaseInvoiceID;
                                var Entry = _mapper.Map<Entry>(EntryMODEL);
                                Entry.PurchaseInvoiceID = purchaseInvoice.PurchaseInvoiceID;

                                var DetailEnt = EntryMODEL.EntryDetailModel;

                                if (purchaseInvoiceModel.SettingModel.TransferToAccounts == true)
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
                                        EntryID = x.EntryID,
                                        StocksDebit = x.StocksDebit,
                                        StocksCredit = x.StocksCredit


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
                                var UserID = loggerHistory.getUserIdFromRequest(Request);

                                loggerHistory.InsertUserLog(UserID, " فاتوره شراء", "تعديل فاتوره شراء", false);

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
                    return Ok(purchaseInvoiceModel);
                }    
            }
            else
            {
                return Ok(3);
            }




        }







        [HttpDelete]
        [Route("~/api/PurchaseInvoice/DeletePurchase/{id}")]
        public IActionResult DeletePurchase(int? id)
        {

            if (id == null )
            {

                return Ok(1);
            }
            var modelPurchase = unitOfWork.PurchaseInvoiceRepository.GetByID(id);
            if (modelPurchase == null)
            {
                return Ok(0);
            }
            var Details = unitOfWork.PurchaseInvoiceDetailRepository.Get(filter: m => m.PurchaseInvoiceID == id);
            // cancle RialBalance  
            decimal? Amounts = 0.0m;
            decimal? newCrited = 0.0m;
            foreach (var item in Details)
            {
                Amounts += item.NetAmmount;
            }
            var account = unitOfWork.PortfolioAccountRepository.GetEntity(filter: x => x.PortfolioID == modelPurchase.PurchaseOrder.PortfolioID).Account;
            newCrited = account.Credit - Amounts;
            account.Credit = newCrited;
            unitOfWork.AccountRepository.Update(account);
            #region
            //Cancel Purchase Invoice From Portofolio Stocks
            _stocksHelper.CancelPurchaseFromStocks(modelPurchase.PurchaseOrder.PortfolioID, Details);
            #endregion
            unitOfWork.PurchaseInvoiceDetailRepository.RemovRange(Details);
            var Entry = unitOfWork.EntryRepository.Get(filter: x => x.PurchaseInvoiceID == id).FirstOrDefault();
            if(Entry != null)
            {
                var EntryDetails = unitOfWork.EntryDetailRepository.Get(filter: a => a.EntryID == Entry.EntryID);
                if (Entry.TransferedToAccounts == true)
                {
                    accountingHelper.CancelTransferToAccounts(EntryDetails.ToList());
                }
                unitOfWork.EntryDetailRepository.RemovRange(EntryDetails);

                unitOfWork.EntryRepository.Delete(Entry.EntryID);
            }
           

            unitOfWork.PurchaseInvoiceRepository.Delete(id);
            var Result = unitOfWork.Save();
            if (Result == 200)
            {
                var UserID = loggerHistory.getUserIdFromRequest(Request);

                loggerHistory.InsertUserLog(UserID, " فاتوره شراء", "حذف فاتوره شراء", false);

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

        [HttpGet]
        [Route("~/api/PurchaseInvoice/GetAllpurchases")]
        public IEnumerable<PurchaseComboList> GetAllpurchases()
        {

            var checks = unitOfWork.PurchaseInvoiceRepository.Get();
            List<PurchaseComboList> sellings = unitOfWork.PurchaseOrderRepository.Get().Select(x => new PurchaseComboList
            {

                Code = x.Code,
                PurchaseOrderID = x.PurchaseOrderID,


            }).ToList();
            List<PurchaseComboList> lists = new List<PurchaseComboList>();
            foreach (var item in sellings)
            {
                if (!checks.Any(m => m.PurchaseOrderID == item.PurchaseOrderID))
                {
                    lists.Add(item);
                }
                if (checks.Any(m => m.PurchaseOrderID == item.PurchaseOrderID &&
                 m.PurchaseOrder.OrderType == true))
                {
                    lists.Add(item);
                }
            }


            return lists;
        }


        [HttpGet]
        [Route("~/api/PurchaseInvoice/GetPortInfo/{id}")]
        public IActionResult GetPortInfo(int id)
        {
            var Info = unitOfWork.PurchaseOrderRepository.GetEntity(filter: x => x.PurchaseOrderID == id);
            PortfolioModel portfolio = new PortfolioModel();
            portfolio.Code = Info.Portfolio.Code;
            portfolio.NameAR = Info.Portfolio.NameAR;
            portfolio.NameEN = Info.Portfolio.NameEN;
            portfolio.PortfolioID = Info.Portfolio.PortfolioID;
            var Debit = unitOfWork.PortfolioAccountRepository.GetEntity(filter: x => x.PortfolioID == Info.PortfolioID).Account.Debit; 
            if (Debit ==null)
            {
                Debit = 0.0m;
            }
            var Credit = unitOfWork.PortfolioAccountRepository.GetEntity(filter: x => x.PortfolioID == Info.PortfolioID).Account.Credit;
            if (Credit == null)
            {
                Credit = 0.0m;
            }
            var DebitOpenningBalance = unitOfWork.PortfolioAccountRepository.GetEntity(filter: x => x.PortfolioID == Info.PortfolioID).Account.DebitOpenningBalance;
            if (DebitOpenningBalance == null)
            {
                DebitOpenningBalance = 0.0m; 
            }
        var CreditOpenningBalance = unitOfWork.PortfolioAccountRepository.GetEntity(filter: x => x.PortfolioID == Info.PortfolioID).Account.CreditOpenningBalance;
            if (CreditOpenningBalance==null)
            {
                CreditOpenningBalance = 0.0m;
            }
            if (DebitOpenningBalance == null && CreditOpenningBalance != null)
            {
                portfolio.TotalRSBalance = -CreditOpenningBalance + (Debit - Credit);

            }
            else if (DebitOpenningBalance != null && CreditOpenningBalance == null)
            {
                portfolio.TotalRSBalance = DebitOpenningBalance + (Debit - Credit);

            }
            else if (DebitOpenningBalance == null && CreditOpenningBalance == null)
            {
                portfolio.TotalRSBalance = Debit - Credit;

            }
            else if (DebitOpenningBalance != null && CreditOpenningBalance != null)
            {
                portfolio.TotalRSBalance = DebitOpenningBalance + (Debit - Credit);

            }








            return Ok(portfolio);
           
        }


    }
}
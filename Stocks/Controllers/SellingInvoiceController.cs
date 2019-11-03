using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BAL.Repositories;
using DAL.Context;
using DAL.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BAL.Model;
using BAL.Helper;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Authorization;

namespace Stocks.Controllers
{
   [Authorize(Roles = "SuperAdmin,Admin,Employee")]
    [Route("api/[controller]")]
    [ApiController]
    public class SellingInvoiceController : Controller
    {

        private UnitOfWork unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAccountingHelper accountingHelper;
        private readonly IStocksHelper _stocksHelper;
        public SellingInvoiceController(StocksContext context, IMapper mapper, IStocksHelper stocksHelper)
        {
            this.unitOfWork = new UnitOfWork(context);
            this._mapper = mapper;
            accountingHelper = new AccountingHelper(context,mapper);
            _stocksHelper = stocksHelper;
        }
        [Route("~/api/SellingInvoice/GetSettingAccounts/{id}")]
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
                ParentAccountID=a.Account.AccoutnParentID,
                ParentAccCode= unitOfWork.AccountRepository.GetEntity(s=>s.AccountID==a.Account.AccoutnParentID).Code,
                ParentAccNameAR = unitOfWork.AccountRepository.GetEntity(s => s.AccountID == a.Account.AccoutnParentID).NameAR,
                ParentAccNameEN = unitOfWork.AccountRepository.GetEntity(s => s.AccountID == a.Account.AccoutnParentID).NameEN,

            });



            return setAccounts;


        }

        //public EntryModel GetEntry(Entry Entry)
        //{
        //    EntryModel entryModel = new EntryModel();

        //    if (Entry != null)
        //    {
        //        var EntryDetails = unitOfWork.EntryDetailRepository.Get(filter: a => a.EntryID == Entry.EntryID);
        //        entryModel.EntryID = Entry.EntryID;
        //        entryModel.Code = Entry.Code;
        //        entryModel.Date = Entry.Date.Value.ToString("d/M/yyyy");
        //        entryModel.DateHijri = DateHelper.GetHijriDate(Entry.Date);
        //        entryModel.NoticeID = Entry.NoticeID;
        //        entryModel.PurchaseOrderID = Entry.PurchaseOrderID;
        //        entryModel.ReceiptID = Entry.ReceiptID;
        //        entryModel.PurchaseOrderID = Entry.PurchaseOrderID;
        //        entryModel.EntryDetailModel = EntryDetails.Select(m => new EntryDetailModel
        //        {
        //            AccCode = m.Account.Code,
        //            AccNameAR = m.Account.NameAR,
        //            AccNameEN = m.Account.NameEN,
        //            AccountID = m.AccountID,
        //            ParentAccountID = m.Account.AccoutnParentID,
        //            ParentAccCode = unitOfWork.AccountRepository.Get(filter: a => a.AccountID == m.Account.AccoutnParentID).Select(s => s.Code).FirstOrDefault(),
        //            ParentAccNameAR = unitOfWork.AccountRepository.Get(filter: a => a.AccountID == m.Account.AccoutnParentID).Select(s => s.NameAR).FirstOrDefault(),
        //            Credit = m.Credit,
        //            Debit = m.Debit,
        //            EntryDetailID = m.EntryDetailID,
        //            EntryID = m.EntryID,


        //        });
        //        entryModel.TransferedToAccounts = Entry.TransferedToAccounts;

        //    }
        //    return entryModel;
        //}

        [HttpGet]//القيد
        [Route("~/api/SellingInvoice/GetEntry/{sellingInvoiceID}")]
        public EntryModel GetEntry(int sellingInvoiceID)
        {
            var Entry = unitOfWork.EntryRepository.Get(x => x.SellingInvoiceID == sellingInvoiceID).SingleOrDefault();
            EntryModel entryModel = new EntryModel();

            if (Entry!=null)
            {
                var EntryDetails = unitOfWork.EntryDetailRepository.Get(filter: a => a.EntryID == Entry.EntryID);
                entryModel.EntryID = Entry.EntryID;
                entryModel.Code = Entry.Code;
                entryModel.Date = Entry.Date.Value.ToString("d/M/yyyy");
                entryModel.DateHijri = DateHelper.GetHijriDate(Entry.Date);
                entryModel.NoticeID = Entry.NoticeID;
                entryModel.PurchaseInvoiceID = Entry.PurchaseInvoiceID;
                entryModel.ReceiptID = Entry.ReceiptID;
                entryModel.SellingInvoiceID = Entry.SellingInvoiceID;
                entryModel.EntryDetailModel = EntryDetails.Select(m => new EntryDetailModel
                {
                    AccCode = m.Account.Code,
                    AccNameAR = m.Account.NameAR,
                    AccNameEN = m.Account.NameEN,
                    AccountID = m.AccountID,
                    ParentAccountID = m.Account.AccoutnParentID,
                    ParentAccCode = unitOfWork.AccountRepository.Get(filter:a=>a.AccountID==m.Account.AccoutnParentID).Select(s=>s.Code).FirstOrDefault(),
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



        [Route("~/api/SellingInvoice/GetSetting")]
        public SettingModel GetSetting(int flag)
        {

            var setsetting = unitOfWork.SettingRepository.Get(filter: x => x.VoucherType == flag).Select(a => new SettingModel
            {

                SettingID = a.SettingID,
                VoucherType = 1,
                AutoGenerateEntry = a.AutoGenerateEntry,
                Code = a.Code,
                DoNotGenerateEntry = a.DoNotGenerateEntry,
                GenerateEntry = a.GenerateEntry,
                TransferToAccounts = a.TransferToAccounts,
                SettingAccs = SettingAccounts(a.SettingID)
             
            }).SingleOrDefault();
            return setsetting;


        }



        [HttpGet]
        [Route("~/api/SellingInvoice/FirstOpen")]
        public IActionResult FirstOpen()
        {
            SellingInvoiceModel model = new SellingInvoiceModel();
            var count = unitOfWork.SellingInvoiceReposetory.Count();
            if (count > 0)
            {
                model.LastCode = unitOfWork.SellingInvoiceReposetory.Last().Code;
                model.Count = count;
            }
            model.SettingModel = GetSetting(1);


            return Ok(model);
        }




        [HttpPost] // يولد قيد يدوي مع ترحيل تلقائي
        [Route("~/api/SellingInvoice/GenerateconstraintManual")]
        public IActionResult GenerateconstraintManual([FromBody] SellingInvoiceModel sellingInvoiceModel)
        {
            if (sellingInvoiceModel.SettingModel.GenerateEntry == true)
            {
                var lastEntry = unitOfWork.EntryRepository.Last();

                int portofolioaccount = unitOfWork.PortfolioAccountRepository.Get(filter: m => m.PortfolioID == sellingInvoiceModel.PortfolioID && m.Type == true)
                    .Select(m => m.AccountID).SingleOrDefault();

                var EntryMODEL = EntriesHelper.InsertCalculatedEntries(portofolioaccount,sellingInvoiceModel, null, null, null, lastEntry);
                var Entry = _mapper.Map<Entry>(EntryMODEL);


                var DetailEnt = EntryMODEL.EntryDetailModel;

                if (sellingInvoiceModel.SettingModel.TransferToAccounts == true)
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



        [HttpPost]// ترحيل يدوي للقيد اليدوي والتلقائي
        [Route("~/api/SellingInvoice/ManualmigrationSellingInvoice")]
        public IActionResult ManualmigrationSellingInvoice([FromBody]EntryModel entryModel)
        {
            //var Entry = unitOfWork.EntryRepository.GetByID(EntryMODEL.EntryID);
            //Entry.TransferedToAccounts = true;
            //unitOfWork.EntryRepository.Update(Entry);
            var Details = entryModel.EntryDetailModel;
            //foreach (var item in Details)
            //{
            //    var detail = _mapper.Map<SellingInvoiceDetail>(item);

            //    unitOfWork.SellingInvoiceDetailRepository.Update(detail);
            //}

  

            accountingHelper.TransferToAccounts(Details.Select(x => new EntryDetail
            {
                EntryDetailID = x.EntryDetailID,
                AccountID = x.AccountID,
                Credit = x.Credit,
                Debit = x.Debit,
                EntryID = x.EntryID


            }).ToList());

            // update entry with transfere flag
            var Entry = unitOfWork.EntryRepository.Get(filter: x => x.EntryID == entryModel.EntryID).SingleOrDefault();
            Entry.TransferedToAccounts = true;
            unitOfWork.EntryRepository.Update(Entry);


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


        [HttpGet]
        [Route("~/api/SellingInvoice/GetLastSellingInvoice")]
        public IActionResult GetLastSellingInvoice()
        {
            var selling = unitOfWork.SellingInvoiceReposetory.Last();


            var sellingInvoiceModel = _mapper.Map<SellingInvoiceModel>(selling);
            //sellingInvoiceModel.PortfolioAccount = unitOfWork.PortfolioAccountRepository.GetEntity(x => x.PortfolioID == selling.PortfolioID && x.Type == true).AccountID;
            //if (sellingInvoiceModel == null)
            //{
            //    return Ok(0);

            //}

            #region portfolio data
            //var portfolio = unitOfWork.PortfolioAccountRepository.GetEntity(x => x.PortfolioID == selling.PortfolioID && x.Type == true);
            //if (portfolio != null)
            //{

            //    sellingInvoiceModel.PortfolioAccount = portfolio.AccountID;

            //    // portfolio data
            //    sellingInvoiceModel.PortfolioCode = portfolio.Portfolio.Code;
            //    sellingInvoiceModel.PortfolioNameAR = portfolio.Portfolio.NameAR;
            //    sellingInvoiceModel.PortfolioNameEN = portfolio.Portfolio.NameEN;
            //    sellingInvoiceModel.PortfolioID = portfolio.Portfolio.PortfolioID;
            //}

            #endregion

            // employee data
            #region employee part
            var employee = unitOfWork.EmployeeRepository.GetEntity(x => x.EmployeeID == selling.EmployeeID);
            if (employee != null)
            {
                sellingInvoiceModel.EmpCode = employee.Code;
                sellingInvoiceModel.EmpNameAR = employee.NameAR;
                sellingInvoiceModel.EmpNameEN = employee.NameEN;
                sellingInvoiceModel.EmployeeID = employee.EmployeeID;

            }
            #endregion

            // date part
            #region Date part
            if (selling.Date != null)
            {

                sellingInvoiceModel.SellDate = selling.Date.Value.ToString("d/M/yyyy");
                sellingInvoiceModel.SellDateHijri = DateHelper.GetHijriDate(selling.Date);
            }

            #endregion




            var Details = unitOfWork.SellingInvoiceDetailRepository.Get(filter: a => a.SellingInvoiceID == selling.SellingInvoiceID)
                            .Select(m => new SellingInvoiceDetailsModel
                            {


                                SellingInvoiceID = m.SellingInvoiceID,
                                SellingInvoiceDetailID = m.SellInvoiceDetailID,
                                BankCommission = m.BankCommission,
                                BankCommissionRate = m.BankCommissionRate,
                                NetAmmount = m.NetAmmount,
                                SelingValue = m.SelingValue,
                                SellingPrice = m.SellingPrice,
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


                sellingInvoiceModel.DetailsModels = Details;
            }

            sellingInvoiceModel.SettingModel = GetSetting(1);

            var check = unitOfWork.EntryRepository.Get(x => x.SellingInvoiceID == selling.SellingInvoiceID).SingleOrDefault();
            if (check != null)
            {
                sellingInvoiceModel.EntryModel = GetEntry(selling.SellingInvoiceID);
            }
            return Ok(sellingInvoiceModel);

        }



        [HttpGet]
        [Route("~/api/SellingInvoice/GetSellingInvoiceByID/{id}")]
        public IActionResult GetSellingInvoiceByID(int id)
        {
            if(id>0)
            {
                var selling = unitOfWork.SellingInvoiceReposetory.GetByID(id);


                var model = _mapper.Map<SellingInvoiceModel>(selling);
                //model.PortfolioAccount = unitOfWork.PortfolioAccountRepository.GetEntity(x => x.PortfolioID == selling.PortfolioID && x.Type == true).AccountID;
                //if (model == null)
                //{
                //    return Ok(model);

                //}


                #region portfolio data
                //var portfolio = unitOfWork.PortfolioAccountRepository.GetEntity(x => x.PortfolioID == selling.PortfolioID && x.Type == true);
                //if (portfolio != null)
                //{

                //    model.PortfolioAccount = portfolio.AccountID;

                //    // portfolio data
                //    model.PortfolioCode = portfolio.Portfolio.Code;
                //    model.PortfolioNameAR = portfolio.Portfolio.NameAR;
                //    model.PortfolioNameEN = portfolio.Portfolio.NameEN;
                //    model.PortfolioID = portfolio.Portfolio.PortfolioID;
                //}

                #endregion

                // employee data
                #region employee part
                var employee = unitOfWork.EmployeeRepository.GetEntity(x => x.EmployeeID == selling.EmployeeID);
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
                if (selling.Date != null)
                {

                    model.SellDate = selling.Date.Value.ToString("d/M/yyyy");
                    model.SellDateHijri = DateHelper.GetHijriDate(selling.Date);
                }

                #endregion


                model.Count = unitOfWork.SellingInvoiceReposetory.Count();

                if (model.Count == 0)
                {
                    return Ok(model);
                }



                var Details = unitOfWork.SellingInvoiceDetailRepository.Get(filter: a => a.SellingInvoiceID == selling.SellingInvoiceID)
                                .Select(m => new SellingInvoiceDetailsModel
                                {


                                    SellingInvoiceID = m.SellingInvoiceID,
                                    SellingInvoiceDetailID = m.SellInvoiceDetailID,
                                    BankCommission = m.BankCommission,
                                    BankCommissionRate = m.BankCommissionRate,
                                    NetAmmount = m.NetAmmount,
                                    SelingValue = m.SelingValue,
                                    SellingPrice = m.SellingPrice,
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

                model.SettingModel = GetSetting(1);
                var check = unitOfWork.EntryRepository.Get(x => x.SellingInvoiceID == selling.SellingInvoiceID).SingleOrDefault();
                if (check != null)
                {
                    model.EntryModel = GetEntry(selling.SellingInvoiceID);
                }

                return Ok(model);
            }
            else
            {
                return Ok(1);

            }


        }


        [HttpGet]
        [Route("~/api/SellingInvoice/Paging/{pageNumber}")]
        public IActionResult PaginationSellingInvoice(int pageNumber)
        {
            if(pageNumber>0)
            {
                var selling = unitOfWork.SellingInvoiceReposetory.Get(page: pageNumber).FirstOrDefault();

                var model = _mapper.Map<SellingInvoiceModel>(selling);

            

                // employee data
                #region employee part
                var employee = unitOfWork.EmployeeRepository.GetEntity(x => x.EmployeeID == selling.EmployeeID);
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
                if (selling.Date != null)
                {

                    model.SellDate = selling.Date.Value.ToString("d/M/yyyy");
                    model.SellDateHijri = DateHelper.GetHijriDate(selling.Date);
                }

                #endregion

                if (model == null)
                {
                    return Ok(model);

                }
                model.PortfolioID = unitOfWork.SellingOrderRepository.GetEntity(filter: x => x.SellingOrderID == selling.SellingOrderID).PortfolioID;
                model.PortfolioCode = unitOfWork.SellingOrderRepository.GetEntity(filter: x => x.SellingOrderID == selling.SellingOrderID).Portfolio.Code;
                model.PortfolioNameAR = unitOfWork.SellingOrderRepository.GetEntity(filter: x => x.SellingOrderID == selling.SellingOrderID).Portfolio.NameAR;
                model.PortfolioNameEN = unitOfWork.SellingOrderRepository.GetEntity(filter: x => x.SellingOrderID == selling.SellingOrderID).Portfolio.NameEN;
                model.Codeselling = unitOfWork.SellingOrderRepository.GetEntity(filter: z => z.SellingOrderID == selling.SellingOrderID).Code;
                model.Count = unitOfWork.SellingInvoiceReposetory.Count();

                if (model.Count == 0)
                {
                    return Ok(model);
                }



                var Details = unitOfWork.SellingInvoiceDetailRepository.Get(filter: a => a.SellingInvoiceID == selling.SellingInvoiceID)
                            .Select(m => new SellingInvoiceDetailsModel
                            {

                                SellingInvoiceID = m.SellingInvoiceID,
                                SellingInvoiceDetailID = m.SellInvoiceDetailID,
                                BankCommission = m.BankCommission,
                                BankCommissionRate = m.BankCommissionRate,
                                NetAmmount = m.NetAmmount,
                                SelingValue = m.SelingValue,
                                SellingPrice = m.SellingPrice,
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

                model.SettingModel = GetSetting(1);

                var check = unitOfWork.EntryRepository.Get(x => x.SellingInvoiceID == selling.SellingInvoiceID).SingleOrDefault();
                if (check != null)
                {
                    model.EntryModel = GetEntry(selling.SellingInvoiceID);
                }

                return Ok(model);
            }
            else
            {
                return Ok(1);

            }
           

        }



        [HttpPost]
        [Route("~/api/SellingInvoice/PostSellingInvoice")]
        public IActionResult PostSellingInvoice([FromBody] SellingInvoiceModel sellingInvoiceModel)
        {
            if (ModelState.IsValid)
            {
               

                int portofolioaccount = 0;
                var Check = unitOfWork.SellingInvoiceReposetory.Get();
                if (Check.Any(m => m.Code == sellingInvoiceModel.Code))
                {

                    return Ok(2);

                }
                else
                {
                    portofolioaccount = unitOfWork.PortfolioAccountRepository.Get(filter: m => m.PortfolioID == sellingInvoiceModel.PortfolioID && m.Type == true).Select(m => m.AccountID).SingleOrDefault();
                    var modelselling = _mapper.Map<SellingInvoice>(sellingInvoiceModel);


                    var Details = sellingInvoiceModel.DetailsModels;
                   
                        unitOfWork.SellingInvoiceReposetory.Insert(modelselling);
                    if(Details != null && Details.Count()>0)
                    {
                        foreach (var item in Details)
                        {
                            SellingInvoiceDetailsModel selingOrderDetailsModel = new SellingInvoiceDetailsModel();
                            selingOrderDetailsModel.SellingInvoiceID = modelselling.SellingInvoiceID;
                            selingOrderDetailsModel.NetAmmount = item.NetAmmount;
                            selingOrderDetailsModel.SelingValue = item.SelingValue;
                            selingOrderDetailsModel.SellingPrice = item.SellingPrice;
                            selingOrderDetailsModel.StockCount = item.StockCount;
                            selingOrderDetailsModel.TaxOnCommission = item.TaxOnCommission;
                            selingOrderDetailsModel.TaxRateOnCommission = item.TaxRateOnCommission;
                            selingOrderDetailsModel.BankCommission = item.BankCommission;
                            selingOrderDetailsModel.BankCommissionRate = item.BankCommissionRate;
                            selingOrderDetailsModel.PartnerID = item.PartnerID;

                            var details = _mapper.Map<SellingInvoiceDetail>(selingOrderDetailsModel);
                            unitOfWork.SellingInvoiceDetailRepository.Insert(details);

                        }

                    }
                    #region Warehouse
                    //Check Stocks Count Allowed For Selling 
                    var Chk = _stocksHelper.CheckStockCountForSelling(sellingInvoiceModel);
                    if (!Chk)
                        return Ok(7);
                    // Transfer From Portofolio Stocks
                    else
                        _stocksHelper.TransferSellingFromStocks(sellingInvoiceModel);
                    #endregion


                    //==================================================لا تولد قيد ===================================
                    if (sellingInvoiceModel.SettingModel.DoNotGenerateEntry == true)
                    {
                        unitOfWork.Save();

                        return Ok(sellingInvoiceModel);
                    }

                    //===============================================================توليد قيد مع ترحيل تلقائي============================
                    else if (sellingInvoiceModel.SettingModel.AutoGenerateEntry == true)
                    {
                        var lastEntry = unitOfWork.EntryRepository.Last();
                    
                        var EntryMODEL = EntriesHelper.InsertCalculatedEntries(portofolioaccount,sellingInvoiceModel, null, null, null, lastEntry);
                        EntryMODEL.SellingInvoiceID = modelselling.SellingInvoiceID;
                        var Entry = _mapper.Map<Entry>(EntryMODEL);
                        var DetailEnt = EntryMODEL.EntryDetailModel;

                        if (sellingInvoiceModel.SettingModel.TransferToAccounts == true)
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
                                var details = _mapper.Map<EntryDetail>(item);

                                unitOfWork.EntryDetailRepository.Insert(details);
                            }
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



                }
            }
            else
            {
                return Ok(3);

            }
        }




        [HttpPut]
        [Route("~/api/SellingInvoice/PutSellingInvoice/{id}")]
        public IActionResult PutSellingInvoice(int id, [FromBody]  SellingInvoiceModel sellingInvoiceModel)
        {
            int portofolioaccount = 0;
            if (sellingInvoiceModel !=null)
            {
                if (id != sellingInvoiceModel.SellingInvoiceID)
                {

                    return Ok(1);

                }
            }
           

            if (ModelState.IsValid)
            {
               

                var Check = unitOfWork.SellingInvoiceReposetory.Get(NoTrack: "NoTrack");
                portofolioaccount = unitOfWork.PortfolioAccountRepository.Get(filter: m => m.PortfolioID == sellingInvoiceModel.PortfolioID && m.Type == true)
                        .Select(m => m.AccountID).SingleOrDefault();
                var sellingInvoice = _mapper.Map<SellingInvoice>(sellingInvoiceModel);
                var NewdDetails = sellingInvoiceModel.DetailsModels;
                var Newdetails = _mapper.Map<IEnumerable<SellingInvoiceDetail>>(NewdDetails);
                var OldDetails = unitOfWork.SellingInvoiceDetailRepository.Get(NoTrack: "NoTrack",filter: m => m.SellingInvoiceID == sellingInvoice.SellingInvoiceID);
                #region Warehouse
                //Cancel Selling Order From Stocks 
                _stocksHelper.CancelSellingFromStocks(sellingInvoiceModel.PortfolioID, OldDetails);
                //Check Stocks Count Allowed For Selling 
                var Chk = _stocksHelper.CheckStockCountForSelling(sellingInvoiceModel);
                if (!Chk)
                    return Ok(7);
                //Transfer From Portofolio Stocks
                else
                 _stocksHelper.TransferSellingFromStocks(sellingInvoiceModel);
                #endregion
                var EntryCheck = unitOfWork.EntryRepository.Get(x => x.SellingInvoiceID == sellingInvoice.SellingInvoiceID, NoTrack: "NoTrack").SingleOrDefault();
              if (EntryCheck != null)
              {

                    // get old entry data
                  var Entry = unitOfWork.EntryRepository.Get(NoTrack: "NoTrack",filter: x => x.SellingInvoiceID == sellingInvoice.SellingInvoiceID).SingleOrDefault();
                  var OldEntryDetails = unitOfWork.EntryDetailRepository.Get(filter: a => a.EntryID == Entry.EntryID);
                  if (Entry.TransferedToAccounts == true)
                  {
                      accountingHelper.CancelTransferToAccounts(OldEntryDetails.ToList());
                  }
                  // delete old entry
                  unitOfWork.EntryDetailRepository.RemovRange(OldEntryDetails);
                  unitOfWork.EntryRepository.Delete(Entry.EntryID);

                  if (Check.Any(m => m.Code != sellingInvoice.Code))
                  {
                      unitOfWork.SellingInvoiceReposetory.Update(sellingInvoice);
                      if (OldDetails.Count()>0 && OldDetails!=null)
                      {
                          unitOfWork.SellingInvoiceDetailRepository.RemovRange(OldDetails);
                      }


                      if (Newdetails != null && Newdetails.Count()>0)
                      {
                          foreach (var item in Newdetails)
                          {
                              item.SellingInvoiceID = sellingInvoice.SellingInvoiceID;
                              item.SellInvoiceDetailID = 0;
                              var details = _mapper.Map<SellingInvoiceDetail>(item);

                              unitOfWork.SellingInvoiceDetailRepository.Insert(details);

                          }
                      }


                      //==================================================لا تولد قيد ===================================
                      if (sellingInvoiceModel.SettingModel.DoNotGenerateEntry == true)
                      {
                          //unitOfWork.EntryRepository.Delete(Entry.EntryID);

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
                        //===================================توليد قيد مع ترحيل تلقائي===================================
                        if (sellingInvoiceModel.SettingModel.AutoGenerateEntry == true)
                        {
                            //var EntryDitails = EntriesHelper.UpdateCalculateEntries(portofolioaccount,Entry.EntryID, sellingInvoiceModel, null, null, null);

                            var lastEntry = unitOfWork.EntryRepository.Last();
                    
                            var EntryMODEL = EntriesHelper.InsertCalculatedEntries(portofolioaccount,sellingInvoiceModel, null, null, null, lastEntry, Entry);
                            EntryMODEL.SellingInvoiceID = sellingInvoiceModel.SellingInvoiceID;
                            var NewEntry = _mapper.Map<Entry>(EntryMODEL);
                            var EntryDitails = EntryMODEL.EntryDetailModel;

                            if (sellingInvoiceModel.SettingModel.TransferToAccounts == true)
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
                                    StocksCredit=x.StocksCredit,
                                    StocksDebit=x.StocksDebit


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
                      
                        var res = unitOfWork.Save();
                        if (res == 200)
                        {


                            return Ok(4);
                        }
                        else if (res == 501)
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
                      if (Check.Any(m => m.Code == sellingInvoice.Code && m.SellingInvoiceID == id))
                      {
                          unitOfWork.SellingInvoiceReposetory.Update(sellingInvoice);
                          if (OldDetails != null)
                          {
                              unitOfWork.SellingInvoiceDetailRepository.RemovRange(OldDetails);

                                var res = unitOfWork.Save();
                                if (res == 200)
                                {

                                    return Ok(4);
                                }
                                else if (res == 501)
                                {
                                    return Ok(5);
                                }
                                else
                                {
                                    return Ok(6);
                                }

                            }


                          if (Newdetails != null && Newdetails.Count()>0)
                          {
                              foreach (var item in Newdetails)
                              {
                                  item.SellingInvoiceID = sellingInvoice.SellingInvoiceID;
                                  item.SellInvoiceDetailID = 0;
                                  var details = _mapper.Map<SellingInvoiceDetail>(item);

                                  unitOfWork.SellingInvoiceDetailRepository.Insert(details);

                              }
                          }


                          //==================================================لا تولد قيد ===================================
                          if (sellingInvoiceModel.SettingModel.DoNotGenerateEntry == true)
                          {
                              //unitOfWork.EntryRepository.Delete(Entry.EntryID);

                            var res = unitOfWork.Save();
                            if (res == 200)
                            {

                                return Ok(4);
                            }
                            else if (res == 501)
                            {
                                return Ok(5);
                            }
                            else
                            {
                                return Ok(6);
                            }
                            }

                          //===================================توليد قيد مع ترحيل تلقائي===================================
                          if (sellingInvoiceModel.SettingModel.AutoGenerateEntry == true)
                          {
                                //var EntryDitails = EntriesHelper.UpdateCalculateEntries(portofolioaccount,Entry.EntryID, sellingInvoiceModel, null, null, null);
                                var lastEntry = unitOfWork.EntryRepository.Last();

                                var EntryMODEL = EntriesHelper.InsertCalculatedEntries(portofolioaccount, sellingInvoiceModel, null, null, null, lastEntry,Entry);
                                EntryMODEL.SellingInvoiceID = sellingInvoiceModel.SellingInvoiceID;
                                var NewEntry = _mapper.Map<Entry>(EntryMODEL);
                                var EntryDitails = EntryMODEL.EntryDetailModel;

                                if (sellingInvoiceModel.SettingModel.TransferToAccounts == true)
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
                                      StocksDebit=x.StocksDebit,
                                      StocksCredit=x.StocksCredit


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
                  return Ok();
              }

            // now We Will Create new Entry As Insert
            else
            {
                if (Check.Any(m => m.Code != sellingInvoice.Code))
                {
                    unitOfWork.SellingInvoiceReposetory.Update(sellingInvoice);
                    if (OldDetails.Count()>0 && OldDetails!=null)
                    {
                        unitOfWork.SellingInvoiceDetailRepository.RemovRange(OldDetails);

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


                    if (Newdetails != null && Newdetails.Count()>0)
                    {
                        foreach (var item in Newdetails)
                        {
                            item.SellingInvoiceID = sellingInvoice.SellingInvoiceID;
                            item.SellInvoiceDetailID = 0;
                            var details = _mapper.Map<SellingInvoiceDetail>(item);

                            unitOfWork.SellingInvoiceDetailRepository.Insert(details);

                        }
                    }


                    //==================================================لا تولد قيد ===================================
                    if (sellingInvoiceModel.SettingModel.DoNotGenerateEntry == true)
                    {


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
                    //===============================================================توليد قيد مع ترحيل تلقائي============================




                        else if (sellingInvoiceModel.SettingModel.AutoGenerateEntry == true)
                    {
                        var lastEntry = unitOfWork.EntryRepository.Last();
                        var EntryMODEL = EntriesHelper.InsertCalculatedEntries(portofolioaccount,sellingInvoiceModel, null, null, null, lastEntry);
                        EntryMODEL.SellingInvoiceID = sellingInvoice.SellingInvoiceID;
                        var Entry = _mapper.Map<Entry>(EntryMODEL);
                        Entry.SellingInvoiceID = sellingInvoice.SellingInvoiceID;

                        var DetailEnt = EntryMODEL.EntryDetailModel;

                        if (sellingInvoiceModel.SettingModel.TransferToAccounts == true)
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
                                StocksCredit=x.StocksCredit,
                                StocksDebit=x.StocksDebit


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


                    //==========================================Second Case OF Code Of Purchase=======================================

                    else
                {
                    if (Check.Any(m => m.Code == sellingInvoice.Code && m.SellingInvoiceID == id))
                    {
                        unitOfWork.SellingInvoiceReposetory.Update(sellingInvoice);
                        if (OldDetails != null  && OldDetails.Count()>0)
                        {
                            unitOfWork.SellingInvoiceDetailRepository.RemovRange(OldDetails);

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


                        if (Newdetails != null && Newdetails.Count()>0)
                        {
                            foreach (var item in Newdetails)
                            {
                                item.SellingInvoiceID = sellingInvoice.SellingInvoiceID;
                                item.SellInvoiceDetailID = 0;
                                var details = _mapper.Map<SellingInvoiceDetail>(item);

                                unitOfWork.SellingInvoiceDetailRepository.Insert(details);

                            }
                        }


                        //==================================================لا تولد قيد ===================================
                        if (sellingInvoiceModel.SettingModel.DoNotGenerateEntry == true)
                        {
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
                        //===============================================================توليد قيد مع ترحيل تلقائي============================




                            else if (sellingInvoiceModel.SettingModel.AutoGenerateEntry == true)
                        {
                            var lastEntry = unitOfWork.EntryRepository.Last();
                            var EntryMODEL = EntriesHelper.InsertCalculatedEntries(portofolioaccount,sellingInvoiceModel, null, null, null, lastEntry);
                            var Entry = _mapper.Map<Entry>(EntryMODEL);
                            Entry.SellingInvoiceID = sellingInvoice.SellingInvoiceID;

                            var DetailEnt = EntryMODEL.EntryDetailModel;

                            if (sellingInvoiceModel.SettingModel.TransferToAccounts == true)
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
                                    StocksDebit=x.StocksDebit,
                                    StocksCredit=x.StocksCredit


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



                return Ok(sellingInvoiceModel);

            }



            }



            else
            {
                return Ok(3);

            }


        }

        [HttpDelete]
        [Route("~/api/SellingInvoice/DeleteSelling/{id}")]
        public IActionResult DeleteSelling(int? id)
        {

            if (id == null)
            {

                return Ok(1);

            }
            var modelSelling = unitOfWork.SellingInvoiceReposetory.GetByID(id);
            if (modelSelling == null)
            {
                return Ok(0);

            }
            var Details = unitOfWork.SellingInvoiceDetailRepository.Get(filter: m => m.SellingInvoiceID == id);
            #region Warehouse
            //Cancel Selling Order From Stocks 
        //    _stocksHelper.CancelSellingFromStocks(modelSelling.PortfolioID, Details);
            #endregion
            unitOfWork.SellingInvoiceDetailRepository.RemovRange(Details);

            var Entry = unitOfWork.EntryRepository.Get(filter: x => x.SellingInvoiceID == id).SingleOrDefault();
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
          

            unitOfWork.SellingInvoiceReposetory.Delete(id);

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


        [HttpGet]
        [Route("~/api/SellingInvoice/GetAllSellings")]
        public IEnumerable<sellingComboList> GetAllSelling()
        {

            var checks = unitOfWork.SellingInvoiceReposetory.Get();
            List<sellingComboList> sellings = unitOfWork.SellingOrderRepository.Get().Select(x => new sellingComboList
            {

                Code = x.Code,
                SellingOrderID = x.SellingOrderID,


            }).ToList();
            List<sellingComboList> lists = new List<sellingComboList>();
            foreach (var item in sellings)
            {
                if (!checks.Any(m => m.SellingOrderID == item.SellingOrderID ))
                {
                    lists.Add(item);
                }
                if (checks.Any(m=> m.SellingOrderID == item.SellingOrderID &&
                 m.SellingOrder.OrderType == true ))
                {
                    lists.Add(item);
                }
            }


            return lists;
        }




        [HttpGet]
        [Route("~/api/SellingInvoice/GetPortfolioINFO/{id}")]
        public IActionResult GetPortfolioINFO(int id)
        {
            var Info = unitOfWork.SellingOrderRepository.Get(filter: x => x.SellingOrderID == id).SingleOrDefault();
            SellingOrderModel sellingOrderModel = new SellingOrderModel();
            sellingOrderModel.SellingOrderID = Info.SellingOrderID;
            sellingOrderModel.PortfolioID = Info.PortfolioID;
            sellingOrderModel.Portfoliocode = Info.Portfolio.Code;
            sellingOrderModel.PortfolioNameAR = Info.Portfolio.NameAR;
            sellingOrderModel.PortfolioAccount = unitOfWork.PortfolioAccountRepository.GetEntity(filter:a=> a.PortfolioID==Info.PortfolioID).AccountID;
            sellingOrderModel.PortfolioAccountName = unitOfWork.PortfolioAccountRepository.GetEntity(filter: a => a.PortfolioID == Info.PortfolioID).Account.NameAR;


            return Ok(sellingOrderModel);





        }
    }
}


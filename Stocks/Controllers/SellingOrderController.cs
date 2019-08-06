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

namespace Stocks.Controllers
{
    public class SellingOrderController : Controller
    {

        private UnitOfWork unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAccountingHelper accountingHelper;
        private readonly IStocksHelper _stocksHelper;
        public SellingOrderController(StocksContext context, IMapper mapper, IStocksHelper stocksHelper)
        {
            this.unitOfWork = new UnitOfWork(context);
            this._mapper = mapper;
            accountingHelper = new AccountingHelper(context,mapper);
            _stocksHelper = stocksHelper;
        }
        [Route("~/api/SellingOrder/GetSettingAccounts/{id}")]
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

        public EntryModel GetEntry(Entry Entry)
        {
            EntryModel entryModel = new EntryModel();

            if (Entry != null)
            {
                var EntryDetails = unitOfWork.EntryDetailRepository.Get(filter: a => a.EntryID == Entry.EntryID);
                entryModel.EntryID = Entry.EntryID;
                entryModel.Code = Entry.Code;
                entryModel.Date = Entry.Date.Value.ToString("dd/MM/yyyy");
                entryModel.DateHijri = DateHelper.GetHijriDate(Entry.Date);
                entryModel.NoticeID = Entry.NoticeID;
                entryModel.PurchaseOrderID = Entry.PurchaseOrderID;
                entryModel.ReceiptID = Entry.ReceiptID;
                entryModel.PurchaseOrderID = Entry.PurchaseOrderID;
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

        [HttpGet]//القيد
        [Route("~/api/SellingOrder/GetEntry/{sellingOrderID}")]
        public EntryModel GetEntry(int sellingOrderID)
        {
            var Entry = unitOfWork.EntryRepository.Get(x => x.SellingOrderID == sellingOrderID).SingleOrDefault();
            EntryModel entryModel = new EntryModel();

            if (Entry!=null)
            {
                var EntryDetails = unitOfWork.EntryDetailRepository.Get(filter: a => a.EntryID == Entry.EntryID);
                entryModel.EntryID = Entry.EntryID;
                entryModel.Code = Entry.Code;
                entryModel.Date = Entry.Date.Value.ToString("dd/MM/yyyy");
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



        [Route("~/api/SellingOrder/GetSetting")]
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
                SettingAccs = SettingAccounts(a.SettingID),

            }).SingleOrDefault();
            return setsetting;


        }



        [HttpGet]
        [Route("~/api/SellingOrder/FirstOpen")]
        public IActionResult FirstOpen()
        {
            SellingOrderModel model = new SellingOrderModel();
            var count = unitOfWork.SellingOrderReposetory.Count();
            if (count > 0)
            {
                model.LastCode = unitOfWork.SellingOrderReposetory.Last().Code;
                model.Count = count;
            }
            model.SettingModel = GetSetting(1);


            return Ok(model);
        }




        [HttpPost] // يولد قيد يدوي مع ترحيل تلقائي
        [Route("~/api/SellingOrder/GenerateconstraintManual")]
        public IActionResult GenerateconstraintManual([FromBody] SellingOrderModel sellingOrderModel)
        {
            if (sellingOrderModel.SettingModel.GenerateEntry == true)
            {
                var lastEntry = unitOfWork.EntryRepository.Last();

                int portofolioaccount = unitOfWork.PortfolioAccountRepository.Get(filter: m => m.PortfolioID == sellingOrderModel.PortfolioID && m.Type == true)
                    .Select(m => m.AccountID).SingleOrDefault();

                var EntryMODEL = EntriesHelper.InsertCalculatedEntries(portofolioaccount,sellingOrderModel, null, null, null, lastEntry);
                var Entry = _mapper.Map<Entry>(EntryMODEL);


                var DetailEnt = EntryMODEL.EntryDetailModel;

                if (sellingOrderModel.SettingModel.TransferToAccounts == true)
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


            unitOfWork.Save();



            return Ok(sellingOrderModel);

        }



        [HttpPost]// ترحيل يدوي للقيد اليدوي والتلقائي
        [Route("~/api/SellingOrder/ManualmigrationSellingOrder")]
        public IActionResult ManualmigrationSellingOrder([FromBody]EntryModel entryModel)
        {
            //var Entry = unitOfWork.EntryRepository.GetByID(EntryMODEL.EntryID);
            //Entry.TransferedToAccounts = true;
            //unitOfWork.EntryRepository.Update(Entry);
            var Details = entryModel.EntryDetailModel;
            //foreach (var item in Details)
            //{
            //    var detail = _mapper.Map<SellingOrderDetail>(item);

            //    unitOfWork.SellingOrderDetailRepository.Update(detail);
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


            unitOfWork.Save();



            return Ok(GetEntry(Entry));

        }


        [HttpGet]
        [Route("~/api/SellingOrder/GetLastSellingOrder")]
        public IActionResult GetLastSellingOrder()
        {
            var selling = unitOfWork.SellingOrderReposetory.Last();


            var sellingOrderModel = _mapper.Map<SellingOrderModel>(selling);
            sellingOrderModel.PortfolioAccount = unitOfWork.PortfolioAccountRepository.GetEntity(x => x.PortfolioID == selling.PortfolioID && x.Type == true).AccountID;
            if (sellingOrderModel == null)
            {
                return Ok(0);

            }

            #region portfolio data
            var portfolio = unitOfWork.PortfolioAccountRepository.GetEntity(x => x.PortfolioID == selling.PortfolioID && x.Type == true);
            if (portfolio != null)
            {

                sellingOrderModel.PortfolioAccount = portfolio.AccountID;

                // portfolio data
                sellingOrderModel.PortfolioCode = portfolio.Portfolio.Code;
                sellingOrderModel.PortfolioNameAR = portfolio.Portfolio.NameAR;
                sellingOrderModel.PortfolioNameEN = portfolio.Portfolio.NameEN;
                sellingOrderModel.PortfolioID = portfolio.Portfolio.PortfolioID;
            }

            #endregion

            // employee data
            #region employee part
            var employee = unitOfWork.EmployeeRepository.GetEntity(x => x.EmployeeID == selling.EmployeeID);
            if (employee != null)
            {
                sellingOrderModel.EmpCode = employee.Code;
                sellingOrderModel.EmpNameAR = employee.NameAR;
                sellingOrderModel.EmpNameEN = employee.NameEN;
                sellingOrderModel.EmployeeID = employee.EmployeeID;

            }
            #endregion

            // date part
            #region Date part
            if (selling.Date != null)
            {

                sellingOrderModel.SellDate = selling.Date.Value.ToString("dd/MM/yyyy");
                sellingOrderModel.SellDateHijri = DateHelper.GetHijriDate(selling.Date);
            }

            #endregion




            var Details = unitOfWork.SellingOrderDetailRepository.Get(filter: a => a.SellingOrderID == selling.SellingOrderID)
                            .Select(m => new SelingOrderDetailsModel
                            {


                                SellingOrderID = m.SellingOrderID,
                                SellOrderDetailID = m.SellOrderDetailID,
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


                sellingOrderModel.DetailsModels = Details;
            }

            sellingOrderModel.SettingModel = GetSetting(1);

            var check = unitOfWork.EntryRepository.Get(x => x.SellingOrderID == selling.SellingOrderID).SingleOrDefault();
            if (check != null)
            {
                sellingOrderModel.EntryModel = GetEntry(selling.SellingOrderID);
            }
            return Ok(sellingOrderModel);

        }



        [HttpGet]
        [Route("~/api/SellingOrder/GetSellingOrderByID/{id}")]
        public IActionResult GetSellingOrderByID(int id)
        {
            if(id>0)
            {
                var selling = unitOfWork.SellingOrderReposetory.GetByID(id);


                var model = _mapper.Map<SellingOrderModel>(selling);
                model.PortfolioAccount = unitOfWork.PortfolioAccountRepository.GetEntity(x => x.PortfolioID == selling.PortfolioID && x.Type == true).AccountID;
                if (model == null)
                {
                    return Ok(model);

                }


                #region portfolio data
                var portfolio = unitOfWork.PortfolioAccountRepository.GetEntity(x => x.PortfolioID == selling.PortfolioID && x.Type == true);
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

                    model.SellDate = selling.Date.Value.ToString("dd/MM/yyyy");
                    model.SellDateHijri = DateHelper.GetHijriDate(selling.Date);
                }

                #endregion


                model.Count = unitOfWork.SellingOrderReposetory.Count();

                if (model.Count == 0)
                {
                    return Ok(model);
                }



                var Details = unitOfWork.SellingOrderDetailRepository.Get(filter: a => a.SellingOrderID == selling.SellingOrderID)
                                .Select(m => new SelingOrderDetailsModel
                                {


                                    SellingOrderID = m.SellingOrderID,
                                    SellOrderDetailID = m.SellOrderDetailID,
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
                var check = unitOfWork.EntryRepository.Get(x => x.SellingOrderID == selling.SellingOrderID).SingleOrDefault();
                if (check != null)
                {
                    model.EntryModel = GetEntry(selling.SellingOrderID);
                }

                return Ok(model);
            }
            else
            {
                return Ok(1);

            }


        }


        [HttpGet]
        [Route("~/api/SellingOrder/Paging/{pageNumber}")]
        public IActionResult PaginationSellingOrder(int pageNumber)
        {
            if(pageNumber>0)
            {
                var selling = unitOfWork.SellingOrderReposetory.Get(page: pageNumber).FirstOrDefault();

                var model = _mapper.Map<SellingOrderModel>(selling);

                #region portfolio data
                var portfolio = unitOfWork.PortfolioAccountRepository.GetEntity(x => x.PortfolioID == selling.PortfolioID && x.Type == true);
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

                    model.SellDate = selling.Date.Value.ToString("dd/MM/yyyy");
                    model.SellDateHijri = DateHelper.GetHijriDate(selling.Date);
                }

                #endregion

                if (model == null)
                {
                    return Ok(model);

                }


                model.Count = unitOfWork.SellingOrderReposetory.Count();

                if (model.Count == 0)
                {
                    return Ok(model);
                }



                var Details = unitOfWork.SellingOrderDetailRepository.Get(filter: a => a.SellingOrderID == selling.SellingOrderID)
                            .Select(m => new SelingOrderDetailsModel
                            {

                                SellingOrderID = m.SellingOrderID,
                                SellOrderDetailID = m.SellOrderDetailID,
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

                var check = unitOfWork.EntryRepository.Get(x => x.SellingOrderID == selling.SellingOrderID).SingleOrDefault();
                if (check != null)
                {
                    model.EntryModel = GetEntry(selling.SellingOrderID);
                }

                return Ok(model);
            }
            else
            {
                return Ok(1);

            }
           

        }



        [HttpPost]
        [Route("~/api/SellingOrder/PostSellingOrder")]
        public IActionResult PostSellingOrder([FromBody] SellingOrderModel sellingOrderModel)
        {
            if (ModelState.IsValid)
            {
                int portofolioaccount = 0;
                var Check = unitOfWork.SellingOrderReposetory.Get();
                if (Check.Any(m => m.Code == sellingOrderModel.Code))
                {

                    return Ok(2);

                }
                else
                {
                    portofolioaccount = unitOfWork.PortfolioAccountRepository.Get(filter: m => m.PortfolioID == sellingOrderModel.PortfolioID && m.Type == true).Select(m => m.AccountID).SingleOrDefault();
                    var modelselling = _mapper.Map<SellingOrder>(sellingOrderModel);


                    var Details = sellingOrderModel.DetailsModels;
                   
                        unitOfWork.SellingOrderReposetory.Insert(modelselling);
                    if(Details != null)
                    {
                        foreach (var item in Details)
                        {
                            SelingOrderDetailsModel selingOrderDetailsModel = new SelingOrderDetailsModel();
                            selingOrderDetailsModel.SellingOrderID = modelselling.SellingOrderID;
                            selingOrderDetailsModel.NetAmmount = item.NetAmmount;
                            selingOrderDetailsModel.SelingValue = item.SelingValue;
                            selingOrderDetailsModel.SellingPrice = item.SellingPrice;
                            selingOrderDetailsModel.StockCount = item.StockCount;
                            selingOrderDetailsModel.TaxOnCommission = item.TaxOnCommission;
                            selingOrderDetailsModel.TaxRateOnCommission = item.TaxRateOnCommission;
                            selingOrderDetailsModel.BankCommission = item.BankCommission;
                            selingOrderDetailsModel.BankCommissionRate = item.BankCommissionRate;
                            selingOrderDetailsModel.PartnerID = item.PartnerID;

                            var details = _mapper.Map<SellingOrderDetail>(selingOrderDetailsModel);
                            unitOfWork.SellingOrderDetailRepository.Insert(details);

                        }

                    }
                    #region Warehouse
                    //Check Stocks Count Allowed For Selling 
                    var Chk = _stocksHelper.CheckStockCount(sellingOrderModel);
                    if (!Chk)
                        return Ok(7);
                    // Transfer From Portofolio Stocks
                    else
                        _stocksHelper.TransferSellingFromStocks(sellingOrderModel);
                    #endregion


                    //==================================================لا تولد قيد ===================================
                    if (sellingOrderModel.SettingModel.DoNotGenerateEntry == true)
                    {
                        unitOfWork.Save();

                        return Ok(sellingOrderModel);
                    }

                    //===============================================================توليد قيد مع ترحيل تلقائي============================
                    else if (sellingOrderModel.SettingModel.AutoGenerateEntry == true)
                    {
                        var lastEntry = unitOfWork.EntryRepository.Last();
                    
                        var EntryMODEL = EntriesHelper.InsertCalculatedEntries(portofolioaccount,sellingOrderModel, null, null, null, lastEntry);
                        EntryMODEL.SellingOrderID = modelselling.SellingOrderID;
                        var Entry = _mapper.Map<Entry>(EntryMODEL);
                        var DetailEnt = EntryMODEL.EntryDetailModel;

                        if (sellingOrderModel.SettingModel.TransferToAccounts == true)
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
                            Entry.TransferedToAccounts = true;
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
                    //================================توليد قيد مع عدم الترحيل====================================== 
                    //else if (sellingOrderModel.SettingModel.GenerateEntry == true)

                    //{
                    //    var lastEntry = unitOfWork.EntryRepository.Last();
                    //    var EntryMODEL = EntriesHelper.InsertCalculatedEntries(sellingOrderModel, null, null, null, lastEntry);
                    //    EntryMODEL.SellingOrderID = modelselling.SellingOrderID;
                    //    var Entry = _mapper.Map<Entry>(EntryMODEL);
                    //    var DetailEnt = EntryMODEL.EntryDetailModel;
                    //    Entry.TransferedToAccounts = false;
                    //    unitOfWork.EntryRepository.Insert(Entry);
                    //    foreach (var item in DetailEnt)
                    //    {
                    //        item.EntryID = Entry.EntryID;
                    //        item.EntryDetailID = 0;
                    //        var details = _mapper.Map<EntryDetail>(item);

                    //        unitOfWork.EntryDetailRepository.Insert(details);
                    //    }
                    //}

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
                    return Ok(sellingOrderModel);



                }
            }
            else
            {
                return Ok(3);

            }
        }




        [HttpPut]
        [Route("~/api/SellingOrder/PutSellingOrder/{id}")]
        public IActionResult PutSellingOrder(int id, [FromBody]  SellingOrderModel sellingOrderModel)
        {
            int portofolioaccount = 0;
            if (sellingOrderModel !=null)
            {
                if (id != sellingOrderModel.SellingOrderID)
                {

                    return Ok(1);

                }
            }
           

            if (ModelState.IsValid)
            {

                var Check = unitOfWork.SellingOrderReposetory.Get(NoTrack: "NoTrack");
                portofolioaccount = unitOfWork.PortfolioAccountRepository.Get(filter: m => m.PortfolioID == sellingOrderModel.PortfolioID && m.Type == true)
                        .Select(m => m.AccountID).SingleOrDefault();
                var sellingOrder = _mapper.Map<SellingOrder>(sellingOrderModel);
                var NewdDetails = sellingOrderModel.DetailsModels;
                var Newdetails = _mapper.Map<IEnumerable<SellingOrderDetail>>(NewdDetails);
                var OldDetails = unitOfWork.SellingOrderDetailRepository.Get(filter: m => m.SellingOrderID == sellingOrder.SellingOrderID);
                #region Warehouse
                //Cancel Selling Order From Stocks 
                _stocksHelper.CancelSellingFromStocks(sellingOrderModel.PortfolioID, OldDetails);
                //Check Stocks Count Allowed For Selling 
                var Chk = _stocksHelper.CheckStockCount(sellingOrderModel);
                if (!Chk)
                    return Ok(7);
                //Transfer From Portofolio Stocks
                else
                 _stocksHelper.TransferSellingFromStocks(sellingOrderModel);
                #endregion
                var EntryCheck = unitOfWork.EntryRepository.Get(x => x.SellingOrderID == sellingOrder.SellingOrderID).SingleOrDefault();
              if (EntryCheck != null)
              {

                  var Entry = unitOfWork.EntryRepository.Get(filter: x => x.SellingOrderID == sellingOrder.SellingOrderID).SingleOrDefault();
                  var OldEntryDetails = unitOfWork.EntryDetailRepository.Get(filter: a => a.EntryID == Entry.EntryID);
                  if (Entry.TransferedToAccounts == true)
                  {
                      accountingHelper.CancelTransferToAccounts(OldEntryDetails.ToList());
                  }
                  unitOfWork.EntryDetailRepository.RemovRange(OldEntryDetails);

                  if (Check.Any(m => m.Code != sellingOrder.Code))
                  {
                      unitOfWork.SellingOrderReposetory.Update(sellingOrder);
                      if (OldDetails != null)
                      {
                          unitOfWork.SellingOrderDetailRepository.RemovRange(OldDetails);
                      }


                      if (Newdetails != null)
                      {
                          foreach (var item in Newdetails)
                          {
                              item.SellingOrderID = sellingOrder.SellingOrderID;
                              item.SellOrderDetailID = 0;
                              var details = _mapper.Map<SellingOrderDetail>(item);

                              unitOfWork.SellingOrderDetailRepository.Insert(details);

                          }
                      }


                      //==================================================لا تولد قيد ===================================
                      if (sellingOrderModel.SettingModel.DoNotGenerateEntry == true)
                      {
                          unitOfWork.EntryRepository.Delete(Entry.EntryID);
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

                            return Ok(sellingOrderModel);
                      }
                        //===================================توليد قيد مع ترحيل تلقائي===================================
                        if (sellingOrderModel.SettingModel.AutoGenerateEntry == true)
                        {
                            var EntryDitails = EntriesHelper.UpdateCalculateEntries(portofolioaccount,Entry.EntryID, sellingOrderModel, null, null, null);

                            if (sellingOrderModel.SettingModel.TransferToAccounts == true)
                            {
                                Entry.TransferedToAccounts = true;
                                unitOfWork.EntryRepository.Update(Entry);
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
                                    EntryID = x.EntryID,
                                    StocksCredit=x.StocksCredit,
                                    StocksDebit=x.StocksDebit


                                }).ToList());
                            }
                            else
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
                            }
                        }
                        //===================================توليد قيد مع  عدم ترحيل=================================== 
                        //if (sellingOrderModel.SettingModel.GenerateEntry == true)

                        //{
                        //    var EntryDitails = EntriesHelper.UpdateCalculateEntries(portofolioaccount,Entry.EntryID, sellingOrderModel, null, null, null);
                        //    Entry.TransferedToAccounts = false;
                        //    unitOfWork.EntryRepository.Update(Entry);
                        //    foreach (var item in EntryDitails)
                        //    {
                        //        item.EntryID = Entry.EntryID;
                        //        item.EntryDetailID = 0;
                        //        var details = _mapper.Map<EntryDetail>(item);

                        //        unitOfWork.EntryDetailRepository.Insert(details);

                        //    }
                        //}

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


                        return Ok(sellingOrderModel);


                  }


                  //==========================================Second Case OF Code Of Purchase=======================================

                  else
                  {
                      if (Check.Any(m => m.Code == sellingOrder.Code && m.SellingOrderID == id))
                      {
                          unitOfWork.SellingOrderReposetory.Update(sellingOrder);
                          if (OldDetails != null)
                          {
                              unitOfWork.SellingOrderDetailRepository.RemovRange(OldDetails);
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
                            }


                          if (Newdetails != null)
                          {
                              foreach (var item in Newdetails)
                              {
                                  item.SellingOrderID = sellingOrder.SellingOrderID;
                                  item.SellOrderDetailID = 0;
                                  var details = _mapper.Map<SellingOrderDetail>(item);

                                  unitOfWork.SellingOrderDetailRepository.Insert(details);

                              }
                          }


                          //==================================================لا تولد قيد ===================================
                          if (sellingOrderModel.SettingModel.DoNotGenerateEntry == true)
                          {
                              unitOfWork.EntryRepository.Delete(Entry.EntryID);
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

                                return Ok(sellingOrderModel);
                          }
                          //===================================توليد قيد مع ترحيل تلقائي===================================
                          if (sellingOrderModel.SettingModel.AutoGenerateEntry == true)
                          {
                              var EntryDitails = EntriesHelper.UpdateCalculateEntries(portofolioaccount,Entry.EntryID, sellingOrderModel, null, null, null);

                              if (sellingOrderModel.SettingModel.TransferToAccounts == true)
                              {
                                  Entry.TransferedToAccounts = true;
                                  unitOfWork.EntryRepository.Update(Entry);
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
                                      EntryID = x.EntryID,
                                      StocksDebit=x.StocksDebit,
                                      StocksCredit=x.StocksCredit


                                  }).ToList());
                              }
                                else
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
                                }
                            }


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


                            return Ok(sellingOrderModel);

                      }


                  }
                  return Ok(sellingOrderModel);
              }

            // now We Will Create new Entry As Insert
            else
            {
                if (Check.Any(m => m.Code != sellingOrder.Code))
                {
                    unitOfWork.SellingOrderReposetory.Update(sellingOrder);
                    if (OldDetails != null)
                    {
                        unitOfWork.SellingOrderDetailRepository.RemovRange(OldDetails);
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
                        }


                    if (Newdetails != null)
                    {
                        foreach (var item in Newdetails)
                        {
                            item.SellingOrderID = sellingOrder.SellingOrderID;
                            item.SellOrderDetailID = 0;
                            var details = _mapper.Map<SellingOrderDetail>(item);

                            unitOfWork.SellingOrderDetailRepository.Insert(details);

                        }
                    }


                    //==================================================لا تولد قيد ===================================
                    if (sellingOrderModel.SettingModel.DoNotGenerateEntry == true)
                    {

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

                            return Ok(sellingOrderModel);
                    }
                    //===============================================================توليد قيد مع ترحيل تلقائي============================



                    else if (sellingOrderModel.SettingModel.AutoGenerateEntry == true)
                    {
                        var lastEntry = unitOfWork.EntryRepository.Last();
                        var EntryMODEL = EntriesHelper.InsertCalculatedEntries(portofolioaccount,sellingOrderModel, null, null, null, lastEntry);
                        EntryMODEL.SellingOrderID = sellingOrder.SellingOrderID;
                        var Entry = _mapper.Map<Entry>(EntryMODEL);
                        Entry.SellingOrderID = sellingOrder.SellingOrderID;

                        var DetailEnt = EntryMODEL.EntryDetailModel;

                        if (sellingOrderModel.SettingModel.TransferToAccounts == true)
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

                    //================================توليد قيد مع عدم الترحيل======================================

                   //if (sellingOrderModel.SettingModel.GenerateEntry == true)

                   // {
                   //     var lastEntry = unitOfWork.EntryRepository.Last();
                   //     var EntryMODEL = EntriesHelper.InsertCalculatedEntries(portofolioaccount,sellingOrderModel, null, null, null, lastEntry);
                   //     EntryMODEL.SellingOrderID = sellingOrder.SellingOrderID;
                   //     var Entry = _mapper.Map<Entry>(EntryMODEL);
                   //     Entry.SellingOrderID = sellingOrder.SellingOrderID;

                   //     var DetailEnt = EntryMODEL.EntryDetailModel;
                   //     Entry.TransferedToAccounts = false;
                   //     unitOfWork.EntryRepository.Insert(Entry);
                   //     foreach (var item in DetailEnt)
                   //     {
                   //         item.EntryID = Entry.EntryID;
                   //         item.EntryDetailID = 0;
                   //         var details = _mapper.Map<EntryDetail>(item);

                   //         unitOfWork.EntryDetailRepository.Insert(details);

                   //     }
                   // }


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


                        return Ok(sellingOrderModel);


                }


                //==========================================Second Case OF Code Of Purchase=======================================

                else
                {
                    if (Check.Any(m => m.Code == sellingOrder.Code && m.SellingOrderID == id))
                    {
                        unitOfWork.SellingOrderReposetory.Update(sellingOrder);
                        if (OldDetails != null)
                        {
                            unitOfWork.SellingOrderDetailRepository.RemovRange(OldDetails);
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
                            }


                        if (Newdetails != null)
                        {
                            foreach (var item in Newdetails)
                            {
                                item.SellingOrderID = sellingOrder.SellingOrderID;
                                item.SellOrderDetailID = 0;
                                var details = _mapper.Map<SellingOrderDetail>(item);

                                unitOfWork.SellingOrderDetailRepository.Insert(details);

                            }
                        }


                        //==================================================لا تولد قيد ===================================
                        if (sellingOrderModel.SettingModel.DoNotGenerateEntry == true)
                        {

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

                                return Ok(sellingOrderModel);
                        }
                        //===============================================================توليد قيد مع ترحيل تلقائي============================



                        else if (sellingOrderModel.SettingModel.AutoGenerateEntry == true)
                        {
                            var lastEntry = unitOfWork.EntryRepository.Last();
                            var EntryMODEL = EntriesHelper.InsertCalculatedEntries(portofolioaccount,sellingOrderModel, null, null, null, lastEntry);
                            var Entry = _mapper.Map<Entry>(EntryMODEL);
                            Entry.SellingOrderID = sellingOrder.SellingOrderID;

                            var DetailEnt = EntryMODEL.EntryDetailModel;

                            if (sellingOrderModel.SettingModel.TransferToAccounts == true)
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
                                Entry.TransferedToAccounts = true;
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
                        ////================================توليد قيد مع عدم الترحيل====================================== 
                        //if (sellingOrderModel.SettingModel.GenerateEntry == true)

                        //{
                        //    var lastEntry = unitOfWork.EntryRepository.Last();
                        //    var EntryMODEL = EntriesHelper.InsertCalculatedEntries(portofolioaccount,sellingOrderModel, null, null, null, lastEntry);
                        //    var Entry = _mapper.Map<Entry>(EntryMODEL);
                        //    Entry.SellingOrderID = sellingOrder.SellingOrderID;

                        //    var DetailEnt = EntryMODEL.EntryDetailModel;
                        //    Entry.TransferedToAccounts = false;
                        //    unitOfWork.EntryRepository.Insert(Entry);
                        //    foreach (var item in DetailEnt)
                        //    {
                        //        item.EntryID = Entry.EntryID;
                        //        item.EntryDetailID = 0;
                        //        var details = _mapper.Map<EntryDetail>(item);

                        //        unitOfWork.EntryDetailRepository.Insert(details);

                        //    }
                        //}


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



                            return Ok(sellingOrderModel);
                    }


                }


                return Ok(sellingOrderModel);

            }



            }



            else
            {
                return Ok(3);

            }


        }

        [HttpDelete]
        [Route("~/api/SellingOrder/DeleteSelling/{id}")]
        public IActionResult DeleteSelling(int? id)
        {

            if (id == null)
            {

                return Ok(1);

            }
            var modelSelling = unitOfWork.SellingOrderReposetory.GetByID(id);
            if (modelSelling == null)
            {
                return Ok(0);

            }
            var Details = unitOfWork.SellingOrderDetailRepository.Get(filter: m => m.SellingOrderID == id);
            #region Warehouse
            //Cancel Selling Order From Stocks 
            _stocksHelper.CancelSellingFromStocks(modelSelling.PortfolioID, Details);
            #endregion
            unitOfWork.SellingOrderDetailRepository.RemovRange(Details);
            //var Entry = unitOfWork.EntryRepository.Get(filter: x=> x.SellingOrderID==id).SingleOrDefault();
            //var EntryDetails = unitOfWork.EntryDetailRepository.Get(filter: a=> a.EntryID==Entry.EntryID); 
            //if (Entry.TransferedToAccounts==true)
            //{
            //    accountingHelper.CancelTransferToAccounts(EntryDetails.ToList());
            //}
            //unitOfWork.EntryDetailRepository.RemovRange(EntryDetails);
            //unitOfWork.EntryRepository.Delete(Entry.EntryID);

            unitOfWork.SellingOrderReposetory.Delete(id);
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

        }

    }
}


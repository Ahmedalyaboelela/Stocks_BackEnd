using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BAL.Helper;
using BAL.Model;
using BAL.Repositories;
using DAL.Context;
using DAL.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace Stocks.Controllers
{
    public class IOSAndroidController : ControllerBase
    {
        private UnitOfWork unitOfWork;
        private readonly IMapper _mapper;
        internal StocksContext Context;
        private readonly IAccountingHelper accountingHelper;
        private readonly IStocksHelper _stocksHelper;

        public IOSAndroidController(StocksContext context, IMapper mapper, IStocksHelper stocksHelper)
        {
            this.unitOfWork = new UnitOfWork(context);
            this._mapper = mapper;
            Context = context;
            accountingHelper = new AccountingHelper(context, mapper);
            _stocksHelper = stocksHelper;


        }
        [Route("~/api/IOSAndroid/GetAllPortfolios/{pageNumber}")]

        public IEnumerable<PortfolioModel> GetAllIOSAndroid(int pageNumber)
        {
            var model = unitOfWork.PortfolioRepository.GetMobilApp(page:pageNumber).Select(m => new PortfolioModel {
                Code = m.Code,
                Description = m.Description,
                EstablishDate = m.EstablishDate.Value.ToString("d/M/yyyy"),
                EstablishDateHijri = DateHelper.GetHijriDate(m.EstablishDate),
                NameAR = m.NameAR,
                NameEN = m.NameEN,
                PortfolioID = m.PortfolioID,
                AccountID = unitOfWork.PortfolioAccountRepository.GetEntity(x => x.PortfolioID == m.PortfolioID).AccountID,
                AccountCode = unitOfWork.PortfolioAccountRepository.GetEntity(x => x.PortfolioID == m.PortfolioID).Account.Code,
                AccountNameAR = unitOfWork.PortfolioAccountRepository.GetEntity(x => x.PortfolioID == m.PortfolioID).Account.NameAR,
                AccountNameEN = unitOfWork.PortfolioAccountRepository.GetEntity(x => x.PortfolioID == m.PortfolioID).Account.NameEN,
                RSBalance= unitOfWork.PortfolioAccountRepository.GetEntity(x => x.PortfolioID == m.PortfolioID).Account.Debit - unitOfWork.PortfolioAccountRepository.GetEntity(x => x.PortfolioID == m.PortfolioID).Account.Credit,
                 TotalStocksCount =m.TotalStocksCount,

                portfolioOpeningStocksArray = unitOfWork.PortfolioOpeningStocksRepository.Get(filter: s => s.PortfolioID == m.PortfolioID).Select(q => new PortfolioOpeningStocksModel
                {
                    PartnerID = q.PartnerID,
                    OpeningStocksCount = q.OpeningStocksCount,
                    OpeningStockValue = q.OpeningStockValue,
                    PartnerCode = q.Partner.Code,
                    PartnerNameAR = q.Partner.NameAR,
                    PartnerNameEN = q.Partner.NameEN,
                    PortfolioID = q.PortfolioID,
                    PortOPenStockID = q.PortOPenStockID,

                }),



            });
            return model;
        }



        [HttpGet]
        [Route("~/api/IOSAndroid/GetPortfolioById/{id}")]

        public IActionResult GetPortfolioById(int id)
        {

            if (id > 0)
            {
                var portfolio = unitOfWork.PortfolioRepository.GetByID(id);
                var model = _mapper.Map<PortfolioModel>(portfolio);
                if (model == null)
                {
                    return Ok(model);
                }


                if (portfolio.EstablishDate != null)
                {
                    model.EstablishDate = portfolio.EstablishDate.Value.ToString("d/M/yyyy");
                }

                  model.EstablishDateHijri = DateHelper.GetHijriDate(portfolio.EstablishDate);


                var PortAccount = unitOfWork.PortfolioAccountRepository

                    .GetEntity(filter: m => m.PortfolioID == portfolio.PortfolioID);

                if (PortAccount != null)
                {

                    model.AccountID = PortAccount.AccountID;
                    model.AccountCode = PortAccount.Account.Code;
                    model.AccountNameAR = PortAccount.Account.NameAR;
                    model.AccountNameEN = PortAccount.Account.NameEN;
                    model.RSBalance = PortAccount.Account.Debit - PortAccount.Account.Credit;
                }





                var OpeningStocks = unitOfWork.PortfolioOpeningStocksRepository

                    .Get(filter: m => m.PortfolioID == portfolio.PortfolioID)
                    .Select(m => new PortfolioOpeningStocksModel
                    {
                        PortOPenStockID = m.PortOPenStockID,
                        OpeningStocksCount = m.OpeningStocksCount,
                        OpeningStockValue = m.OpeningStockValue,
                        PartnerID = m.PartnerID,
                        PartnerCode = m.Partner.Code,
                        PartnerNameAR = m.Partner.NameAR,
                        PartnerNameEN = m.Partner.NameEN,
                        PortfolioID = m.PortfolioID,
                        PortfolioCode = m.Portfolio.Code,
                        PortfolioNameAR = m.Portfolio.NameAR,
                        PortfolioNameEN = m.Portfolio.NameEN,


                    });

                var currentstocks = unitOfWork.PortfolioTransactionsRepository.Get(filter: m => m.PortfolioID == portfolio.PortfolioID)
                    .Select(m => new PortfolioTransactionModel
                    {
                        PortTransID = m.PortTransID,
                        CurrentStocksCount = m.CurrentStocksCount,
                        CurrentStockValue = m.CurrentStockValue,
                        PartnerID = m.PartnerID,
                        partenerCode = m.Partner.Code,
                        partenerNameAR = m.Partner.NameAR,
                        partenerNameEN = m.Partner.NameEN,
                        PortfolioID = m.PortfolioID,



                    });


                if (OpeningStocks != null)
                {
                    model.portfolioOpeningStocksArray = OpeningStocks;

                }
                if (currentstocks != null)
                {
                    model.TotalStocksCount = 0;
                    model.RSBalance = 0;
                    foreach (var item in currentstocks)
                    {

                        model.TotalStocksCount += item.CurrentStocksCount;
                    }

                    foreach (var item2 in currentstocks)
                    {
                        model.RSBalance += item2.CurrentStockValue;

                    }


                    return Ok(model);


                }
                else
                    return Ok(1);
            }
            else
            {
                return Ok(3);
            }
        }

        [HttpGet]
        [Route("~/api/IOSAndroid/GetAllportEmpSelling/{EmpID}")]

        public IEnumerable<SellingOrderModel> GetAllportEmpSelling(int EmpID)
        {
            var model = unitOfWork.SellingOrderReposetory.Get(filter: x => x.EmployeeID == EmpID).Select(m => new SellingOrderModel
            {
                Code = m.Code,
                EmployeeID = m.EmployeeID,
                EmpNameAR = unitOfWork.EmployeeRepository.GetEntity(x => x.EmployeeID == EmpID).NameAR,
                EmpCode = unitOfWork.EmployeeRepository.GetEntity(x => x.EmployeeID == EmpID).Code,
                EmpNameEN = unitOfWork.EmployeeRepository.GetEntity(x => x.EmployeeID == EmpID).NameEN,
                PayWay = m.PayWay,
                PortfolioID = m.PortfolioID,
                SellDate = m.Date.Value.ToString("d/m/yyyy"),
                SellDateHijri=DateHelper.GetHijriDate(m.Date),
                SellingOrderID=m.SellingOrderID,
                PortfolioCode=unitOfWork.PortfolioRepository.GetEntity(filter:x=> x.PortfolioID==m.PortfolioID).Code,
                PortfolioNameAR= unitOfWork.PortfolioRepository.GetEntity(filter: x => x.PortfolioID == m.PortfolioID).NameAR,
                PortfolioNameEN= unitOfWork.PortfolioRepository.GetEntity(filter: x => x.PortfolioID == m.PortfolioID).NameEN,
                DetailsModels= unitOfWork.SellingOrderDetailRepository.Get(filter: x => x.SellingOrderID == m.SellingOrderID).Select(a=> new SelingOrderDetailsModel {
                BankCommission=a.BankCommission,
                SellingOrderID=a.SellingOrderID,
                BankCommissionRate=a.BankCommissionRate,
                NetAmmount=a.NetAmmount,
                PartnerID=a.PartnerID,
                PartnerCode=unitOfWork.PartnerRepository.GetEntity(filter:q=> q.PartnerID==a.PartnerID).Code,
                PartnerNameAR= unitOfWork.PartnerRepository.GetEntity(filter: q => q.PartnerID == a.PartnerID).NameAR,
                PartnerNameEN= unitOfWork.PartnerRepository.GetEntity(filter: q => q.PartnerID == a.PartnerID).NameEN,
                SelingValue=a.SelingValue,
                SellingPrice=a.SellingPrice,
                SellOrderDetailID=a.SellOrderDetailID,
                StockCount=a.StockCount,
                TaxOnCommission=a.TaxOnCommission,
                TaxRateOnCommission=a.TaxRateOnCommission,
                
                }),
                





            });
            return model;
        }





        [Route("~/api/IOSAndroid/GetAllportEmppurchase/{EmpID}")]
        public IEnumerable<PurchaseOrderModel> GetAllportEmppurchase(int EmpID)
        {
            var model = unitOfWork.PurchaseOrderRepository.Get(filter: x => x.EmployeeID == EmpID).Select(m => new PurchaseOrderModel
            {
                Code = m.Code,
                EmployeeID = m.EmployeeID,
                EmpNameAR = unitOfWork.EmployeeRepository.GetEntity(x => x.EmployeeID == EmpID).NameAR,
                EmpCode = unitOfWork.EmployeeRepository.GetEntity(x => x.EmployeeID == EmpID).Code,
                EmpNameEN = unitOfWork.EmployeeRepository.GetEntity(x => x.EmployeeID == EmpID).NameEN,
                PayWay = m.PayWay,
                PortfolioID = m.PortfolioID,
                PurchaseDate = m.Date.Value.ToString("d/m/yyyy"),
                PurchaseDateHijri = DateHelper.GetHijriDate(m.Date),
                PurchaseOrderID = m.PurchaseOrderID,
                PortfolioCode = unitOfWork.PortfolioRepository.GetEntity(filter: x => x.PortfolioID == m.PortfolioID).Code,
                PortfolioNameAR = unitOfWork.PortfolioRepository.GetEntity(filter: x => x.PortfolioID == m.PortfolioID).NameAR,
                PortfolioNameEN = unitOfWork.PortfolioRepository.GetEntity(filter: x => x.PortfolioID == m.PortfolioID).NameEN,
                DetailsModels = unitOfWork.PurchaseOrderDetailRepository.Get(filter: x => x.PurchaseID == m.PurchaseOrderID).Select(a => new PurchaseOrderDetailModel
                {
                    BankCommission = a.BankCommission,
                    PurchaseID = a.PurchaseID,
                    BankCommissionRate = a.BankCommissionRate,
                    NetAmmount = a.NetAmmount,
                    PartnerID = a.PartnerID,
                    PartnerCode = unitOfWork.PartnerRepository.GetEntity(filter: q => q.PartnerID == a.PartnerID).Code,
                    PartnerNameAR = unitOfWork.PartnerRepository.GetEntity(filter: q => q.PartnerID == a.PartnerID).NameAR,
                    PartnerNameEN = unitOfWork.PartnerRepository.GetEntity(filter: q => q.PartnerID == a.PartnerID).NameEN,
                    PurchaseValue = a.PurchaseValue,
                    PurchasePrice = a.PurchasePrice,
                    PurchaseOrderDetailID = a.PurchaseOrderDetailID,
                    StockCount = a.StockCount,
                    TaxOnCommission = a.TaxOnCommission,
                    TaxRateOnCommission = a.TaxRateOnCommission,

                }),






            });
            return model;
        }







        [Route("~/api/IOSAndroid/GetAllEmps")]
        public IActionResult GetAllEmps()
        {
            var Emps = unitOfWork.EmployeeRepository.Get(filter: x => x.IsInternal == true).Select(m => new EmployeeModel {

                EmployeeID=m.EmployeeID,
                Code=m.Code,
                NameAR=m.NameAR,
                NameEN=m.NameEN,


            });
            
            if (Emps == null)
            {
                return Ok(0);
            }

            return Ok(Emps);
        }














        [HttpGet]
        [Route("~/api/IOSAndroid/CodePur")]
        public string CodePur()
        {
            
          
                 var LastCode = unitOfWork.PurchaseOrderRepository.Last().Code; 

            if (LastCode == null)
            {
                LastCode = "1";
            }
               
            
          
        

            return LastCode;
        }


        [HttpGet]
        [Route("~/api/IOSAndroid/CodeSell")]
        public string CodeSell()
        {

            var LastCode = unitOfWork.SellingOrderReposetory.Last().Code;

            if (LastCode == null)
            {
                LastCode = "1";
            }





            return LastCode;




            
        }


        [Route("~/api/IOSAndroid/GetAllports")]
        public IEnumerable<GetAllPortsIOS> GetAllports()
        {
            var model = unitOfWork.PortfolioRepository.Get().Select(m => new GetAllPortsIOS
            {
                PortfolioID=m.PortfolioID,
                Code = m.Code,
                NameAR=m.NameAR,
                NameEN=m.NameEN,
                PortfolioAccount=unitOfWork.PortfolioAccountRepository.GetEntity(filter:x=>x.PortfolioID==m.PortfolioID).AccountID
            });
            return model;
        }

        [Route("~/api/IOSAndroid/GetpartenersByport/{portID}")]
        public IEnumerable<PortfolioTransactionModel> GetAllparteners(int portID)
        {
            var model = unitOfWork.PortfolioTransactionsRepository.Get(filter:x=> x.PortfolioID==portID).Select(m => new PortfolioTransactionModel
            {
                PortfolioID = portID,
               CurrentStocksCount=m.CurrentStocksCount,
               CurrentStockValue=m.CurrentStockValue,
               PartnerID=m.PartnerID,
               partenerCode=unitOfWork.PartnerRepository.GetEntity(filter: a=> a.PartnerID==m.PartnerID).Code,
               partenerNameAR= unitOfWork.PartnerRepository.GetEntity(filter: a => a.PartnerID == m.PartnerID).NameAR,
               partenerNameEN= unitOfWork.PartnerRepository.GetEntity(filter: a => a.PartnerID == m.PartnerID).NameEN,
               PortTransID=m.PortTransID
            });
            return model;
        }



        [HttpPost]
        [Route("~/api/IOSAndroid/Purchase")]
        public IActionResult Purchase([FromBody] PurchaseOrderModel purchaseOrderModel)
        {
            if (ModelState.IsValid)
            {
                var Check = unitOfWork.PurchaseOrderRepository.Get();
                if (Check.Any(m => m.Code == purchaseOrderModel.Code))
                {

                    return Ok(2);
                }
                else
                {
                    purchaseOrderModel.SettingModel = unitOfWork.SettingRepository.Get(filter: x => x.VoucherType == 2).Select(m => new SettingModel {
                        VoucherType=2,
                        AutoGenerateEntry=m.AutoGenerateEntry,
                        Code=m.Code,
                        DoNotGenerateEntry=m.DoNotGenerateEntry,
                        GenerateEntry=m.GenerateEntry,
                        SettingID=m.SettingID,
                        TransferToAccounts=m.TransferToAccounts,
                        SettingAccs=unitOfWork.SettingAccountRepository.Get(filter: x=>x.SettingID==m.SettingID).Select(a=> new SettingAccountModel {
                            SettingID=a.SettingID,
                            AccCode=a.Account.Code,
                            AccNameAR=a.Account.NameAR,
                            AccNameEN=a.Account.NameEN,
                            AccountID=a.AccountID,
                            SettingAccountID=a.SettingAccountID,
                            AccountType=a.AccountType,
                            
                        })

                    }).SingleOrDefault();

                    var purchaseOrder = _mapper.Map<PurchaseOrder>(purchaseOrderModel);
                    int portofolioaccount = unitOfWork.PortfolioAccountRepository.Get(filter: m => m.PortfolioID == purchaseOrderModel.PortfolioID && m.Type == true).Select(m => m.AccountID).SingleOrDefault();


                    var Details = purchaseOrderModel.DetailsModels;

                    unitOfWork.PurchaseOrderRepository.Insert(purchaseOrder);
                    if (Details != null && Details.Count() > 0)
                    {
                        foreach (var item in Details)
                        {
                            PurchaseOrderDetailModel purchaseOrderDetailModel = new PurchaseOrderDetailModel();
                            purchaseOrderDetailModel.PurchaseID = purchaseOrder.PurchaseOrderID;
                            purchaseOrderDetailModel.NetAmmount = item.NetAmmount;
                            purchaseOrderDetailModel.StockCount = item.StockCount;
                            purchaseOrderDetailModel.TaxOnCommission = item.TaxOnCommission;
                            purchaseOrderDetailModel.TaxRateOnCommission = item.TaxRateOnCommission;
                            purchaseOrderDetailModel.BankCommission = item.BankCommission;
                            purchaseOrderDetailModel.BankCommissionRate = item.BankCommissionRate;
                            purchaseOrderDetailModel.PurchaseValue = item.PurchaseValue;
                            purchaseOrderDetailModel.PurchasePrice = item.PurchasePrice;
                            purchaseOrderDetailModel.PartnerID = item.PartnerID;
                            var details = _mapper.Map<PurchaseOrderDetail>(purchaseOrderDetailModel);
                            unitOfWork.PurchaseOrderDetailRepository.Insert(details);

                        }
                    }


                    #region Warehouse
                    // Add Purchase Order Stocks Count To Portofolio
                    _stocksHelper.TransferPurchaseToStocks(purchaseOrderModel);
                    #endregion
                    //==================================================لا تولد قيد ===================================
                    if (purchaseOrderModel.SettingModel.DoNotGenerateEntry == true)
                    {
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

                    //===============================================================توليد قيد مع ترحيل تلقائي============================



                    else if (purchaseOrderModel.SettingModel.AutoGenerateEntry == true)
                    {
                        var lastEntry = unitOfWork.EntryRepository.Last();
                        var EntryMODEL = EntriesHelper.InsertCalculatedEntries(portofolioaccount, null, purchaseOrderModel, null, null, lastEntry);
                        EntryMODEL.PurchaseOrderID = purchaseOrder.PurchaseOrderID;
                        var Entry = _mapper.Map<Entry>(EntryMODEL);
                        var DetailEnt = EntryMODEL.EntryDetailModel;

                        if (purchaseOrderModel.SettingModel.TransferToAccounts == true)
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


        [HttpPost]
        [Route("~/api/IOSAndroid/Selling")]
        public IActionResult Selling([FromBody] SellingOrderModel sellingOrderModel)
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


                    sellingOrderModel.SettingModel = unitOfWork.SettingRepository.Get(filter: x => x.VoucherType == 1).Select(m => new SettingModel
                    {
                        VoucherType = 1,
                        AutoGenerateEntry = m.AutoGenerateEntry,
                        Code = m.Code,
                        DoNotGenerateEntry = m.DoNotGenerateEntry,
                        GenerateEntry = m.GenerateEntry,
                        SettingID = m.SettingID,
                        TransferToAccounts = m.TransferToAccounts,
                        SettingAccs = unitOfWork.SettingAccountRepository.Get(filter: x => x.SettingID == m.SettingID).Select(a => new SettingAccountModel
                        {
                            SettingID = a.SettingID,
                            AccCode = a.Account.Code,
                            AccNameAR = a.Account.NameAR,
                            AccNameEN = a.Account.NameEN,
                            AccountID = a.AccountID,
                            SettingAccountID = a.SettingAccountID,
                            AccountType = a.AccountType,

                        })

                    }).SingleOrDefault();
                    portofolioaccount = unitOfWork.PortfolioAccountRepository.Get(filter: m => m.PortfolioID == sellingOrderModel.PortfolioID && m.Type == true).Select(m => m.AccountID).SingleOrDefault();
                    var modelselling = _mapper.Map<SellingOrder>(sellingOrderModel);


                    var Details = sellingOrderModel.DetailsModels;

                    unitOfWork.SellingOrderReposetory.Insert(modelselling);
                    if (Details != null && Details.Count() > 0)
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
                    var Chk = _stocksHelper.CheckStockCountForSelling(sellingOrderModel);
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

                        var EntryMODEL = EntriesHelper.InsertCalculatedEntries(portofolioaccount, sellingOrderModel, null, null, null, lastEntry);
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


















        







    }





























}





using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BAL.Helper;
using BAL.Model;
using BAL.Repositories;
using DAL.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Stocks.Controllers
{
    public class IOSAndroidController : Controller
    {
        private UnitOfWork unitOfWork;
        private readonly IMapper _mapper;
        internal StocksContext Context;

        public IOSAndroidController(StocksContext context, IMapper mapper)
        {
            this.unitOfWork = new UnitOfWork(context);
            this._mapper = mapper;
            Context = context;


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
        [Route("~/api/IOSAndroid/GetbyID/{id}")]

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







    }






}





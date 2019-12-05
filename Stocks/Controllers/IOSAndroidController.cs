using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
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
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace Stocks.Controllers
{
    [Authorize(Roles = "SuperAdmin,Admin,Employee")]
    [Route("api/[controller]")]
    [ApiController]
    public class IOSAndroidController : ControllerBase
    {
        private UnitOfWork unitOfWork;
        private readonly IMapper _mapper;
        internal StocksContext Context;
        private readonly IAccountingHelper accountingHelper;
        private readonly IStocksHelper _stocksHelper;
        private readonly ApplicationSettings _appSettings;
        private LoggerHistory loggerHistory;
        public IOSAndroidController(StocksContext context, IMapper mapper, IStocksHelper stocksHelper, IOptions<ApplicationSettings> appSettings)
        {
            this.unitOfWork = new UnitOfWork(context);
            this._mapper = mapper;
            Context = context;
            accountingHelper = new AccountingHelper(context, mapper);
            _stocksHelper = stocksHelper;
            _appSettings = appSettings.Value;
            loggerHistory = new LoggerHistory(context, mapper);
           

        }

        #region GetAllPortfolios{pageNumber}
        [HttpGet]
        [Route("~/api/IOSAndroid/GetAllPortfolios/{pageNumber}")]
        public IEnumerable<PortfolioModel> GetAllPortfolios(int pageNumber)
        {
            if ( unitOfWork.PortfolioRepository.GetMobilApp(page: pageNumber).Count() !=0)
            {
                var model = unitOfWork.PortfolioRepository.GetMobilApp(page: pageNumber).Select(m => new PortfolioModel
                {
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
                    RSBalance = unitOfWork.PortfolioAccountRepository.GetEntity(x => x.PortfolioID == m.PortfolioID).Account.Debit - unitOfWork.PortfolioAccountRepository.GetEntity(x => x.PortfolioID == m.PortfolioID).Account.Credit,
                    TotalStocksCount = m.TotalStocksCount,

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
            else
            {
                return null;
            }
          
        }
        #endregion

        #region GetPortfolioById
        [HttpGet]
        [Route("~/api/IOSAndroid/GetPortfolioById/{id}")]

        public IActionResult GetPortfolioById(int id)
        {

            if (id > 0)
            { 
                if (unitOfWork.PortfolioRepository.GetByID(id)==null)
                {
                    return null;
                }
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
                return null;
            }
        }
        #endregion

        #region GetAllEmps
        [HttpGet]
        [Route("~/api/IOSAndroid/GetAllEmps")]
        public IActionResult GetAllEmps()
        { 
            if ( unitOfWork.EmployeeRepository.Get().Count() !=0)
            {
                var Emps = unitOfWork.EmployeeRepository.Get(filter: x => x.IsInternal == true).Select(m => new EmployeeModel
                {

                    EmployeeID = m.EmployeeID,
                    Code = m.Code,
                    NameAR = m.NameAR,
                    NameEN = m.NameEN,


                });

                if (Emps == null)
                {
                    return Ok(0);
                }

                return Ok(Emps);
            }
            else
            {
                return null;
            }
           
        }
        #endregion


        #region GetAllPortfolios
        [HttpGet]
        [Route("~/api/IOSAndroid/GetAllports")] 
        public IEnumerable<GetAllPortsIOS> GetAllports()
        { 
            if (unitOfWork.PortfolioRepository.Get().Count()!=0)
            {
                var model = unitOfWork.PortfolioRepository.Get().Select(m => new GetAllPortsIOS
                {
                    PortfolioID = m.PortfolioID,
                    Code = m.Code,
                    NameAR = m.NameAR,
                    NameEN = m.NameEN,
                    PortfolioAccount = unitOfWork.PortfolioAccountRepository.GetEntity(filter: x => x.PortfolioID == m.PortfolioID).AccountID
                });
                return model;
            }
            else
            {
                return null;
            }
           
        }

        #endregion

        #region GetpartenersByport
        [HttpGet]
        [Route("~/api/IOSAndroid/GetpartenersByport/{portID}")]
        public IEnumerable<PortfolioTransactionModel> GetAllparteners(int portID)
        {
            if (unitOfWork.PortfolioTransactionsRepository.Get(filter: x => x.PortfolioID == portID).Count() !=0)
            {
                var model = unitOfWork.PortfolioTransactionsRepository.Get(filter: x => x.PortfolioID == portID).Select(m => new PortfolioTransactionModel
                {
                    PortfolioID = portID,
                    CurrentStocksCount = m.CurrentStocksCount,
                    CurrentStockValue = m.CurrentStockValue,
                    PartnerID = m.PartnerID,
                    partenerCode = unitOfWork.PartnerRepository.GetEntity(filter: a => a.PartnerID == m.PartnerID).Code,
                    partenerNameAR = unitOfWork.PartnerRepository.GetEntity(filter: a => a.PartnerID == m.PartnerID).NameAR,
                    partenerNameEN = unitOfWork.PartnerRepository.GetEntity(filter: a => a.PartnerID == m.PartnerID).NameEN,
                    PortTransID = m.PortTransID
                });
                return model;
            }
            else
            {
                return null;
            }
           
        }
        #endregion

        #region GetAllsellingOrders{pageNumber}
        [HttpGet]
        [Route("~/api/IOSAndroid/GetAllsellingOrders/{pageNumber}")]
        public IEnumerable<SellingOrderModel> GetAllsellingOrders(int pageNumber)
        {
            if (unitOfWork.SellingOrderRepository.GetMobilApp(page: pageNumber).Count() != 0)
            {
                var model = unitOfWork.SellingOrderRepository.GetMobilApp(page: pageNumber).Select(m => new SellingOrderModel
                {
                    SellingOrderID = m.SellingOrderID,
                    Code = m.Code,
                   
                });
                return model;
            }
            else
            {
                return null;
            }

        }
        #endregion

        #region GetAllPurchaseOrders{pageNumber}
        [HttpGet]
        [Route("~/api/IOSAndroid/GetAllPurchaseOrders/{pageNumber}")]
        public IEnumerable<PurchaseOrderModel> GetAllPurchaseOrders(int pageNumber)
        {
            if (unitOfWork.PurchaseOrderRepository.GetMobilApp(page: pageNumber).Count() != 0)
            {
                var model = unitOfWork.PurchaseOrderRepository.GetMobilApp(page: pageNumber).Select(m => new PurchaseOrderModel
                {
                    PurchaseOrderID = m.PurchaseOrderID,
                    Code = m.Code,
                    
                });
                return model;
            }
            else
            {
                return null;
            }

        }
        #endregion

        #region GetAllSellingInvoice{pageNumber}
        [HttpGet]
        [Route("~/api/IOSAndroid/GetAllSelingInvoice/{pageNumber}")]
        public IEnumerable<SellingInvoiceModel> GetAllSelingInvoice(int pageNumber)
        {
            if (unitOfWork.SellingInvoiceReposetory.GetMobilApp(page: pageNumber).Count() != 0)
            {
                var model = unitOfWork.SellingInvoiceReposetory.GetMobilApp(page: pageNumber).Select(m => new SellingInvoiceModel
                {
                    SellingInvoiceID = m.SellingInvoiceID,
                    Code = m.Code,
                   
                });
                return model;

            }
            else
            {
                return null;
            }
        }


        #endregion


        #region GetAllPurchaseInvoice{pageNumber}
        [HttpGet]
        [Route("~/api/IOSAndroid/GetAllPurchaseInvoice/{pageNumber}")]
        public IEnumerable<PurchaseInvoiceModel> GetAllPurchaseInvoice(int pageNumber)
        {
            if (unitOfWork.PurchaseInvoiceRepository.GetMobilApp(page: pageNumber).Count() != 0)
            {
                var model = unitOfWork.PurchaseInvoiceRepository.GetMobilApp(page: pageNumber).Select(m => new PurchaseInvoiceModel
                {
                    PurchaseInvoiceID = m.PurchaseInvoiceID,
                    Code = m.Code,
                   newRialBalance= GetAllNetAmounts(m.PurchaseInvoiceID,m.PurchaseOrder.PortfolioID),
                    






            });
                return model;

            }
            else
            {
                    return null;
            }
        }

     public decimal?   GetAllNetAmounts( int PurchaseInvoiceID,int PortfolioID) {

            decimal? oldNetAmmount = 0.0m;
          var  Details = unitOfWork.PurchaseInvoiceDetailRepository.Get(filter: a => a.PurchaseInvoiceID == PurchaseInvoiceID);
            foreach (var item in Details)
            {
                oldNetAmmount += item.NetAmmount;

            }
            decimal? newRialBalance = _stocksHelper.RialBalancUpdate(PortfolioID, oldNetAmmount);
            return newRialBalance;
        }
        #endregion

        #region GetAllParteners
        [HttpGet]
        [Route("~/api/IOSAndroid/GetAllParteners")]
        public IEnumerable<PartenerModel> GetAllParteners()
        { 
            if (unitOfWork.PartnerRepository.Get().Count()==0)
            {
                return null;
            }
            var parteners = unitOfWork.PartnerRepository.Get().Select(m=> new PartenerModel{
            
                PartnerID=m.PartnerID,
                NameAR=m.NameAR,
                NameEN=m.NameEN,
                Code=m.Code,
});
            return parteners;

        }
        #endregion

        #region  FirstPurchaseInvoice
        [HttpGet]
        [Route("~/api/IOSAndroid/CodePurchaseInvoice")]
        public string CodePur()
        {
            var LastCode = "";
            if (unitOfWork.PurchaseInvoiceRepository.Count() != 0)
            {
                LastCode = unitOfWork.PurchaseInvoiceRepository.Last().Code;

            }
            return LastCode;
        }
        #endregion

        #region FirstOpenPurchaseOrder
        [HttpGet]
        [Route("~/api/IOSAndroid/CodePurchaseOrder")]
        public IActionResult CodePurchaseOrder()
        {
            var LastCode = "";


            if (unitOfWork.PurchaseOrderRepository.Count() != 0)
            {

                LastCode = unitOfWork.PurchaseOrderRepository.Last().Code;
            }


            return Ok(LastCode);
        }
        #endregion

        #region  FirstOpensellingInvoice
        [HttpGet]
        [Route("~/api/IOSAndroid/CodeSellingInvoice")]
        public string CodeSell()
        {

            var LastCode = "";
            if (unitOfWork.SellingInvoiceReposetory.Count() != 0)
            {
                LastCode = unitOfWork.SellingInvoiceReposetory.Last().Code;

            }

            return LastCode;


        }
        #endregion

        #region FirstOpenSellingOrder
        [HttpGet]
        [Route("~/api/IOSAndroid/CodeSellingOrder")]
        public IActionResult CodeSellingOrder()
        {

            var LastCode = "";


            if (unitOfWork.SellingOrderRepository.Count() != 0)
            {

                LastCode = unitOfWork.SellingOrderRepository.Last().Code;
            }


            return Ok(LastCode);
        }

        #endregion


        #region GetCompoPurchasesOrders
        [HttpGet]
        [Route("~/api/IOSAndroid/GetCompoPurchasesOrders")]
        public IEnumerable<PurchaseComboList> GetAllpurchases()
        { 
            if (unitOfWork.PurchaseInvoiceRepository.Get().Count()==0)
            {
                return null;
            }
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
        #endregion

        #region Get CompoListSellingOrders
        [HttpGet]
        [Route("~/api/IOSAndroid/CompoListSellingOrders")]
        public IEnumerable<sellingComboList> GetAllSelling()
        { 
            if (unitOfWork.SellingInvoiceReposetory.Get().Count()==0)
            {
                return null;
            }
            var checks = unitOfWork.SellingInvoiceReposetory.Get();
            List<sellingComboList> sellings = unitOfWork.SellingOrderRepository.Get().Select(x => new sellingComboList
            {

                Code = x.Code,
                SellingOrderID = x.SellingOrderID,


            }).ToList();
            List<sellingComboList> lists = new List<sellingComboList>();
            foreach (var item in sellings)
            {
                if (!checks.Any(m => m.SellingOrderID == item.SellingOrderID))
                {
                    lists.Add(item);
                }
                if (checks.Any(m => m.SellingOrderID == item.SellingOrderID &&
                 m.SellingOrder.OrderType == true))
                {
                    lists.Add(item);
                }
            }


            return lists;
        }
        #endregion


        #region Get PurchaseInvoices by orderID
        [HttpGet]
        [Route("~/api/IOSAndroid/PurchaseInvoicesbyorderID/{id}")]
        public List<PurchaseInvoiceDetailModel> getpurchaseInvoise(int? id)
        {

            string Con = _appSettings.Report_Connection;


            using (SqlConnection cnn = new SqlConnection(Con))
            {
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = cnn;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"SELECT  PurchaseInvoices.Code ,
               CONVERT(DATE, PurchaseInvoices.Date, 110)   ,
               Partners.NameAR ,
		       PurchaseInvoiceDetails.StockCount ,
		       PurchaseInvoiceDetails.PurchasePrice ,
               PurchaseInvoiceDetails.NetAmmount  ,
		       SUM(PurchaseInvoiceDetails.StockCount) OVER(ORDER BY PurchaseInvoices.Date ROWS BETWEEN UNBOUNDED PRECEDING AND 0 PRECEDING) as StockBalance
               FROM dbo.PurchaseInvoices
               INNER JOIN dbo.PurchaseInvoiceDetails
               ON PurchaseInvoiceDetails.PurchaseInvoiceID = PurchaseInvoices.PurchaseInvoiceID
               INNER JOIN dbo.Partners
               ON Partners.PartnerID = PurchaseInvoiceDetails.PartnerID
               WHERE PurchaseInvoices.PurchaseOrderID = " + id + "";

                cnn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                List<PurchaseInvoiceDetailModel> PurchaseInvoices = new List<PurchaseInvoiceDetailModel>();
                while (reader.Read())
                {
                    PurchaseInvoiceDetailModel item = new PurchaseInvoiceDetailModel();
                    item.Code = reader.GetString(0);
                    item.ExeDate = reader.GetDateTime(1).ToString();
                    item.PartnerNameAR = reader.GetString(2);
                    item.StockCount = reader.GetFloat(3);
                    item.PurchasePrice = reader.GetDecimal(4);
                    item.NetAmmount = reader.GetDecimal(5);
                    item.StockBalance = reader.GetDouble(6);
                    PurchaseInvoices.Add(item);
                }
                reader.Close();
                cnn.Close();
                return PurchaseInvoices;
            }
        }
        #endregion

        #region Get sellingInvoises By orderID
        [HttpGet]
        [Route("~/api/IOSAndroid/sellingInvoisesByorderID/{id}")]
        public List<SellingInvoiceDetailsModel> getsellingInvoise(int? id)
        {

            string Con = _appSettings.Report_Connection;


            using (SqlConnection cnn = new SqlConnection(Con))
            {
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = cnn;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @" WITH q2 AS
                                  (SELECT ROW_NUMBER() OVER(
                                 ORDER BY SN.SellingInvoiceID) AS RowNum, SN.Code AS InvoiceNum ,CONVERT(DATE, SN.Date,110)  AS ExeDate
                                 ,P.NameAR AS PartnerName,SD.StockCount AS StockCount,SD.SellingPrice AS SellingPrice
                                 ,SD.NetAmmount AS NetAmount,SoD.StockCount - SD.StockCount AS pp
                                 FROM dbo.SellingInvoices AS SN
                                 INNER JOIN dbo.SellingInvoiceDetails AS SD 
                                 ON SD.SellingInvoiceID = SN.SellingInvoiceID
                                 INNER JOIN dbo.Partners AS P 
                                 ON P.PartnerID = SD.PartnerID
                                 INNER JOIN dbo.SellingOrders AS SO
                                 ON SO.SellingOrderID = SN.SellingOrderID
                                 INNER JOIN dbo.SellingOrderDetails AS SoD
                                 ON SoD.SellingOrderID = SO.SellingOrderID
                                 WHERE SN.SellingOrderID = " + id + ") SELECT q2.RowNum ,q2.InvoiceNum,q2.ExeDate,q2.PartnerName,q2.StockCount,q2.SellingPrice,q2.pp , q2.NetAmount,CASE WHEN q2.RowNum=1 THEN q2.pp ELSE (SUM(q2.pp)OVER ( ORDER BY q2.ExeDate ROWS BETWEEN UNBOUNDED PRECEDING AND 1 PRECEDING))-q2.StockCount END AS balance FROM q2";
                cnn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                List<SellingInvoiceDetailsModel> SellingInvoiceDetailsModel = new List<SellingInvoiceDetailsModel>();
                while (reader.Read())
                {
                    SellingInvoiceDetailsModel item = new SellingInvoiceDetailsModel();
                    item.RowNum = reader.GetInt64(0);
                    item.Code = reader.GetString(1);
                    item.ExeDate = reader.GetDateTime(2).ToString("d/M/yyyy");
                    item.PartnerNameAR = reader.GetString(3);
                    item.StockCount = reader.GetFloat(4);
                    item.SellingPrice = reader.GetDecimal(5);
                    item.RestStocks = reader.GetFloat(6);
                    item.NetAmmount = reader.GetDecimal(7);

                    item.StockBalance = reader.GetDouble(8);
                    SellingInvoiceDetailsModel.Add(item);
                }
                reader.Close();
                cnn.Close();
                return SellingInvoiceDetailsModel;
            }
        }
        #endregion

        #region Get purchaseInvoiseportfolioInfo By orderID
        [HttpGet]
        [Route("~/api/IOSAndroid/purchaseInvoiseportfolioInfo/{id}")]
        public IActionResult GetPortInfo(int id)
        { 
            if (unitOfWork.PurchaseOrderRepository.GetEntity(filter: x => x.PurchaseOrderID == id)==null)
            {
                return null;
            }
            var Info = unitOfWork.PurchaseOrderRepository.GetEntity(filter: x => x.PurchaseOrderID == id);
            PortfolioModel portfolio = new PortfolioModel();
            portfolio.Code = Info.Portfolio.Code;
            portfolio.NameAR = Info.Portfolio.NameAR;
            portfolio.NameEN = Info.Portfolio.NameEN;
            portfolio.PortfolioID = Info.Portfolio.PortfolioID;
            var Debit = unitOfWork.PortfolioAccountRepository.GetEntity(filter: x => x.PortfolioID == Info.PortfolioID).Account.Debit;
            if (Debit == null)
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
            if (CreditOpenningBalance == null)
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
        #endregion
        
     
        #region Get sellingInvoisePortfolioInfoBy orderID
        [HttpGet]
        [Route("~/api/IOSAndroid/sellingInvoisePortfolioInfoByorderID/{id}")]
        public IActionResult GetPortfolioINFO(int id)
        { 
            if (unitOfWork.SellingOrderRepository.Get(filter: x => x.SellingOrderID == id).SingleOrDefault()==null)
            {
                return null;
            }
            var Info = unitOfWork.SellingOrderRepository.Get(filter: x => x.SellingOrderID == id).SingleOrDefault();
            SellingOrderModel sellingOrderModel = new SellingOrderModel();
            sellingOrderModel.SellingOrderID = Info.SellingOrderID;
            sellingOrderModel.PortfolioID = Info.PortfolioID;
            sellingOrderModel.Portfoliocode = Info.Portfolio.Code;
            sellingOrderModel.PortfolioNameAR = Info.Portfolio.NameAR;
            sellingOrderModel.PortfolioAccount = unitOfWork.PortfolioAccountRepository.GetEntity(filter: a => a.PortfolioID == Info.PortfolioID).AccountID;
            sellingOrderModel.PortfolioAccountName = unitOfWork.PortfolioAccountRepository.GetEntity(filter: a => a.PortfolioID == Info.PortfolioID).Account.NameAR;


            return Ok(sellingOrderModel);





        }
        #endregion

        #region GetSellingPartners
        [HttpGet]
        [Route("~/api/IOSAndroid/GetSellingPartners/{id}")]
        public IEnumerable<PortfolioPartners> GetSellingPartners(int id)
        {
             
            if (unitOfWork.SellingOrderDetailRepository.Get(filter: a => a.SellingOrder.SellingOrderID == id).Count()==0)
            {
                return null;
            }
            // partners in Selling
            var transPartners = unitOfWork.SellingOrderDetailRepository.Get(filter: a => a.SellingOrder.SellingOrderID == id).Select(p => new PortfolioPartners
            {
                PartnerID = p.PartnerID,
                Code = p.Partner.Code,
                NameAR = p.Partner.NameAR,
                NameEN = p.Partner.NameEN,




            });
            return transPartners;

        }
        #endregion


        #region GetpurchasePartners
        [HttpGet]
        [Route("~/api/IOSAndroid/GetpurchasePartners/{id}")]
        public IEnumerable<PortfolioPartners> GetpurchasePartners(int id)
        { 
            if (unitOfWork.PurchaseOrderDetailRepository.Get(filter: a => a.PurchaseOrder.PurchaseOrderID == id).Count()==0)
            {
                return null;
            }

            // partners in purchase
            var transPartners = unitOfWork.PurchaseOrderDetailRepository.Get(filter: a => a.PurchaseOrder.PurchaseOrderID == id).Select(p => new PortfolioPartners
            {
                PartnerID = p.PartnerID,
                Code = p.Partner.Code,
                NameAR = p.Partner.NameAR,
                NameEN = p.Partner.NameEN,




            });
            
            return transPartners;

        }
        #endregion


        #region GetsellingorderByID
        [HttpGet]
        [Route("~/api/IOSAndroid/GetsellingorderByID/{id}")]
        public IActionResult Pagination(int id)
        {
            if (id > 0)
            {
                var sellingorder = unitOfWork.SellingOrderRepository.GetByID(id);
                var model = _mapper.Map<SellingOrderModel>(sellingorder);

                #region Date part 
                if (sellingorder.OrderDate != null)
                {
                    model.OrderDateGorg = sellingorder.OrderDate.ToString("d/M/yyyy");
                    model.OrderDateHigri = DateHelper.GetHijriDate(sellingorder.OrderDate);
                }



                #endregion

                #region  Details
                var Details = unitOfWork.SellingOrderDetailRepository

                    .Get(filter: m => m.SellingOrderID == sellingorder.SellingOrderID)
                    .Select(m => new SellingOrderDetailModel
                    {
                        PartnerID = m.PartnerID,
                        PartnerNameAr = m.Partner.NameAR,
                        PriceType = m.PriceType,
                        SellingOrderID = m.SellingOrderID,
                        SellOrderDetailID = m.SellOrderDetailID,
                        StockCount = m.StockCount,
                        PartnerCode = m.Partner.Code




                    });




                if (Details != null)
                {
                    model.sellingOrderDetailModels = Details;

                }

                #endregion
                model.Count = unitOfWork.SellingOrderRepository.Count();
                model.Portfoliocode = unitOfWork.PortfolioRepository.GetEntity(filter: a => a.PortfolioID == sellingorder.PortfolioID).Code;

                return Ok(model);


            }
            else
                return Ok(1);
        }

        #endregion

        #region Insert sellingorder

        [HttpPost]
        [Route("~/api/IOSAndroid/Postselling")]
        public IActionResult Postselling([FromBody] SellingOrderModel sellingOrderModel)
        {

            if (ModelState.IsValid)
            {

                var Check = unitOfWork.SellingOrderRepository.Get();
                if (sellingOrderModel == null)
                {
                    return Ok(0);
                }
                if (Check.Any(m => m.Code == sellingOrderModel.Code))
                {
                    return Ok(2);
                }
                else
                {
                    if (sellingOrderModel.OrderDateGorg == null)
                    {
                        sellingOrderModel.OrderDateGorg = DateTime.Now.ToString("d/M/yyyy");
                    }





                    var model = _mapper.Map<SellingOrder>(sellingOrderModel);

                    #region Warehouse
                    //Check Stocks Count Allowed For Selling 
                    var Chk = _stocksHelper.CheckStockCountForSelling(sellingOrderModel);
                    if (!Chk)
                        return Ok(7);

                    #endregion


                    unitOfWork.SellingOrderRepository.Insert(model);





                    if (sellingOrderModel.sellingOrderDetailModels != null)
                    {
                        foreach (var item in sellingOrderModel.sellingOrderDetailModels)
                        {
                            SellingOrderDetailModel detail = new SellingOrderDetailModel();
                            detail.PartnerID = item.PartnerID;
                            detail.SellingOrderID = model.SellingOrderID;
                            detail.PriceType = item.PriceType;
                            detail.StockCount = item.StockCount;
                            detail.SellOrderDetailID = 0;
                            var ob = _mapper.Map<SellingOrderDetail>(detail);
                            unitOfWork.SellingOrderDetailRepository.Insert(ob);
                        }
                    }




                    var Result = unitOfWork.Save();
                    if (Result == 200)
                    {
                        var UserID = loggerHistory.getUserIdFromRequest(Request);

                        loggerHistory.InsertUserLog(UserID, " امر البيع", "اضافه امر البيع", false);
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
        #endregion


        #region Update sellingOrder
        [HttpPut]
        [Route("~/api/IOSAndroid/UpdatesellingOrder/{id}")]
        public IActionResult UpdatesellingOrder(int id, [FromBody] SellingOrderModel sellingOrderModel)
        {
            if (id != sellingOrderModel.SellingOrderID)
            {

                return Ok(1);
            }

            if (ModelState.IsValid)
            {
                if (sellingOrderModel.OrderDateGorg == null)
                {
                    sellingOrderModel.OrderDateGorg = DateTime.Now.ToString();
                }

                var model = _mapper.Map<SellingOrder>(sellingOrderModel);
                #region Warehouse
                //Check Stocks Count Allowed For Selling 
                var Chk = _stocksHelper.CheckStockCountForSelling(sellingOrderModel);
                if (!Chk)
                    return Ok(7);

                #endregion


                var Check = unitOfWork.SellingOrderRepository.Get(NoTrack: "NoTrack");

                if (Check.Any(m => m.Code == sellingOrderModel.Code))
                {
                    unitOfWork.SellingOrderRepository.Update(model);


                    // Details
                    var oldDetails = unitOfWork.SellingOrderDetailRepository

                    .Get(filter: m => m.SellingOrderID == model.SellingOrderID);

                    if (oldDetails != null)
                    {

                        unitOfWork.SellingOrderDetailRepository.RemovRange(oldDetails);

                    }
                    if (sellingOrderModel.sellingOrderDetailModels != null)
                    {

                        foreach (var item in sellingOrderModel.sellingOrderDetailModels)
                        {
                            item.SellingOrderID = sellingOrderModel.SellingOrderID;
                            item.SellOrderDetailID = 0;
                            var newDetail = _mapper.Map<SellingOrderDetail>(item);

                            unitOfWork.SellingOrderDetailRepository.Insert(newDetail);

                        }


                    }


                }



                else
                {
                    if (Check.Any(m => m.Code != sellingOrderModel.Code && m.SellingOrderID == id))
                    {

                        unitOfWork.SellingOrderRepository.Update(model);

                        // Details
                        var oldDetails = unitOfWork.SellingOrderDetailRepository

                        .Get(filter: m => m.SellingOrderID == model.SellingOrderID);

                        if (oldDetails != null)
                        {

                            unitOfWork.SellingOrderDetailRepository.RemovRange(oldDetails);

                        }
                        if (sellingOrderModel.sellingOrderDetailModels != null)
                        {

                            foreach (var item in sellingOrderModel.sellingOrderDetailModels)
                            {
                                item.SellingOrderID = sellingOrderModel.SellingOrderID;
                                item.SellOrderDetailID = 0;
                                var newDetail = _mapper.Map<SellingOrderDetail>(item);

                                unitOfWork.SellingOrderDetailRepository.Insert(newDetail);

                            }


                        }

                    }
                }
                var result = unitOfWork.Save();
                if (result == 200)
                {
                    var UserID = loggerHistory.getUserIdFromRequest(Request);

                    loggerHistory.InsertUserLog(UserID, " امر البيع", "تعديل امر البيع", false);
                    return Ok(4);
                }
                else if (result == 501)
                {
                    return Ok(5);
                }
                else
                {
                    return Ok(6);
                }
            }
            else
            {
                return Ok(6);
            }
        }

        #endregion


        #region Delete sellingOrder
        [HttpDelete]
        [Route("~/api/IOSAndroid/DeletesellingOrder/{id}")]
        public IActionResult DeletesellingOrder(int? id)
        {

            if (id > 0)
            {
                var sellingorder = unitOfWork.SellingOrderRepository.GetByID(id);
                if (sellingorder == null)
                {
                    return Ok(6);
                }
                int Invoives = unitOfWork.SellingInvoiceReposetory.Get(filter: x => x.SellingOrderID == id).Count();
                if (Invoives != 0)
                {
                    return Ok(5);
                }
                var Details = unitOfWork.SellingOrderDetailRepository.Get(filter: x => x.SellingOrderID == sellingorder.SellingOrderID);
                if (Details != null)
                {
                    unitOfWork.SellingOrderDetailRepository.RemovRange(Details);
                }




                unitOfWork.SellingOrderRepository.Delete(sellingorder);
                var Result = unitOfWork.Save();
                if (Result == 200)
                {
                    var UserID = loggerHistory.getUserIdFromRequest(Request);

                    loggerHistory.InsertUserLog(UserID, " امر البيع", "حذف امر البيع", false);
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



        #region GetPurchaseOrderBYID
        [HttpGet]
        [Route("~/api/IOSAndroid/GetPurchaseOrderBYID/{id}")]
        public IActionResult GetPurchaseOrderBYID(int id)
        {
            if (id > 0)
            {
                var purchaseorder = unitOfWork.PurchaseOrderRepository.GetByID(id);
                var model = _mapper.Map<PurchaseOrderModel>(purchaseorder);

                #region Date part 
                if (purchaseorder.OrderDate != null)
                {
                    model.OrderDate = purchaseorder.OrderDate.ToString("d/M/yyyy");
                    model.OrderDateHijri = DateHelper.GetHijriDate(purchaseorder.OrderDate);
                }



                #endregion

                #region  Details
                var Details = unitOfWork.PurchaseOrderDetailRepository

                    .Get(filter: m => m.PurchaseOrderID == purchaseorder.PurchaseOrderID)
                    .Select(m => new PurchaseOrderDetailModel
                    {
                        PartnerID = m.PartnerID,
                        PartnerNameAr = m.Partner.NameAR,
                        PriceType = m.PriceType,
                        PurchaseOrderID = m.PurchaseOrderID,
                        PurchaseOrderDetailID = m.PurchaseOrderDetailID,
                        StockCount = m.StockCount,
                        PartnerCode = m.Partner.Code




                    });
                
                if (Details != null)
                {
                    model.purchaseordersDetailsModels = Details;

                }

                #endregion
                model.Count = unitOfWork.SellingOrderRepository.Count();
                model.PortfolioCode = unitOfWork.PortfolioRepository.GetEntity(filter: a => a.PortfolioID == purchaseorder.PortfolioID).Code;

                return Ok(model);


            }
            else
                return Ok(1);
        }

        #endregion



        #region Insert PurchaseOrder
        [HttpPost]
        [Route("~/api/IOSAndroid/PurchaseOrderAdd")]
        public IActionResult PurchaseOrderAdd([FromBody] PurchaseOrderModel purchaseOrderModel)
        {

            if (ModelState.IsValid)
            {

                var Check = unitOfWork.PurchaseOrderRepository.Get();
                if (purchaseOrderModel == null)
                {
                    return Ok(0);
                }
                if (Check.Any(m => m.Code == purchaseOrderModel.Code))
                {
                    return Ok(2);
                }
                else
                {
                    if (purchaseOrderModel.OrderDate == null)
                    {
                        purchaseOrderModel.OrderDate = DateTime.Now.ToString("d/M/yyyy");
                    }

                    //   #region Warehouse
                    //   // Add Purchase Invoice Stocks Count To Portofolio
                    //decimal? RialBalance=   _stocksHelper.RialBalanc(purchaseOrderModel.PortfolioID);
                    //   if (RialBalance==null) {
                    //       return Ok(7);
                    //   }
                    //   decimal totalPartenersRial=0.0m;
                    //   foreach (var item in purchaseOrderModel.purchaseordersDetailsModels)
                    //   {
                    //       totalPartenersRial +=item.
                    //   }
                    //   #endregion

                    var model = _mapper.Map<PurchaseOrder>(purchaseOrderModel);




                    unitOfWork.PurchaseOrderRepository.Insert(model);





                    if (purchaseOrderModel.purchaseordersDetailsModels != null)
                    {
                        foreach (var item in purchaseOrderModel.purchaseordersDetailsModels)
                        {
                            PurchaseOrderDetailModel detail = new PurchaseOrderDetailModel();
                            detail.PartnerID = item.PartnerID;
                            detail.PurchaseOrderID = model.PurchaseOrderID;
                            detail.PriceType = item.PriceType;
                            detail.StockCount = item.StockCount;
                            detail.PurchaseOrderDetailID = 0;
                            var ob = _mapper.Map<PurchaseOrderDetail>(detail);
                            unitOfWork.PurchaseOrderDetailRepository.Insert(ob);
                        }
                    }




                    var Result = unitOfWork.Save();
                    if (Result == 200)
                    {
                        var UserID = loggerHistory.getUserIdFromRequest(Request);

                        loggerHistory.InsertUserLog(UserID, " امر الشراء", "اضافه امر الشراء", false);
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

        #endregion

        #region update PurchasePrder

        [HttpPut]
        [Route("~/api/IOSAndroid/UpdatePurchasePrder/{id}")]
        public IActionResult UpdatePurchasePrder(int id, [FromBody] PurchaseOrderModel purchaseOrderModel)
        {
            if (id != purchaseOrderModel.PurchaseOrderID)
            {

                return Ok(1);
            }

            if (ModelState.IsValid)
            {
                if (purchaseOrderModel.OrderDate == null)
                {
                    purchaseOrderModel.OrderDate = DateTime.Now.ToString();
                }

                var model = _mapper.Map<PurchaseOrder>(purchaseOrderModel);

                var Check = unitOfWork.PurchaseOrderRepository.Get(NoTrack: "NoTrack");

                if (Check.Any(m => m.Code == purchaseOrderModel.Code))
                {
                    unitOfWork.PurchaseOrderRepository.Update(model);


                    // Details
                    var oldDetails = unitOfWork.PurchaseOrderDetailRepository

                    .Get(filter: m => m.PurchaseOrderID == model.PurchaseOrderID);

                    if (oldDetails != null)
                    {

                        unitOfWork.PurchaseOrderDetailRepository.RemovRange(oldDetails);

                    }
                    if (purchaseOrderModel.purchaseordersDetailsModels != null)
                    {

                        foreach (var item in purchaseOrderModel.purchaseordersDetailsModels)
                        {
                            item.PurchaseOrderID = purchaseOrderModel.PurchaseOrderID;
                            item.PurchaseOrderDetailID = 0;
                            var newDetail = _mapper.Map<PurchaseOrderDetail>(item);

                            unitOfWork.PurchaseOrderDetailRepository.Insert(newDetail);

                        }


                    }


                }



                else
                {
                    if (Check.Any(m => m.Code != purchaseOrderModel.Code && m.PurchaseOrderID == id))
                    {

                        unitOfWork.PurchaseOrderRepository.Update(model);

                        // Details
                        var oldDetails = unitOfWork.PurchaseOrderDetailRepository

                        .Get(filter: m => m.PurchaseOrderID == model.PurchaseOrderID);

                        if (oldDetails != null)
                        {

                            unitOfWork.PurchaseOrderDetailRepository.RemovRange(oldDetails);

                        }
                        if (purchaseOrderModel.purchaseordersDetailsModels != null)
                        {

                            foreach (var item in purchaseOrderModel.purchaseordersDetailsModels)
                            {
                                item.PurchaseOrderID = purchaseOrderModel.PurchaseOrderID;
                                item.PurchaseOrderDetailID = 0;
                                var newDetail = _mapper.Map<PurchaseOrderDetail>(item);

                                unitOfWork.PurchaseOrderDetailRepository.Insert(newDetail);

                            }


                        }

                    }
                }
                var result = unitOfWork.Save();
                if (result == 200)
                {
                    var UserID = loggerHistory.getUserIdFromRequest(Request);

                    loggerHistory.InsertUserLog(UserID, " امر الشراء", "تعديل امر الشراء", false);
                    return Ok(4);
                }
                else if (result == 501)
                {
                    return Ok(5);
                }
                else
                {
                    return Ok(6);
                }
            }
            else
            {
                return Ok(6);
            }
        }

        #endregion


        #region Delete PurchasePrder

        [HttpDelete]
        [Route("~/api/IOSAndroid/DeletePurchasePrder/{id}")]

        public IActionResult DeletePurchasePrder(int? id)
        {

            if (id > 0)
            {
                var purchaseorder = unitOfWork.PurchaseOrderRepository.GetByID(id);
                if (purchaseorder == null)
                {
                    return Ok(6);
                }
                var Invoices = unitOfWork.PurchaseInvoiceRepository.Get(filter: x => x.PurchaseOrderID == id).Count();

                if (Invoices != 0)
                {
                    return Ok(5);
                }
                var Details = unitOfWork.PurchaseOrderDetailRepository.Get(filter: x => x.PurchaseOrderID == purchaseorder.PurchaseOrderID);
                if (Details != null)
                {
                    unitOfWork.PurchaseOrderDetailRepository.RemovRange(Details);
                }




                unitOfWork.PurchaseOrderRepository.Delete(purchaseorder);
                var Result = unitOfWork.Save();
                if (Result == 200)
                {
                    var UserID = loggerHistory.getUserIdFromRequest(Request);

                    loggerHistory.InsertUserLog(UserID, " امر الشراء", "حذف امر الشراء", false);
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



        #region GetSellingInvoiceByID
        [HttpGet]
        [Route("~/api/IOSAndroid/GetSellingInvoiceByID/{id}")]
        public IActionResult PaginationSellingInvoice(int id)
        {
            if (id > 0)
            {
                var selling = unitOfWork.SellingInvoiceReposetory.GetByID(id);

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
                return Ok(model);
            }
            else
            {
                return Ok(1);

            }


        }
        #endregion


        #region Insert sellingInvoice
        [HttpPost]
        [Route("~/api/IOSAndroid/PostSellingInvoice")]
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
                    if (Details != null && Details.Count() > 0)
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
                    float? totalStocksInvoices = 0.0f;

                    foreach (var item in sellingInvoiceModel.DetailsModels)
                    {
                        float stocksPartener = unitOfWork.SellingOrderDetailRepository.GetEntity(filter: a => a.SellingOrderID == sellingInvoiceModel.SellingOrderID && a.PartnerID == item.PartnerID).StockCount;
                        totalStocksInvoices = _stocksHelper.sumOfstocksOnInvoice(sellingInvoiceModel.SellingOrderID, item.PartnerID);
                        if (totalStocksInvoices < stocksPartener)
                        {
                            _stocksHelper.TransferSellingFromStocks(sellingInvoiceModel);
                        }
                        else
                        {
                            return Ok(7);
                        }
                    }



                    #endregion


                    //==================================================لا تولد قيد ===================================
                    if (sellingInvoiceModel.SettingModel.DoNotGenerateEntry == true)
                    {
                        var Resul = unitOfWork.Save();
                        if (Resul == 200)
                        {
                            var UserID = loggerHistory.getUserIdFromRequest(Request);

                            loggerHistory.InsertUserLog(UserID, " فاتوره بيع", "اضافه فاتوره بيع", false);
                            return Ok(4);
                        }
                        else if (Resul == 501)
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

                        var EntryMODEL = EntriesHelper.InsertCalculatedEntries(portofolioaccount, sellingInvoiceModel, null, null, null, lastEntry);
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
                        var UserID = loggerHistory.getUserIdFromRequest(Request);

                        loggerHistory.InsertUserLog(UserID, " فاتوره بيع", "اضافه فاتوره بيع", false);
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

        #endregion

        #region PutSellingInvoice
        [HttpPut]
        [Route("~/api/IOSAndroid/PutSellingInvoice/{id}")]

        public IActionResult PutSellingInvoice(int id, [FromBody]  SellingInvoiceModel sellingInvoiceModel)
        {
            int portofolioaccount = 0;
            if (sellingInvoiceModel != null)
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
                var OldDetails = unitOfWork.SellingInvoiceDetailRepository.Get(NoTrack: "NoTrack", filter: m => m.SellingInvoiceID == sellingInvoice.SellingInvoiceID);
                #region Warehouse

                float? totalStocksInvoices = 0.0f;
                if (OldDetails != null)
                {
                    _stocksHelper.CancelSellingFromStocks(sellingInvoiceModel.PortfolioID, OldDetails);
                }
                foreach (var item in sellingInvoiceModel.DetailsModels)
                {
                    float stocksPartener = unitOfWork.SellingOrderDetailRepository.GetEntity(filter: a => a.SellingOrderID == sellingInvoiceModel.SellingOrderID && a.PartnerID == item.PartnerID).StockCount;
                    totalStocksInvoices = _stocksHelper.sumOfstocksOnInvoiceUpdate(sellingInvoiceModel.SellingOrderID, item.PartnerID, item.SellingInvoiceDetailID);
                    if (totalStocksInvoices < stocksPartener)
                    {
                        _stocksHelper.TransferSellingFromStocks(sellingInvoiceModel);
                    }
                    else
                    {
                        return Ok(7);
                    }
                }



                #endregion


                var EntryCheck = unitOfWork.EntryRepository.Get(x => x.SellingInvoiceID == sellingInvoice.SellingInvoiceID, NoTrack: "NoTrack").SingleOrDefault();
                if (EntryCheck != null)
                {

                    // get old entry data
                    var Entry = unitOfWork.EntryRepository.Get(NoTrack: "NoTrack", filter: x => x.SellingInvoiceID == sellingInvoice.SellingInvoiceID).SingleOrDefault();
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
                        if (OldDetails.Count() > 0 && OldDetails != null)
                        {
                            unitOfWork.SellingInvoiceDetailRepository.RemovRange(OldDetails);
                        }


                        if (Newdetails != null && Newdetails.Count() > 0)
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
                                var UserID = loggerHistory.getUserIdFromRequest(Request);

                                loggerHistory.InsertUserLog(UserID, " فاتوره بيع", "تعديل فاتوره بيع", false);
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

                            var EntryMODEL = EntriesHelper.InsertCalculatedEntries(portofolioaccount, sellingInvoiceModel, null, null, null, lastEntry, Entry);
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

                        var res = unitOfWork.Save();
                        if (res == 200)
                        {
                            var UserID = loggerHistory.getUserIdFromRequest(Request);

                            loggerHistory.InsertUserLog(UserID, " فاتوره بيع", "تعديل فاتوره بيع", false);

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
                                    var UserID = loggerHistory.getUserIdFromRequest(Request);

                                    loggerHistory.InsertUserLog(UserID, " فاتوره بيع", "تعديل فاتوره بيع", false);
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


                            if (Newdetails != null && Newdetails.Count() > 0)
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
                                    var UserID = loggerHistory.getUserIdFromRequest(Request);

                                    loggerHistory.InsertUserLog(UserID, " فاتوره بيع", "تعديل فاتوره بيع", false);
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

                                var EntryMODEL = EntriesHelper.InsertCalculatedEntries(portofolioaccount, sellingInvoiceModel, null, null, null, lastEntry, Entry);
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
                                        StocksDebit = x.StocksDebit,
                                        StocksCredit = x.StocksCredit


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

                                loggerHistory.InsertUserLog(UserID, " فاتوره بيع", "تعديل فاتوره بيع", false);
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
                        if (OldDetails.Count() > 0 && OldDetails != null)
                        {
                            unitOfWork.SellingInvoiceDetailRepository.RemovRange(OldDetails);

                            var Result = unitOfWork.Save();
                            if (Result == 200)
                            {
                                var UserID = loggerHistory.getUserIdFromRequest(Request);

                                loggerHistory.InsertUserLog(UserID, " فاتوره بيع", "تعديل فاتوره بيع", false);
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


                        if (Newdetails != null && Newdetails.Count() > 0)
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
                                var UserID = loggerHistory.getUserIdFromRequest(Request);

                                loggerHistory.InsertUserLog(UserID, " فاتوره بيع", "تعديل فاتوره بيع", false);

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
                            var EntryMODEL = EntriesHelper.InsertCalculatedEntries(portofolioaccount, sellingInvoiceModel, null, null, null, lastEntry);
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

                            loggerHistory.InsertUserLog(UserID, " فاتوره بيع", "تعديل فاتوره بيع", false);
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
                            if (OldDetails != null && OldDetails.Count() > 0)
                            {
                                unitOfWork.SellingInvoiceDetailRepository.RemovRange(OldDetails);

                                var Result = unitOfWork.Save();
                                if (Result == 200)
                                {
                                    var UserID = loggerHistory.getUserIdFromRequest(Request);

                                    loggerHistory.InsertUserLog(UserID, " فاتوره بيع", "تعديل فاتوره بيع", false);
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


                            if (Newdetails != null && Newdetails.Count() > 0)
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
                                    var UserID = loggerHistory.getUserIdFromRequest(Request);

                                    loggerHistory.InsertUserLog(UserID, " فاتوره بيع", "تعديل فاتوره بيع", false);

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
                                var EntryMODEL = EntriesHelper.InsertCalculatedEntries(portofolioaccount, sellingInvoiceModel, null, null, null, lastEntry);
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

                                loggerHistory.InsertUserLog(UserID, " فاتوره بيع", "تعديل فاتوره بيع", false);
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

        #endregion

        #region Delete SellingInvoice
        [HttpDelete]
        [Route("~/api/IOSAndroid/DeleteSellingInvoice/{id}")]
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
            _stocksHelper.CancelSellingFromStocks(modelSelling.SellingOrder.PortfolioID, Details);
            #endregion
            unitOfWork.SellingInvoiceDetailRepository.RemovRange(Details);

            var Entry = unitOfWork.EntryRepository.Get(filter: x => x.SellingInvoiceID == id).SingleOrDefault();
            if (Entry != null)
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
                var UserID = loggerHistory.getUserIdFromRequest(Request);

                loggerHistory.InsertUserLog(UserID, " فاتوره بيع", "حذف فاتوره بيع", false);
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
        #endregion



        #region GetPurchaseInvoiceByID
        [HttpGet]
        [Route("~/api/IOSAndroid/GetPurchaseInvoiceByID/{id}")]
        public IActionResult PaginationPurchaseInvoice(int id)
        {
            if (id > 0)
            {

                var UserID = loggerHistory.getUserIdFromRequest(Request);
                var purchase = unitOfWork.PurchaseInvoiceRepository.GetByID(id);
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


        

           


        




        #endregion

        #region Insert PurchaseInvoice
        [HttpPost]
        [Route("~/api/IOSAndroid/PostPurchaseInvoice")]
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



                    else if (purchaseInvoiceModel.SettingModel.AutoGenerateEntry == true)
                    {
                        var lastEntry = unitOfWork.EntryRepository.Last();
                        var EntryMODEL = EntriesHelper.InsertCalculatedEntries(portofolioaccount, null, purchaseInvoiceModel, null, null, lastEntry);
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
        #endregion



        #region Update PurchaseInvoice
        [HttpPut]
        [Route("~/api/IOSAndroid/PutPurchaseInvoice/{id}")]
        public IActionResult PutPurchaseInvoice(int id, [FromBody]  PurchaseInvoiceModel purchaseInvoiceModel)
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
                            var EntryMODEL = EntriesHelper.InsertCalculatedEntries(portofolioaccount, null, purchaseInvoiceModel, null, null, lastEntry, Entry);
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
                                var EntryMODEL = EntriesHelper.InsertCalculatedEntries(portofolioaccount, null, purchaseInvoiceModel, null, null, lastEntry, Entry);
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
                        if (OldDetails != null && OldDetails.Count() > 0)
                        {
                            unitOfWork.PurchaseInvoiceDetailRepository.RemovRange(OldDetails);

                        }


                        if (Newdetails != null && Newdetails.Count() > 0)
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
                            var EntryMODEL = EntriesHelper.InsertCalculatedEntries(portofolioaccount, null, purchaseInvoiceModel, null, null, lastEntry);
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
                            if (OldDetails != null && OldDetails.Count() > 0)
                            {
                                unitOfWork.PurchaseInvoiceDetailRepository.RemovRange(OldDetails);

                            }


                            if (Newdetails != null && Newdetails.Count() > 0)
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
                                var EntryMODEL = EntriesHelper.InsertCalculatedEntries(portofolioaccount, null, purchaseInvoiceModel, null, null, lastEntry);
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





        #endregion


        #region Delete PurchaseInvoice
        [HttpDelete]
        [Route("~/api/IOSAndroid/DeletePurchaseInvoice/{id}")]
        public IActionResult DeletePurchase(int? id)
        {

            if (id == null)
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
            if (Entry != null)
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

        #endregion
        
        #region GetHistory BY UserID
        [HttpGet]
        [Route("~/api/IOSAndroid/GetHistory")]
        public IActionResult GetHistory()
        {
            var UserID = loggerHistory.getUserIdFromRequest(Request); 
            if ((unitOfWork.UserLogRepository.Get(filter: x => x.UserId == UserID && x.MobileView == false)).Count()==0)
            {
                return null;
            }
            var HistoryList = unitOfWork.UserLogRepository.Get(filter: x => x.UserId == UserID && x.MobileView == false).Select(m => new UserLogModel {
                OperationName = m.OperationName,
                PageName = m.PageName,
                UserId = m.UserId,
                MobileView = m.MobileView,
                UserLogID = m.UserLogID,
                UserName = m.User.UserName,
                OperationDate = m.OperationDate.ToString("d/MM/yyyy"),
                time = m.OperationDate.ToString("HH:mm"),


            }); 
         

           
                return Ok(HistoryList);
            
           
        }
        #endregion

  

    }
}





using System;
using System.Collections.Generic;
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
using System.Data.Sql;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using Stimulsoft.Report.Dictionary;
using Stimulsoft.Report;
using System.Data;

namespace Stocks.Controllers
{
   [Authorize(Roles = "SuperAdmin,Admin,Employee")]
    [Route("api/[controller]")]
    
    public class PurchaseOrderController : ControllerBase
    {
        private readonly ApplicationSettings _appSettings;
       
        private UnitOfWork unitOfWork;
        private readonly IMapper _mapper;
        private LoggerHistory loggerHistory;
        private readonly IStocksHelper _stocksHelper;
        public PurchaseOrderController(StocksContext context, IMapper mapper , IOptions<ApplicationSettings> appSettings, IStocksHelper stocksHelper)
        {
            _appSettings = appSettings.Value;
            this.unitOfWork = new UnitOfWork(context);
            this._mapper = mapper;
            loggerHistory = new LoggerHistory(context, mapper);
            _stocksHelper = stocksHelper;


        }


        #region FirstOpen
        [HttpGet]
        [Route("~/api/PurchaseOrder/FirstOpen")]
        public IActionResult FirstOpen()
        {
            PurchaseOrderModel model = new PurchaseOrderModel();

            model.Count = unitOfWork.PurchaseOrderRepository.Count();
            if (model.Count > 0)
            {

                model.LastCode = unitOfWork.PurchaseOrderRepository.Last().Code;
            }


            return Ok(model);
        }

        #endregion


        #region Pagination
        [HttpGet]
        [Route("~/api/PurchaseOrder/Paging/{pageNumber}")]
        public IActionResult Pagination(int pageNumber)
        {
            if (pageNumber > 0)
            {
                var purchaseorder = unitOfWork.PurchaseOrderRepository.Get(page: pageNumber).FirstOrDefault();
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
                        TradingValue=m.TradingValue,
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


        #region Insert Methods
        [HttpPost]
        [Route("~/api/PurchaseOrder/Add")]
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
                            detail.TradingValue = item.TradingValue;
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

        #region Update Methods
        [HttpPut]
        [Route("~/api/PurchaseOrder/Update/{id}")]
        public IActionResult Update(int id, [FromBody] PurchaseOrderModel purchaseOrderModel)
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


        #region Delete Methods

        [HttpDelete]
        [Route("~/api/PurchaseOrder/Delete/{id}")]
        public IActionResult Delete(int? id)
        {

            if (id > 0)
            {
                var purchaseorder = unitOfWork.PurchaseOrderRepository.GetByID(id);
                if (purchaseorder == null)
                {
                    return Ok(6);
                }
                var Invoices = unitOfWork.PurchaseInvoiceRepository.Get(filter: x => x.PurchaseOrderID == id).Count();
    
                if (Invoices!=0)
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


        [HttpGet]
        [Route("~/api/PurchaseOrder/getpartCode/{id}")]
        public IActionResult getpartCode(int? id)
        {

            if (id != null || id != 0)
            {
                var c = unitOfWork.PartnerRepository.GetByID(id);
                partenerCodeModel codeModel = new partenerCodeModel();
                codeModel.Code = c.Code;
                codeModel.NameAr = c.NameAR;
                return Ok(codeModel);
            }
            else
            {
                return Ok();
            }
            
        }


        [HttpGet]
        [Route("~/api/order/getparteners")]
        public IActionResult getparteners()
        {

           
                var p = unitOfWork.PartnerRepository.Get().Select(a=> new PartenerModel {

                    NameAR=a.NameAR,
                    Code=a.Code,
                    NameEN=a.NameEN,
                    PartnerID=a.PartnerID,


                });

                return Ok(p);
            
          

        }




        [HttpGet]
        [Route("~/api/PurchaseOrder/getPurchaseInvoise/{id}")]
        public List<PurchaseInvoiceDetailModel> getsellingInvoise(int? id)
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
               WHERE PurchaseInvoices.PurchaseOrderID = "+id+"";

               cnn.Open();
               SqlDataReader reader = cmd.ExecuteReader();
               List<PurchaseInvoiceDetailModel> PurchaseInvoices = new List<PurchaseInvoiceDetailModel>();
               while (reader.Read())
                {
                    PurchaseInvoiceDetailModel item = new PurchaseInvoiceDetailModel();
                    item.Code = reader.GetString(0);
                    item.ExeDate = reader.GetDateTime(1).ToString("d/MM/yyyy"); ;
                    item.PartnerNameAR= reader.GetString(2);
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



        //[Route("~/api/PurchaseOrder/getTotalStocks/{PurchaseInvoiceID}/{PartnerID}")]
        //public float getTotalStocks(int? PurchaseInvoiceID, int? PartnerID)
        //{
        //    float totalStocks = 0.0f;
        //    var DetailsModels = unitOfWork.PurchaseInvoiceDetailRepository.Get(filter: a => a.PurchaseInvoiceID == PurchaseInvoiceID && a.PartnerID== PartnerID);

        //    foreach (var item in DetailsModels)
        //    {
        //        totalStocks += item.StockCount;
        //    }

        //    return totalStocks;

        //}

        //public float getTotalStocksOrder(int? PurchaseOrderID, int? PartnerID)
        //{
        //    float totalStocks = 0.0f;
        //    var DetailsModels = unitOfWork.PurchaseOrderDetailRepository.Get(filter: a => a.PurchaseOrderID == PurchaseOrderID && a.PartnerID == PartnerID);

        //    foreach (var item in DetailsModels)
        //    {
        //        totalStocks += item.StockCount;
        //    }

        //    return totalStocks;

        //}

        #region GetHistory BY UserID
        [HttpGet]
        [Route("~/api/IOS/GetHistory")]
        public IActionResult GetHistory()
        {
            var UserID = loggerHistory.getUserIdFromRequest(HttpContext.Request);
            var HistoryList = unitOfWork.UserLogRepository.Get(filter: x => x.UserId == UserID && x.MobileView == false).Select(m => new UserLogModel
            {
                OperationName = m.OperationName,
                PageName = m.PageName,
                UserId = m.UserId,
                MobileView = m.MobileView,
                UserLogID = m.UserLogID,
                UserName = m.User.UserName,

            });


            return Ok(HistoryList);
        }
        #endregion



    }
}
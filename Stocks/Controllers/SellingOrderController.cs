using System;
using System.Collections.Generic;
using System.Data;
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
using Microsoft.Extensions.Options;

namespace Stocks.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class sellingorderController : ControllerBase
    {
        #region CTOR & Definitions
        private UnitOfWork unitOfWork;
        private readonly IMapper _mapper;
        private readonly ApplicationSettings _appSettings;
        public sellingorderController(StocksContext context, IMapper mapper, IOptions<ApplicationSettings> appSettings)
        {
            _appSettings = appSettings.Value;
            this._mapper = mapper;
            this.unitOfWork = new UnitOfWork(context);

        }
        #endregion


        #region FirstOpen
        [HttpGet]
        [Route("~/api/sellingorder/FirstOpen")]
        public IActionResult FirstOpen()
       {
            SellingOrderModel model = new SellingOrderModel();

            model.Count = unitOfWork.SellingOrderRepository.Count();
            if (model.Count > 0)
            {

                model.LastCode = unitOfWork.SellingOrderRepository.Last().Code;
            }
           

            return Ok(model);
        }

        #endregion


        #region Pagination
        [HttpGet]
        [Route("~/api/sellingorder/Paging/{pageNumber}")]
        public IActionResult Pagination(int pageNumber)
        {
            if (pageNumber > 0)
            {
                var sellingorder = unitOfWork.SellingOrderRepository.Get(page: pageNumber).FirstOrDefault();
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
                        PartnerCode=m.Partner.Code




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



        #region Insert Methods
        [HttpPost]
        [Route("~/api/sellingorder/Add")]
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
        [Route("~/api/sellingorder/Update/{id}")]
        public IActionResult Update(int id, [FromBody] SellingOrderModel sellingOrderModel)
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
        [Route("~/api/sellingorder/Delete/{id}")]
        public IActionResult Delete(int? id)
        {

            if (id > 0)
            {
                var sellingorder = unitOfWork.SellingOrderRepository.GetByID(id);
                if (sellingorder == null)
                {
                    return Ok(6);
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
        [Route("~/api/sellingorder/getpartCode/{id}")]
        public partenerCodeModel getpartCode(int? id)
        {


            var c = unitOfWork.PartnerRepository.GetByID(id);
            partenerCodeModel codeModel = new partenerCodeModel();
            codeModel.Code = c.Code;
            codeModel.NameAr = c.NameAR;
            return codeModel;
        }




        [HttpGet]
        [Route("~/api/sellingorder/getsellingInvoise/{id}")]
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
                                 WHERE SN.SellingOrderID = "+id+") SELECT q2.RowNum ,q2.InvoiceNum,q2.ExeDate,q2.PartnerName,q2.StockCount,q2.SellingPrice,q2.pp , q2.NetAmount,CASE WHEN q2.RowNum=1 THEN q2.pp ELSE (SUM(q2.pp)OVER ( ORDER BY q2.ExeDate ROWS BETWEEN UNBOUNDED PRECEDING AND 1 PRECEDING))-q2.StockCount END AS balance FROM q2";
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



        [HttpGet]
        [Route("~/api/orders/getparteners/{id}")]
        public IActionResult getparteners(int? id)
        {

            if (id != null || id != 0)
            {
                var p = unitOfWork.PortfolioTransactionsRepository.Get(filter: x => x.PortfolioID == id).Select(a => new PartenerModel
                {

                    NameAR = a.Partner.NameAR,
                    Code = a.Partner.Code,
                    NameEN = a.Partner.NameEN,
                    PartnerID = a.PartnerID,


                });

                return Ok(p);
            }
            else
            {
                return Ok();
            }

        }
        [Route("~/api/Order/Getname/{id}")]
        public IActionResult Getpartenername(int id)
        {
            var n = unitOfWork.PartnerRepository.GetEntity(filter: x => x.PartnerID == id);
            PortfolioPartners portfolioPartners = new PortfolioPartners();
            portfolioPartners.NameAR = n.NameAR;
            portfolioPartners.NameEN = n.NameEN;
            portfolioPartners.Code = n.Code;
            portfolioPartners.PartnerID = n.PartnerID;
            


            return Ok(portfolioPartners);

        }




    }
}
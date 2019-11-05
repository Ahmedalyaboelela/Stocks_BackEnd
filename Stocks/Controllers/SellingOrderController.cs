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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Stocks.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class sellingorderController : ControllerBase
    {
        #region CTOR & Definitions
        private UnitOfWork unitOfWork;
        private readonly IMapper _mapper;

        public sellingorderController(StocksContext context, IMapper mapper)
        {
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
        public IActionResult PostEmp([FromBody] SellingOrderModel sellingOrderModel)
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
        public IActionResult getsellingInvoise(int? id)
        {
            var IncoiceModel = unitOfWork.SellingInvoiceReposetory.Get(filter: x => x.SellingOrderID == id).Select(m => new INFOInvoices {

                Code = m.Code,
                SellDate = m.Date.Value.ToString("d/M/yyyy"),
                SellDateHijri = DateHelper.GetHijriDate(m.Date),
                TotalStockCount = getTotalStocks(m.SellingInvoiceID),
                SellingPrice = unitOfWork.SellingInvoiceDetailRepository.GetEntity(filter: x => x.SellingInvoiceID == m.SellingInvoiceID).SellingPrice,
                StockCount= unitOfWork.SellingInvoiceDetailRepository.GetEntity(filter: x => x.SellingInvoiceID == m.SellingInvoiceID).StockCount,
                NetAmmount=unitOfWork.SellingInvoiceDetailRepository.GetEntity(filter: x => x.SellingInvoiceID == m.SellingInvoiceID).NetAmmount,



            });


            

            return Ok(IncoiceModel);

        }
        public float getTotalStocks(int? id)
        {
            float totalStocks =0.0f;
            var DetailsModels = unitOfWork.SellingInvoiceDetailRepository.Get(filter: a => a.SellingInvoiceID == id);
         
            foreach (var item in DetailsModels)
            {
                totalStocks += item.StockCount;
            }

            return totalStocks;

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
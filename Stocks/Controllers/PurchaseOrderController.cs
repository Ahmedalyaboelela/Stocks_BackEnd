﻿using System;
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

namespace Stocks.Controllers
{
  //  [Authorize(Roles = "SuperAdmin,Admin,Employee")]
    [Route("api/[controller]")]
    
    public class PurchaseOrderController : ControllerBase
    {
        private UnitOfWork unitOfWork;
        private readonly IMapper _mapper;
        public PurchaseOrderController(StocksContext context, IMapper mapper)
        {
            this.unitOfWork = new UnitOfWork(context);
            this._mapper = mapper;
         
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
                        PartnerCode = m.Partner.Code




                    });




                if (Details != null)
                {
                    model.purchaseInvoiceDetailsModels = Details;

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
        public IActionResult PostEmp([FromBody] PurchaseOrderModel purchaseOrderModel)
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

                    var model = _mapper.Map<PurchaseOrder>(purchaseOrderModel);




                    unitOfWork.PurchaseOrderRepository.Insert(model);





                    if (purchaseOrderModel.purchaseInvoiceDetailsModels != null)
                    {
                        foreach (var item in purchaseOrderModel.purchaseInvoiceDetailsModels)
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
                    if (purchaseOrderModel.purchaseInvoiceDetailsModels != null)
                    {

                        foreach (var item in purchaseOrderModel.purchaseInvoiceDetailsModels)
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
                        if (purchaseOrderModel.purchaseInvoiceDetailsModels != null)
                        {

                            foreach (var item in purchaseOrderModel.purchaseInvoiceDetailsModels)
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

                var Details = unitOfWork.PurchaseOrderDetailRepository.Get(filter: x => x.PurchaseOrderID == purchaseorder.PurchaseOrderID);
                if (Details != null)
                {
                    unitOfWork.PurchaseOrderDetailRepository.RemovRange(Details);
                }




                unitOfWork.SellingOrderRepository.Delete(purchaseorder);
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
        [Route("~/api/PurchaseOrder/getsellingInvoise/{id}")]
        public IActionResult getsellingInvoise(int? id)
        {
            var IncoiceModel = unitOfWork.PurchaseInvoiceRepository.Get(filter: x => x.PurchaseOrderID == id).Select(m => new INFOInvoices
            {

                Code = m.Code,
                purchaseDate = m.Date.Value.ToString("d/M/yyyy"),
                purchaseDateHijri = DateHelper.GetHijriDate(m.Date),
                TotalStockCount = getTotalStocks(m.PurchaseInvoiceID),
                purchasePrice = unitOfWork.PurchaseInvoiceDetailRepository.GetEntity(filter: x => x.PurchaseInvoiceID == m.PurchaseInvoiceID).PurchasePrice,
                StockCount = unitOfWork.PurchaseInvoiceDetailRepository.GetEntity(filter: x => x.PurchaseInvoiceID == m.PurchaseInvoiceID).StockCount,
                NetAmmount = unitOfWork.PurchaseInvoiceDetailRepository.GetEntity(filter: x => x.PurchaseInvoiceID == m.PurchaseInvoiceID).NetAmmount,



            });




            return Ok(IncoiceModel);

        }
        public float getTotalStocks(int? id)
        {
            float totalStocks = 0.0f;
            var DetailsModels = unitOfWork.PurchaseInvoiceDetailRepository.Get(filter: a => a.PurchaseInvoiceID == id);

            foreach (var item in DetailsModels)
            {
                totalStocks += item.StockCount;
            }

            return totalStocks;

        }

    }
}
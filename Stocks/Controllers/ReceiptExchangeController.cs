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
    public class ReceiptExchangeController : ControllerBase
    {
        #region CTOR & Definitions
        private UnitOfWork unitOfWork;
        private readonly IMapper _mapper;

        public ReceiptExchangeController(StocksContext context, IMapper mapper)
        {
            this.unitOfWork = new UnitOfWork(context);
            this._mapper = mapper;
        }

        #endregion


        #region GET Methods

        public ReceiptExchangeModel GetReceiptExchange(ReceiptExchange RecExc,bool type)
        {
            var model = _mapper.Map<ReceiptExchangeModel>(RecExc);
            if (model == null)
            {
                return model;
            }

            #region Date part

            model.Date = RecExc.Date.Value.ToString("dd/MM/yyyy");
            model.DateHijri = DateHelper.GetHijriDate(RecExc.Date);

            model.ChiqueDate = RecExc.ChiqueDate.Value.ToString("dd/MM/yyyy");
            model.ChiqueDateHijri = DateHelper.GetHijriDate(RecExc.ChiqueDate);
            #endregion

            #region Details part
            var RecExcDetails = unitOfWork.ReceiptExchangeDetailRepository

                .Get(filter: m => m.ReceiptID == RecExc.ReceiptID)
                .Select(m => new ReceiptExchangeDetailModel
                {
                    ReceiptExchangeID = m.ReceiptExchangeID,
                    ReceiptExchangeAmount=m.ReceiptExchangeAmount,
                    ReceiptID = m.ReceiptID,
                    AccountID = m.AccountID,
                    AccNameAR = m.Account.Code,
                    AccNameEN=m.Account.NameEN,
                    ChiqueNumber = m.ChiqueNumber,
                    Type = m.Type
                });
            if (RecExcDetails != null)
                model.RecExcDetails = RecExcDetails;

            #endregion

            model.Count = unitOfWork.ReceiptExchangeRepository.Get(filter:m=>m.Type==type).Count();
            model.CurrencyNameAR = RecExc.Currency.NameAR;
            model.CurrencyNameEN= RecExc.Currency.NameEN;
            model.CurrencyCode = RecExc.Currency.Code;


            return model;
        }

        [HttpGet]
        [Route("~/api/ReceiptExchange/GetLast/{type}")]
        public IActionResult GetLast(bool type)
        {
           
            // get last Receipt or Exchange
            var RecExc = unitOfWork.ReceiptExchangeRepository.Get(filter:m=>m.Type==type).Last();
            if(RecExc !=null)
                return Ok(GetReceiptExchange(RecExc,type));
            else
                return Ok("Not Found");

        }


        [HttpGet]
        [Route("~/api/ReceiptExchange/Paging/{pageNumber}/{type}")]
        public IActionResult Pagination(int pageNumber,bool type)
        {
            if (pageNumber > 0)
            {

                var RecExc = unitOfWork.ReceiptExchangeRepository.Get(filter: m => m.Type == type, page: pageNumber).FirstOrDefault();
                if(RecExc != null)
                    return Ok(GetReceiptExchange(RecExc,type));
                else
                    return Ok("Not Found");

            }
            else
                return Ok("enter valid page number ! ");
        }


        [HttpGet]
        [Route("~/api/ReceiptExchange/Get/{id}/{type}")]

        public IActionResult GetById(int id,bool type)
        {

            if (id > 0)
            {

                var RecExc = unitOfWork.ReceiptExchangeRepository.Get(filter: m => m.Type == type &&m.ReceiptID==id).FirstOrDefault();
                if (RecExc != null)
                    return Ok(GetReceiptExchange(RecExc, type));
                else
                    return Ok("Not Found");


            }
            else
                return Ok("Invalid  Id !");
        }


        [Route("~/api/ReceiptExchange/GetAll/{type}")]
        public IActionResult GetAll(bool type)
        {
            var RecExcs = unitOfWork.ReceiptExchangeRepository.Get(filter: m => m.Type == type).ToList();

            var model = _mapper.Map<IEnumerable<ReceiptExchangeModel>>(RecExcs).ToList();

            if (model == null)
            {
                return Ok(model);
            }

            for (int i = 0; i < RecExcs.Count(); i++)
            {
                for (int j = i; j < model.Count(); j++)
                {
                    if (model[j].ReceiptID == RecExcs[i].ReceiptID)
                    {
                        #region Date part

                        model[j].Date = RecExcs[i].Date.Value.ToString("dd/MM/yyyy");
                        model[j].DateHijri = DateHelper.GetHijriDate(RecExcs[i].Date);

                        model[j].ChiqueDate = RecExcs[i].ChiqueDate.Value.ToString("dd/MM/yyyy");
                        model[j].ChiqueDateHijri = DateHelper.GetHijriDate(RecExcs[i].ChiqueDate);
                        #endregion

                        model[j].Count= unitOfWork.ReceiptExchangeRepository.Get(filter: m => m.Type == type).Count();

                        #region Details part
                        var RecExcDetails = unitOfWork.ReceiptExchangeDetailRepository

                            .Get(filter: m => m.ReceiptID == RecExcs[i].ReceiptID)
                            .Select(m => new ReceiptExchangeDetailModel
                            {
                                ReceiptExchangeID = m.ReceiptExchangeID,
                                ReceiptExchangeAmount=m.ReceiptExchangeAmount,
                                ReceiptID = m.ReceiptID,
                                AccountID = m.AccountID,
                                AccNameAR = m.Account.Code,
                                AccNameEN = m.Account.NameEN,
                                ChiqueNumber = m.ChiqueNumber,
                                Type = m.Type

                            });
                        if (RecExcDetails != null)
                            model[j].RecExcDetails = RecExcDetails;

                        #endregion


                    }
                    else
                        continue;


                }

            }


            return Ok(model);
        }

        #endregion

        #region Insert Methods
        [HttpPost]
        [Route("~/api/ReceiptExchange/Add")]
        public IActionResult PostItem([FromBody] ReceiptExchangeModel recExcModel)
        {

            if (ModelState.IsValid)
            {

                var Check = unitOfWork.ReceiptExchangeRepository.Get();
                if (recExcModel == null)
                {
                    return Ok("no scueess");
                }
                if (Check.Any(m => m.Code == recExcModel.Code))
                {
                    return Ok("الرمز موجود مسبقا");
                }
                else
                {
                    // map model to entity
                    var model = _mapper.Map<ReceiptExchange>(recExcModel);


                    var recExcDetails = recExcModel.RecExcDetails;

                    var RecExcDetails = _mapper.Map<IEnumerable<ReceiptExchangeDetail>>(recExcModel.RecExcDetails);
                    try
                    {
                        // insert main data of receipt exchange 
                        unitOfWork.ReceiptExchangeRepository.Insert(model);

                        if (RecExcDetails != null)
                        {
                            foreach (var item in recExcDetails)
                            {
                                var obj = _mapper.Map<ReceiptExchangeDetail>(item);
                                obj.ReceiptID = model.ReceiptID;
                                unitOfWork.ReceiptExchangeDetailRepository.Insert(obj);
                            }
                        }


                        bool CheckSave = unitOfWork.Save();



                        if (CheckSave == true)
                        {
                            return Ok(recExcModel);
                        }
                        else
                        {
                            return Ok("خطأ في ادخال البيانات");
                        }

                    }
                    catch (Exception ex)
                    {
                        return Ok("خطأ في ادخال البيانات");

                    }


                }



            }
            else
            {
                return BadRequest("Bad Request !");
            }
        }
        #endregion


        #region Update Methods
        [HttpPut]
        [Route("~/api/ReceiptExchange/Update/{id}/{type}")]
        public IActionResult Update(int id,bool type, [FromBody] ReceiptExchangeModel Model)
        {
            if (id != Model.ReceiptID)
            {

                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                // map main data to entity
                var model = _mapper.Map<ReceiptExchange>(Model);

                var recExcDetail = Model.RecExcDetails;
                //var EmpolyeeCard = _mapper.Map<IEnumerable<EmployeeCard>>(empolyeeCard);

                var Check = unitOfWork.ReceiptExchangeRepository.Get(NoTrack: "NoTrack", filter: m => m.Type == type);
                var oldDetail = unitOfWork.ReceiptExchangeDetailRepository.Get(NoTrack: "NoTrack", filter: m => m.ReceiptID == model.ReceiptID);
                if (oldDetail != null)
                {
                    unitOfWork.ReceiptExchangeDetailRepository.RemovRange(oldDetail);

                }

                if (Check.Any(m => m.Code != Model.Code))
                {
                    unitOfWork.ReceiptExchangeRepository.Update(model);


                    foreach (var item in recExcDetail)
                    {
                        item.ReceiptID = model.ReceiptID;
                        item.ReceiptExchangeID = 0;
                        var newDetail = _mapper.Map<ReceiptExchangeDetail>(item);

                        unitOfWork.ReceiptExchangeDetailRepository.Insert(newDetail);

                    }

                    var Result = unitOfWork.Save();
                    if (Result == true)
                        return Ok(Model);
                    else
                        return Ok("حدث خطا");

                }
                else
                {
                    if (Check.Any(m => m.Code == Model.Code && m.ReceiptID == id))
                    {

                        unitOfWork.ReceiptExchangeRepository.Update(model);
                        
                        foreach (var item in recExcDetail)
                        {
                            item.ReceiptID = model.ReceiptID;
                            item.ReceiptExchangeID = 0;
                            var neweDetail = _mapper.Map<ReceiptExchangeDetail>(item);

                            unitOfWork.ReceiptExchangeDetailRepository.Insert(neweDetail);

                        }

                        var Result = unitOfWork.Save();
                        if (Result == true)
                            return Ok(Model);
                        else
                            return Ok("حدث خطا");
                    }
                    else
                    {
                        return Ok("الرمز موجود مسبقا");
                    }
                }

            }
            else
            {
                return BadRequest(ModelState);
            }
        }
        #endregion


        #region Delete Methods

        [HttpDelete]
        [Route("~/api/ReceiptExchange/Delete/{id}")]
        public IActionResult Delete(int? id)
        {

            if (id == null)
            {

                return BadRequest();
            }
            //var RecExc = unitOfWork.ReceiptExchangeRepository.Get(filter: m => m.Type == type && m.ReceiptID == id).FirstOrDefault();
            var RecExc = unitOfWork.ReceiptExchangeRepository.GetByID(id);

            if (RecExc == null)
            {
                return BadRequest();
            }
            var recDetails = unitOfWork.ReceiptExchangeDetailRepository.Get(filter: m => m.ReceiptID == id);



            unitOfWork.ReceiptExchangeDetailRepository.RemovRange(recDetails);
            unitOfWork.ReceiptExchangeRepository.Delete(RecExc);
            var Result = unitOfWork.Save();
            if (Result == true)
            {
                return Ok("item deleted .");
            }
            else
            {
                return NotFound();
            }

        }

        #endregion
    }
}
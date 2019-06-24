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
    public class NoticeController : ControllerBase
    {
        #region CTOR & Definitions
        private UnitOfWork unitOfWork;
        private readonly IMapper _mapper;

        public NoticeController(StocksContext context, IMapper mapper)
        {
            this.unitOfWork = new UnitOfWork(context);
            this._mapper = mapper;
        }

        #endregion

        #region GET Methods

        public NoticeModel GetNotice(Notice notice, bool type)
        {
            var model = _mapper.Map<NoticeModel>(notice);
            if (model == null)
            {
                return model;
            }

            #region Date part

            model.NoticeDate = notice.NoticeDate.Value.ToString("dd/MM/yyyy");
            model.NoticeDateHijri = DateHelper.GetHijriDate(notice.NoticeDate);
            #endregion

            #region Details part
            var Details = unitOfWork.NoticeDetailRepository

                .Get(filter: m => m.NoticeID == notice.NoticeID)
                .Select(m => new NoticeDetailModel
                {
                    NoticeDetailID = m.NoticeDetailID,
                    NoticeID = m.NoticeID,
                    CreditDebitMoney = m.CreditDebitMoney,
                    CreditorDebitStocks = m.CreditorDebitStocks
                });
            if (Details != null)
                model.NoticeModelDetails = Details;

            #endregion

            model.Count = unitOfWork.NoticeRepository.Get(filter: m => m.Type == type).Count();

            model.PortfolioNameAR = notice.Portfolio.NameAR;
            model.PortfolioNameEN = notice.Portfolio.NameEN;
            model.PortfolioCode = notice.Portfolio.Code;

            model.EmployeeNameAR = notice.Employee.NameAR;
            model.EmployeeNameEN = notice.Employee.NameEN;
            model.EmployeeCode = notice.Employee.Code;

            return model;
        }

        [HttpGet]
        [Route("~/api/Notice/GetLast/{type}")]
        public IActionResult GetLast(bool type)
        {

            // get last Receipt or Exchange
            var notice = unitOfWork.NoticeRepository.Get(filter: m => m.Type == type).Last();
            if (notice != null)
                return Ok(GetNotice(notice, type));
            else
                return Ok("Not Found");

        }


        [HttpGet]
        [Route("~/api/Notice/Paging/{pageNumber}/{type}")]
        public IActionResult Pagination(int pageNumber, bool type)
        {
            if (pageNumber > 0)
            {

                var notice = unitOfWork.NoticeRepository.Get(filter: m => m.Type == type, page: pageNumber).FirstOrDefault();
                if (notice != null)
                    return Ok(GetNotice(notice, type));
                else
                    return Ok("Not Found");

            }
            else
                return Ok("enter valid page number ! ");
        }


        [HttpGet]
        [Route("~/api/Notice/Get/{id}/{type}")]

        public IActionResult GetById(int id, bool type)
        {

            if (id > 0)
            {

                var notice = unitOfWork.NoticeRepository.Get(filter: m => m.Type == type && m.NoticeID == id).FirstOrDefault();
                if (notice != null)
                    return Ok(GetNotice(notice, type));
                else
                    return Ok("Not Found");


            }
            else
                return Ok("Invalid  Id !");
        }


        [Route("~/api/Notice/GetAll/{type}")]
        public IActionResult GetAll(bool type)
        {
            var notices = unitOfWork.NoticeRepository.Get(filter: m => m.Type == type).ToList();

            var model = _mapper.Map<IEnumerable<NoticeModel>>(notices).ToList();

            if (model == null)
            {
                return Ok(model);
            }

            for (int i = 0; i < notices.Count(); i++)
            {
                for (int j = i; j < model.Count(); j++)
                {
                    if (model[j].NoticeID == notices[i].NoticeID)
                    {
                        #region Date part

                        model[j].NoticeDate = notices[i].NoticeDate.Value.ToString("dd/MM/yyyy");
                        model[j].NoticeDateHijri = DateHelper.GetHijriDate(notices[i].NoticeDate);
                        #endregion

                        model[j].Count = unitOfWork.NoticeRepository.Get(filter: m => m.Type == type).Count();

                        #region Details part
                        var Details = unitOfWork.NoticeDetailRepository

                            .Get(filter: m => m.NoticeID == notices[i].NoticeID)
                            .Select(m => new NoticeDetailModel
                            {
                                NoticeDetailID = m.NoticeDetailID,
                                NoticeID = m.NoticeID,
                                CreditDebitMoney = m.CreditDebitMoney,
                                CreditorDebitStocks = m.CreditorDebitStocks

                            });
                        if (Details != null)
                            model[j].NoticeModelDetails = Details;

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
        [Route("~/api/Notice/Add")]
        public IActionResult PostItem([FromBody] NoticeModel Model)
        {

            if (ModelState.IsValid)
            {

                var Check = unitOfWork.NoticeRepository.Get();
                if (Model == null)
                {
                    return Ok("no scueess");
                }
                if (Check.Any(m => m.Code == Model.Code))
                {
                    return Ok("الرمز موجود مسبقا");
                }
                else
                {
                    // map model to entity
                    var model = _mapper.Map<Notice>(Model);


                    //var Details = Model.NoticeModelDetails;

                    var Details = _mapper.Map<IEnumerable<NoticeDetail>>(Model.NoticeModelDetails);
                    try
                    {
                        // insert main data of receipt exchange 
                        unitOfWork.NoticeRepository.Insert(model);

                        if (Details != null)
                        {
                            foreach (var item in Model.NoticeModelDetails)
                            {
                                var obj = _mapper.Map<NoticeDetail>(item);
                                obj.NoticeID = model.NoticeID;
                                unitOfWork.NoticeDetailRepository.Insert(obj);
                            }
                        }


                        bool CheckSave = unitOfWork.Save();



                        if (CheckSave == true)
                        {
                            return Ok(Model);
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
        [Route("~/api/Notice/Update/{id}/{type}")]
        public IActionResult Update(int id, bool type, [FromBody] NoticeModel Model)
        {
            if (id != Model.NoticeID)
            {

                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                // map main data to entity
                var model = _mapper.Map<Notice>(Model);

                var Detail = Model.NoticeModelDetails;
                //var EmpolyeeCard = _mapper.Map<IEnumerable<EmployeeCard>>(empolyeeCard);

                var Check = unitOfWork.NoticeRepository.Get(NoTrack: "NoTrack", filter: m => m.Type == type);
                var oldDetail = unitOfWork.NoticeDetailRepository.Get(NoTrack: "NoTrack", filter: m => m.NoticeID == model.NoticeID);
                unitOfWork.NoticeDetailRepository.RemovRange(oldDetail);


                if (Check.Any(m => m.Code != Model.Code))
                {
                    unitOfWork.NoticeRepository.Update(model);


                    foreach (var item in Detail)
                    {
                        item.NoticeID = model.NoticeID;
                        item.NoticeDetailID = 0;
                        var newDetail = _mapper.Map<NoticeDetail>(item);

                        unitOfWork.NoticeDetailRepository.Insert(newDetail);

                    }

                    var Result = unitOfWork.Save();
                    if (Result == true)
                        return Ok(Model);
                    else
                        return Ok("حدث خطا");

                }
                else
                {
                    if (Check.Any(m => m.Code == Model.Code && m.NoticeID == id))
                    {

                        unitOfWork.NoticeRepository.Update(model);

                        foreach (var item in Detail)
                        {
                            item.NoticeID = model.NoticeID;
                            item.NoticeDetailID = 0;
                            var neweDetail = _mapper.Map<NoticeDetail>(item);

                            unitOfWork.NoticeDetailRepository.Insert(neweDetail);

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
        [Route("~/api/Notice/Delete/{id}")]
        public IActionResult Delete(int? id)
        {

            if (id == null)
            {

                return BadRequest();
            }
            //var RecExc = unitOfWork.ReceiptExchangeRepository.Get(filter: m => m.Type == type && m.ReceiptID == id).FirstOrDefault();
            var notice = unitOfWork.NoticeRepository.GetByID(id);

            if (notice == null)
            {
                return BadRequest();
            }
            var noticeDetails = unitOfWork.NoticeDetailRepository.Get(filter: m => m.NoticeID == id);



            unitOfWork.NoticeDetailRepository.RemovRange(noticeDetails);
            unitOfWork.NoticeRepository.Delete(notice);
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
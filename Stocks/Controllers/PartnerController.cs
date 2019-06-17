using System;
using System.Collections.Generic;
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
using Microsoft.EntityFrameworkCore;

namespace Stocks.Controllers
{//
    [Route("api/[controller]")]
    [ApiController]
    public class PartnerController : ControllerBase
    {
        #region CTOR & Definitions
        private UnitOfWork unitOfWork;
        private readonly IMapper _mapper;

        public PartnerController(StocksContext context,IMapper mapper)
        {
            this.unitOfWork = new UnitOfWork(context);
            this._mapper = mapper;
        }
        #endregion

        #region GET Methods
        [Route("~/api/Partner/GetLast")]
        public IActionResult GetLastPartner()
        {
            var partner = unitOfWork.PartnerRepository.Last();

            try
            {
                var model = _mapper.Map<PartenerModel>(partner);

                if (model == null)
                {
                    return Ok( model);
                }

                // deal with date
                model.IssueDate = partner.IssueDate.Value.ToString("dd/MM/yyyy");
                model.IssueDateHijri = DateHelper.GetHijriDate(partner.IssueDate);

                model.AccountNameAr = partner.Account.NameAR;
                model.AccountNameEn = partner.Account.NameEN;

                model.CountryNameAr = partner.Country.NameAR;
                model.CountryNameEn = partner.Country.NameEN;

                model.Count = unitOfWork.PartnerRepository.Count();
                if (model.Count == 0)
                {
                    return Ok( model);

                }

                return Ok( model);
            }
            catch (NullReferenceException ex)
            {

                return Ok("No data found ");
            }
            catch (SqlException ex)
            {
                return Ok("Data error !");

                // If there is some problem with database
            }
            catch (Exception ex)
            {
                return Ok("Technical problem !");

                // Handle all unexpected errors
            }

        }

        [HttpGet]
        [Route("~/api/Partner/Paging/{pageNumber}")]
        public IActionResult Pagination(int pageNumber)
        {
            try
            {
                if (pageNumber > 0)
                {
                    var partner = unitOfWork.PartnerRepository.Get(page: pageNumber).FirstOrDefault();

                    var model = _mapper.Map<PartenerModel>(partner);
                    if (model == null)
                    {
                        return Ok(model);
                    }

                    // deal with date
                    model.IssueDate = partner.IssueDate.Value.ToString("dd/MM/yyyy");
                    model.IssueDateHijri = DateHelper.GetHijriDate(partner.IssueDate);

                    model.AccountNameAr = partner.Account.NameAR;
                    model.AccountNameEn = partner.Account.NameEN;

                    model.CountryNameAr = partner.Country.NameAR;
                    model.CountryNameEn = partner.Country.NameEN;

                    model.Count = unitOfWork.PartnerRepository.Count();
                    if (model.Count == 0)
                    {
                        return Ok(model);

                    }
                    return Ok(model);


                }
                else
                    return Ok();
           
            }
            catch (DbUpdateException ex)
            {
                // What to do if CategoryID parameter is null
                return Ok("null");
            }
            catch (FormatException ex)
            {
                // If CategoryID parameter is not a number
                return Ok("not number");
            }
            catch (SqlException ex)
            {
                // If there is some problem with database
                return Ok("database");
            }
            catch (Exception ex)
            {
                // Handle all unexpected errors
                return Ok("error");
            }

        }

        [HttpGet]
        [Route("~/api/Partner/Get/{id}")]

        public PartenerModel GetpartnerById(int id)
        {
            var partner = unitOfWork.PartnerRepository.GetByID(id);

            var model = _mapper.Map<PartenerModel>(partner);
            if (model == null)
            {
                return model;
            }
            else
            {

                // deal with date
                model.IssueDate = partner.IssueDate.Value.ToString("dd/MM/yyyy");
                model.IssueDateHijri = DateHelper.GetHijriDate(partner.IssueDate);

                model.AccountNameAr = partner.Account.NameAR;
                model.AccountNameEn = partner.Account.NameEN;

                model.CountryNameAr = partner.Country.NameAR;
                model.CountryNameEn = partner.Country.NameEN;

                model.Count = unitOfWork.PartnerRepository.Count();
                if (model.Count == 0)
                {
                    return model;

                }

                return model;
            }

        }



        [Route("~/api/Partner/GetAll")]
        public IEnumerable<PartenerModel> GetAllPartners()
        {
            var partner = unitOfWork.PartnerRepository.Get().ToList();
            var model = _mapper.Map<IEnumerable<PartenerModel>>(partner).ToList();

            if (model == null)
            {
                return model;
            }

            for (int i = 0; i < partner.Count(); i++)
            {
                for (int j = i; j < model.Count(); j++)
                {
                    if (model[j].PartnerID == partner[i].PartnerID)
                    {
                        // deal with date
                        model[j].IssueDate = partner[i].IssueDate.Value.ToString("dd/MM/yyyy");
                        model[j].IssueDateHijri = DateHelper.GetHijriDate(partner[i].IssueDate);

                        model[j].AccountNameAr = partner[i].Account.NameAR;
                        model[j].AccountNameEn = partner[i].Account.NameEN;
                        model[j].CountryNameAr = partner[i].Country.NameAR;
                        model[j].CountryNameEn = partner[i].Country.NameEN;


                    }
                    else
                        continue;


                }

            }
          

            return (model);
        }
        #endregion

        #region Insert Method

        [HttpPost]
        [Route("~/api/Partner/AddPartner")]
        public IActionResult PostPartner([FromBody] PartenerModel partnerModel)
        {
            if (ModelState.IsValid)
            {
                var model = _mapper.Map<Partner>(partnerModel);
                if (model == null)
                {
                    return Ok(model);
                }
                var Check = unitOfWork.PartnerRepository.Get();
                if (Check.Any(m => m.Code == partnerModel.Code))
                {

                    return Ok("الرمز موجود مسبقا");
                }
                else
                {
                    if (Check.Any(m => m.NameAR == partnerModel.NameAR))
                    {

                        return Ok("الاسم موجود مسبقا");
                    }
                    else
                    {
                        unitOfWork.PartnerRepository.Insert(model);
                        var Result = unitOfWork.Save();
                        if (Result == true)
                            return Ok(partnerModel);
                        else
                            return NotFound();
                    }
                }




            }
            else
            {
                return BadRequest();
            }
        }
        #endregion

        #region Update Method
        [HttpPut]
        [Route("~/api/Partner/EditPartner/{id}")]

        public IActionResult PutPartner(int id, [FromBody] PartenerModel partnerModel)
        {
            

            if (ModelState.IsValid)
            {
                var checkAccount = unitOfWork.PartnerRepository.Get(NoTrack: "NoTrack", filter: a => a.PartnerID == id);
                if (checkAccount != null)
                {
                    var model = _mapper.Map<Partner>(partnerModel);
                    if (model == null)
                    {
                        return Ok(model);

                    }

                    var Check = unitOfWork.PartnerRepository.Get(NoTrack: "NoTrack");
                    if (!Check.Any(m => m.Code == partnerModel.Code))
                    {

                        unitOfWork.PartnerRepository.Update(model);
                        var Result = unitOfWork.Save();
                        if (Result == true)
                            return Ok(partnerModel);
                        else
                            return Ok("حدث خطأ غير متوقع");

                    }
                    else
                    {
                        if (Check.Any(m => m.Code == partnerModel.Code && m.PartnerID == id))
                        {

                            unitOfWork.PartnerRepository.Update(model);
                            var Result = unitOfWork.Save();
                            if (Result == true)
                                return Ok(partnerModel);
                            else
                                return NotFound();
                        }
                        else
                        {
                            return Ok("الرمز موجود مسبقا");
                        }
                    }
                }
                else
                    return BadRequest("Invalid Partner !");


            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        #endregion

        #region Delete Method
        [HttpDelete]
        [Route("~/api/Partner/DeletePartner/{id}")]

        public IActionResult DeletePartner(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            else
            {
                var account = unitOfWork.PartnerRepository.GetByID(id);
                if (account == null)
                {
                    return BadRequest();
                }
                else
                {

                    unitOfWork.PartnerRepository.Delete(id);
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
                                return Ok("Item related with another data .");

                            }
                        }
                    }
                    return Ok("Item deleted successfully .");




                }
            }
        }
        #endregion

    }
}
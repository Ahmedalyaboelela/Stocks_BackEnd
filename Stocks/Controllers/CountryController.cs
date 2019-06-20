﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BAL.Model;
using BAL.Repositories;
using DAL.Context;
using DAL.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Stocks.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountryController : ControllerBase
    {
        #region CTOR & Definitions
        private UnitOfWork unitOfWork;
        private readonly IMapper _mapper;

        public CountryController(StocksContext context, IMapper mapper)
        {
            this._mapper = mapper;
            this.unitOfWork = new UnitOfWork(context);

        }
        #endregion

        #region GET Methods
        [Route("~/api/Country/GetLast")]
        public CountryModel GetLastCountry()
        {
            var country = unitOfWork.CountryRepository.Last();

            var model = _mapper.Map<CountryModel>(country);
            if (model == null)
            {
                return model;
            }
           

            model.Count = unitOfWork.CountryRepository.Count();
            if (model.Count == 0)
            {
                return model;

            }

            return model;



        }

        [HttpGet]
        [Route("~/api/Country/Paging/{pageNumber}")]
        public CountryModel Pagination(int pageNumber)
        {
            if (pageNumber > 0)
            {
                var country = unitOfWork.CountryRepository.Get(page: pageNumber).FirstOrDefault();
                var model = _mapper.Map<CountryModel>(country);
                if (model == null)
                {
                    return model;
                }

                model.Count = unitOfWork.CountryRepository.Count();
                if (model.Count == 0)
                {
                    return model;

                }
                return model;

            }
            else
                return null;

        }

        [HttpGet]
        [Route("~/api/Country/Get/{id}")]

        public CountryModel GetCountryById(int id)
        {
            var country = unitOfWork.CountryRepository.GetByID(id);

            var model = _mapper.Map<CountryModel>(country);
            if (model == null)
            {
                return model;
            }
            else
            {
              
                model.Count = unitOfWork.CountryRepository.Count();
                if (model.Count == 0)
                {
                    return model;

                }

                return model;
            }

        }



        [Route("~/api/Country/GetAll")]
        public IEnumerable<CountryModel> GetAllCountries()
        {
            var country = unitOfWork.CountryRepository.Get();
            var model = _mapper.Map<IEnumerable<CountryModel>>(country);

            if (model == null)
            {
                return model;
            }

            return (model);
        }

        #endregion

        #region Insert Method
        [HttpPost]
        [Route("~/api/Country/AddCountry/")]
        public IActionResult PostCountry([FromBody] CountryModel countryModel)
        {
            if (ModelState.IsValid)
            {
                var model = _mapper.Map<Country>(countryModel);
                if (model == null)
                {
                    return Ok(model);
                }
                var Check = unitOfWork.CountryRepository.Get();
                
                if (Check.Any(m => m.NameAR == countryModel.NameAR))
                {

                    return Ok("الاسم موجود مسبقا");
                }
                else
                {
                    unitOfWork.CountryRepository.Insert(model);
                    var Result = unitOfWork.Save();
                    if (Result == true)
                        return Ok(countryModel);
                    else
                        return NotFound();
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
        [Route("~/api/Country/EditCountry/{id}")]
        public IActionResult PutCountry(int id, [FromBody] CountryModel countryModel)
        {


            if (ModelState.IsValid)
            {
                var checkCountry = unitOfWork.CountryRepository.Get(NoTrack: "NoTrack", filter: a => a.CountryID == id);
                if (checkCountry != null)
                {
                    var model = _mapper.Map<Country>(countryModel);
                    if (model == null)
                    {
                        return Ok(model);

                    }

                    var Check = unitOfWork.CountryRepository.Get(NoTrack: "NoTrack");
                
                    if (!Check.Any(m => m.NameAR == countryModel.NameAR && m.CountryID != id))
                    {

                        unitOfWork.CountryRepository.Update(model);
                        var Result = unitOfWork.Save();
                        if (Result == true)
                            return Ok(countryModel);
                        else
                            return NotFound();
                    }
                    else
                    {
                        return Ok(" الاسم  موجود مسبقا");
                    }
                    
                }
                else
                    return BadRequest("Invalid Country !");


            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        #endregion

        #region Delete Method
        [HttpDelete]
        [Route("~/api/Country/DeleteCountry/{id}")]
        public IActionResult DeleteCountry(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            else
            {
                var country = unitOfWork.CountryRepository.GetByID(id);
                if (country == null)
                {
                    return BadRequest();
                }
                else
                {

                    unitOfWork.CountryRepository.Delete(id);
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
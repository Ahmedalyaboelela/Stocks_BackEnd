using System;
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
        [HttpGet]
        [Route("~/api/Country/FirstOpen")]
        public IActionResult FirstOpen()
        {
            CountryModel model = new CountryModel();
            model.LastCode = unitOfWork.PartnerRepository.Last().Code;
            model.Count = unitOfWork.PartnerRepository.Count();
            return Ok(model);
        }


        [Route("~/api/Country/GetLast")]
        public IActionResult GetLastCountry()
        {
            var country = unitOfWork.CountryRepository.Last();

            var model = _mapper.Map<CountryModel>(country);
            if (model == null)
            {
                return Ok(0);
            }
           

            model.Count = unitOfWork.CountryRepository.Count();
           
            return Ok(model);

        }

        [HttpGet]
        [Route("~/api/Country/Paging/{pageNumber}")]
        public IActionResult Pagination(int pageNumber)
        {
            if (pageNumber > 0)
            {
                var country = unitOfWork.CountryRepository.Get(page: pageNumber).FirstOrDefault();
                var model = _mapper.Map<CountryModel>(country);
                if (model == null)
                {
                    return Ok(0);
                }

                model.Count = unitOfWork.CountryRepository.Count();
                
                return Ok(model);

            }
            else
                return Ok(1);

        }

        [HttpGet]
        [Route("~/api/Country/Get/{id}")]

        public IActionResult GetCountryById(int id)
        {
            if (id > 0)
            {
                var country = unitOfWork.CountryRepository.GetByID(id);

                var model = _mapper.Map<CountryModel>(country);
                if (model == null)
                {
                    return Ok(0);
                }
                else
                {

                    model.Count = unitOfWork.CountryRepository.Count();
                  
                    return Ok(model);
                }
            }
            else
                return Ok(1);

        }



        [Route("~/api/Country/GetAll")]
        public IActionResult GetAllCountries()
        {
            var country = unitOfWork.CountryRepository.Get();
            var model = _mapper.Map<IEnumerable<CountryModel>>(country);

            if (model == null)
            {
                return Ok(0);
            }

            return Ok(model);
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
                    return Ok(0);
                }
                var Check = unitOfWork.CountryRepository.Get();
                
                if (Check.Any(m => m.NameAR == countryModel.NameAR))
                {

                    return Ok(2);
                }
                else
                {
                    unitOfWork.CountryRepository.Insert(model);
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
                                return Ok(5);

                            }
                            else
                                return Ok(6);
                        }
                    }

                    return Ok(countryModel);
                }
            }
            else
            {
                return Ok(3);
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
                        return Ok(0);

                    }

                    var Check = unitOfWork.CountryRepository.Get(NoTrack: "NoTrack");
                
                    if (!Check.Any(m => m.NameAR == countryModel.NameAR && m.CountryID != id))
                    {

                        unitOfWork.CountryRepository.Update(model);

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
                                    return Ok(5);

                                }
                                else
                                    return Ok(6);
                            }
                        }
                        return Ok(countryModel);
                    }
                    else
                    {
                        return Ok(2);
                    }
                    
                }
                else
                    return Ok(1);


            }
            else
            {
                return Ok(3);
            }
        }

        #endregion

        #region Delete Method
        [HttpDelete]
        [Route("~/api/Country/DeleteCountry/{id}")]
        public IActionResult DeleteCountry(int? id)
        {
            if (id > 0)
            {
                var country = unitOfWork.CountryRepository.GetByID(id);
                if (country == null)
                {
                    return Ok(0);
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
                                return Ok(5);

                            }
                            else
                                return Ok(6);
                        }
                    }
                    return Ok(4);

                }
            }
           
            else
                return Ok(1);
            
        }
        #endregion
    }
}
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
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Stocks.Controllers
{
    public class CurrenciesController : Controller
    {
        #region CTOR & Definitions
        private UnitOfWork unitOfWork;
        private readonly IMapper _mapper;

        public CurrenciesController(StocksContext context, IMapper mapper)
        {
            this.unitOfWork = new UnitOfWork(context);
            this._mapper = mapper;
        }
        #endregion

        #region GET Methods
        [HttpGet]
        [Route("~/api/Currency/FirstOpen")]
        public IActionResult FirstOpen()
        {
            CurrencyModel currencyModel = new CurrencyModel();
            var count = unitOfWork.CurrencyRepository.Count();
            if(count>0)
            {
                currencyModel.LastCode = unitOfWork.CurrencyRepository.Last().Code;
                currencyModel.Count = count;
            }
          
            return Ok(currencyModel);
        }


        [Route("~/api/Currency/GetLast")]
        public IActionResult GetLastAccount()
        {
            var currency = unitOfWork.CurrencyRepository.Last();

            var model = _mapper.Map<CurrencyModel>(currency);
            if (model == null)
            {
                return Ok(0);
            }



            model.Count = unitOfWork.CurrencyRepository.Count();

            return Ok(model);



        }

        [HttpGet]
        [Route("~/api/Currency/Paging/{pageNumber}")]
        public IActionResult Pagination(int pageNumber)
        {
            if (pageNumber > 0)
            {
                var currency = unitOfWork.CurrencyRepository.Get(page: pageNumber).FirstOrDefault();
                var model = _mapper.Map<CurrencyModel>(currency);
                if (model == null)
                {
                    return Ok(0);
                }




                model.Count = unitOfWork.CurrencyRepository.Count();

                return Ok(model);

            }
            else
                return Ok(1);

        }

        [HttpGet]
        [Route("~/api/Currency/Get/{id}")]

        public IActionResult GetCurrencyById(int id)
        {
            if (id > 0)
            {
                var currency = unitOfWork.CurrencyRepository.GetByID(id);

                var model = _mapper.Map<CurrencyModel>(currency);
                if (model == null)
                {
                    return Ok(0);
                }
                else
                {


                    model.Count = unitOfWork.CurrencyRepository.Count();


                    return Ok(model);
                }
            }
            else
                return Ok(1);

        }




        #endregion

        #region Insert Method

        [HttpPost]
        [Route("~/api/Currency/AddCurrency/")]
        public IActionResult AddCurrency([FromBody] CurrencyModel currencyModel)
        {
            if (ModelState.IsValid)
            {
                var model = _mapper.Map<Currency>(currencyModel);
                if (model == null)
                {
                    return Ok(0);
                }
                var Check = unitOfWork.CurrencyRepository.Get();
                if (Check.Any(m => m.Code == currencyModel.Code))
                {

                    return Ok(2);
                }

                else
                {
                    unitOfWork.CurrencyRepository.Insert(model);
                   
                        var result = unitOfWork.Save();
                        if (result == 200)
                        {
                            return Ok(7);
                        }
                    
               
                    return Ok(currencyModel);
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
        [Route("~/api/Currency/EditCurrency/{id}")]

        public IActionResult Putcurrency(int id, [FromBody] CurrencyModel currencyModel)
        {

            if (id != currencyModel.CurrencyID)
            {

                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                var model = _mapper.Map<Currency>(currencyModel);
                if (model == null)
                {
                    return Ok(0);
                }
                var Check = unitOfWork.CurrencyRepository.Get(NoTrack: "NoTrack");

                if (Check.Any(m => m.Code == currencyModel.Code))
                {
                    unitOfWork.CurrencyRepository.Update(model);

                    var Result = unitOfWork.Save();
                    if (Result == 200)
                    {
                        return Ok(7);
                    }


                    return Ok(currencyModel);


                }
                else
                {
                    if (Check.Any(m => m.Code != currencyModel.Code && m.CurrencyID == currencyModel.CurrencyID))
                    {
                        unitOfWork.CurrencyRepository.Update(model);

                        var Result = unitOfWork.Save();
                        if (Result == 200)
                        {
                            return Ok(7);
                        }


                        return Ok(currencyModel);
                    }


                }
            }

            else
            {
                return Ok(3);
            }
            return Ok(currencyModel);
        }

        #endregion

        #region Delete Method
        [HttpDelete]
        [Route("~/api/Currency/DeleteCurrency/{id}")]

        public IActionResult DeleteCurrency(int? id)
        {
            if (id == null)
            {
                return Ok(1);
            }

            else
            {
                var account = unitOfWork.CurrencyRepository.GetByID(id);
                if (account == null)
                {
                    return Ok(0);
                }
                else
                {

                    unitOfWork.CurrencyRepository.Delete(id);
                    try
                    {
                    var result= unitOfWork.Save();
                        if (result == 200)
                        {
                            return Ok(7);
                        }
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
        }
        #endregion
    }
}
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
{
    [Route("api/[controller]")]
    [ApiController]
    public class PortfolioController : ControllerBase
    {
        
        #region CTOR & Definitions
        private UnitOfWork unitOfWork;
        private readonly IMapper _mapper;

        public PortfolioController(StocksContext context, IMapper mapper)
        {
            this.unitOfWork = new UnitOfWork(context);
            this._mapper = mapper;
        }
        #endregion

        #region Get Methods
        public PortfolioModel GetPortfolio(Portfolio portfolio)
        {
            var model = _mapper.Map<PortfolioModel>(portfolio);
            if (model == null)
            {
                return model;
            }
            #region Date part

            model.EstablishDate = portfolio.EstablishDate.Value.ToString("dd/MM/yyyy");
            model.EstablishDateHijri = DateHelper.GetHijriDate(portfolio.EstablishDate);

            #endregion

            #region Accounts part
            var PortAccount = unitOfWork.PortfolioAccountRepository

                .GetEntity(filter: m => m.PortfolioID == portfolio.PortfolioID);

            if (PortAccount != null) {

                model.AccountID = PortAccount.AccountID;
                model.AccountCode = PortAccount.Account.Code;
                model.AccountNameAR = PortAccount.Account.NameAR;
                model.AccountNameEN = PortAccount.Account.NameEN;

            }
               

            #endregion

            #region Shareholders part
            var OpeningStocks = unitOfWork.PortfolioOpeningStocksRepository

                .Get(filter: m => m.PortfolioID == portfolio.PortfolioID)
                .Select(m => new PortfolioOpeningStocksModel
                {
                    PortOPenStockID = m.PortOPenStockID,
                    OpeningStocksCount = m.OpeningStocksCount,
                    PartnerID = m.PartnerID,
                    PartnerCode = m.Partner.Code,
                    PartnerNameAR = m.Partner.NameAR,
                    PartnerNameEN = m.Partner.NameEN,
                    PortfolioID = m.PortfolioID,
                    PortfolioCode = m.Portfolio.Code,
                    PortfolioNameAR = m.Portfolio.NameAR,
                    PortfolioNameEN = m.Portfolio.NameEN

                });
            if (OpeningStocks != null) {
                model.portfolioOpeningStocksModels = OpeningStocks; 
                foreach (var item in OpeningStocks)
                {
                    model.TotalStocksCount += item.OpeningStocksCount;
                }
                //var currentStocks = unitOfWork.PortfolioTransactions.GetEntity(x=> x.PortfolioID==portfolio.PortfolioID).CurrentStocksCount;
            }
               
            

            #endregion




            model.Count = unitOfWork.PortfolioRepository.Count();
            

            return model;
        }

        [HttpGet]
        [Route("~/api/Portfolio/FirstOpen")]
        public IActionResult FirstOpen()
        {
            PortfolioModel model = new PortfolioModel();
            var count = unitOfWork.PartnerRepository.Count();
            if(count>0)
            {
                model.LastCode = unitOfWork.PartnerRepository.Last().Code;
                model.Count = count;
            }
            
            return Ok(model);
        }

        [HttpGet]
        [Route("~/api/Portfolio/GetLast")]
        public IActionResult GetLastPortfolio()
        {
            var portfolio = unitOfWork.PortfolioRepository.Last();
            return Ok(GetPortfolio(portfolio));
        }


        [HttpGet]
        [Route("~/api/Portfolio/Paging/{pageNumber}")]
        public IActionResult Pagination(int pageNumber)
        {
            if (pageNumber > 0)
            {
                var portfolio = unitOfWork.PortfolioRepository.Get(page: pageNumber).FirstOrDefault();


                return Ok(GetPortfolio(portfolio));
            }
            else
                return Ok(1);
        }


        [HttpGet]
        [Route("~/api/Portfolio/Get/{id}")]

        public IActionResult GetPortfolioById(int id)
        {

            if (id > 0)
            {
                var portfolio = unitOfWork.PortfolioRepository.GetByID(id);


                return Ok(GetPortfolio(portfolio));


            }
            else
                return Ok(1);
        }


   

        #endregion


        #region Insert Methods
        [HttpPost]
        [Route("~/api/Portfolio/Add")]
        public IActionResult PostEmp([FromBody] PortfolioModel portModel)
        {

            if (ModelState.IsValid)
            {

                var Check = unitOfWork.PortfolioRepository.Get();
                if (portModel == null)
                {
                    return Ok(0);
                }
                if (Check.Any(m => m.Code == portModel.Code))
                {
                    return Ok(2);
                }
                else
                {

                    var model = _mapper.Map<Portfolio>(portModel);

                    #region Bind List Accounts & Shareholders

                   
                    var OpeningStocks = portModel.portfolioOpeningStocksModels;


                    #endregion

                    
                        unitOfWork.PortfolioRepository.Insert(model);

                    // portfolio accounts
                     
                    if (portModel.AccountID != null)
                    {
                        PortfolioAccountModel portfolioAccountModel = new PortfolioAccountModel();
                        portfolioAccountModel.AccountID = portModel.AccountID;
                        portfolioAccountModel.PortfolioID = model.PortfolioID;
                        portfolioAccountModel.Type = true;

                   var portfolioAccount= _mapper.Map<PortfolioAccount>(portfolioAccountModel);
                        unitOfWork.PortfolioAccountRepository.Insert(portfolioAccount);

                    }
                        // shareholders
                        if (OpeningStocks != null)
                        {
                            foreach (var item in OpeningStocks)
                            {
                                if (item.PortOPenStockID == 0)
                                {
                                    item.PortfolioID = model.PortfolioID;
                                    var obj = _mapper.Map<PortfolioOpeningStocks>(item);

                                    unitOfWork.PortfolioOpeningStocksRepository.Insert(obj);
                                }
                                else
                                {
                                    var obj = _mapper.Map<PortfolioOpeningStocks>(item);

                                    unitOfWork.PortfolioOpeningStocksRepository.Update(obj);
                                }


                            }
                        }
                    //CurrentStocks
                    var current = portModel.portfolioTransactionModels; 
                    if (current != null)
                    { 
                        foreach (var item in current)
                        {

                            item.PortfolioID = model.PortfolioID;
                            item.PortTransID = 0;
                            var obj = _mapper.Map<PortfolioTransaction>(item);
                            unitOfWork.PortfolioTransactions.Insert(obj);
                        }
                    }

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
                        return Ok(model);

                   

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
        [Route("~/api/Portfolio/Update/{id}")]
        public IActionResult Update(int id, [FromBody] PortfolioModel portModel)
        {
            if (id != portModel.PortfolioID)
            {

                return Ok(1);
            }

            if (ModelState.IsValid)
            {
                var model = _mapper.Map<Portfolio>(portModel);

             

                var OpeningStocks = portModel.portfolioOpeningStocksModels;

               

                var Check = unitOfWork.PortfolioRepository.Get(NoTrack: "NoTrack");

                if (!Check.Any(m => m.Code == portModel.Code))
                {
                    unitOfWork.PortfolioRepository.Update(model);

                    // portfolio accounts

                    if (portModel.AccountID != null)
                    {
                        PortfolioAccountModel portfolioAccountModel = new PortfolioAccountModel();
                        portfolioAccountModel.AccountID = portModel.AccountID;
                        portfolioAccountModel.PortfolioID = model.PortfolioID;
                        portfolioAccountModel.Type = true;
                        var portfolioAccount = _mapper.Map<PortfolioAccount>(portfolioAccountModel);
                        unitOfWork.PortfolioAccountRepository.Update(portfolioAccount);


                    }

                    // shareholders
                    var oldHolders = unitOfWork.PortfolioOpeningStocksRepository

                    .Get(filter: m => m.PortfolioID == model.PortfolioID);

                    if (oldHolders != null)
                    {

                        unitOfWork.PortfolioOpeningStocksRepository.RemovRange(oldHolders);

                    }

                    foreach (var item in OpeningStocks)
                    {
                        item.PortfolioID = model.PortfolioID;
                        var newHolder = _mapper.Map<PortfolioOpeningStocks>(item);

                        unitOfWork.PortfolioOpeningStocksRepository.Insert(newHolder);

                    }
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
                    return Ok(portModel);
                }
                else
                {
                    if (Check.Any(m => m.Code == portModel.Code && m.PortfolioID == id))
                    {

                        unitOfWork.PortfolioRepository.Update(model);
                        unitOfWork.Save();

                        // portfolio accounts
                        if (portModel.AccountID != null)
                        {
                            PortfolioAccountModel portfolioAccountModel = new PortfolioAccountModel();
                            portfolioAccountModel.AccountID = portModel.AccountID;
                            portfolioAccountModel.PortfolioID = model.PortfolioID;
                            portfolioAccountModel.Type = true;
                            var portfolioAccount = _mapper.Map<PortfolioAccount>(portfolioAccountModel);
                            unitOfWork.PortfolioAccountRepository.Update(portfolioAccount);


                        }



                        // shareholders
                        var oldHolders = unitOfWork.PortfolioOpeningStocksRepository

                        .Get(filter: m => m.PortfolioID == model.PortfolioID);

                        if (oldHolders !=null)
                        {

                            unitOfWork.PortfolioOpeningStocksRepository.RemovRange(oldHolders);


                        }
                        foreach (var item in OpeningStocks)
                        {
                            item.PortfolioID = model.PortfolioID;
                            var newHolder = _mapper.Map<PortfolioOpeningStocks>(item);

                            unitOfWork.PortfolioOpeningStocksRepository.Insert(newHolder);

                        }

                        //  unitOfWork.EmpCardRepository.AddRange(EmpolyeeCard);
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
                        return Ok(portModel);
                    }
                    else
                    {
                        return Ok(2);
                    }
                }

            }
            else
            {
                return Ok(3);
            }
        }
        #endregion


        #region Delete Methods

        [HttpDelete]
        [Route("~/api/Portfolio/Delete/{id}")]
        public IActionResult DeletePortfolio(int? id)
        {
            
            if (id>0)
            {
                var portfolio = unitOfWork.PortfolioRepository.GetByID(id);
                if (portfolio == null)
                {
                    return BadRequest();
                }
                var PortAccount = unitOfWork.PortfolioAccountRepository.GetEntity(filter: m => m.PortfolioID == id);
                unitOfWork.PortfolioAccountRepository.Delete(PortAccount.PortfolioAccountID);

               var Shareholders = unitOfWork.PortfolioOpeningStocksRepository.Get(filter: m => m.PortfolioID == id);

                if (Shareholders.Count() > 0)
                {
                    unitOfWork.PortfolioOpeningStocksRepository.RemovRange(Shareholders);

                }


                unitOfWork.PortfolioRepository.Delete(portfolio);
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

                //var Result = unitOfWork.Save();
                //if (Result == true)
                //{
                //    return Ok(4);
                //}
                //else
                //{
                //    return NotFound();
                //} 
            }
            else
                return Ok(1);


        }

        #endregion
    }
}
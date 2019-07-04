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

                .Get(filter: m => m.PortfolioID == portfolio.PortfolioID)
                .Select(m => new PortfolioAccountModel
                {
                    PortfolioAccountID = m.PortfolioAccountID,
                    Type = m.Type,
                    AccountID = m.AccountID,
                    AccountCode = m.Account.Code,
                    AccountNameAR = m.Account.NameAR,
                    AccountNameEN = m.Account.NameEN,
                    PortfolioID = m.PortfolioID,
                    PortfolioCode = m.Portfolio.Code,
                    PortfolioNameAR = m.Portfolio.NameAR,
                    PortfolioNameEN = m.Portfolio.NameEN,

                });
            if (PortAccount != null)
                model.folioAccounts = PortAccount;

            #endregion

            #region Shareholders part
            var PortShareholders = unitOfWork.PortfolioShareholderRepository

                .Get(filter: m => m.PortfolioID == portfolio.PortfolioID)
                .Select(m => new PortfolioShareholderModel
                {
                    PortShareID = m.PortShareID,
                    Amount = m.Amount,
                    Percentage = m.Percentage,
                    StocksCount = m.StocksCount,
                    Notes = m.Notes,
                    PartnerID = m.PartnerID,
                    PartnerCode = m.Partner.Code,
                    PartnerNameAR = m.Partner.NameAR,
                    PartnerNameEN = m.Partner.NameEN,
                    PortfolioID = m.PortfolioID,
                    PortfolioCode = m.Portfolio.Code,
                    PortfolioNameAR = m.Portfolio.NameAR,
                    PortfolioNameEN = m.Portfolio.NameEN

                });
            if (PortShareholders != null)
                model.Shareholders = PortShareholders;

            #endregion

            model.Count = unitOfWork.PortfolioRepository.Count();

            return model;
        }

        [HttpGet]
        [Route("~/api/Portfolio/FirstOpen")]
        public IActionResult FirstOpen()
        {
            PortfolioModel model = new PortfolioModel();
            model.LastCode = unitOfWork.PartnerRepository.Last().Code;
            model.Count = unitOfWork.PartnerRepository.Count();
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


        [Route("~/api/Portfolio/GetAll")]
        public IActionResult GetAllPortfolioes()
        {
            var portfolioes = unitOfWork.PortfolioRepository.Get().ToList();
            var model = _mapper.Map<IEnumerable<PortfolioModel>>(portfolioes).ToList();

            if (model == null)
            {
                return Ok(0);
            }

            for (int i = 0; i < portfolioes.Count(); i++)
            {
                for (int j = i; j < model.Count(); j++)
                {
                    if (model[j].PortfolioID == portfolioes[i].PortfolioID)
                    {
                        #region Date part

                        model[j].EstablishDate = portfolioes[i].EstablishDate.Value.ToString("dd/MM/yyyy");
                        model[j].EstablishDateHijri = DateHelper.GetHijriDate(portfolioes[i].EstablishDate);
                        #endregion

                        #region Accounts Part
                        var PortCard = unitOfWork.PortfolioAccountRepository

                            .Get(filter: m => m.PortfolioID == portfolioes[i].PortfolioID)
                            .Select(m => new PortfolioAccountModel
                            {
                                PortfolioAccountID = m.PortfolioAccountID,
                                Type = m.Type,
                                AccountID = m.AccountID,
                                AccountCode = m.Account.Code,
                                AccountNameAR = m.Account.NameAR,
                                AccountNameEN = m.Account.NameEN,
                                PortfolioID = m.PortfolioID,
                                PortfolioCode = m.Portfolio.Code,
                                PortfolioNameAR = m.Portfolio.NameAR,
                                PortfolioNameEN = m.Portfolio.NameEN,
                            });
                        if (PortCard != null)
                            model[j].folioAccounts = PortCard;

                        #endregion

                        #region shareholders Part
                        var shareholders = unitOfWork.PortfolioShareholderRepository

                            .Get(filter: m => m.PortfolioID == portfolioes[i].PortfolioID)
                         .Select(m => new PortfolioShareholderModel
                         {
                             PortShareID = m.PortShareID,
                             Amount = m.Amount,
                             Percentage = m.Percentage,
                             StocksCount = m.StocksCount,
                             Notes = m.Notes,
                             PartnerID = m.PartnerID,
                             PartnerCode = m.Partner.Code,
                             PartnerNameAR = m.Partner.NameAR,
                             PartnerNameEN = m.Partner.NameEN,
                             PortfolioID = m.PortfolioID,
                             PortfolioCode = m.Portfolio.Code,
                             PortfolioNameAR = m.Portfolio.NameAR,
                             PortfolioNameEN = m.Portfolio.NameEN,

                         });
                        if (shareholders != null)
                            model[j].Shareholders = shareholders;

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

                    var portAccounts = portModel.folioAccounts;
                    var portShareholderes = portModel.Shareholders;


                    //var PortAccounts = _mapper.Map<IEnumerable<PortfolioAccountModel>>(portAccounts);
                    //var PortShareholders = _mapper.Map<IEnumerable<PortfolioShareholderModel>>(portShareholderes); 
                    #endregion

                    
                        unitOfWork.PortfolioRepository.Insert(model);

                        // portfolio accounts
                        if (portAccounts != null)
                        {
                            foreach (var item in portAccounts)
                            {

                                item.PortfolioID = model.PortfolioID;
                                var obj = _mapper.Map<PortfolioAccount>(item);

                                unitOfWork.PortfolioAccountRepository.Insert(obj);


                            }
                        }

                        // shareholders
                        if (portShareholderes != null)
                        {
                            foreach (var item in portShareholderes)
                            {
                                if (item.PortShareID == 0)
                                {
                                    item.PortfolioID = model.PortfolioID;
                                    var obj = _mapper.Map<PortfolioShareHolder>(item);

                                    unitOfWork.PortfolioShareholderRepository.Insert(obj);
                                }
                                else
                                {
                                    var obj = _mapper.Map<PortfolioShareHolder>(item);

                                    unitOfWork.PortfolioShareholderRepository.Update(obj);
                                }


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

                var portAccounts = portModel.folioAccounts;

                var portShareholders = portModel.Shareholders;

                //var EmpolyeeCard = _mapper.Map<IEnumerable<EmployeeCard>>(empolyeeCard);

                var Check = unitOfWork.PortfolioRepository.Get(NoTrack: "NoTrack");

                if (!Check.Any(m => m.Code == portModel.Code))
                {
                    unitOfWork.PortfolioRepository.Update(model);

                    // portfolio accounts
                    var oldAccounts = unitOfWork.PortfolioAccountRepository

                      .Get(filter: m => m.PortfolioID == model.PortfolioID);

                    if (oldAccounts != null)
                    {

                        unitOfWork.PortfolioAccountRepository.RemovRange(oldAccounts); 
                    }


                    foreach (var item in portAccounts)
                    {
                        item.PortfolioID = model.PortfolioID;
                        var newAccount = _mapper.Map<PortfolioAccount>(item);

                        unitOfWork.PortfolioAccountRepository.Insert(newAccount);

                    }


                    // shareholders
                    var oldHolders = unitOfWork.PortfolioShareholderRepository

                    .Get(filter: m => m.PortfolioID == model.PortfolioID);

                    if (oldHolders != null)
                    {

                        unitOfWork.PortfolioShareholderRepository.RemovRange(oldHolders);

                    }

                    foreach (var item in portShareholders)
                    {
                        item.PortfolioID = model.PortfolioID;
                        var newHolder = _mapper.Map<PortfolioShareHolder>(item);

                        unitOfWork.PortfolioShareholderRepository.Insert(newHolder);

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
                        var oldAccounts = unitOfWork.PortfolioAccountRepository

                          .Get(filter: m => m.PortfolioID == model.PortfolioID);

                        if (oldAccounts != null)
                        {

                            unitOfWork.PortfolioAccountRepository.RemovRange(oldAccounts);

                        }

                        foreach (var item in portAccounts)
                        {
                            item.PortfolioID = model.PortfolioID;
                            var newAccount = _mapper.Map<PortfolioAccount>(item);

                            unitOfWork.PortfolioAccountRepository.Insert(newAccount);

                        }


                        // shareholders
                        var oldHolders = unitOfWork.PortfolioShareholderRepository

                        .Get(filter: m => m.PortfolioID == model.PortfolioID);

                        if (oldHolders !=null)
                        {

                            unitOfWork.PortfolioShareholderRepository.RemovRange(oldHolders);


                        }
                        foreach (var item in portShareholders)
                        {
                            item.PortfolioID = model.PortfolioID;
                            var newHolder = _mapper.Map<PortfolioShareHolder>(item);

                            unitOfWork.PortfolioShareholderRepository.Insert(newHolder);

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
                var PortAccounts = unitOfWork.PortfolioAccountRepository.Get(filter: m => m.PortfolioID == id);

                if (PortAccounts.Count() > 0)
                {
                    unitOfWork.PortfolioAccountRepository.RemovRange(PortAccounts);

                }
                var Shareholders = unitOfWork.PortfolioShareholderRepository.Get(filter: m => m.PortfolioID == id);

                if (Shareholders.Count() > 0)
                {
                    unitOfWork.PortfolioShareholderRepository.RemovRange(Shareholders);

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
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
          
            model.EstablishDate = portfolio.EstablishDate.Value.ToString("yyyy/MM/dd");
          //  model.EstablishDateHijri = DateHelper.GetHijriDate(portfolio.EstablishDate);

            #endregion

            #region Accounts part
            var PortAccount = unitOfWork.PortfolioAccountRepository

                .GetEntity(filter: m => m.PortfolioID == portfolio.PortfolioID);

            if (PortAccount != null) {

                model.AccountID = PortAccount.AccountID;
                model.AccountCode = PortAccount.Account.Code;
                model.AccountNameAR = PortAccount.Account.NameAR;
                model.AccountNameEN = PortAccount.Account.NameEN;
                model.RSBalance = PortAccount.Account.Debit - PortAccount.Account.Credit;
            }
           

            #endregion

            #region Shareholders part
            var OpeningStocks = unitOfWork.PortfolioOpeningStocksRepository

                .Get(filter: m => m.PortfolioID == portfolio.PortfolioID)
                .Select(m => new PortfolioOpeningStocksModel
                {
                    PortOPenStockID = m.PortOPenStockID,
                    OpeningStocksCount = m.OpeningStocksCount, 
                    OpeningStockValue=m.OpeningStockValue,
                    PartnerID = m.PartnerID,
                    PartnerCode = m.Partner.Code,
                    PartnerNameAR = m.Partner.NameAR,
                    PartnerNameEN = m.Partner.NameEN,
                    PortfolioID = m.PortfolioID,
                    PortfolioCode = m.Portfolio.Code,
                    PortfolioNameAR = m.Portfolio.NameAR,
                    PortfolioNameEN = m.Portfolio.NameEN,
                    

                });
            if (OpeningStocks != null) {
                model.portfolioOpeningStocksArray = OpeningStocks;
                model.TotalStocksCount = 0;
                model.RSBalance = 0;
                foreach (var item in OpeningStocks)
                {
                   
                    model.TotalStocksCount += item.OpeningStocksCount;
                }
              
                foreach (var item2 in OpeningStocks)
                {
                    model.RSBalance += item2.OpeningStockValue;
                    
                }

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
            var count = unitOfWork.PortfolioRepository.Count();
            if(count>0)
            {
                model.LastCode = unitOfWork.PortfolioRepository.Last().Code;
                model.Count = count;
            }
            else
            {
                return Ok(0);
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

        [Route("~/api/Portfolio/GetAll")]
        public IEnumerable<PortfolioModel> GetPortfolio()
        {

            var Portfoli = unitOfWork.PortfolioRepository.Get().Select(a => new PortfolioModel
            {
                PortfolioID = a.PortfolioID,
                Code = a.Code,
                Description = a.Description,
                EstablishDate = a.EstablishDate.Value.ToString("dd/MM/yyyy"),
                EstablishDateHijri = DateHelper.GetHijriDate(a.EstablishDate),
                AccountID = unitOfWork.PortfolioAccountRepository.GetEntity(x => x.PortfolioID == a.PortfolioID).AccountID,
                AccountCode = unitOfWork.PortfolioAccountRepository.GetEntity(x => x.PortfolioID == a.PortfolioID).Account.Code,
                AccountNameAR = unitOfWork.PortfolioAccountRepository.GetEntity(x => x.PortfolioID == a.PortfolioID).Account.NameAR,
                AccountNameEN = unitOfWork.PortfolioAccountRepository.GetEntity(x => x.PortfolioID == a.PortfolioID).Account.NameEN,
                PortfolioAccountDebit = unitOfWork.PortfolioAccountRepository.GetEntity(x => x.PortfolioID == a.PortfolioID).Account.Debit,
                NameAR = a.NameAR,
                NameEN = a.NameEN,
                TotalStocksCount = a.TotalStocksCount,
                portfolioTransactionModels = unitOfWork.PortfolioTransactionsRepository.Get(filter: s => s.PortfolioID == a.PortfolioID).Select(q => new PortfolioTransactionModel
                {
                    PartnerID = q.PartnerID,
                    CurrentStocksCount = q.CurrentStocksCount,
                    CurrentStockValue = q.CurrentStockValue,
                    partenerCode = q.Partner.Code,
                    partenerNameAR = q.Partner.NameAR,
                    partenerNameEN = q.Partner.NameEN,
                    PortfolioID = q.PortfolioID,
                    PortTransID = q.PortTransID,
                }),


            });



            return Portfoli;


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

                   
                    var OpeningStocks = portModel.portfolioOpeningStocksArray;


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

                        //CurrentStocks
                            foreach (var item in OpeningStocks)
                            {
                            PortfolioTransactionModel portfolioTransaction = new PortfolioTransactionModel();
                            portfolioTransaction.PortfolioID= model.PortfolioID;
                            portfolioTransaction.PartnerID = item.PartnerID;
                            portfolioTransaction.CurrentStockValue = item.OpeningStockValue;
                            portfolioTransaction.CurrentStocksCount = item.OpeningStocksCount;
                            var ob = _mapper.Map<PortfolioTransaction>(portfolioTransaction);
                                unitOfWork.PortfolioTransactionsRepository.Insert(ob);
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
                var currentStocks = unitOfWork.PortfolioTransactionsRepository.Get(x => x.PortfolioID == model.PortfolioID);
                foreach (var item in currentStocks)
                {
                    if (item.HasTransaction == true)
                    {
                        return Ok(5);
                    }

                }


                var OpeningStocks = portModel.portfolioOpeningStocksArray;
                var Check = unitOfWork.PortfolioRepository.Get(NoTrack: "NoTrack");

                if (!Check.Any(m => m.Code == portModel.Code))
                {
                    unitOfWork.PortfolioRepository.Update(model);

                    // portfolio accounts
                    unitOfWork.PortfolioRepository.Update(model);
                    unitOfWork.Save();
                    var oldAccount = unitOfWork.PortfolioAccountRepository.GetEntity(x => x.PortfolioID == model.PortfolioID);
                    unitOfWork.PortfolioAccountRepository.Delete(oldAccount.PortfolioAccountID);
                    if (portModel.AccountID != null)
                    {
                        PortfolioAccountModel portfolioAccountModel = new PortfolioAccountModel();
                        portfolioAccountModel.AccountID = portModel.AccountID;
                        portfolioAccountModel.PortfolioID = model.PortfolioID;
                        portfolioAccountModel.Type = true;
                        var portfolioAccount = _mapper.Map<PortfolioAccount>(portfolioAccountModel);
                        unitOfWork.PortfolioAccountRepository.Insert(portfolioAccount);


                    }

                    // shareholders
                    var oldHolders = unitOfWork.PortfolioOpeningStocksRepository

                    .Get(filter: m => m.PortfolioID == model.PortfolioID);

                    if (oldHolders != null)
                    {

                        unitOfWork.PortfolioOpeningStocksRepository.RemovRange(oldHolders);

                    }
                    if (OpeningStocks != null)
                    {

                        foreach (var item5 in OpeningStocks)
                        {
                            item5.PortfolioID = model.PortfolioID;
                            var newHolder = _mapper.Map<PortfolioOpeningStocks>(item5);

                            unitOfWork.PortfolioOpeningStocksRepository.Insert(newHolder);

                        }

                        //CurrentStocks

                        if (currentStocks != null)
                        {
                            unitOfWork.PortfolioTransactionsRepository.RemovRange(currentStocks);
                        }
                        foreach (var item6 in OpeningStocks)
                        {
                            PortfolioTransactionModel portfolioTransaction = new PortfolioTransactionModel();
                            portfolioTransaction.PortfolioID = model.PortfolioID;
                            portfolioTransaction.PartnerID = item6.PartnerID;
                            portfolioTransaction.CurrentStockValue = item6.OpeningStockValue;
                            portfolioTransaction.CurrentStocksCount = item6.OpeningStocksCount;
                            var obj = _mapper.Map<PortfolioTransaction>(portfolioTransaction);
                            unitOfWork.PortfolioTransactionsRepository.Insert(obj);
                        }
                    }




                }
                else
                {
                    if (Check.Any(m => m.Code == portModel.Code && m.PortfolioID == id))
                    {

                        unitOfWork.PortfolioRepository.Update(model);

                        var oldAccount = unitOfWork.PortfolioAccountRepository.GetEntity(x => x.PortfolioID == model.PortfolioID);
                        unitOfWork.PortfolioAccountRepository.Delete(oldAccount.PortfolioAccountID);
                        // portfolio accounts
                        if (portModel.AccountID != null)
                        {
                            PortfolioAccountModel portfolioAccountModel = new PortfolioAccountModel();
                            portfolioAccountModel.AccountID = portModel.AccountID;
                            portfolioAccountModel.PortfolioID = model.PortfolioID;
                            portfolioAccountModel.Type = true;
                            var portfolioAccount = _mapper.Map<PortfolioAccount>(portfolioAccountModel);
                            unitOfWork.PortfolioAccountRepository.Insert(portfolioAccount);


                        }



                        // shareholders
                        var oldHolders = unitOfWork.PortfolioOpeningStocksRepository

                        .Get(filter: m => m.PortfolioID == model.PortfolioID);

                        if (oldHolders != null)
                        {

                            unitOfWork.PortfolioOpeningStocksRepository.RemovRange(oldHolders);


                        }
                        if (OpeningStocks != null)
                        {

                            foreach (var item2 in OpeningStocks)
                            {
                                item2.PortfolioID = model.PortfolioID;
                                var newHolder = _mapper.Map<PortfolioOpeningStocks>(item2);

                                unitOfWork.PortfolioOpeningStocksRepository.Insert(newHolder);

                            }

                            //CurrentStocks                           
                            if (currentStocks != null)
                            {
                                unitOfWork.PortfolioTransactionsRepository.RemovRange(currentStocks);
                            }
                            foreach (var item3 in OpeningStocks)
                            {
                                PortfolioTransactionModel portfolioTransaction = new PortfolioTransactionModel();
                                portfolioTransaction.PortfolioID = model.PortfolioID;
                                portfolioTransaction.PartnerID = item3.PartnerID;
                                portfolioTransaction.CurrentStockValue = item3.OpeningStockValue;
                                portfolioTransaction.CurrentStocksCount = item3.OpeningStocksCount;
                                var obj = _mapper.Map<PortfolioTransaction>(portfolioTransaction);
                                unitOfWork.PortfolioTransactionsRepository.Insert(obj);
                            }
                        }


                    var result = unitOfWork.Save();
                    if (result == 200)
                    {
                        return Ok("Succeeded");
                    }
                    else
                    {
                        return Ok(6);
                    }



                }
                else
                    {
                        if (Check.Any(m => m.Code == portModel.Code && m.PortfolioID == id))
                        {

                            unitOfWork.PortfolioRepository.Update(model);
                            
                            var oldAccount = unitOfWork.PortfolioAccountRepository.GetEntity(x => x.PortfolioID == model.PortfolioID);
                            unitOfWork.PortfolioAccountRepository.Delete(oldAccount.PortfolioAccountID);
                            // portfolio accounts
                            if (portModel.AccountID != null)
                            {
                                PortfolioAccountModel portfolioAccountModel = new PortfolioAccountModel();
                                portfolioAccountModel.AccountID = portModel.AccountID;
                                portfolioAccountModel.PortfolioID = model.PortfolioID;
                                portfolioAccountModel.Type = true;
                                var portfolioAccount = _mapper.Map<PortfolioAccount>(portfolioAccountModel);
                                unitOfWork.PortfolioAccountRepository.Insert(portfolioAccount);


                            }



                            // shareholders
                            var oldHolders = unitOfWork.PortfolioOpeningStocksRepository

                            .Get(filter: m => m.PortfolioID == model.PortfolioID);

                            if (oldHolders != null)
                            {

                                unitOfWork.PortfolioOpeningStocksRepository.RemovRange(oldHolders);


                            }
                            if (OpeningStocks != null)
                            {

                                foreach (var item2 in OpeningStocks)
                                {
                                    item2.PortfolioID = model.PortfolioID;
                                    var newHolder = _mapper.Map<PortfolioOpeningStocks>(item2);

                                    unitOfWork.PortfolioOpeningStocksRepository.Insert(newHolder);

                                }

                                //CurrentStocks                           
                                if (currentStocks != null)
                                {
                                    unitOfWork.PortfolioTransactionsRepository.RemovRange(currentStocks);
                                }
                                foreach (var item3 in OpeningStocks)
                                {
                                    PortfolioTransactionModel portfolioTransaction = new PortfolioTransactionModel();
                                    portfolioTransaction.PortfolioID = model.PortfolioID;
                                    portfolioTransaction.PartnerID = item3.PartnerID;
                                    portfolioTransaction.CurrentStockValue = item3.OpeningStockValue;
                                    portfolioTransaction.CurrentStocksCount = item3.OpeningStocksCount;
                                    var obj = _mapper.Map<PortfolioTransaction>(portfolioTransaction);
                                    unitOfWork.PortfolioTransactionsRepository.Insert(obj);
                                }
                            }

                        var result = unitOfWork.Save();
                        if (result == 200)
                        {
                            return Ok("Succeeded");
                        }
                        else
                        {
                            return Ok(6);
                        }



                    }
                    else
                        {
                            return Ok(2);
                        }
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
                var currentstocks = unitOfWork.PortfolioTransactionsRepository.Get(x=> x.PortfolioID==portfolio.PortfolioID);
                foreach (var item in currentstocks)
                {
                    if (item.HasTransaction==true)
                    {
                        return Ok(5);
                    }
                }
                unitOfWork.PortfolioTransactionsRepository.RemovRange(currentstocks);
                var PortAccount = unitOfWork.PortfolioAccountRepository.GetEntity(filter: m => m.PortfolioID == id);
                unitOfWork.PortfolioAccountRepository.Delete(PortAccount.PortfolioAccountID);

               var Shareholders = unitOfWork.PortfolioOpeningStocksRepository.Get(filter: m => m.PortfolioID == id);

                if (Shareholders.Count() > 0)
                {
                    unitOfWork.PortfolioOpeningStocksRepository.RemovRange(Shareholders);

                }


                unitOfWork.PortfolioRepository.Delete(portfolio);
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
        [Route("~/api/Portfolio/GetAllaccounts")]
        public IActionResult GetAllaccounts()
        {
            var account = unitOfWork.AccountRepository.Get(filter: a => a.AccountType == false);
            var model = _mapper.Map<IEnumerable<AccountModel>>(account);

            if (model == null)
            {
                return Ok(0);
            }

            return Ok(model);
        }

        [Route("~/api/Portfolio/GetAllPartners")]
        public IActionResult GetAllPartners()
        {
            var partners = unitOfWork.PartnerRepository.Get();
            var model = _mapper.Map<IEnumerable<PartenerModel>>(partners);

            if (model == null)
            {
                return Ok(0);
            }

            return Ok(model);
        }
    }
}
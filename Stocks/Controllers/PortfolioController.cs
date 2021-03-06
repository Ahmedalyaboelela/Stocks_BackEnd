﻿using System;
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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Stocks.Controllers
{
    [Authorize(Roles = "SuperAdmin,Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class PortfolioController : ControllerBase
    {
        
        #region CTOR & Definitions
        private UnitOfWork unitOfWork;
        private readonly IMapper _mapper;
        private LoggerHistory loggerHistory;
        public PortfolioController(StocksContext context, IMapper mapper)
        {
            this.unitOfWork = new UnitOfWork(context);
            this._mapper = mapper;
            loggerHistory = new LoggerHistory(context, mapper);
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
          
            if(portfolio.EstablishDate != null)
            {
                model.EstablishDate = portfolio.EstablishDate.Value.ToString("d/M/yyyy");
            }
           
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
                    

                }).ToList();

         


            if (OpeningStocks != null) {
                model.portfolioOpeningStocksArray = OpeningStocks;
               
            } 
            //if (currentstocks != null)
            //{
            //    model.TotalStocksCount = 0;
               
            //    foreach (var item in currentstocks)
            //    {

            //        model.TotalStocksCount += item.CurrentStocksCount;
            //    }

               

            //}



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
        /*    else
            {              
                return Ok(0);
            }*/
            
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
                EstablishDate = a.EstablishDate.Value.ToString("d/M/yyyy"),
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


        [Route("~/api/Portfolio/GetAllportparteners/{id}")]
        public IEnumerable<PortfolioTransactionModel> GetParteners(int id)
        {

            var Parteners = unitOfWork.PortfolioTransactionsRepository.Get(filter: s => s.PortfolioID == id).Select(q => new PortfolioTransactionModel
            {
                PartnerID = q.PartnerID,
                CurrentStocksCount = q.CurrentStocksCount,
                CurrentStockValue = q.CurrentStockValue,
                partenerCode = q.Partner.Code,
                partenerNameAR = q.Partner.NameAR,
                partenerNameEN = q.Partner.NameEN,
                PortfolioID = q.PortfolioID,
                PortTransID = q.PortTransID,
            });

            return Parteners;


        }


        [Route("~/api/Portfolio/portAccount/{id}")]
        public PortfolioAccountModel portAccount(int id)
        {

            var Account = unitOfWork.PortfolioAccountRepository.Get(filter: s => s.PortfolioID == id).Select(q => new PortfolioAccountModel
            {
                PortfolioID=q.PortfolioID,
                AccountCode=q.Account.Code,
                AccountID=q.AccountID,
                AccountNameAR=q.Account.NameAR,
                AccountNameEN=q.Account.NameEN,
                Credit=q.Account.Credit,
                Debit=q.Account.Debit,
                PortfolioAccountID=q.PortfolioAccountID,
                PortfolioCode=q.Portfolio.Code,
                PortfolioNameAR=q.Portfolio.NameAR,
                Type=q.Type,
                PortfolioNameEN=q.Portfolio.NameEN
               
            }).SingleOrDefault();

            return Account;


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
                    // عدد الاسهم الحاليه
                    if (portModel.portfolioTransactionModels != null)
                    {
                        portModel.TotalStocksCount = 0;

                        foreach (var item in portModel.portfolioTransactionModels)
                        {

                            portModel.TotalStocksCount += item.CurrentStocksCount;
                        }



                    }
                    if (portModel.EstablishDate ==null)
                    {
                        portModel.EstablishDate = DateTime.Now.ToString();
                    }
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
                        var UserID = loggerHistory.getUserIdFromRequest(Request);

                        loggerHistory.InsertUserLog(UserID, "بطاقه المحفظه", "اضافه المحفظه", true);
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
                if (portModel.EstablishDate == null)
                {
                    portModel.EstablishDate = DateTime.Now.ToString();
                }
                var model = _mapper.Map<Portfolio>(portModel);
                var currentstocks = unitOfWork.PortfolioTransactionsRepository.Get(filter: x => x.PortfolioID == model.PortfolioID);

                //var Checkselles = unitOfWork.SellingInvoiceReposetory.Get(filter: q => q.PortfolioID == model.PortfolioID);
                //if (Checkselles.Any(m => m.PortfolioID == model.PortfolioID))
                //{
                //    return Ok(5);
                //}

                var Checkpurches = unitOfWork.PurchaseOrderRepository.Get(filter: q => q.PortfolioID == model.PortfolioID);
                if (Checkpurches.Any(m => m.PortfolioID == model.PortfolioID))
                {
                    return Ok(5);
                }

                var currentStocks = unitOfWork.PortfolioTransactionsRepository.Get(x => x.PortfolioID == model.PortfolioID);
                //foreach (var item in currentStocks)
                //{
                //    if (item.HasTransaction == true)
                //    {
                //        return Ok(5);
                //    }

                //}
                // عدد الاسهم الحاليه
                if (portModel.portfolioTransactionModels != null)
                {
                    portModel.TotalStocksCount = 0;

                    foreach (var item in portModel.portfolioTransactionModels)
                    {

                        portModel.TotalStocksCount += item.CurrentStocksCount;
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
                            var UserID = loggerHistory.getUserIdFromRequest(Request);

                            loggerHistory.InsertUserLog(UserID, "بطاقه المحفظه", "تعديل المحفظه", true);
                            return Ok(4);
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
                                var UserID = loggerHistory.getUserIdFromRequest(Request);

                                loggerHistory.InsertUserLog(UserID, "بطاقه المحفظه", "تعديل المحفظه", true);
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
                    var UserID = loggerHistory.getUserIdFromRequest(Request);

                    loggerHistory.InsertUserLog(UserID, "بطاقه المحفظه", "تعديل المحفظه", true);
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



                var currentstocks = unitOfWork.PortfolioTransactionsRepository.Get(filter: x => x.PortfolioID==portfolio.PortfolioID);
              
                //var Checkselles = unitOfWork.SellingInvoiceReposetory.Get(filter: q=> q.PortfolioID==portfolio.PortfolioID);
                //if (Checkselles.Any(m => m.PortfolioID == portfolio.PortfolioID))
                //{
                //    return Ok(5);
                //}

                var Checkpurches = unitOfWork.PurchaseOrderRepository.Get(filter: q => q.PortfolioID == portfolio.PortfolioID);
                if (Checkpurches.Any(m => m.PortfolioID == portfolio.PortfolioID))
                {
                    return Ok(5);
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
                    var UserID = loggerHistory.getUserIdFromRequest(Request);

                    loggerHistory.InsertUserLog(UserID, "بطاقه المحفظه", "حذف المحفظه", true);

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
        [Route("~/api/Portfolio/GetAccountInfo/{id}")]
        public IActionResult GetAccountInfo(int id)
        {
            var account = unitOfWork.AccountRepository.GetEntity(filter:x=> x.AccountID== id);
            var model = _mapper.Map<AccountModel>(account); 
           
            if (model.Debit == null)
            {
                model.Debit = 0.0m;
            }
            if (model.Credit == null)
            {
                model.Credit = 0.0m;
            }
            if (model.DebitOpenningBalance == null && model.CreditOpenningBalance != null)
            {
                model.RealBalance = - model.CreditOpenningBalance + (model.Debit - model.Credit);
                if (model != null)
                {
                    return Ok(model);
                }
            } 
            else if (model.DebitOpenningBalance != null && model.CreditOpenningBalance == null)
            {
                model.RealBalance = model.DebitOpenningBalance + (model.Debit - model.Credit);
                if (model != null)
                {
                    return Ok(model);
                }
            }
            else if (model.DebitOpenningBalance == null && model.CreditOpenningBalance == null)
            {
                model.RealBalance =  model.Debit - model.Credit;
                if (model != null)
                {
                    return Ok(model);
                }
            }
            else if (model.DebitOpenningBalance != null && model.CreditOpenningBalance != null)
            {
                model.RealBalance = model.DebitOpenningBalance + (model.Debit - model.Credit);
                if (model != null)
                {
                    return Ok(model);
                }
            }


            return Ok(0);
        }

    }
}
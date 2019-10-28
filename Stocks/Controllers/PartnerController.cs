
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
        [HttpGet]
        [Route("~/api/Partner/FirstOpen")]
        public IActionResult FirstOpen()
        {
            PartenerModel model = new PartenerModel();
            var count = unitOfWork.PartnerRepository.Count();
            // Edited By Ahmed Ayman
            if (count > 0)
            {
                model.LastCode = unitOfWork.PartnerRepository.Last().Code;
                model.Count = count;


            }
            var countries = unitOfWork.CountryRepository.Get();
            if (countries.Count() > 0)
            {
                model.Countries = countries.Select(m => new CountryModel
                {
                    CountryID = m.CountryID,
                    NameAR = m.NameAR,
                    NameEN = m.NameEN

                });
            }
            return Ok(model);
        }

        [Route("~/api/Partner/GetLast")]
        public IActionResult GetLastPartner()
        {
            var partner = unitOfWork.PartnerRepository.Last();

            try
            {
                var model = _mapper.Map<PartenerModel>(partner);

                if (model == null)
                {
                    return Ok( 0);
                }

                // deal with date
                if (partner.IssueDate != null)
                {

                    model.IssueDate = partner.IssueDate.Value.ToString("d/M/yyyy");
                    model.IssueDateHijri = DateHelper.GetHijriDate(partner.IssueDate);

                }

                if(partner.Account !=null)
                {

                    model.AccountNameAr = partner.Account.NameAR;
                    model.AccountNameEn = partner.Account.NameEN;
                }

                if(partner.Country!=null)
                {
                    model.CountryNameAr = partner.Country.NameAR;
                    model.CountryNameEn = partner.Country.NameEN;
                }
              

                model.Count = unitOfWork.PartnerRepository.Count();
                var countries = unitOfWork.CountryRepository.Get();
                if(countries !=null || countries.Count()>0)
                {
                    model.Countries=countries.Select(m => new CountryModel
                    {
                        CountryID = m.CountryID,
                        NameAR = m.NameAR,
                        NameEN = m.NameEN

                    });
                }
                //model.Countries = unitOfWork.CountryRepository.Get().Select(m => new CountryModel
                //{
                //    CountryID = m.CountryID,
                //    NameAR = m.NameAR,
                //    NameEN = m.NameEN

                //});

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
                        return Ok(0);
                    }

                    // deal with date
                    if(partner.IssueDate!=null)
                    {

                        model.IssueDate = partner.IssueDate.Value.ToString("d/M/yyyy");
                        model.IssueDateHijri = DateHelper.GetHijriDate(partner.IssueDate);

                    }

                    if (partner.Account != null)
                    {

                        model.AccountNameAr = partner.Account.NameAR;
                        model.AccountNameEn = partner.Account.NameEN;
                    }

                    if (partner.Country != null)
                    {
                        model.CountryNameAr = partner.Country.NameAR;
                        model.CountryNameEn = partner.Country.NameEN;
                    }

                    var countries = unitOfWork.CountryRepository.Get();
                    if (countries.Count() > 0)
                    {
                        model.Countries = countries.Select(m => new CountryModel
                        {
                            CountryID = m.CountryID,
                            NameAR = m.NameAR,
                            NameEN = m.NameEN

                        });
                    }

                    model.Count = unitOfWork.PartnerRepository.Count();


                    return Ok(model);
                }
                else
                    return Ok(1);
           
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

        public IActionResult GetpartnerById(int id)
        {
            if (id > 0)
            {
                var partner = unitOfWork.PartnerRepository.GetByID(id);

                var model = _mapper.Map<PartenerModel>(partner);
                if (model == null)
                {
                    return Ok(0);
                }
                else
                {

                    // deal with date
                    if (partner.IssueDate != null)
                    {

                        model.IssueDate = partner.IssueDate.Value.ToString("d/M/yyyy");
                        model.IssueDateHijri = DateHelper.GetHijriDate(partner.IssueDate);

                    }
                   
                    if (partner.Account != null)
                    {

                        model.AccountNameAr = partner.Account.NameAR;
                        model.AccountNameEn = partner.Account.NameEN;
                    }

                    if (partner.Country != null)
                    {
                        model.CountryNameAr = partner.Country.NameAR;
                        model.CountryNameEn = partner.Country.NameEN;
                    }

                    var countries = unitOfWork.CountryRepository.Get();
                    if (countries.Count() > 0)
                    {
                        model.Countries = countries.Select(m => new CountryModel
                        {
                            CountryID = m.CountryID,
                            NameAR = m.NameAR,
                            NameEN = m.NameEN

                        });
                    }

                    model.Count = unitOfWork.PartnerRepository.Count();

                    return Ok(model);
                }

            }
            else
                return Ok(1);
        }



        [Route("~/api/Partner/GetAll")]
        public IActionResult GetAllPartners()
        {
            var partner = unitOfWork.PartnerRepository.Get().ToList();
            var model = _mapper.Map<IEnumerable<PartenerModel>>(partner).ToList();

            if (model == null)
            {
                return Ok(0);
            }

            for (int i = 0; i < partner.Count(); i++)
            {
                for (int j = i; j < model.Count(); j++)
                {
                    if (model[j].PartnerID == partner[i].PartnerID)
                    {
                        // deal with date


                        //model[j].IssueDate = partner[i].IssueDate.Value.ToString("d/M/yyyy");
                        //model[j].IssueDateHijri = DateHelper.GetHijriDate(partner[i].IssueDate);


                        if (partner[i].IssueDate != null)
                        {

                            model[j].IssueDate = partner[i].IssueDate.Value.ToString("d/M/yyyy");
                            model[j].IssueDateHijri = DateHelper.GetHijriDate(partner[i].IssueDate);

                        }

                        if(partner[i].Account !=null)
                        {

                            model[j].AccountNameAr = partner[i].Account.NameAR;
                            model[j].AccountNameEn = partner[i].Account.NameEN;
                        }

                        if(partner[i].Country !=null)
                        {

                            model[j].CountryNameAr = partner[i].Country.NameAR;
                            model[j].CountryNameEn = partner[i].Country.NameEN;
                        }

                        var countries = unitOfWork.CountryRepository.Get();
                        if(countries !=null || countries.Count()>0)
                        {
                            model[j].Countries = countries.Select(m => new CountryModel
                            {
                                CountryID = m.CountryID,
                                NameAR = m.NameAR,
                                NameEN = m.NameEN

                            });
                        }
                       


                    }
                    else
                        continue;


                }

            }
          

            return Ok(model);
        }



        [Route("~/api/Partner/GetAllPortfolioPartners/{id}")]
        public IEnumerable<PortfolioPartners>GetAllPortfolioPartners(int id)
        {


            // partners in portfolio transactions
            var transPartners = unitOfWork.PortfolioTransactionsRepository.Get(filter: a => a.PortfolioID == id).Select(p => new PortfolioPartners
            {
                PartnerID = p.PartnerID,
                Code = p.Partner.Code,
                NameAR = p.Partner.NameAR,
                NameEN = p.Partner.NameEN,
                StocksCount = p.CurrentStocksCount,
                StocksValue = p.CurrentStockValue



            });
            return transPartners;





        }
        [HttpGet]
        [Route("~/api/Order/Getname/{id}")]
        public PortfolioPartners Getname(int id)
        {


            var part = unitOfWork.PartnerRepository.GetEntity(filter: a => a.PartnerID == id);
            PortfolioPartners portfolio = new PortfolioPartners();
            portfolio.NameAR = part.NameAR;
            portfolio.NameEN = part.NameEN;
            portfolio.Code = part.Code;
            portfolio.PartnerID = part.PartnerID;
           

            return portfolio;





        }



        #endregion

        #region Insert Method

        [HttpPost]
        [Route("~/api/Partner/AddPartner")]
        public IActionResult PostPartner([FromBody] PartenerModel partnerModel)
        {
            if (ModelState.IsValid)
            { 
                if (partnerModel.IssueDate=="" || partnerModel.IssueDate ==null )
                {
                    partnerModel.IssueDate = DateTime.Now.ToString("d/M/yyyy");
                }
                var model = _mapper.Map<Partner>(partnerModel);
                if (model == null)
                {
                    return Ok(0);
                }
                var Check = unitOfWork.PartnerRepository.Get();
                if (Check.Any(m => m.Code == partnerModel.Code))
                {

                    return Ok(2);
                }
                else
                {
                    if (Check.Any(m => m.NameAR == partnerModel.NameAR))
                    {

                        return Ok(2);
                    }
                    else
                    {
                        unitOfWork.PartnerRepository.Insert(model);
                        var Result = unitOfWork.Save();
                        if (Result == 200)
                        {
                            partnerModel.Count = unitOfWork.PartnerRepository.Count();

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




            }
            else
            {
                return Ok(3);
            }
        }
        #endregion

        #region Update Method
        [HttpPut]
        [Route("~/api/Partner/EditPartner/{id}")]

        public IActionResult PutPartner(int id, [FromBody] PartenerModel partnerModel)
        {

            if (id<0)
            {
                return Ok(1);
            }

            if (ModelState.IsValid)
            {
                if (partnerModel.IssueDate == "" || partnerModel.IssueDate == null)
                {
                    partnerModel.IssueDate = DateTime.Now.ToString("d/M/yyyy");
                }
                var check1 = unitOfWork.PartnerRepository.Get(NoTrack: "NoTrack", filter: a => a.PartnerID == id);
                if (check1 != null)
                {
                    var model = _mapper.Map<Partner>(partnerModel);
                    if (model == null)
                    {
                        return Ok(0);

                    }

                    var Check = unitOfWork.PartnerRepository.Get(NoTrack: "NoTrack");
                    if (!Check.Any(m => m.Code == partnerModel.Code))
                    {

                        unitOfWork.PartnerRepository.Update(model);
                        var Result = unitOfWork.Save();
                        if (Result == 200)
                        {
                            partnerModel.Count = unitOfWork.PartnerRepository.Count();

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
                        if (Check.Any(m => m.Code == partnerModel.Code && m.PartnerID == id))
                        {

                            unitOfWork.PartnerRepository.Update(model);
                            var Result = unitOfWork.Save();
                            if (Result == 200)
                            {
                                partnerModel.Count = unitOfWork.PartnerRepository.Count();

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
                            return Ok(2);
                        }
                    }
                }
                else
                    return Ok(0);


            }
            else
            {
                return Ok(3);
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
                return Ok(1);
            }

            else
            {
                var account = unitOfWork.PartnerRepository.GetByID(id);
                if (account == null)
                {
                    return Ok(0);
                }
                else
                {

                    unitOfWork.PartnerRepository.Delete(id);
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
        }
        #endregion

    }

               
}
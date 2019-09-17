﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BAL.Helper;
using BAL.Model;
using BAL.Repositories;
using DAL.Context;
using DAL.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Stocks.Controllers
{
    public class ReportSettingController : ControllerBase
    {
        #region CTOR & Definitions
        private UnitOfWork unitOfWork;
        private readonly IMapper _mapper;
        public ReportSettingController(StocksContext context, IMapper mapper)
        {
            this._mapper = mapper;
            this.unitOfWork = new UnitOfWork(context);
        }
        #endregion

        #region GET Methods
        [Route("~/api/ReportSetting/GetAllReportSetting/{portID}/{DayDate}")]

        public IActionResult GetAllPortfolios( int portID ,string DayDate)
        {

            DateTime date = DateTime.Parse(DayDate);
            var Check = unitOfWork.ReportSettingRepository.Get(filter: x => x.PortfolioID == portID && x.CurrentDate == date);
            var portfolio = unitOfWork.PortfolioRepository.GetEntity(filter: a => a.PortfolioID == portID);



            if (Check != null && Check.Count()>0)
            {
                var model = Check.Select(m => new ReportSettingModel
                {

                    CurrentDate = date.ToString("dd/M/yyyy"),
                    PartnerID = m.PartnerID,
                    PartnerCode = unitOfWork.PartnerRepository.GetEntity(filter: a => a.PartnerID == m.PartnerID).Code,
                    PartnerNameAR = unitOfWork.PartnerRepository.GetEntity(filter: a => a.PartnerID == m.PartnerID).NameAR,
                    PartnerNameEN = unitOfWork.PartnerRepository.GetEntity(filter: a => a.PartnerID == m.PartnerID).NameEN,
                    PortfolioID = m.PortfolioID,
                    PortfolioCode = portfolio.Code,
                    PortfolioNameAR = portfolio.NameAR,
                    PortfolioNameEN = portfolio.NameEN,
                    ReportSettingID = m.ReportSettingID,
                    DailyStockValue = m.DailyStockValue,


                });
                return Ok(model);


            }
            else
            {
                var model = unitOfWork.PortfolioTransactionsRepository.Get(filter: x => x.PortfolioID == portID).Select(x => new ReportSettingModel {

                    PortfolioID=x.PortfolioID,
                    PortfolioCode = portfolio.Code,
                    PortfolioNameAR = portfolio.NameAR,
                    PortfolioNameEN = portfolio.NameEN,
                    PartnerID = x.PartnerID,
                    PartnerCode = unitOfWork.PartnerRepository.GetEntity(filter: a => a.PartnerID == x.PartnerID).Code,
                    PartnerNameAR = unitOfWork.PartnerRepository.GetEntity(filter: a => a.PartnerID == x.PartnerID).NameAR,
                    PartnerNameEN = unitOfWork.PartnerRepository.GetEntity(filter: a => a.PartnerID == x.PartnerID).NameEN,
                    ReportSettingID = 0,
                    DailyStockValue = 0,
                    CurrentDate=DateTime.UtcNow.Date.ToString("dd/M/yyyy")
                });
                return Ok(model);

            }


        }

        #endregion

        #region Insert & Update Method

        [HttpPost]
        [Route("~/api/ReportSetting/AddReportSetting/{portID}/{DayDate}")]
        public IActionResult PostReportSetting([FromBody] ReportSettingModel [] reportSettingModels, int portID, string DayDate)
        {
            DateTime date = DateTime.Parse(DayDate);

            var Check = unitOfWork.ReportSettingRepository.Get(NoTrack: "NoTrack");

            if((Check.Any(m => m.PortfolioID == portID && m.CurrentDate==date))==true)
            {
                unitOfWork.ReportSettingRepository.RemovRange(Check);
               
                if (reportSettingModels != null)
                {
                    foreach (var item in reportSettingModels)
                    {
                        ReportSetting reportSetting = new ReportSetting();
                        reportSetting.CurrentDate = DateTime.Parse(item.CurrentDate);
                        reportSetting.DailyStockValue = item.DailyStockValue;
                        reportSetting.PartnerID = item.PartnerID;
                        reportSetting.PortfolioID = item.PortfolioID;
                        reportSetting.ReportSettingID = item.ReportSettingID;
                        unitOfWork.ReportSettingRepository.Insert(reportSetting);
                    }
                   
                }
                var result = unitOfWork.Save();
                if (result == 200)
                {
                    return Ok(4);
                }
                else if (result == 501)
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
                if (reportSettingModels != null)
                {
                    foreach (var item in reportSettingModels)
                    {
                        ReportSetting reportSetting = new ReportSetting();
                        reportSetting.CurrentDate = DateTime.Parse(item.CurrentDate);
                        reportSetting.DailyStockValue = item.DailyStockValue;
                        reportSetting.PartnerID = item.PartnerID;
                        reportSetting.PortfolioID = item.PortfolioID;
                        reportSetting.ReportSettingID = item.ReportSettingID;
                        unitOfWork.ReportSettingRepository.Insert(reportSetting);
                    }

                }
                var result = unitOfWork.Save();
                if (result == 200)
                {
                    return Ok(4);
                }
                else if (result == 501)
                {
                    return Ok(5);
                }
                else
                {
                    return Ok(6);
                }
            }

                

        }
            #endregion

        }
}
﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using BAL.Helper;
using BAL.Model;
using BAL.Repositories;
using DAL.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Stimulsoft.Report;
using Stimulsoft.Report.Dictionary;
using Stimulsoft.Report.Mvc;

namespace Stocks.Controllers
{
    [Authorize(Roles = "SuperAdmin,Admin")]
    [Produces("application/json")]
    [Route("api/ReportViewer")]
    public class ReportViewerController : Controller
    {
        #region CTOR & Definitions
        private readonly ApplicationSettings _appSettings;
        private UnitOfWork unitOfWork;
        public ReportViewerController( IOptions<ApplicationSettings> appSettings
        ,StocksContext context)
        {
            _appSettings = appSettings.Value;
            this.unitOfWork = new UnitOfWork(context);
        }
        #endregion
        #region  Retreive Reports



        // Retrieve Resultofportofolio Report after sending parameters
        [HttpPost]
        [Route("~/api/ReportViewer/ResultOfPortofolio")]
        public string ResultOfPortofolioWork([FromBody] JObject data)
        {
            #region ReportCalculation
            DateTime ToDate;
            string todate = data.GetValue("todate").ToString();
            int portofolioid = Convert.ToInt32(data.GetValue("portofolioid"));
            int accid = unitOfWork.PortfolioAccountRepository.Get(filter: m => m.PortfolioID == portofolioid)
           .Select(m => m.AccountID).SingleOrDefault();
            decimal? debit = unitOfWork.AccountRepository.Get(filter: m => m.AccountID == accid)
                .Select(m => m.Debit).SingleOrDefault() ?? 0;
            decimal? credit = unitOfWork.AccountRepository.Get(filter: m => m.AccountID == accid)
                .Select(m => m.Credit).SingleOrDefault() ?? 0;
            decimal? opendebit = unitOfWork.AccountRepository.Get(filter: m => m.AccountID == accid)
           .Select(m => m.DebitOpenningBalance).SingleOrDefault() ?? 0;
            decimal? opencredit = unitOfWork.AccountRepository.Get(filter: m => m.AccountID == accid)
                .Select(m => m.CreditOpenningBalance).SingleOrDefault() ?? 0;
            decimal? RiyalBalance = debit - credit;
            decimal? RiyalOpenBalance = opendebit - opencredit;
            decimal? StocksOpenVal = 0;
            var openstocks = unitOfWork.PortfolioOpeningStocksRepository.Get(filter: m => m.PortfolioID == portofolioid).ToList();
            foreach (var item in openstocks)
            {
                StocksOpenVal += item.OpeningStockValue;
            }
            if (todate != string.Empty)
            {
                ToDate = DateHelper.ChangeDateFormat(todate);
            }
            else
            {
                ToDate = DateTime.Now;
            }
            #endregion

            StiReport report = new StiReport();
            var path = StiNetCoreHelper.MapPath(this, "Reports/RPT_ResultOfPortofolioWork.mrt");
            report.Load(path);
            report["@ToDate"] = ToDate;
            report["@PortofolioID"] = portofolioid;
            report["RiyalBalance"] = RiyalBalance;
            report["RiyalOpenBalance"] = RiyalOpenBalance;
            report["StocksValue"] = StocksOpenVal;
            var dbMS_SQL = (StiSqlDatabase)report.Dictionary.Databases["MS SQL"];
            dbMS_SQL.ConnectionString =  _appSettings.Report_Connection;
            report.Render(false);
            return report.SaveDocumentJsonToString();

        }
        #endregion




        #region   portfolio Evaluateport


        //RPT_Evaluateport

        // Retrieve Resultofportofolio Report after sending parameters
        [HttpGet]
        [Route("~/api/ReportViewer/portfolioEvaluateport/{endDate}/{portID}")]
        public string portfolioEvaluateport(int portID,string endDate)
        {
            DateTime EndDate = DateTime.Parse(endDate);
           
        decimal Balance = 756876;
            StiReport report = new StiReport();
            var path = StiNetCoreHelper.MapPath(this, "Reports/RPT_Evaluateport");
            report.Load(path);
            report["@enddate"] =EndDate;
            report["@portID"] = portID;
            report["@Balance"] = Balance;
            var dbMS_SQL = (StiSqlDatabase)report.Dictionary.Databases["MS SQL"];
            dbMS_SQL.ConnectionString = "@" + _appSettings.Report_Connection;
            report.Render(false);
            return report.SaveDocumentJsonToString();

        }
        #endregion 
    }
}
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BAL.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
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
        public ReportViewerController( IOptions<ApplicationSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }
        #endregion
        #region  Retreive Reports



        // Retrieve Resultofportofolio Report after sending parameters
        [HttpPost]
        [Route("~/api/ReportViewer/ResultOfPortofolio")]
        public string ResultOfPortofolioWork()
        {
            StiReport report = new StiReport();
            var path = StiNetCoreHelper.MapPath(this, "Reports/Report20.mrt");
            report.Load(path);
            report["@name"] = 1;
            var dbMS_SQL = (StiSqlDatabase)report.Dictionary.Databases["MS SQL"];
            dbMS_SQL.ConnectionString = "@" +_appSettings.Report_Connection;
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
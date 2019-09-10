using System;
using System.Collections.Generic;
using System.Globalization;
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


        [HttpGet]
        [Route("~/api/ReportViewer/CompaniesSharesInPortfolio")]
        public string CompaniesSharesInPortfolio([FromQuery]int portfolioID, [FromQuery] string fromDate, [FromQuery] string toDate)
        {
            StiReport report = new StiReport();
            var path = StiNetCoreHelper.MapPath(this, "Reports/RPT_CompaniesSharesInPortfolio.mrt");
            DateTime fDate;
            if (fromDate != null)
            {
                fDate = DateTime.ParseExact(fromDate, "d/M/yyyy", CultureInfo.InvariantCulture);
            }
            else
            {
                fDate = new DateTime(2000,1,1);
            }
            DateTime tDate = DateTime.ParseExact(toDate, "d/M/yyyy", CultureInfo.InvariantCulture);
            if(tDate < fDate)
            {
                return "Bad Request";
            }
            report.Load(path);
            report["@PortfolioID"] = portfolioID;
            report["@FromDate"] = fDate.ToString("yyyy-MM-dd");
            report["@ToDate"] = tDate.ToString("yyyy-MM-dd");
            var dbMS_SQL = (StiSqlDatabase)report.Dictionary.Databases["MS SQL"];
            dbMS_SQL.ConnectionString = _appSettings.Report_Connection;
            report.Render(false);
            return report.SaveDocumentJsonToString();
        }
        #endregion 
    }
}
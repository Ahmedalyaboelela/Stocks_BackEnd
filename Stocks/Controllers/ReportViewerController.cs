using System;
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

        #region Selling & Purchase Stocks 
        [HttpGet]
        [Route("~/api/ReportViewer/SellPurchase/{portId}/{partId}/{startDate}/{endDate}")]
        public string SellPurchase(int portId,int partId,string startDate,string endDate)
        {
            
            StiReport report = new StiReport();
            var path = StiNetCoreHelper.MapPath(this, "Reports/Report1.mrt");
            report.Load(path);
            report["@portfolioId"] = portId;
            report["@partnerId"] = partId;
            report["@startdate"] = DateTime.Parse(startDate).ToString("yyyy-MM-dd");
            report["@enddate"] = DateTime.Parse(endDate).ToString("yyyy-MM-dd") ;

            var dbMS_SQL = (StiSqlDatabase)report.Dictionary.Databases["MS SQL"];
            dbMS_SQL.ConnectionString =  _appSettings.Report_Connection;
            report.Render(false);
            return report.SaveDocumentJsonToString();

        }
        #endregion
    }
}
using System;
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
        public string ResultOfPortofolioWork(string todate ,int portofolioid)
        {
            int accid = unitOfWork.PortfolioAccountRepository.Get(filter: m => m.PortfolioID == portofolioid)
                .Select(m => m.AccountID).SingleOrDefault();
            decimal? debit = unitOfWork.AccountRepository.Get(filter: m => m.AccountID == accid)
                .Select(m => m.Debit).SingleOrDefault()??0;
            decimal? credit = unitOfWork.AccountRepository.Get(filter: m => m.AccountID == accid)
                .Select(m => m.Credit).SingleOrDefault()??0;
            decimal? RiyalBalance = debit - credit;
            todate = "06/09/2019";
            portofolioid = 1008;
            DateTime ToDate = DateHelper.ChangeDateFormat(todate);
            StiReport report = new StiReport();
            var path = StiNetCoreHelper.MapPath(this, "Reports/RPT_ResultOfPortofolioWork.mrt");
            report.Load(path);
            report["@ToDate"] = ToDate;
            report["@PortofolioID"] = portofolioid;
            report["RiyalBalance"] = RiyalBalance;

            var dbMS_SQL = (StiSqlDatabase)report.Dictionary.Databases["MS SQL"];
            dbMS_SQL.ConnectionString =  _appSettings.Report_Connection;
            report.Render(false);
            return report.SaveDocumentJsonToString();

        }
        #endregion




        #region   portfolio Evaluateport


        //RPT_Evaluateport

        // Retrieve Resultofportofolio Report after sending parameters
        [HttpPost]
        [Route("~/api/ReportViewer/portfolioEvaluateport")]
        public string portfolioEvaluateport([FromBody] JObject data)
        {
            string ToDate = data.GetValue("todate").ToString();            DateTime todate = DateHelper.ChangeDateFormat(ToDate);            int portID = Convert.ToInt32(data.GetValue("portfolioId"));
            int accid = unitOfWork.PortfolioAccountRepository.Get(filter: m => m.PortfolioID == portID)
               .Select(m => m.AccountID).SingleOrDefault();
            decimal? debit = unitOfWork.AccountRepository.Get(filter: m => m.AccountID == accid)
                .Select(m => m.Debit).SingleOrDefault() ?? 0;
            decimal? credit = unitOfWork.AccountRepository.Get(filter: m => m.AccountID == accid)
                .Select(m => m.Credit).SingleOrDefault() ?? 0;
            decimal? RiyalBalance = debit - credit;

            StiReport report = new StiReport();
            var path = StiNetCoreHelper.MapPath(this, "Reports/RPT_Evaluateport.mrt");
            report.Load(path);
            report["@enddate"] = todate;
            report["@portID"] = portID;
            report["@RiyalBalance"] = RiyalBalance;
            var dbMS_SQL = (StiSqlDatabase)report.Dictionary.Databases["MS SQL"];
            dbMS_SQL.ConnectionString = _appSettings.Report_Connection;
            report.Render(false);
            return report.SaveDocumentJsonToString();

        }
        #endregion 
    }
}
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
        public ReportViewerController(IOptions<ApplicationSettings> appSettings
        , StocksContext context)
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
            dbMS_SQL.ConnectionString = _appSettings.Report_Connection;
            report.Render(false);

            return report.SaveDocumentJsonToString();

        }

        // Retrieve CashMovementReyalPortofolio Report after sending parameters
        [HttpPost]
        [Route("~/api/ReportViewer/CashMovementReyalPortofolio")]
        public string CashMovementReyalPortofolio([FromBody] JObject data)
        {
            #region ReportCalculation
            DateTime? ToDate, FromDate,UntillDate,PortofolioDate;
            string todate = data.GetValue("todate").ToString();
            string fromdate = data.GetValue("fromdate").ToString();
            string untilldate = data.GetValue("untilldate").ToString();
            int portofolioid = Convert.ToInt32(data.GetValue("portofolioid"));

            if (untilldate == string.Empty && fromdate ==string.Empty && todate ==string.Empty)
            {
                UntillDate = DateTime.Now;
                FromDate = null;
                ToDate = null;
            }
            else
            {
                if (untilldate == string.Empty)
                {
                    UntillDate = null;
                }

                else
                {
                    UntillDate = DateHelper.ChangeDateFormat(untilldate);
                }

                if (fromdate == string.Empty)
                {
                    FromDate = null;
                }

                else
                {
                    FromDate = DateHelper.ChangeDateFormat(fromdate);
                }

                if (todate == string.Empty)
                {
                    ToDate = null;
                }

                else
                {
                    ToDate = DateHelper.ChangeDateFormat(todate);
                }


            }
            PortofolioDate = unitOfWork.PortfolioRepository.Get(filter: m => m.PortfolioID == portofolioid).Select(m => m.EstablishDate).SingleOrDefault();
            #endregion

            StiReport report = new StiReport();
            var path = StiNetCoreHelper.MapPath(this, "Reports/RPT_CashMovementReyalPortofolio.mrt");
            report.Load(path);
            report["@FromDate"] = FromDate;
            report["@ToDate"] = ToDate;
            report["@UntillDate"] = UntillDate;
            report["@PortofolioDate"] = PortofolioDate;
            report["@PortofolioID"] = portofolioid;

            var dbMS_SQL = (StiSqlDatabase)report.Dictionary.Databases["MS SQL"];
            dbMS_SQL.ConnectionString = _appSettings.Report_Connection;
            report.Render(false);
            return report.SaveDocumentJsonToString();

        }
        #endregion



        #region Selling & Purchase Stocks 
        [HttpPost]
        [Route("~/api/ReportViewer/SellPurchase")]
        public string SellPurchase([FromBody] JObject data)
        {
            #region ReportCalculation
            DateTime? StartDate, EndDate;
            string startDate = data.GetValue("startDate").ToString();
            string endDate = data.GetValue("endDate").ToString();
            int portId = Convert.ToInt32(data.GetValue("portfolioId"));
            int partId = Convert.ToInt32(data.GetValue("partnerId"));
            if (startDate != string.Empty)
            {
                StartDate = DateHelper.ChangeDateFormat(startDate);
            }
            else
            {
                StartDate = null;
            }
            if (endDate != string.Empty)
            {
                EndDate = DateHelper.ChangeDateFormat(endDate);
            }
            else
            {
                EndDate = DateTime.Now;
            }
            #endregion


            StiReport report = new StiReport();
            var path = StiNetCoreHelper.MapPath(this, "Reports/RPT_SellingPurchasing.mrt");
            report.Load(path);
            report["@portfolioId"] = portId;
            report["@partnerId"] = partId;
            report["@startdate"] = StartDate;
            report["@enddate"] = EndDate;

            var dbMS_SQL = (StiSqlDatabase)report.Dictionary.Databases["MS SQL"];
            dbMS_SQL.ConnectionString = _appSettings.Report_Connection;
            report.Render(false);

            return report.SaveDocumentJsonToString();
        }

        #endregion

        #region   portfolio Evaluateport


        //RPT_Evaluateport  Earnings report

        // Retrieve Resultofportofolio Report after sending parameters
        [HttpPost]
        [Route("~/api/ReportViewer/portfolioEvaluateport")]
        public string portfolioEvaluateport([FromBody] JObject data)
        {
            DateTime ToDate;
            string todate = data.GetValue("todate").ToString();
            int portID = Convert.ToInt32(data.GetValue("portID"));
            int accid = unitOfWork.PortfolioAccountRepository.Get(filter: m => m.PortfolioID == portID)
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



            if (todate != string.Empty)
            {
                ToDate = DateHelper.ChangeDateFormat(todate);
            }
            else
            {
                ToDate = DateTime.Now;
            }

            StiReport report = new StiReport();
            var path = StiNetCoreHelper.MapPath(this, "Reports/RPT_Evaluateport.mrt");
            report.Load(path);
            report["@enddate"] = ToDate;
            report["@portID"] = portID;
            report["RiyalBalance"] = RiyalBalance;


            var dbMS_SQL = (StiSqlDatabase)report.Dictionary.Databases["MS SQL"];
            dbMS_SQL.ConnectionString = _appSettings.Report_Connection;
            report.Render(false);
            return report.SaveDocumentJsonToString();

        }
        #endregion








        #region   portfolio Evaluateport


        //RPT_Evaluateport  Earnings report

        // Retrieve Resultofportofolio Report after sending parameters
        [HttpPost]
        [Route("~/api/ReportViewer/Earningsreport")]
        public string Earningsreport([FromBody] JObject data)
        {
            DateTime? ToDate;
            DateTime? Firstdate;
            DateTime? Enddate;
            string todate = data.GetValue("todate").ToString();
            string firstdate = data.GetValue("firstdate").ToString();
            string enddate = data.GetValue("enddate").ToString();
            int portID = Convert.ToInt32(data.GetValue("portID"));
            if (todate == string.Empty && firstdate == string.Empty && enddate == string.Empty) {
                ToDate = DateTime.Now;

            }
            else
            {
                if (todate == string.Empty)
                    ToDate = null;
                else
                    ToDate = DateHelper.ChangeDateFormat(todate);
                if (firstdate == string.Empty)
                    Firstdate = null;
                else
                    Firstdate = DateHelper.ChangeDateFormat(firstdate);
                if (enddate == string.Empty)
                    Enddate = null;
                else
                    Enddate = DateHelper.ChangeDateFormat(enddate);
            


            StiReport report = new StiReport();
            var path = StiNetCoreHelper.MapPath(this, "Reports/RPT_Earnings.mrt");
            report.Load(path);
            report["@todate"] = ToDate;
            report["@startdate"] = Firstdate;
            report["@enddate"] = Enddate;
            report["@portID"] = portID;

            var dbMS_SQL = (StiSqlDatabase)report.Dictionary.Databases["MS SQL"];
            dbMS_SQL.ConnectionString = _appSettings.Report_Connection;
            report.Render(false);
            return report.SaveDocumentJsonToString();

        }
            return null;
        }
        #endregion



        #region Profits in year
        [HttpGet]
        [Route("~/api/ReportViewer/ProfitsYear")]
        public string ProfitsInYear([FromQuery]int portfolioID, [FromQuery] string fromDate, [FromQuery] string toDate)
        {
            DateTime fDate = DateTime.ParseExact(fromDate, "d/M/yyyy", CultureInfo.InvariantCulture);
            DateTime tDate = DateTime.ParseExact(toDate, "d/M/yyyy", CultureInfo.InvariantCulture);
            if(fDate.Year != tDate.Year)
            {
                return "Bad Request";
            }
            StiReport report = new StiReport();
            var path = StiNetCoreHelper.MapPath(this, "Reports/RPT_ProfitsOnSameYear.mrt");
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


        #region total profits in all years
        [HttpPost]
        [Route("~/api/ReportViewer/TotalProfitsAllYears")]
        public string TotalProfitsAllYears([FromBody] JObject data)
        {
            DateTime? StartDate, EndDate;
            string startDate = data.GetValue("startDate").ToString();
            string endDate = data.GetValue("endDate").ToString();
            int? portId = null;
            if ( data.GetValue("portId").ToString()!="")
               Convert.ToInt32(data.GetValue("portId"));

            if (portId == 0)
                portId = null;

            if (startDate != string.Empty)
            {
                StartDate = DateHelper.ChangeDateFormat(startDate);

            }
            else
            {
                StartDate = null;
            }
            if (endDate != string.Empty)
            {
                EndDate = DateHelper.ChangeDateFormat(endDate);
            }
            else
            {
                EndDate = DateTime.Now;
            }

            StiReport report = new StiReport();
            var path = StiNetCoreHelper.MapPath(this, "Reports/RPT_TotalProfitsInYears.mrt");
            report.Load(path);
            report["@portfolioId"] = portId;
            report["@startdate"] = StartDate;
            report["@enddate"] = EndDate;

            var dbMS_SQL = (StiSqlDatabase)report.Dictionary.Databases["MS SQL"];
            dbMS_SQL.ConnectionString = _appSettings.Report_Connection;
            report.Render(false);

            return report.SaveDocumentJsonToString();
        }
        #endregion

        [HttpPost]
        [Route("~/api/ReportViewer/CompaniesSharesInPortfolio")]
        public string CompaniesSharesInPortfolio([FromBody] JObject data)
        {

            #region ReportCalculation
            DateTime StartDate, EndDate;
            string fromDate = data.GetValue("fromDate").ToString();
            string toDate = data.GetValue("toDate").ToString();
            int portfolioID = Convert.ToInt32(data.GetValue("portfolioID"));
            if (fromDate != string.Empty)
            {
                StartDate = DateHelper.ChangeDateFormat(fromDate);
            }
            else
            {
                StartDate = DateTime.Now;
            }
            if (toDate != string.Empty)
            {
                EndDate = DateHelper.ChangeDateFormat(toDate);
            }
            else
            {
                EndDate = DateTime.Now;
            }
            if (EndDate < StartDate)
            {
                return "Bad Request";
            }
            #endregion
            StiReport report = new StiReport();
            var path = StiNetCoreHelper.MapPath(this, "Reports/RPT_CompaniesSharesInPortfolio.mrt");



            report.Load(path);
            report["@PortfolioID"] = portfolioID;
            report["@FromDate"] = StartDate;
            report["@ToDate"] = EndDate;
            var dbMS_SQL = (StiSqlDatabase)report.Dictionary.Databases["MS SQL"];
            dbMS_SQL.ConnectionString = _appSettings.Report_Connection;
            report.Render(false);
            return report.SaveDocumentJsonToString();
        }











        //[HttpGet]
        //[Route("~/api/ReportViewer/ProfitsDistributor")]
        //public string ProfitsDistributor([FromBody] JObject data)
        //{
        //    DateTime ToDate;
           
        //    int portID = Convert.ToInt32(data.GetValue("portID"));
        //   // int accid = unitOfWork.PortfolioAccountRepository.Get(filter: m => m.PortfolioID == portID)
        //   //.Select(m => m.AccountID).SingleOrDefault();
        //   // decimal? debit = unitOfWork.AccountRepository.Get(filter: m => m.AccountID == accid)
        //   //     .Select(m => m.Debit).SingleOrDefault() ?? 0;
        //   // decimal? credit = unitOfWork.AccountRepository.Get(filter: m => m.AccountID == accid)
        //   //     .Select(m => m.Credit).SingleOrDefault() ?? 0;
        //   // decimal? opendebit = unitOfWork.AccountRepository.Get(filter: m => m.AccountID == accid)
        //   //.Select(m => m.DebitOpenningBalance).SingleOrDefault() ?? 0;
        //   // decimal? opencredit = unitOfWork.AccountRepository.Get(filter: m => m.AccountID == accid)
        //   //     .Select(m => m.CreditOpenningBalance).SingleOrDefault() ?? 0;
        //   // decimal? RiyalBalance = debit - credit;



        //    if (todate != string.Empty)
        //    {
        //        ToDate = DateHelper.ChangeDateFormat(todate);
        //    }
        //    else
        //    {
        //        ToDate = DateTime.Now;
        //    }

        //    StiReport report = new StiReport();
        //    var path = StiNetCoreHelper.MapPath(this, "Reports/RPT_Evaluateport.mrt");
        //    report.Load(path);
        //    report["@enddate"] = ToDate;
        //    report["@portID"] = portID;
        //    report["RiyalBalance"] = RiyalBalance;


        //    var dbMS_SQL = (StiSqlDatabase)report.Dictionary.Databases["MS SQL"];
        //    dbMS_SQL.ConnectionString = _appSettings.Report_Connection;
        //    report.Render(false);
        //    return report.SaveDocumentJsonToString();
        //}


    }
}
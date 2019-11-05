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


        //[HttpPost]
        //[Route("~/api/ReportViewer/savereport")]
        //public IActionResult SaveReport()
        //{
        //    var report = StiNetCoreDesigner.GetReportObject(this);

        //    var path = StiNetCoreHelper.MapWebRootPath(this, "Reports/RPT_ResultOfPortofolioWork.mrt");
        //    report.Save(path);

        //    return StiNetCoreDesigner.SaveReportResult(this);
        //}


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
            report["@portID"] = portId;
            report["@partenerID"] = partId;
            report["@StartDate"] = StartDate;
            report["@ToDate"] = EndDate;

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
            if (todate == string.Empty && firstdate == string.Empty && enddate == string.Empty)
            {
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
            if (fDate.Year != tDate.Year)
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
            //int? portId = null;
            //if ( data.GetValue("portId").ToString()!="")
            //   Convert.ToInt32(data.GetValue("portId"));
            int portID = Convert.ToInt32(data.GetValue("portfolioId"));

            //if (portId == 0)
            //    portId = null;

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
            report["@portfolioId"] = portID;
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









        //	الارباح الموزعة للسهم الواحد وعدد الاسهم المجانية 10

        [HttpPost]
        [Route("~/api/ReportViewer/ProfitsDistributor")]
        public string ProfitsDistributor([FromBody] JObject data)
        {
            DateTime Enddate;

            DateTime Startdate;
            int portID = Convert.ToInt32(data.GetValue("portID"));
            string startdate = data.GetValue("startdate").ToString();
            string enddate = data.GetValue("enddate").ToString();
            Enddate = DateHelper.ChangeDateFormat(enddate);
            Startdate = DateHelper.ChangeDateFormat(startdate);




            StiReport report = new StiReport();
            var path = StiNetCoreHelper.MapPath(this, "Reports/RPT_ProfitsDistributor.mrt");
            report.Load(path);
            report["@enddate"] = Enddate;
            report["@startdate"] = Startdate;
            report["@portID"] = portID;



            var dbMS_SQL = (StiSqlDatabase)report.Dictionary.Databases["MS SQL"];
            dbMS_SQL.ConnectionString = _appSettings.Report_Connection;
            report.Render(false);
            return report.SaveDocumentJsonToString();
        }



        //    9-	توزيع الارباح المحصلة على السنة موزعة على الارباع

        [HttpPost]
        [Route("~/api/ReportViewer/Earningscollected")]
        public string Earningscollected([FromBody] JObject data)
        {



            int portID = Convert.ToInt32(data.GetValue("portID"));
            string year = data.GetValue("year").ToString();




            StiReport report = new StiReport();
            var path = StiNetCoreHelper.MapPath(this, "Reports/RPT_Earningscollected.mrt");
            report.Load(path);
            report["@Year"] = year;

            report["@portID"] = portID;



            var dbMS_SQL = (StiSqlDatabase)report.Dictionary.Databases["MS SQL"];
            dbMS_SQL.ConnectionString = _appSettings.Report_Connection;
            report.Render(false);
            return report.SaveDocumentJsonToString();
        }

        #endregion


        #region Printing Reports
        #region Print NoticeCredit
        // Print Notice Credit
        [HttpPost]
        [Route("~/api/ReportViewer/printCreditnote")]
        public string printCreditnote([FromBody] JObject data)
        {

            int NoticeID = Convert.ToInt32(data.GetValue("NoticeID"));
            StiReport report = new StiReport();
            var path = StiNetCoreHelper.MapPath(this, "Reports/Print_NoticeCredit.mrt");
            report.Load(path);
            report["@NoticeID1"] = NoticeID;
            report["@NoticeID2"] = NoticeID;
            report["@NoticeID3"] = NoticeID;
            var dbMS_SQL = (StiSqlDatabase)report.Dictionary.Databases["MS SQL"];
            dbMS_SQL.ConnectionString = _appSettings.Report_Connection;
            report.Render(false);

            return report.SaveDocumentJsonToString();

        }


        // Print Notice Debit
        [HttpPost]
        [Route("~/api/ReportViewer/printDebitnote")]
        public string printDebitnote([FromBody] JObject data)
        {

            int NoticeID = Convert.ToInt32(data.GetValue("NoticeID"));
            StiReport report = new StiReport();
            var path = StiNetCoreHelper.MapPath(this, "Reports/Print_NoticeDebit.mrt");
            report.Load(path);
            report["@NoticeID1"] = NoticeID;
            report["@NoticeID2"] = NoticeID;
            report["@NoticeID3"] = NoticeID;
            var dbMS_SQL = (StiSqlDatabase)report.Dictionary.Databases["MS SQL"];
            dbMS_SQL.ConnectionString = _appSettings.Report_Connection;
            report.Render(false);

            return report.SaveDocumentJsonToString();

        }

        // Print Riyal Reciept
        [HttpPost]
        [Route("~/api/ReportViewer/printRiyalReciept")]
        public string printRiyalReciept([FromBody] JObject data)
        {

            int NoticeID = Convert.ToInt32(data.GetValue("NoticeID"));
            StiReport report = new StiReport();
            var path = StiNetCoreHelper.MapPath(this, "Reports/Print_RiyalRecieptVoucher.mrt");
            report.Load(path);
            report["@NoticeID1"] = NoticeID;
            report["@NoticeID2"] = NoticeID;
            report["@NoticeID3"] = NoticeID;
            var dbMS_SQL = (StiSqlDatabase)report.Dictionary.Databases["MS SQL"];
            dbMS_SQL.ConnectionString = _appSettings.Report_Connection;
            report.Render(false);

            return report.SaveDocumentJsonToString();

        }

        // Print Check Reciept
        [HttpPost]
        [Route("~/api/ReportViewer/printCheckReciept")]
        public string printCheckReciept([FromBody] JObject data)
        {

            int NoticeID = Convert.ToInt32(data.GetValue("NoticeID"));
            StiReport report = new StiReport();
            var path = StiNetCoreHelper.MapPath(this, "Reports/Print_CheckRecieptVoucher.mrt");
            report.Load(path);
            report["@NoticeID1"] = NoticeID;
            report["@NoticeID2"] = NoticeID;
            report["@NoticeID3"] = NoticeID;
            var dbMS_SQL = (StiSqlDatabase)report.Dictionary.Databases["MS SQL"];
            dbMS_SQL.ConnectionString = _appSettings.Report_Connection;
            report.Render(false);

            return report.SaveDocumentJsonToString();

        }

        // Print Riyal Exchange
        [HttpPost]
        [Route("~/api/ReportViewer/printRiyalExchange")]
        public string printRiyalExchange([FromBody] JObject data)
        {

            int NoticeID = Convert.ToInt32(data.GetValue("NoticeID"));
            StiReport report = new StiReport();
            var path = StiNetCoreHelper.MapPath(this, "Reports/Print_RiyalExchangeVoucher.mrt");
            report.Load(path);
            report["@NoticeID1"] = NoticeID;
            report["@NoticeID2"] = NoticeID;
            report["@NoticeID3"] = NoticeID;
            var dbMS_SQL = (StiSqlDatabase)report.Dictionary.Databases["MS SQL"];
            dbMS_SQL.ConnectionString = _appSettings.Report_Connection;
            report.Render(false);

            return report.SaveDocumentJsonToString();

        }

        // Print Check Exchange
        [HttpPost]
        [Route("~/api/ReportViewer/printCheckExchange")]
        public string printCheckExchange([FromBody] JObject data)
        {

            int NoticeID = Convert.ToInt32(data.GetValue("NoticeID"));
            StiReport report = new StiReport();
            var path = StiNetCoreHelper.MapPath(this, "Reports/Print_CheckExchangeVoucher.mrt");
            report.Load(path);
            report["@NoticeID1"] = NoticeID;
            report["@NoticeID2"] = NoticeID;
            report["@NoticeID3"] = NoticeID;
            var dbMS_SQL = (StiSqlDatabase)report.Dictionary.Databases["MS SQL"];
            dbMS_SQL.ConnectionString = _appSettings.Report_Connection;
            report.Render(false);

            return report.SaveDocumentJsonToString();

        }

        // Print Country
        [HttpPost]
        [Route("~/api/ReportViewer/printCountry")]
        public string printCountry([FromBody] JObject data)
        {

            int CountryID = Convert.ToInt32(data.GetValue("CountryID"));
            StiReport report = new StiReport();
            var path = StiNetCoreHelper.MapPath(this, "Reports/Print_Country.mrt");
            report.Load(path);
            report["@CountryID"] = CountryID;
            var dbMS_SQL = (StiSqlDatabase)report.Dictionary.Databases["MS SQL"];
            dbMS_SQL.ConnectionString = _appSettings.Report_Connection;
            report.Render(false);

            return report.SaveDocumentJsonToString();

        }

        // Print Country
        [HttpPost]
        [Route("~/api/ReportViewer/printPartner")]
        public string printPartner([FromBody] JObject data)
        {

            int CountryID = Convert.ToInt32(data.GetValue("PartnerID"));
            StiReport report = new StiReport();
            var path = StiNetCoreHelper.MapPath(this, "Reports/Print_Partner.mrt");
            report.Load(path);
            report["@PartnerID"] = CountryID;
            var dbMS_SQL = (StiSqlDatabase)report.Dictionary.Databases["MS SQL"];
            dbMS_SQL.ConnectionString = _appSettings.Report_Connection;
            report.Render(false);

            return report.SaveDocumentJsonToString();

        }

        #endregion
        #endregion

    }
}
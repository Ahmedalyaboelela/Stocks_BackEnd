using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using BAL.Helper;
using BAL.Model;
using BAL.Repositories;
using DAL.Context;
using DAL.Entities;
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

        public StocksContext Context;

        public ReportViewerController(IOptions<ApplicationSettings> appSettings
        , StocksContext context)
        {
            _appSettings = appSettings.Value;
            this.unitOfWork = new UnitOfWork(context);
            Context = context;
        }
        #endregion

        #region New Eddition Of Stim
        [HttpGet]
        [Route("~/api/ReportViewer/getReportList")]
        public List<ReportFile> getReportList(int reportType, int reportTypeID)
        {
            var List = Context.ReportFiles.Where(x => x.ReportType == reportType && x.ReportTypeId == reportTypeID).ToList();
            return List;
        }

        [HttpPost]
        [Route("~/api/ReportViewer/setDefaultReport")]
        public string setDefaultReport(int reportId, int reportType, int reportTypeId)
        {
            var report = unitOfWork.ReportFileRepository.GetByID(reportId);
            if (report != null)
            {
                report.IsDefault = true;
                try
                {
                     unitOfWork.ReportFileRepository.Update(report);
                    unitOfWork.Save();
                    return "OK";

                }
                catch (Exception ex)
                {

                    return ex.Message;
                }
            }
            return "Error";
        }
        [HttpPost]
        [Route("~/api/ReportViewer/deleteReport")]
        public string deleteReport(int reportID)
        {
            try
            {
                unitOfWork.ReportFileRepository.Delete(reportID);
                unitOfWork.Save();
                return "OK";
            }
            catch (Exception ex)
            {

                return ex.Message;
            }
        }
        #endregion

    }
}
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using AutoMapper;
using BAL.Helper;
using BAL.Model;
using BAL.Repositories;
using DAL.Context;
using DAL.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Stimulsoft.Report;
using Stimulsoft.Report.Mvc;
using Stocks.Models;

namespace Stocks.Controllers
{
    public class ReportSettingController : Controller
    {
        #region CTOR & Definitions
        private UnitOfWork unitOfWork;
        private readonly IMapper _mapper;
        private LoggerHistory loggerHistory;
       
        public ReportSettingController(StocksContext context, IMapper mapper)
        {
            this._mapper = mapper;
            this.unitOfWork = new UnitOfWork(context);
           
        loggerHistory = new LoggerHistory(context, mapper);
    }
        #endregion

        #region GET Methods
        [HttpPost]
        [Route("~/api/ReportSetting/GetAllReportSetting")]

        public IActionResult GetAllPortfolios([FromBody] JObject data)
        {
          //  string date2 = data.GetValue("DayDate").ToString("d/M/yyyy");
            DateTime date = DateHelper.ChangeDateFormat(data.GetValue("DayDate").ToString());
            int portID= Convert.ToInt32(data.GetValue("PortfolioID"));

            var Check = unitOfWork.ReportSettingRepository.Get(filter: x => x.PortfolioID == portID && x.CurrentDate == date);
            var portfolio = unitOfWork.PortfolioRepository.GetEntity(filter: a => a.PortfolioID == portID);



            if (Check != null && Check.Count()>0)
            {
                var model = Check.Select(m => new ReportSettingModel
                {

                    CurrentDate = date.ToString("d/M/yyyy"),
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
                    CurrentDate=DateTime.UtcNow.Date.ToString("d/M/yyyy")
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
             DayDate = DayDate.Replace('-', '/');
              DateTime date = DateHelper.ChangeDateFormat(DayDate) ;
            // DateTime date = DateTime.Parse(DayDate); 
            if (unitOfWork.ReportSettingRepository.Get().Count() != 0)
            { 
                if (unitOfWork.ReportSettingRepository.Get(filter: x => x.CurrentDate == date && x.PortfolioID == portID).Count() != 0)
                {
                    var oldreportSettings = unitOfWork.ReportSettingRepository.Get(filter: x => x.CurrentDate == date && x.PortfolioID == portID);

                    unitOfWork.ReportSettingRepository.RemovRange(oldreportSettings);
                    foreach (var item in reportSettingModels)
                    {
                        var detail = _mapper.Map<ReportSetting>(item);
                        detail.ReportSettingID = 0;
                        unitOfWork.ReportSettingRepository.Insert(detail);
                    }
                }
                else
                {
                    foreach (var item in reportSettingModels)
                    {
                        var detail = _mapper.Map<ReportSetting>(item);
                        detail.ReportSettingID = 0;
                        unitOfWork.ReportSettingRepository.Insert(detail);
                    }
                }
            }
            else
            {
                if (reportSettingModels != null)
                {
                    foreach (var item in reportSettingModels)
                    {
                        var detail = _mapper.Map<ReportSetting>(item);
                        detail.ReportSettingID = 0;
                        unitOfWork.ReportSettingRepository.Insert(detail);
                    }

                }
            }     
                var result = unitOfWork.Save();
                if (result == 200)
                {
                    var UserID = loggerHistory.getUserIdFromRequest(Request);

                    loggerHistory.InsertUserLog(UserID, " اعدادات القيود", "حفظ اعدادات القيود", false);
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
        #endregion
        [HttpGet]
        public string GetFile(int id)

        {
            StiReport report = new StiReport();
            report.Dictionary.DataStore.Clear();
            report.Load(Path.GetFullPath("SimpleList.mrt"));
            report.Dictionary.Variables["id"].ValueObject = id;
            string data = report.SaveToJsonString();

            //string filePath = Path.GetFullPath("SimpleList.mrt");

            //StreamReader rd = new StreamReader(filePath);
            //string data = rd.ReadToEnd();
            //rd.Close();

            return data;
        }

        [HttpGet]
        [Route("~/api/ReportSetting/getReportForDesigner")]
        public string getReportForDesigner(string reportName)
        {
            var path = StiNetCoreHelper.MapPath(this, "/Reports/" + reportName + ".mrt");
            StreamReader rd = new StreamReader(path);
            string data = rd.ReadToEnd();
            rd.Close();
            return data; 
        }

        [HttpPost]
        [Route("~/api/ReportSetting/SaveFile")]
        public string SaveFile([FromBody] DemoData jsonString)
        {

            var reportName = jsonString.fileName;
            try
            {
                string filePath = StiNetCoreHelper.MapPath(this, "/Reports/" + reportName + ".mrt");
                StreamWriter wr = new StreamWriter(filePath);
                wr.Write(jsonString.data);
                wr.Close();
                return "Report Saved Successfully";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }


        [HttpPost]
        [Route("GetDataSource")]
        public void GetDataSource()
        {
            try
            {
                var command = (CommandJson)new DataContractJsonSerializer(typeof(CommandJson)).ReadObject(HttpContext.Request.Body);
                Result result = new Result();

                if (command.Database == "MS SQL")
                {
                    result = MSSQLAdapter.Process(command);
                }
                //if (command.Database == "PostgreSQL") result = PostgreSQLAdapter.Process(command);

                var serializer = new DataContractJsonSerializer(typeof(Result));

                serializer.WriteObject(HttpContext.Response.Body, result);
                

                HttpContext.Response.Body.Flush();
            }
            catch { }


        }

    }
}
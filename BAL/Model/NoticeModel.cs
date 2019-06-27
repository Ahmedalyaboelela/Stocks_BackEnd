using System;
using System.Collections.Generic;
using System.Text;

namespace BAL.Model
{
    public class NoticeModel
    {
        public int NoticeID { get; set; }
        public string Code { get; set; }
        public string NoticeDate { get; set; }
        public string NoticeDateHijri { get; set; }

        public bool Type { get; set; }

        public int PortfolioID { get; set; }
        public string PortfolioCode { get; set; }
        public string PortfolioNameAR { get; set; }
        public string PortfolioNameEN { get; set; }

        public int EmployeeID { get; set; }
        public string EmployeeCode { get; set; }
        public string EmployeeNameAR { get; set; }
        public string EmployeeNameEN { get; set; }

        public int Count { get; set; }
        public IEnumerable<NoticeDetailModel> NoticeModelDetails { get; set; }
        public SettingModel SettingScreen { get; set; }

    }
}

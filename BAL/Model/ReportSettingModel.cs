using System;
using System.Collections.Generic;
using System.Text;

namespace BAL.Model
{
 public class ReportSettingModel
    {
        public int ReportSettingID { get; set; }

        public int PortfolioID { get; set; }
        public string PortfolioNameAR { get; set; }
        public string PortfolioNameEN { get; set; }
        public string PortfolioCode { get; set; }

        public int PartnerID { get; set; }
        public string PartnerNameAR { get; set; }
        public string PartnerNameEN { get; set; }
        public string PartnerCode { get; set; }

        public string CurrentDate { get; set; }
        public string CurrentDate2 { get; set; }


        public decimal DailyStockValue { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace BAL.Model
{
   public class PortfolioShareholderModel
    {
        public int PortShareID { get; set; }

        public decimal Amount { get; set; }

        public float Percentage { get; set; }

        public int StocksCount { get; set; }

        public string Notes { get; set; }

        public int PortfolioID { get; set; }

        public string PortfolioCode { get; set; }

        public string PortfolioNameAR { get; set; }

        public string PortfolioNameEN { get; set; }

        public int PartnerID { get; set; }

        public string PartnerCode { get; set; }

        public string PartnerNameAR { get; set; }

        public string PartnerNameEN { get; set; }
    }
}

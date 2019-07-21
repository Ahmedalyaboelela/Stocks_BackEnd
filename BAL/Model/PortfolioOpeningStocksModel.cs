using System;
using System.Collections.Generic;
using System.Text;

namespace BAL.Model
{
   public class PortfolioOpeningStocksModel
    {
        public int PortOPenStockID { get; set; }

        public float OpeningStocksCount { get; set; }

        public decimal OpeningStockValue { get; set; }


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

using System;
using System.Collections.Generic;
using System.Text;

namespace BAL.Model
{
    public class PortfolioTransactionModel
    {
     
        public int PortTransID { get; set; }
        public float CurrentStocksCount { get; set; }

       
        public decimal CurrentStockValue { get; set; }

        public int PortfolioID { get; set; }
        public int PartnerID { get; set; }
        public string partenerNameAR { get; set; }
        public string partenerCode{ get; set; }
        public string partenerNameEN { get; set; }
       

    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace BAL.Model
{
   public class PortfolioAccountModel
    {
        public int PortfolioAccountID { get; set; }

        public int? AccountID { get; set; }

        public string AccountCode { get; set; }

        public string AccountNameAR { get; set; }

        public string AccountNameEN { get; set; }

        public int PortfolioID { get; set; }

        public string PortfolioCode { get; set; }

        public string PortfolioNameAR { get; set; }

        public string PortfolioNameEN { get; set; }

        public bool Type { get; set; }
        public decimal? Credit { get; set; }
        public decimal? Debit { get; set; }



    }
}

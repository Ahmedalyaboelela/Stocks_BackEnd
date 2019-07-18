using System;
using System.Collections.Generic;
using System.Text;

namespace BAL.Model
{
    public class PortfolioModel
    {
        public int PortfolioID { get; set; }
        
        public string Code { get; set; }
        
        public string NameAR { get; set; }
        
        public string NameEN { get; set; }

        public string EstablishDate { get; set; }

        public string EstablishDateHijri { get; set; }

        public string Description { get; set; }

        public float? OpeningStocksCount { get; set; }

        public float? StocksCount { get; set; }
      
    

        public int Count { get; set; }

        public string LastCode { get; set; }
        public IEnumerable<PortfolioAccountModel> folioAccounts { get; set; }

        public IEnumerable<PortfolioOpeningStocksModel> portopeningmodels { get; set; }

   
    }
}

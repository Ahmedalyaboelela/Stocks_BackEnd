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

        public float? TotalStocksCount { get; set; }
      
        public int Count { get; set; }
        public decimal? RSBalance { get; set; }

        public string LastCode { get; set; } 
        public int? AccountID { get; set; }
        public string AccountNameAR { get; set; }
        public string AccountNameEN { get; set; }
        public string AccountCode{ get; set; }

       
     //    public IEnumerable<PortfolioAccountModel> folioAccounts { get; set; }

        public IEnumerable<PortfolioOpeningStocksModel> portfolioOpeningStocksArray { get; set; }
      //  public IEnumerable<PortfolioTransactionModel> portfolioTransactionModels { get; set; }


    }
}

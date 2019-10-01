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

        //public DateTime? DistributionDate { get; set; }
        //public DateTime? DistributionDateHijri { get; set; }
        public string DistributionDate { get; set; }
        public string DistributionDateHijri { get; set; }


        public bool Type { get; set; }

        public int PortfolioID { get; set; }
        public string PortfolioCode { get; set; }
        public string PortfolioNameAR { get; set; }
        public string PortfolioNameEN { get; set; }

        public int? PortfolioAccountID { get; set; }
        public string PortfolioAccountNameAR { get; set; }
        public string PortfolioAccountNameEN { get; set; }
        public string PortfolioAccountCode { get; set; }

        public int EmployeeID { get; set; }
        public string EmployeeCode { get; set; }
        public string EmployeeNameAR { get; set; }
        public string EmployeeNameEN { get; set; }
        public int CurrencyID { get; set; }
        public string CurrencyCode { get; set; }
        public string CurrencyNameAR { get; set; }
        public string CurrencyNameEN { get; set; }
        public string LastCode { get; set; }
        public int Count { get; set; }
        public IEnumerable<NoticeDetailModel> NoticeModelDetails { get; set; }
        public IEnumerable<PortfolioTransactionModel> portfolioTransactionModels { get; set; }
        public SettingModel SettingModel { get; set; }
         
        public EntryModel EntryModel { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace BAL.Model
{
    public class NoticeDetailModel
    {
        public int NoticeDetailID { get; set; }

      
        public decimal? Debit { get; set; }
       
        public decimal? Credit { get; set; }

        public float? StocksDebit { get; set; }

        public float? StocksCredit { get; set; }

        public int AccountID { get; set; }

        public int NoticeID { get; set; }

        public int? PartnerID { get; set; }

        public string PartnerName { get; set; }

        public string AccNameAR { get; set; }

        public string AccNameEN { get; set; }

        public string AccCode { get; set; }
        

    }
}

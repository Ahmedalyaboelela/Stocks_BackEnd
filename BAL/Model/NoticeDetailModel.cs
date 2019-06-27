using System;
using System.Collections.Generic;
using System.Text;

namespace BAL.Model
{
    public class NoticeDetailModel
    {
        public int NoticeDetailID { get; set; }
        public decimal? CreditDebitMoney { get; set; }
        public float? CreditorDebitStocks { get; set; }
        public int NoticeID { get; set; }
    }
}

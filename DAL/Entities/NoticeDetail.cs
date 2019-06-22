using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Entities
{
    public class NoticeDetail
    {
        [Key]
        public int NoticeDetailID { get; set; }
        public decimal CreditDebitMoney { get; set; }
        public float CreditorDebitStocks { get; set; }

        public int NoticeID { get; set; }

        [ForeignKey("NoticeID")]
        public virtual Notice Notice { get; set; }
    }
}

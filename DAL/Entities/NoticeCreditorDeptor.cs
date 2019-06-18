using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Entities
{
    public class NoticeCreditorDeptor
    {
        [Key]
        public int NoticeCredDepID { get; set; }
        public int NoticeID { get; set; }
        public decimal CreditDebitMoney { get; set; }
        public float CreditorDebitStocks { get; set; }
   
        [Display(Name ="Company Name")]
        public int PartnerID { get; set; }

        [ForeignKey("PartnerID")]
        public virtual Partner Partner { get; set; }

        [ForeignKey("NoticeID")]
        public virtual Notice Notice { get; set; }

    }
}

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

        [Column(TypeName = "decimal(10,2)")]
        public decimal Debit { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        public decimal Credit { get; set; }

        public float? StocksDebit { get; set; }

        public float? StocksCredit { get; set; }

        public int AccountID { get; set; }

       public int NoticeID { get; set; }


        [ForeignKey("AccountID")]
        public virtual Account Account { get; set; }


        [ForeignKey("NoticeID")]
        public virtual Notice Notice { get; set; }
    }
}

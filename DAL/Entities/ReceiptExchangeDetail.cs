using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Entities
{
    public class ReceiptExchangeDetail
    {
        [Key]
        public int ReceiptExchangeID { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        public decimal? Debit { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        public decimal? Credit { get; set; }
        public int AccountID { get; set; }

        public bool? DetailType { get; set; }

        public int ReceiptID { get; set; }

        [ForeignKey("ReceiptID")]
        public virtual ReceiptExchange ReceiptExchange { get; set; }

        [ForeignKey("AccountID")]
        public virtual Account Account { get; set; }
    }
}

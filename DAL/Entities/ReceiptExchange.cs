using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Entities
{
    public class ReceiptExchange
    {
        [Key]
        public int ReceiptID { get; set; }
        [Required]
        [Column(TypeName = "nvarchar(100)")]
        public string Code { get; set; }
        public DateTime? Date { get; set; }
        [Column(TypeName = "nvarchar(150)")]
        public string Handling { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        public decimal? RecieptValue { get; set; }

        [Column(TypeName = "nvarchar(150)")]
        public string BankName { get; set; }
        public bool Type { get; set; }

        public bool ReceiptExchangeType { get; set; }
        public int? ChiqueNumber { get; set; }
        public DateTime? ChiqueDate { get; set; }

        [Column(TypeName = "nvarchar(150)")]
        public string TaxNumber { get; set; }

        [Column(TypeName = "nvarchar(MAX)")]
        public string Description { get; set; }


        public virtual Entry Entry { get; set; }

        public virtual ICollection<ReceiptExchangeDetail> ReceiptExchangeDetails { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Entities
{
    public class Currency
    {
        [Key]
        public int CurrencyID { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(100)")]
        public string Code { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(150)")]
        public string NameAR { get; set; }

        [Column(TypeName = "nvarchar(150)")]
        public string NameEN { get; set; }

        public float PartValue { get; set; }

        [Column(TypeName = "nvarchar(150)")]
        public string PartName { get; set; }
        public float CurrencyValue { get; set; }

        public virtual ICollection<ReceiptExchange> ReceiptExchanges { get; set; }

        
    }
}

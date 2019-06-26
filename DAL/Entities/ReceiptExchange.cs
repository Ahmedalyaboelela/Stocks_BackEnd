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
        public int? CurrencyID { get; set; }
        public bool Type { get; set; }
        public int? ChiqueNumber { get; set; }
        public DateTime? ChiqueDate { get; set; }

        [Column(TypeName = "nvarchar(MAX)")]
        public string Description { get; set; }


        [ForeignKey("CurrencyID")]
        public virtual Currency Currency { get; set; }

        public virtual Entry Entry { get; set; }
    }
}

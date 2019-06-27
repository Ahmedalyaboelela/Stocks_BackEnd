using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Entities
{
    public class EntryDetail
    {
        [Key]
        public int EntryDetailID { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        public decimal? Debit { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        public decimal? Credit { get; set; }

        public float? StocksDebit { get; set; }

        public float? StocksCredit { get; set; }

        public int AccountID { get; set; }

        public int EntryID { get; set; }

        [ForeignKey("AccountID")]
        public virtual Account Account { get; set; }

        [ForeignKey("EntryID")]
        public virtual Entry Entry { get; set; }
    }
}

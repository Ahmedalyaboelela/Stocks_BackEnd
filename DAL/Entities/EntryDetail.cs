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
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public int AccountID { get; set; }

        [ForeignKey("AccountID")]
        public virtual Account Account { get; set; }
    }
}

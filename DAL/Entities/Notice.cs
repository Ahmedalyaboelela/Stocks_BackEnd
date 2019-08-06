using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Entities
{
    public class Notice
    {
        [Key]
        public int NoticeID { get; set; }
        [Required]
        [Column(TypeName = "nvarchar(100)")]
        public string Code { get; set; }
        public DateTime? NoticeDate { get; set; }
        public bool Type { get; set; }

        [Column(TypeName = "nvarchar(150)")]
        public string TaxNumber { get; set; }
        [Column(TypeName = "nvarchar(MAX)")]
        public string Description { get; set; }
        public int PortfolioID { get; set; }
        public int EmployeeID { get; set; }
        public int? CurrencyID { get; set; }

        [ForeignKey("PortfolioID")]
        public virtual Portfolio Portfolio { get; set; }

        [ForeignKey("EmployeeID")]
        public virtual Employee Employee { get; set; }

        [ForeignKey("CurrencyID")]
        public virtual Currency Currency { get; set; }
        public virtual ICollection<NoticeDetail> NoticeDetails { get; set; }

        public virtual Entry Entry { get; set; }

    }
}

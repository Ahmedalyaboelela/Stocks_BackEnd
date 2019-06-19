using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Entities
{
    public class PurchaseOrder
    {
        [Key]
        public int PurchaseOrderID { get; set; }
        [Required]
        [Column(TypeName = "nvarchar(100)")]
        public string Code { get; set; }

        public int PayWay { get; set; }

        public DateTime? Date { get; set; }

        public int EmployeeID { get; set; }

        public int PortfolioID { get; set; }

        [ForeignKey("EmployeeID")]
        public virtual Employee Employee { get; set; }

        [ForeignKey("PortfolioID")]
        public virtual Portfolio Portfolio { get; set; }

        public virtual ICollection<PurchaseOrderDetail> PurchaseOrderStocks { get; set; }
       // public virtual ICollection<Entry> Entries { get; set; }

    }
}

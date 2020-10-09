using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Entities
{
    public class SellingOrder
    {
        [Key]
        public int SellingOrderID { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(100)")]
        public string Code { get; set; }

        public DateTime OrderDate { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

        public int? OrderPeriod { get; set; }
        public string OrderRemarks { get; set; }


        public bool OrderType { get; set; }

        public int PortfolioID { get; set; }
        public int? EmployeeID { get; set; }



        [ForeignKey("PortfolioID")]
        public virtual Portfolio Portfolio { get; set; }

        public virtual ICollection<SellingOrderDetail> SellingOrderDetails { get; set; }

        public virtual ICollection<SellingInvoice> SellingInvoices { get; set; }



    }
}

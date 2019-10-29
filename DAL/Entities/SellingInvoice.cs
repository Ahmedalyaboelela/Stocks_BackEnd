using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Entities
{
    public class SellingInvoice
    {
        [Key]
        public int SellingInvoiceID { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(100)")]
        public string Code { get; set; }


        public DateTime? Date { get; set; }

        [Column(TypeName = "nvarchar(MAX)")]
        public string Description { get; set; }

        public int EmployeeID { get; set; }


        public int SellingOrderID { get; set; }

        [ForeignKey("EmployeeID")]
        public virtual Employee Employee { get; set; }



        [ForeignKey("SellingOrderID")]
        public virtual SellingOrder SellingOrder { get; set; }

        public virtual Entry  Entry { get; set; }

       public virtual ICollection<SellingInvoiceDetail> SellingInvoiceDetails { get; set; }

    }
}

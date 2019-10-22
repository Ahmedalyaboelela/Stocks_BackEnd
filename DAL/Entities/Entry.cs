using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Entities
{
    public class Entry
    {

        [Key]
        public int EntryID { get; set; }
        [Required]
        [Column(TypeName = "nvarchar(100)")]
        public string Code { get; set; }
        public DateTime? Date { get; set; }

        public int? SellingInvoiceID { get; set; }

        public int? PurchaseOrderID { get; set; }

        public int? ReceiptID { get; set; }

        public int? NoticeID { get; set; }

        public bool TransferedToAccounts { get; set; }

        [ForeignKey("SellingInvoiceID")]
        public virtual SellingInvoice SellingInvoice { get; set; }

        [ForeignKey("PurchaseOrderID")]
        public virtual PurchaseOrder PurchaseOrder { get; set; }

        [ForeignKey("ReceiptID")]
        public virtual ReceiptExchange ReceiptExchange { get; set; }

        [ForeignKey("NoticeID")]
        public virtual Notice Notice { get; set; }



    }
}

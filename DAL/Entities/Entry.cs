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

        public int? SellingOrderID { get; set; }

        public int? PurchaseOrderID { get; set; }

        public int? ReceiptID { get; set; }

        public int? NoticeID { get; set; }

        public bool TransferedToAccounts { get; set; }

        [ForeignKey("SellingOrderID")]
        public virtual SellingOrder SellingOrder { get; set; }

        [ForeignKey("PurchaseOrderID")]
        public virtual PurchaseOrder PurchaseOrder { get; set; }

        [ForeignKey("ReceiptID")]
        public virtual ReceiptExchange ReceiptExchange { get; set; }

        [ForeignKey("NoticeID")]
        public virtual Notice Notice { get; set; }



    }
}

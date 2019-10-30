using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Entities
{
    public class PurchaseInvoiceDetail
    {
        [Key]
        public int PurchaseInvoiceDetailID { get; set; }

        public float StockCount { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal PurchasePrice { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        public decimal PurchaseValue { get; set; }
        public float BankCommissionRate { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal BankCommission { get; set; }
        public float TaxRateOnCommission { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal TaxOnCommission { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        public decimal NetAmmount { get; set; }
        public int PurchaseInvoiceID { get; set; }

        public int PartnerID { get; set; }

        [ForeignKey("PurchaseInvoiceID")]
        public virtual PurchaseInvoice PurchaseInvoice { get; set; }

        [ForeignKey("PartnerID")]
        public virtual Partner Partner { get; set; }
    }
}

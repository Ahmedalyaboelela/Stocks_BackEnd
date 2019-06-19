using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Entities
{
    public class SellOrderStock
    {
        [Key]
        public int SellStockID { get; set; }
        public int SellID { get; set; }
        public decimal BankCommittion { get; set; }
        public float BankCommittionRate { get; set; }
        [Display(Name = "Company Name")]
        public int PartnerID { get; set; }
        public decimal PurchaseValue { get; set; }
        public decimal StockPrice { get; set; }
        public int StockCount { get; set; }
        public int TaxOnCommission { get; set; }
        public float TaxRateOnCommission { get; set; }
        public decimal TotalCost { get; set; }

        [ForeignKey("SellID")]
        public virtual SellOrder SellOrder { get; set; }

        [ForeignKey("PartnerID")]
        public virtual Partner Partner { get; set; }
    }
}

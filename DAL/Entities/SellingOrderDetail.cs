using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Entities
{
    public class SellingOrderDetail
    {
        [Key]
        public int SellOrderDetailID { get; set; }
        public int StockCount { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal SellingPrice { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal SelingValue { get; set; }

        public float BankCommissionRate { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        public decimal BankCommission { get; set; }

        public float TaxRateOnCommission { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        public decimal TaxOnCommission { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        public decimal NetAmmount { get; set; }

        public int SellingOrderID { get; set; }



        [ForeignKey("SellingOrderID")]
        public virtual SellingOrder SellOrder { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace BAL.Model
{
 public   class PurchaseOrderDetailModel
    {
        public int PurchaseOrderDetailID { get; set; }

        public int StockCount { get; set; }

        
        public decimal PurchasePrice { get; set; }
       
        public decimal PurchaseValue { get; set; }
        public float BankCommissionRate { get; set; }

       
        public decimal BankCommission { get; set; }
        public float TaxRateOnCommission { get; set; }

       
        public decimal TaxOnCommission { get; set; }
       
        public decimal NetAmmount { get; set; }
        public int PurchaseID { get; set; }
    }
}

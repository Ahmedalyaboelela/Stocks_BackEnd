using System;
using System.Collections.Generic;
using System.Text;

namespace BAL.Model
{
    class SelingOrderDetailsModel
    {
        public int SellOrderDetailID { get; set; }
        public int StockCount { get; set; }

       
        public decimal SellingPrice { get; set; }

        
        public decimal SelingValue { get; set; }

        public float BankCommissionRate { get; set; }
        
        public decimal BankCommission { get; set; }

        public float TaxRateOnCommission { get; set; }
        
        public decimal TaxOnCommission { get; set; }
       
        public decimal NetAmmount { get; set; }

        public int SellingOrderID { get; set; }

    }
}

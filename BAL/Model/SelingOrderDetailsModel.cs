using System;
using System.Collections.Generic;
using System.Text;

namespace BAL.Model
{
  public  class SelingOrderDetailsModel
    {
        public int SellOrderDetailID { get; set; }
        public float StockCount { get; set; }

       
        public decimal SellingPrice { get; set; }

        
        public decimal SelingValue { get; set; }

        public float BankCommissionRate { get; set; }
        
        public decimal BankCommission { get; set; }

        public float TaxRateOnCommission { get; set; }
        
        public decimal TaxOnCommission { get; set; }
       
        public decimal NetAmmount { get; set; }

        public int SellingOrderID { get; set; }
        public int PartnerID { get; set; }
        public string PartnerNameAR { get; set; }
        public string PartnerNameEN { get; set; }
        public string PartnerCode { get; set; }

    }
}

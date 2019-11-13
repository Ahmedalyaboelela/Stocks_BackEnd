using System;
using System.Collections.Generic;
using System.Text;

namespace BAL.Model
{
  public  class SellingInvoiceDetailsModel
    {
        public int SellingInvoiceDetailID { get; set; }
        public float StockCount { get; set; }
  
              public float StocksCount { get; set; }
        public string Code { get; set; }
        public string ExeDate { get; set; }
        public Int64 RowNum { get; set; }

        public double StockBalance { get; set; }
        public decimal SellingPrice { get; set; }

        
        public decimal SelingValue { get; set; }

        public float BankCommissionRate { get; set; }
        
        public decimal BankCommission { get; set; }

        public float TaxRateOnCommission { get; set; }
        
        public decimal TaxOnCommission { get; set; }
       
        public decimal NetAmmount { get; set; }
        public float RestStocks { get; set; }
        public int SellingInvoiceID { get; set; }
        public int PartnerID { get; set; }
        public string PartnerNameAR { get; set; }
        public string PartnerNameEN { get; set; }
        public string PartnerCode { get; set; }
       
        public decimal? StocksValue { get; set; }

    }
}

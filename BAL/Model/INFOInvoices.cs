using System;
using System.Collections.Generic;
using System.Text;

namespace BAL.Model
{
 public   class INFOInvoices
    {
       
        public string Code { get; set; }
        public string SellDate { get; set; }
        public string SellDateHijri { get; set; }

        public string purchaseDate { get; set; }
       
        public string purchaseDateHijri { get; set; }
        public float StockCount { get; set; }

        public decimal NetAmmount { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal SellingPrice { get; set; }
        public float Sumtotalstocks { get; set; }
        public string PartnerNameAR { get; set; }






    }
}

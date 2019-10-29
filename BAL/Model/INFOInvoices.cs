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
        public float StockCount { get; set; }
        public float TotalStockCount { get; set; }
        public decimal SellingPrice { get; set; }
        public decimal NetAmmount { get; set; }
    }
}

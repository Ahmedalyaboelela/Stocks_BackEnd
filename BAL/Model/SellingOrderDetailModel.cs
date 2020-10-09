using System;
using System.Collections.Generic;
using System.Text;

namespace BAL.Model
{
 public   class SellingOrderDetailModel
    {
        public int SellOrderDetailID { get; set; }

        public float StockCount { get; set; }

        public int PriceType { get; set; }

        public int SellingOrderID { get; set; }

        public int PartnerID { get; set; }

        public decimal? TradingValue { get; set; }

        public string Remarks { get; set; }

        public string PartnerNameAr { get; set; }
        public string PartnerCode { get; set; }

    }
}

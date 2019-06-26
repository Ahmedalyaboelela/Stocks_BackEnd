using System;
using System.Collections.Generic;
using System.Text;

namespace BAL.Model
{
   public class ReceiptExchangeDetailModel
    {
        public int ReceiptExchangeID { get; set; }
        public decimal ReceiptExchangeAmount { get; set; }
        public int AccountID { get; set; }
        public string AccNameAR { get; set; }
        public string AccNameEN { get; set; }
        public int? ChiqueNumber { get; set; }
        public bool Type { get; set; }
        public int ReceiptID { get; set; }
    }
}

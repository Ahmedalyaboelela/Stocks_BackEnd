using System;
using System.Collections.Generic;
using System.Text;

namespace BAL.Model
{
    public class ReceiptExchangeModel
    {
        public int ReceiptID { get; set; }
        public string Code { get; set; }
        public string Date { get; set; }
        public string DateHijri { get; set; }

        public int CurrencyID { get; set; }
        public string CurrencyCode { get; set; }
        public string CurrencyNameAR { get; set; }
        public string CurrencyNameEN { get; set; }
        public bool Type { get; set; }
        public int? ChiqueNumber { get; set; }
        public string ChiqueDate { get; set; }
        public string ChiqueDateHijri { get; set; }

        public string Description { get; set; }

        public int Count { get; set; }
        public IEnumerable<ReceiptExchangeDetailModel> RecExcDetails { get; set; }

        public SettingModel SettingModel { get; set; }
         
        public EntryModel EntryModel { get; set; }

    }
}

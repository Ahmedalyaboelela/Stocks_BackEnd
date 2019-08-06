using System;
using System.Collections.Generic;
using System.Text;

namespace BAL.Model
{
    public class ReceiptExchangeModel
    {
        public int ReceiptID { get; set; }
       
        public string Code { get; set; }
        // public DateTime? Date { get; set; }
        public string Date { get; set; }
        public string DateHijri { get; set; }

        public string Handling { get; set; }
     
        public decimal? RecieptValue { get; set; }

       
        public string BankName { get; set; }
        public bool Type { get; set; }

        public bool ReceiptExchangeType { get; set; }
        public int? ChiqueNumber { get; set; }
        // public DateTime? ChiqueDate { get; set; }

        public string ChiqueDate { get; set; }
        public string ChiqueDateHijri { get; set; }

        public string TaxNumber { get; set; }

        
        public string Description { get; set; }

        public string LastCode { get; set; }

        public int Count { get; set; }
        public IEnumerable<ReceiptExchangeDetailModel> RecExcDetails { get; set; }

        public SettingModel SettingModel { get; set; }
         
        public EntryModel EntryModel { get; set; }

    }
}

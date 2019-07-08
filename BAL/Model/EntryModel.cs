using System;
using System.Collections.Generic;
using System.Text;

namespace BAL.Model
{
     public class EntryModel
    {
        public int EntryID { get; set; }

        public string Code { get; set; }

        public string Date { get; set; }

        public string DateHijri { get; set; }

        public int? SellingOrderID { get; set; }

        public int? PurchaseOrderID { get; set; }

        public int? ReceiptID { get; set; }

        public int? NoticeID { get; set; }

        public int Count { get; set; }

        public string LastCode { get; set; }

        public virtual IEnumerable<EntryDetailModel> EntryDetailModel { get; set; }


    }
}

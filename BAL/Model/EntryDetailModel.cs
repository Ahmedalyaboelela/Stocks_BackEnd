using System;
using System.Collections.Generic;
using System.Text;

namespace BAL.Model
{
    public class EntryDetailModel
    {
        public int EntryDetailID { get; set; }

        public int EntryID { get; set; }

        public decimal? Debit { get; set; }

        public decimal? Credit { get; set; }

        public float? StocksDebit { get; set; }

        public float? StocksCredit { get; set; }

        public int AccountID { get; set; }

        public string AccNameAR { get; set; }

        public string AccNameEN { get; set; }

        public string AccCode { get; set; }

        public int? ParentAccountID { get; set; }

        public string ParentAccNameAR { get; set; }

        public string ParentAccNameEN { get; set; }

        public string ParentAccCode { get; set; }
    }
}

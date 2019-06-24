using System;
using System.Collections.Generic;
using System.Text;

namespace BAL.Model
{
  public  class EntryDetailsModel
    {
        public int EntryDetailID { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public int AccountID { get; set; }

        public int EntryID { get; set; }
    }
}

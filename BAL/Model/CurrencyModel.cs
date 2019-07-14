using System;
using System.Collections.Generic;
using System.Text;

namespace BAL.Model
{
  public  class CurrencyModel
    {
        public int CurrencyID { get; set; }

       
        public string Code { get; set; }

        
        public string NameAR { get; set; }

       
        public string NameEN { get; set; }

        public float PartValue { get; set; }

        
        public string PartName { get; set; }
        public float CurrencyValue { get; set; }
        public int Count { get; set; }
        public string LastCode { get; set; }

    }
}

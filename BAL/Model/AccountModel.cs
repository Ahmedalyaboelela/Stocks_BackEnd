using System;
using System.Collections.Generic;
using System.Text;

namespace BAL.Model
{
    public class AccountModel
    {
        public int AccountID { get; set; }

        
        public string Code { get; set; }

       
        public string NameAR { get; set; }

       
        public string NameEN { get; set; }

        public bool AccountType { get; set; }

        public int? AccountCategory { get; set; }

        public int? AccountRefrence { get; set; }

        
        public decimal? Debit { get; set; }

        
        public decimal? Credit { get; set; }

       
        public decimal? DebitLimit { get; set; }

        
        public decimal? CreditLimit { get; set; }

        
        public decimal? DebitOpenningBalance { get; set; }

        
        public decimal? CreditOpenningBalance { get; set; }

        
        public string Address { get; set; }

        
        public string Phone1 { get; set; }

        
        public string Phone2 { get; set; }

        
        public string Fax { get; set; }

        
        public string Telex { get; set; }

       
        public string Mobile { get; set; }

        
        public string Email { get; set; }

        
        public string Notes { get; set; }

       
        public string TaxNum { get; set; }

        //  public bool IsActive { get; set; }

        public decimal? RealBalance { get; set; }
        public int? AccoutnParentID { get; set; }
        public int Count { get; set; }
        public string NameArParent { get; set; }
        public string NameEnParent { get; set; }

        public string LastCode { get; set; }


    }
}

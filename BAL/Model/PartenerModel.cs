using System;
using System.Collections.Generic;
using System.Text;

namespace BAL.Model
{
    public class PartenerModel
    {
        public int PartnerID { get; set; }
        
        public string Code { get; set; }
        
        public string NameAR { get; set; }
        
        public string NameEN { get; set; }

        public int IdentityType { get; set; }
        
        public string IdentityNumber { get; set; }
        
        public string IssuePlace { get; set; }

        //public DateTime? IssueDate { get; set; }
        public string IssueDate { get; set; }


        public string IssueDateHijri { get; set; }

        public string Address { get; set; }
        
        public string Mobile { get; set; }
        
        public string Phone1 { get; set; }
        
        public string Phone2 { get; set; }
        
        public string Fax { get; set; }
        
        public string ConvertNumber { get; set; }
        
        public string Email { get; set; }

        public int AccountID { get; set; }

        public string AccountNameAr { get; set; }

        public string AccountNameEn { get; set; }

        public int? CountryID { get; set; }
        public string CountryNameAr { get; set; }
        public string CountryNameEn { get; set; }

        public int Count { get; set; }
    }
}

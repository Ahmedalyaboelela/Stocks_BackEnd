using System;
using System.Collections.Generic;
using System.Text;

namespace BAL.Model
{
    public class EmployeeCardModel
    {
        public int EmpCardId { get; set; }

        public int CardType { get; set; }
       
        public string Code { get; set; }
        
        public string IssuePlace { get; set; }
        
        public string IssueDate { get; set; }

        public string IssueDateHigri { get; set; }

        public string EndDate { get; set; }

        public string EndDateHigri { get; set; }

        public string RenewalDate { get; set; }

        public string RenewalDateHigri { get; set; }

        public decimal Fees { get; set; }
        
        public string Notes { get; set; }
        
        public int EmployeeID { get; set; }

        public int Count { get; set; }
    }
}

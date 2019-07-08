using System;
using System.Collections.Generic;
using System.Text;

namespace BAL.Model
{
    public class EmployeeModel
    {
        public int EmployeeID { get; set; }
        
        public string Code { get; set; }
        
        public string NameAR { get; set; }
        
        public string NameEN { get; set; }
        
        public string ImagePath { get; set; }
        
        public string Nationality { get; set; }

        public int Religion { get; set; }

        public bool IsActive { get; set; }

        public bool IsInternal { get; set; }
        
        public string Profession { get; set; }
        
        public string PassportProfession { get; set; }
        
        public string Email { get; set; }

        public string Mobile { get; set; }

        public string BirthDate { get; set; }

        public string BirthDateHijri { get; set; }


        public int Age { get; set; }
        
        public string BankAccNum { get; set; }

        public int Count { get; set; }

        public string LastCode { get; set; }

        public IEnumerable<EmployeeCardModel> EmployeeCards { get; set; }

    }
}

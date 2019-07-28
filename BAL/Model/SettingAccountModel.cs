using System;
using System.Collections.Generic;
using System.Text;

namespace BAL.Model
{
    public class SettingAccountModel
    {
        public int SettingAccountID { get; set; }

        public int SettingID { get; set; }
        public string Code { get; set; }


        public int AccountID { get; set; }
        public string AccCode { get; set; }
        public string AccNameAR { get; set; }
        public string AccNameEN { get; set; }

        public int? ParentAccountID { get; set; }
        public string ParentAccCode { get; set; }
        public string ParentAccNameAR { get; set; }
        public string ParentAccNameEN { get; set; }

        public int AccountType { get; set; }

    }
}

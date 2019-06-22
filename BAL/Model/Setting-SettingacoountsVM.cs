using System;
using System.Collections.Generic;
using System.Text;

namespace BAL.Model
{
    class Setting_SettingacoountsVM
    {
        public int SettingID { get; set; }
        
        public string Code { get; set; }
        public bool DoNotGenerateEntry { get; set; }

        public bool GenerateEntry { get; set; }

        public bool AutoGenerateEntry { get; set; }

        public int VoucherType { get; set; }

        public int SettingAccountID { get; set; }

       
        public int AccountID { get; set; }

        public int AccountType { get; set; }
    }
}

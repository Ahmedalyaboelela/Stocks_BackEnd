using System;
using System.Collections.Generic;
using System.Text;

namespace BAL.Model
{
   public class SettingModel
    {
        public int SettingID { get; set; }
        
        public string Code { get; set; }
        public bool DoNotGenerateEntry { get; set; }

        public bool GenerateEntry { get; set; }

        public bool AutoGenerateEntry { get; set; }

        public int VoucherType { get; set; }

        public virtual IEnumerable<SettingAccountModel> SettingAccs { get; set; }
    }
}

﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Entities
{
    public class Setting
    {
        [Key]
        public int SettingID { get; set; }

        public bool DoNotGenerateEntry { get; set; }

        public bool GenerateEntry { get; set; }

        public bool AutoGenerateEntry { get; set; }

        public int VoucherType { get; set; }

        public virtual ICollection<SettingAccount> SettingAccounts { get; set; }
    }
}

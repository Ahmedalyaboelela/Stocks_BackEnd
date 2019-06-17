using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Entities
{
    public class SettingAccount
    {
        [Key]
        public int SettingAccountID { get; set; }

        public int SettingID { get; set; }

        public int AccountID { get; set; }

        public int AccountType { get; set; }


        [ForeignKey("AccountID")]
        public virtual Account Account { get; set; }

        [ForeignKey("SettingID")]
        public virtual Setting Setting { get; set; }
    }
}

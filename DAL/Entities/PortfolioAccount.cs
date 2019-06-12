using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Entities
{
    public class PortfolioAccount
    {
        [Key]
        public int PortfolioAccountID { get; set; }
        public int AccountID { get; set; }

        public int PortfolioID { get; set; }

        public bool Type { get; set; }

        [ForeignKey("AccountID")]
        public virtual Account Account { get; set; }

        [ForeignKey("PortfolioID")]
        public virtual Portfolio Portfolio { get; set; }
    }
}

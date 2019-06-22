using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Entities
{
    public class PortfolioShareHolder
    {
        [Key]
        public int PortShareID { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        public decimal Amount { get; set; }
        public float Percentage { get; set; }
        public int StocksCount { get; set; }
        [Column(TypeName = "nvarchar(MAX)")]
        public string Notes { get; set; }
        public int PortfolioID { get; set; }
        public int PartnerID { get; set; }

        [ForeignKey("PortfolioID")]
        public virtual Portfolio Portfolio { get; set; }


        [ForeignKey("PartnerID")]
        public virtual Partner Partner { get; set; }
    }
}

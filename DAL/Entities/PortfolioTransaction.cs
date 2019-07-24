using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Entities
{
    public class PortfolioTransaction
    {
        [Key]
        public int PortTransID { get; set; }
        public float CurrentStocksCount { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal CurrentStockValue { get; set; }

        public int PortfolioID { get; set; }
        public int PartnerID { get; set; }
         
        public bool HasTransaction { get; set; }

        [ForeignKey("PortfolioID")]
        public virtual Portfolio Portfolio { get; set; }


        [ForeignKey("PartnerID")]
        public virtual Partner Partner { get; set; }
    }
}

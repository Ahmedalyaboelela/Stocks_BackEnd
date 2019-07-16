using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Entities
{
    public class PortfolioOpeningStocks
    {
        [Key]
        public int PortOPenStockID { get; set; }

        public float StocksCount { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal StockValue { get; set; }

        public int PortfolioID { get; set; }
        public int PartnerID { get; set; }

        [ForeignKey("PortfolioID")]
        public virtual Portfolio Portfolio { get; set; }


        [ForeignKey("PartnerID")]
        public virtual Partner Partner { get; set; }
    }
}

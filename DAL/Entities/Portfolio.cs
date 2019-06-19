using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Entities
{
    public class Portfolio
    {
        [Key]
        public int PortfolioID { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(100)")]
        public string Code { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(150)")]
        public string NameAR { get; set; }
        
        [Column(TypeName = "nvarchar(150)")]
        public string NameEN { get; set; }
        public DateTime? EstablishDate { get; set; }

        [Column(TypeName = "nvarchar(MAX)")]
        public string Description { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? StockValue { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        public decimal? PortfolioCapital { get; set; }

        public int? PartnerCount { get; set; }

        public float? StocksCount { get; set; }

        public virtual ICollection<Portfolioshareholder> Portfolioshareholders { get; set; }
        public virtual ICollection<PortfolioAccount> PortfolioAccounts { get; set; }
        public virtual ICollection<SellingOrder> SellOrders { get; set; }
        public virtual ICollection<PurchaseOrder> PurchaseOrders { get; set; }
        public virtual ICollection<Notice> Notices { get; set; }




    }
}

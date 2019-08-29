using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Entities
{
    public class ReportSetting
    {
        [Key]
        public int ReportSettingID { get; set; }

        public int PortfolioID { get; set; }

        public int PartnerID { get; set; }

        public DateTime CurrentDate { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal DailyStockValue { get; set; }

        [ForeignKey("PortfolioID")]
        public virtual Portfolio Portfolio { get; set; }

        [ForeignKey("PartnerID")]
        public virtual Partner Partner { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Entities
{
    public class SellingOrderDetail
    {
        [Key]
        public int SellOrderDetailID { get; set; }

        public float StockCount { get; set; }

        public int PriceType { get; set; }

        public int SellingOrderID { get; set; }

        public int PartnerID { get; set; }



        [ForeignKey("SellingOrderID")]
        public virtual SellingOrder SellingOrder { get; set; }

        [ForeignKey("PartnerID")]
        public virtual Partner Partner { get; set; }
    }
}

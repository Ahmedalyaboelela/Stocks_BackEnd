using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Entities
{
    public class Entry
    {
        [Key]
        public int EntryID { get; set; }
        [Required]
        [Column(TypeName = "nvarchar(100)")]
        public string Code { get; set; }
        public DateTime? Date { get; set; }
   //     public int SellID { get; set; }
  //      public int PurchaseID { get; set; }
    //    public int ReceiptExchangeID { get; set; }
      //  public int NoticeID { get; set; }
     //   public int CurrencyID { get; set; }

   //     [ForeignKey("SellID")]
     //   public virtual SellOrder SellOrder { get; set; }

//        [ForeignKey("PurchaseID")]
  //      public virtual PurchaseOrder PurchaseOrder { get; set; }

    //    [ForeignKey("ReceiptExchangeID")]
      //  public virtual ReceiptExchange ReceiptExchange { get; set; }

   //     [ForeignKey("NoticeID")]
     //   public virtual Notice Notice { get; set; }

        //[foreignkey("currencyid")]
        //public virtual currency currency { get; set; }

    }
}

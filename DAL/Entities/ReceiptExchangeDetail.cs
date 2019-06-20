﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Entities
{
    public class ReceiptExchangeDetail
    {
        [Key]
        public int ReceiptExchangeID { get; set; }
      //  public int ReceiptID { get; set; }
        public decimal ReceiptExchangeAmount { get; set; }
        public int AccountID { get; set; }
        public int ChiqueNumber { get; set; }
        public bool Type { get; set; }

  //      [ForeignKey("ReceiptID")]
    //    public virtual ReceiptExchange ReceiptExchange { get; set; }

        [ForeignKey("AccountID")]
        public virtual Account Account { get; set; }
        

    }
}
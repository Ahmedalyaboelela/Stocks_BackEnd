﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Entities
{
    public class Currency
    {
        [Key]
    //    public virtual ICollection<Entry> Entries { get; set; }
        public virtual ICollection<ReceiptExchange> ReceiptExchanges { get; set; }


    }
}
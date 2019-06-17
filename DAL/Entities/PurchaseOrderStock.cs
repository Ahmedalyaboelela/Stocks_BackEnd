using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DAL.Entities
{
    public class PurchaseOrderStock
    {
        [Key]
        public int PurchaseStockID { get; set; }
        public int PurchaseID { get; set; }
        public decimal BankCommittion { get; set; }
        public float BankCommittionRate { get; set; }
        public string CompanyName { get; set; }

    }
}

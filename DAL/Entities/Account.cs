using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DAL.Entities
{
    public class Account
    {
        [Key]
        public int AccountID { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(100)")]
        public string Code { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(150)")]
        public string NameAR { get; set; }

        [Column(TypeName = "nvarchar(150)")]
        public string NameEN { get; set; }

        public bool AccountType { get; set; }

        public int AccountCategory { get; set; }

        public int AccountRefrence { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? Debit { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? Credit { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? DebitLimit { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? CreditLimit { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? DebitOpenningBalance { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? CreditOpenningBalance { get; set; }

        [Column(TypeName = "nvarchar(MAX)")]
        public string Address { get; set; }

        [Column(TypeName = "nvarchar(150)")]
        public string Phone1 { get; set; }

        [Column(TypeName = "nvarchar(150)")]
        public string Phone2 { get; set; }

        [Column(TypeName = "nvarchar(150)")]
        public string Fax { get; set; }

        [Column(TypeName = "nvarchar(150)")]
        public string Telex { get; set; }

        [Column(TypeName = "nvarchar(150)")]
        public string Mobile { get; set; }

        [Column(TypeName = "nvarchar(150)")]
        public string Email { get; set; }

        [Column(TypeName = "nvarchar(MAX)")]
        public string Notes { get; set; }

        [Column(TypeName = "nvarchar(250)")]
        public string TaxNum { get; set; }

        public bool IsActive { get; set; }

        public float? StocksDebit { get; set; }

        public float? StocksCredit { get; set; }

        public int? AccoutnParentID { get; set; }

        [ForeignKey("AccoutnParentID")]
        public virtual ICollection<Account> SubAccounts { get; set; }

        public virtual ICollection<EntryDetail> EntryDetails { get; set; }

        public virtual ICollection<Partner> Partners { get; set; }

        public virtual ICollection<PortfolioAccount> PortfolioAccounts { get; set; }

        public virtual ICollection<ReceiptExchangeDetail> ReceiptExchangeDetails { get; set; }

        




    }
}


using DAL.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Context
{
   public class StocksContext : IdentityDbContext
    {

        public StocksContext(DbContextOptions<StocksContext> options) : base(options)
        {

        }
        public DbSet<SellingInvoice> SellingInvoices { get; set; }

        public DbSet<SellingOrder> SellingOrders { get; set; }

        public DbSet<PurchaseOrder> PurchaseOrders { get; set; }

        public DbSet<PurchaseInvoice> PurchaseInvoices { get; set; }

        public DbSet<Entry> Entries { get; set; }

        public DbSet<PurchaseInvoiceDetail> PurchaseInvoiceDetails { get; set; }

        public DbSet<PurchaseOrderDetail> PurchaseOrderDetails { get; set; }
        public DbSet<SellingInvoiceDetail> SellingInvoiceDetails { get; set; }

        public DbSet<SellingOrderDetail> SellingOrderDetails { get; set; }
        public DbSet<EntryDetail> EntryDetails { get; set; }

        public DbSet<Account> Accounts { get; set; }

        public DbSet<Country> Countries { get; set; }
        public DbSet<Partner> Partners { get; set; }

        public DbSet<PartnerAttachment> PartnerAttachments { get; set; }
        public DbSet<Currency> Currencies { get; set; }

        public DbSet<Portfolio> Portfolios { get; set; }

        public DbSet<PortfolioAccount> PortfolioAccounts  { get; set; }

        public DbSet<PortfolioOpeningStocks> PortfolioOpeningStocks { get; set; }

        public DbSet<PortfolioTransaction> PortfolioTransactions { get; set; }

        
        public DbSet<ReceiptExchange> ReceiptExchanges { get; set; }
        public DbSet<ReceiptExchangeDetail> ReceiptExchangeDetails { get; set; }

        public DbSet<Setting> Settings { get; set; }
        public DbSet<SettingAccount> SettingAccounts { get; set; }
        public DbSet<Notice> Notices { get; set; }
        public DbSet<NoticeDetail> NoticeDetails { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<EmployeeCard> EmployeeCards { get; set; }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }

        public DbSet<ReportSetting> ReportSettings { get; set; }

        public DbSet<UserLog> UserLogs { get; set; }

    }
}

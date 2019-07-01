﻿// <auto-generated />
using System;
using DAL.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DAL.Migrations
{
    [DbContext(typeof(StocksContext))]
    [Migration("20190627163736_changesindb27-6-2019")]
    partial class changesindb2762019
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.11-servicing-32099")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("DAL.Entities.Account", b =>
                {
                    b.Property<int>("AccountID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("AccountCategory");

                    b.Property<int>("AccountRefrence");

                    b.Property<bool>("AccountType");

                    b.Property<int?>("AccoutnParentID");

                    b.Property<string>("Address")
                        .HasColumnType("nvarchar(MAX)");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("nvarchar(100)");

                    b.Property<decimal?>("Credit")
                        .HasColumnType("decimal(10,2)");

                    b.Property<decimal?>("CreditLimit")
                        .HasColumnType("decimal(10,2)");

                    b.Property<decimal?>("CreditOpenningBalance")
                        .HasColumnType("decimal(10,2)");

                    b.Property<decimal?>("Debit")
                        .HasColumnType("decimal(10,2)");

                    b.Property<decimal?>("DebitLimit")
                        .HasColumnType("decimal(10,2)");

                    b.Property<decimal?>("DebitOpenningBalance")
                        .HasColumnType("decimal(10,2)");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(150)");

                    b.Property<string>("Fax")
                        .HasColumnType("nvarchar(150)");

                    b.Property<bool>("IsActive");

                    b.Property<string>("Mobile")
                        .HasColumnType("nvarchar(150)");

                    b.Property<string>("NameAR")
                        .IsRequired()
                        .HasColumnType("nvarchar(150)");

                    b.Property<string>("NameEN")
                        .HasColumnType("nvarchar(150)");

                    b.Property<string>("Notes")
                        .HasColumnType("nvarchar(MAX)");

                    b.Property<string>("Phone1")
                        .HasColumnType("nvarchar(150)");

                    b.Property<string>("Phone2")
                        .HasColumnType("nvarchar(150)");

                    b.Property<string>("TaxNum")
                        .HasColumnType("nvarchar(250)");

                    b.Property<string>("Telex")
                        .HasColumnType("nvarchar(150)");

                    b.HasKey("AccountID");

                    b.HasIndex("AccoutnParentID");

                    b.ToTable("Accounts");
                });

            modelBuilder.Entity("DAL.Entities.Country", b =>
                {
                    b.Property<int>("CountryID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("NameAR")
                        .IsRequired()
                        .HasColumnType("nvarchar(150)");

                    b.Property<string>("NameEN")
                        .HasColumnType("nvarchar(150)");

                    b.HasKey("CountryID");

                    b.ToTable("Countries");
                });

            modelBuilder.Entity("DAL.Entities.Currency", b =>
                {
                    b.Property<int>("CurrencyID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("nvarchar(100)");

                    b.Property<float>("CurrencyValue");

                    b.Property<string>("NameAR")
                        .IsRequired()
                        .HasColumnType("nvarchar(150)");

                    b.Property<string>("NameEN")
                        .HasColumnType("nvarchar(150)");

                    b.Property<string>("PartName")
                        .HasColumnType("nvarchar(150)");

                    b.Property<float>("PartValue");

                    b.HasKey("CurrencyID");

                    b.ToTable("Currencies");
                });

            modelBuilder.Entity("DAL.Entities.Employee", b =>
                {
                    b.Property<int>("EmployeeID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Age");

                    b.Property<string>("BankAccNum")
                        .HasColumnType("nvarchar(100)");

                    b.Property<DateTime?>("BirthDate");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(150)");

                    b.Property<string>("ImagePath")
                        .HasColumnType("nvarchar(150)");

                    b.Property<bool>("IsActive");

                    b.Property<bool>("IsInternal");

                    b.Property<string>("Mobile")
                        .HasColumnType("nvarchar(150)");

                    b.Property<string>("NameAR")
                        .IsRequired()
                        .HasColumnType("nvarchar(150)");

                    b.Property<string>("NameEN")
                        .HasColumnType("nvarchar(150)");

                    b.Property<string>("Nationality")
                        .HasColumnType("nvarchar(150)");

                    b.Property<string>("PassportProfession")
                        .HasColumnType("nvarchar(150)");

                    b.Property<string>("Profession")
                        .HasColumnType("nvarchar(150)");

                    b.Property<int>("Religion");

                    b.HasKey("EmployeeID");

                    b.ToTable("Employees");
                });

            modelBuilder.Entity("DAL.Entities.EmployeeCard", b =>
                {
                    b.Property<int>("EmpCardId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("CardType");

                    b.Property<string>("Code")
                        .HasColumnType("nvarchar(100)");

                    b.Property<int>("EmployeeID");

                    b.Property<DateTime?>("EndDate");

                    b.Property<decimal>("Fees")
                        .HasColumnType("decimal(10,2)");

                    b.Property<DateTime?>("IssueDate");

                    b.Property<string>("IssuePlace")
                        .HasColumnType("nvarchar(150)");

                    b.Property<string>("Notes")
                        .HasColumnType("nvarchar(MAX)");

                    b.Property<DateTime?>("RenewalDate");

                    b.HasKey("EmpCardId");

                    b.HasIndex("EmployeeID");

                    b.ToTable("EmployeeCards");
                });

            modelBuilder.Entity("DAL.Entities.Entry", b =>
                {
                    b.Property<int>("EntryID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("nvarchar(100)");

                    b.Property<DateTime?>("Date");

                    b.Property<int?>("NoticeID");

                    b.Property<int?>("PurchaseOrderID");

                    b.Property<int?>("ReceiptID");

                    b.Property<int?>("SellingOrderID");

                    b.Property<bool>("TransferedToAccounts");

                    b.HasKey("EntryID");

                    b.HasIndex("NoticeID")
                        .IsUnique()
                        .HasFilter("[NoticeID] IS NOT NULL");

                    b.HasIndex("PurchaseOrderID")
                        .IsUnique()
                        .HasFilter("[PurchaseOrderID] IS NOT NULL");

                    b.HasIndex("ReceiptID")
                        .IsUnique()
                        .HasFilter("[ReceiptID] IS NOT NULL");

                    b.HasIndex("SellingOrderID")
                        .IsUnique()
                        .HasFilter("[SellingOrderID] IS NOT NULL");

                    b.ToTable("Entries");
                });

            modelBuilder.Entity("DAL.Entities.EntryDetail", b =>
                {
                    b.Property<int>("EntryDetailID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("AccountID");

                    b.Property<decimal?>("Credit")
                        .HasColumnType("decimal(10,2)");

                    b.Property<decimal?>("Debit")
                        .HasColumnType("decimal(10,2)");

                    b.Property<int>("EntryID");

                    b.Property<float?>("StocksCredit");

                    b.Property<float?>("StocksDebit");

                    b.HasKey("EntryDetailID");

                    b.HasIndex("AccountID");

                    b.HasIndex("EntryID");

                    b.ToTable("EntryDetails");
                });

            modelBuilder.Entity("DAL.Entities.Notice", b =>
                {
                    b.Property<int>("NoticeID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("nvarchar(100)");

                    b.Property<int?>("CurrencyID");

                    b.Property<int>("EmployeeID");

                    b.Property<DateTime?>("NoticeDate");

                    b.Property<int>("PortfolioID");

                    b.Property<bool>("Type");

                    b.HasKey("NoticeID");

                    b.HasIndex("CurrencyID");

                    b.HasIndex("EmployeeID");

                    b.HasIndex("PortfolioID");

                    b.ToTable("Notices");
                });

            modelBuilder.Entity("DAL.Entities.NoticeDetail", b =>
                {
                    b.Property<int>("NoticeDetailID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("AccountID");

                    b.Property<decimal?>("Credit")
                        .HasColumnType("decimal(10,2)");

                    b.Property<decimal?>("Debit")
                        .HasColumnType("decimal(10,2)");

                    b.Property<int>("NoticeID");

                    b.Property<float?>("StocksCredit");

                    b.Property<float?>("StocksDebit");

                    b.HasKey("NoticeDetailID");

                    b.HasIndex("AccountID");

                    b.HasIndex("NoticeID");

                    b.ToTable("NoticeDetails");
                });

            modelBuilder.Entity("DAL.Entities.Partner", b =>
                {
                    b.Property<int>("PartnerID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("AccountID");

                    b.Property<string>("Address")
                        .HasColumnType("nvarchar(MAX)");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("ConvertNumber")
                        .HasColumnType("nvarchar(150)");

                    b.Property<int?>("CountryID");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(150)");

                    b.Property<string>("Fax")
                        .HasColumnType("nvarchar(150)");

                    b.Property<string>("IdentityNumber")
                        .HasColumnType("nvarchar(150)");

                    b.Property<int>("IdentityType");

                    b.Property<DateTime?>("IssueDate");

                    b.Property<string>("IssuePlace")
                        .HasColumnType("nvarchar(150)");

                    b.Property<string>("Mobile")
                        .HasColumnType("nvarchar(150)");

                    b.Property<string>("NameAR")
                        .IsRequired()
                        .HasColumnType("nvarchar(150)");

                    b.Property<string>("NameEN")
                        .HasColumnType("nvarchar(150)");

                    b.Property<string>("Phone1")
                        .HasColumnType("nvarchar(150)");

                    b.Property<string>("Phone2")
                        .HasColumnType("nvarchar(150)");

                    b.HasKey("PartnerID");

                    b.HasIndex("AccountID");

                    b.HasIndex("CountryID");

                    b.ToTable("Partners");
                });

            modelBuilder.Entity("DAL.Entities.Portfolio", b =>
                {
                    b.Property<int>("PortfolioID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(MAX)");

                    b.Property<DateTime?>("EstablishDate");

                    b.Property<string>("NameAR")
                        .IsRequired()
                        .HasColumnType("nvarchar(150)");

                    b.Property<string>("NameEN")
                        .HasColumnType("nvarchar(150)");

                    b.Property<int?>("PartnerCount");

                    b.Property<decimal?>("PortfolioCapital")
                        .HasColumnType("decimal(10,2)");

                    b.Property<decimal?>("StockValue")
                        .HasColumnType("decimal(10,2)");

                    b.Property<float?>("StocksCount");

                    b.HasKey("PortfolioID");

                    b.ToTable("Portfolios");
                });

            modelBuilder.Entity("DAL.Entities.PortfolioAccount", b =>
                {
                    b.Property<int>("PortfolioAccountID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("AccountID");

                    b.Property<int>("PortfolioID");

                    b.Property<bool>("Type");

                    b.HasKey("PortfolioAccountID");

                    b.HasIndex("AccountID");

                    b.HasIndex("PortfolioID");

                    b.ToTable("PortfolioAccounts");
                });

            modelBuilder.Entity("DAL.Entities.PortfolioShareHolder", b =>
                {
                    b.Property<int>("PortShareID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<decimal>("Amount")
                        .HasColumnType("decimal(10,2)");

                    b.Property<string>("Notes")
                        .HasColumnType("nvarchar(MAX)");

                    b.Property<int>("PartnerID");

                    b.Property<float>("Percentage");

                    b.Property<int>("PortfolioID");

                    b.Property<int>("StocksCount");

                    b.HasKey("PortShareID");

                    b.HasIndex("PartnerID");

                    b.HasIndex("PortfolioID");

                    b.ToTable("PortfolioShareHolders");
                });

            modelBuilder.Entity("DAL.Entities.PurchaseOrder", b =>
                {
                    b.Property<int>("PurchaseOrderID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("nvarchar(100)");

                    b.Property<DateTime?>("Date");

                    b.Property<int>("EmployeeID");

                    b.Property<int>("PayWay");

                    b.Property<int>("PortfolioID");

                    b.HasKey("PurchaseOrderID");

                    b.HasIndex("EmployeeID");

                    b.HasIndex("PortfolioID");

                    b.ToTable("PurchaseOrders");
                });

            modelBuilder.Entity("DAL.Entities.PurchaseOrderDetail", b =>
                {
                    b.Property<int>("PurchaseOrderDetailID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<decimal>("BankCommission")
                        .HasColumnType("decimal(10,2)");

                    b.Property<float>("BankCommissionRate");

                    b.Property<decimal>("NetAmmount")
                        .HasColumnType("decimal(10,2)");

                    b.Property<int>("PurchaseID");

                    b.Property<decimal>("PurchasePrice")
                        .HasColumnType("decimal(10,2)");

                    b.Property<decimal>("PurchaseValue")
                        .HasColumnType("decimal(10,2)");

                    b.Property<int>("StockCount");

                    b.Property<decimal>("TaxOnCommission")
                        .HasColumnType("decimal(10,2)");

                    b.Property<float>("TaxRateOnCommission");

                    b.HasKey("PurchaseOrderDetailID");

                    b.HasIndex("PurchaseID");

                    b.ToTable("PurchaseOrderDetails");
                });

            modelBuilder.Entity("DAL.Entities.ReceiptExchange", b =>
                {
                    b.Property<int>("ReceiptID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime?>("ChiqueDate");

                    b.Property<int?>("ChiqueNumber");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("nvarchar(100)");

                    b.Property<int?>("CurrencyID");

                    b.Property<DateTime?>("Date");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(MAX)");

                    b.Property<bool>("Type");

                    b.HasKey("ReceiptID");

                    b.HasIndex("CurrencyID");

                    b.ToTable("ReceiptExchanges");
                });

            modelBuilder.Entity("DAL.Entities.ReceiptExchangeDetail", b =>
                {
                    b.Property<int>("ReceiptExchangeID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("AccountID");

                    b.Property<int?>("ChiqueNumber");

                    b.Property<decimal?>("Credit")
                        .HasColumnType("decimal(10,2)");

                    b.Property<decimal?>("Debit")
                        .HasColumnType("decimal(10,2)");

                    b.Property<int>("ReceiptID");

                    b.HasKey("ReceiptExchangeID");

                    b.HasIndex("AccountID");

                    b.HasIndex("ReceiptID");

                    b.ToTable("ReceiptExchangeDetails");
                });

            modelBuilder.Entity("DAL.Entities.SellingOrder", b =>
                {
                    b.Property<int>("SellingOrderID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("nvarchar(100)");

                    b.Property<DateTime?>("Date");

                    b.Property<int>("EmployeeID");

                    b.Property<int>("PayWay");

                    b.Property<int>("PortfolioID");

                    b.HasKey("SellingOrderID");

                    b.HasIndex("EmployeeID");

                    b.HasIndex("PortfolioID");

                    b.ToTable("SellingOrders");
                });

            modelBuilder.Entity("DAL.Entities.SellingOrderDetail", b =>
                {
                    b.Property<int>("SellOrderDetailID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<decimal>("BankCommission")
                        .HasColumnType("decimal(10,2)");

                    b.Property<float>("BankCommissionRate");

                    b.Property<decimal>("NetAmmount")
                        .HasColumnType("decimal(10,2)");

                    b.Property<decimal>("SelingValue")
                        .HasColumnType("decimal(10,2)");

                    b.Property<int>("SellingOrderID");

                    b.Property<decimal>("SellingPrice")
                        .HasColumnType("decimal(10,2)");

                    b.Property<int>("StockCount");

                    b.Property<decimal>("TaxOnCommission")
                        .HasColumnType("decimal(10,2)");

                    b.Property<float>("TaxRateOnCommission");

                    b.HasKey("SellOrderDetailID");

                    b.HasIndex("SellingOrderID");

                    b.ToTable("SellingOrderDetails");
                });

            modelBuilder.Entity("DAL.Entities.Setting", b =>
                {
                    b.Property<int>("SettingID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("AutoGenerateEntry");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("nvarchar(100)");

                    b.Property<bool?>("DoNotGenerateEntry");

                    b.Property<bool>("GenerateEntry");

                    b.Property<bool>("TransferToAccounts");

                    b.Property<int>("VoucherType");

                    b.HasKey("SettingID");

                    b.ToTable("Settings");
                });

            modelBuilder.Entity("DAL.Entities.SettingAccount", b =>
                {
                    b.Property<int>("SettingAccountID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("AccountID");

                    b.Property<int>("AccountType");

                    b.Property<int>("SettingID");

                    b.HasKey("SettingAccountID");

                    b.HasIndex("AccountID");

                    b.HasIndex("SettingID");

                    b.ToTable("SettingAccounts");
                });

            modelBuilder.Entity("DAL.Entities.Account", b =>
                {
                    b.HasOne("DAL.Entities.Account")
                        .WithMany("SubAccounts")
                        .HasForeignKey("AccoutnParentID");
                });

            modelBuilder.Entity("DAL.Entities.EmployeeCard", b =>
                {
                    b.HasOne("DAL.Entities.Employee", "Employee")
                        .WithMany("EmployeeCards")
                        .HasForeignKey("EmployeeID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("DAL.Entities.Entry", b =>
                {
                    b.HasOne("DAL.Entities.Notice", "Notice")
                        .WithOne("Entry")
                        .HasForeignKey("DAL.Entities.Entry", "NoticeID");

                    b.HasOne("DAL.Entities.PurchaseOrder", "PurchaseOrder")
                        .WithOne("Entry")
                        .HasForeignKey("DAL.Entities.Entry", "PurchaseOrderID");

                    b.HasOne("DAL.Entities.ReceiptExchange", "ReceiptExchange")
                        .WithOne("Entry")
                        .HasForeignKey("DAL.Entities.Entry", "ReceiptID");

                    b.HasOne("DAL.Entities.SellingOrder", "SellingOrder")
                        .WithOne("Entry")
                        .HasForeignKey("DAL.Entities.Entry", "SellingOrderID");
                });

            modelBuilder.Entity("DAL.Entities.EntryDetail", b =>
                {
                    b.HasOne("DAL.Entities.Account", "Account")
                        .WithMany("EntryDetails")
                        .HasForeignKey("AccountID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("DAL.Entities.Entry", "Entry")
                        .WithMany()
                        .HasForeignKey("EntryID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("DAL.Entities.Notice", b =>
                {
                    b.HasOne("DAL.Entities.Currency", "Currency")
                        .WithMany()
                        .HasForeignKey("CurrencyID");

                    b.HasOne("DAL.Entities.Employee", "Employee")
                        .WithMany("Notices")
                        .HasForeignKey("EmployeeID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("DAL.Entities.Portfolio", "Portfolio")
                        .WithMany("Notices")
                        .HasForeignKey("PortfolioID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("DAL.Entities.NoticeDetail", b =>
                {
                    b.HasOne("DAL.Entities.Account", "Account")
                        .WithMany()
                        .HasForeignKey("AccountID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("DAL.Entities.Notice", "Notice")
                        .WithMany("NoticeDetails")
                        .HasForeignKey("NoticeID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("DAL.Entities.Partner", b =>
                {
                    b.HasOne("DAL.Entities.Account", "Account")
                        .WithMany("Partners")
                        .HasForeignKey("AccountID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("DAL.Entities.Country", "Country")
                        .WithMany("Partners")
                        .HasForeignKey("CountryID");
                });

            modelBuilder.Entity("DAL.Entities.PortfolioAccount", b =>
                {
                    b.HasOne("DAL.Entities.Account", "Account")
                        .WithMany("PortfolioAccounts")
                        .HasForeignKey("AccountID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("DAL.Entities.Portfolio", "Portfolio")
                        .WithMany("PortfolioAccounts")
                        .HasForeignKey("PortfolioID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("DAL.Entities.PortfolioShareHolder", b =>
                {
                    b.HasOne("DAL.Entities.Partner", "Partner")
                        .WithMany("PortfolioShareHolders")
                        .HasForeignKey("PartnerID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("DAL.Entities.Portfolio", "Portfolio")
                        .WithMany("PortfolioShareHolders")
                        .HasForeignKey("PortfolioID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("DAL.Entities.PurchaseOrder", b =>
                {
                    b.HasOne("DAL.Entities.Employee", "Employee")
                        .WithMany("PurchaseOrders")
                        .HasForeignKey("EmployeeID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("DAL.Entities.Portfolio")
                        .WithMany("PurchaseOrders")
                        .HasForeignKey("PortfolioID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("DAL.Entities.PurchaseOrderDetail", b =>
                {
                    b.HasOne("DAL.Entities.PurchaseOrder", "PurchaseOrder")
                        .WithMany("PurchaseOrderDetails")
                        .HasForeignKey("PurchaseID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("DAL.Entities.ReceiptExchange", b =>
                {
                    b.HasOne("DAL.Entities.Currency", "Currency")
                        .WithMany("ReceiptExchanges")
                        .HasForeignKey("CurrencyID");
                });

            modelBuilder.Entity("DAL.Entities.ReceiptExchangeDetail", b =>
                {
                    b.HasOne("DAL.Entities.Account", "Account")
                        .WithMany("ReceiptExchangeDetails")
                        .HasForeignKey("AccountID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("DAL.Entities.ReceiptExchange", "ReceiptExchange")
                        .WithMany()
                        .HasForeignKey("ReceiptID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("DAL.Entities.SellingOrder", b =>
                {
                    b.HasOne("DAL.Entities.Employee", "Employee")
                        .WithMany("SellingOrders")
                        .HasForeignKey("EmployeeID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("DAL.Entities.Portfolio")
                        .WithMany("SellOrders")
                        .HasForeignKey("PortfolioID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("DAL.Entities.SellingOrderDetail", b =>
                {
                    b.HasOne("DAL.Entities.SellingOrder", "SellOrder")
                        .WithMany("SellingOrderDetails")
                        .HasForeignKey("SellingOrderID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("DAL.Entities.SettingAccount", b =>
                {
                    b.HasOne("DAL.Entities.Account", "Account")
                        .WithMany()
                        .HasForeignKey("AccountID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("DAL.Entities.Setting", "Setting")
                        .WithMany("SettingAccounts")
                        .HasForeignKey("SettingID")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}

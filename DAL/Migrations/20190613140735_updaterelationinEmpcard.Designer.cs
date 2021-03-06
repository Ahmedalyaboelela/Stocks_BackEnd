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
    [Migration("20190613140735_updaterelationinEmpcard")]
    partial class updaterelationinEmpcard
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

                    b.Property<bool>("AccountType");

                    b.Property<int?>("AccoutnParentID");

                    b.Property<string>("Address")
                        .HasColumnType("nvarchar(MAX)");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("nvarchar(100)");

                    b.Property<decimal>("CreditLimit")
                        .HasColumnType("decimal(10,2)");

                    b.Property<decimal>("DebitLimit")
                        .HasColumnType("decimal(10,2)");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(150)");

                    b.Property<string>("Fax")
                        .HasColumnType("nvarchar(150)");

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

                    b.Property<string>("NameAR")
                        .IsRequired()
                        .HasColumnType("nvarchar(150)");

                    b.Property<string>("NameEN")
                        .HasColumnType("nvarchar(150)");

                    b.HasKey("CountryID");

                    b.ToTable("Countries");
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

            modelBuilder.Entity("DAL.Entities.Portfolioshareholder", b =>
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

                    b.ToTable("Portfolioshareholders");
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

            modelBuilder.Entity("DAL.Entities.Portfolioshareholder", b =>
                {
                    b.HasOne("DAL.Entities.Partner", "Partner")
                        .WithMany()
                        .HasForeignKey("PartnerID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("DAL.Entities.Portfolio", "Portfolio")
                        .WithMany("Portfolioshareholders")
                        .HasForeignKey("PortfolioID")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}

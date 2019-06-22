using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class ChangesInTablesRelations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NoticeDetails_Partners_PartnerID",
                table: "NoticeDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_Portfolioshareholders_Partners_PartnerID",
                table: "Portfolioshareholders");

            migrationBuilder.DropForeignKey(
                name: "FK_Portfolioshareholders_Portfolios_PortfolioID",
                table: "Portfolioshareholders");

            migrationBuilder.DropForeignKey(
                name: "FK_ReceiptExchanges_Accounts_AccountID",
                table: "ReceiptExchanges");

            migrationBuilder.DropForeignKey(
                name: "FK_ReceiptExchanges_Currency_CurrencyID",
                table: "ReceiptExchanges");

            migrationBuilder.DropTable(
                name: "PurchaseOrderStocks");

            migrationBuilder.DropTable(
                name: "SellOrderStocks");

            migrationBuilder.DropTable(
                name: "SellOrders");

            migrationBuilder.DropIndex(
                name: "IX_ReceiptExchanges_AccountID",
                table: "ReceiptExchanges");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Portfolioshareholders",
                table: "Portfolioshareholders");

            migrationBuilder.DropIndex(
                name: "IX_NoticeDetails_PartnerID",
                table: "NoticeDetails");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Currency",
                table: "Currency");

            migrationBuilder.DropColumn(
                name: "AccountID",
                table: "ReceiptExchanges");

            migrationBuilder.DropColumn(
                name: "PartnerID",
                table: "NoticeDetails");

            migrationBuilder.RenameTable(
                name: "Portfolioshareholders",
                newName: "PortfolioShareHolders");

            migrationBuilder.RenameTable(
                name: "Currency",
                newName: "Currencies");

            migrationBuilder.RenameColumn(
                name: "PurchaseWay",
                table: "PurchaseOrders",
                newName: "PayWay");

            migrationBuilder.RenameColumn(
                name: "PurchaseID",
                table: "PurchaseOrders",
                newName: "PurchaseOrderID");

            migrationBuilder.RenameIndex(
                name: "IX_Portfolioshareholders_PortfolioID",
                table: "PortfolioShareHolders",
                newName: "IX_PortfolioShareHolders_PortfolioID");

            migrationBuilder.RenameIndex(
                name: "IX_Portfolioshareholders_PartnerID",
                table: "PortfolioShareHolders",
                newName: "IX_PortfolioShareHolders_PartnerID");

            migrationBuilder.RenameColumn(
                name: "NoticeCredDepID",
                table: "NoticeDetails",
                newName: "NoticeDetailID");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Settings",
                type: "nvarchar(100)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "ReceiptExchangeAmount",
                table: "ReceiptExchangeDetails",
                type: "decimal(10,2)",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AddColumn<int>(
                name: "ReceiptID",
                table: "ReceiptExchangeDetails",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EntryID",
                table: "EntryDetails",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NoticeID",
                table: "Entries",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PurchaseOrderID",
                table: "Entries",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReceiptID",
                table: "Entries",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SellingOrderID",
                table: "Entries",
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "DebitLimit",
                table: "Accounts",
                type: "decimal(10,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "CreditLimit",
                table: "Accounts",
                type: "decimal(10,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)");

            migrationBuilder.AddColumn<int>(
                name: "AccountRefrence",
                table: "Accounts",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "Credit",
                table: "Accounts",
                type: "decimal(10,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CreditOpenningBalance",
                table: "Accounts",
                type: "decimal(10,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Debit",
                table: "Accounts",
                type: "decimal(10,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DebitOpenningBalance",
                table: "Accounts",
                type: "decimal(10,2)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Accounts",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Currencies",
                type: "nvarchar(100)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_PortfolioShareHolders",
                table: "PortfolioShareHolders",
                column: "PortShareID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Currencies",
                table: "Currencies",
                column: "CurrencyID");

            migrationBuilder.CreateTable(
                name: "PurchaseOrderDetails",
                columns: table => new
                {
                    PurchaseOrderDetailID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    StockCount = table.Column<int>(nullable: false),
                    PurchasePrice = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    PurchaseValue = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    BankCommissionRate = table.Column<float>(nullable: false),
                    BankCommission = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    TaxRateOnCommission = table.Column<float>(nullable: false),
                    TaxOnCommission = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    NetAmmount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    PurchaseID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseOrderDetails", x => x.PurchaseOrderDetailID);
                    table.ForeignKey(
                        name: "FK_PurchaseOrderDetails_PurchaseOrders_PurchaseID",
                        column: x => x.PurchaseID,
                        principalTable: "PurchaseOrders",
                        principalColumn: "PurchaseOrderID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SellingOrders",
                columns: table => new
                {
                    SellingOrderID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    PayWay = table.Column<int>(nullable: false),
                    Date = table.Column<DateTime>(nullable: true),
                    EmployeeID = table.Column<int>(nullable: false),
                    PortfolioID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SellingOrders", x => x.SellingOrderID);
                    table.ForeignKey(
                        name: "FK_SellingOrders_Employees_EmployeeID",
                        column: x => x.EmployeeID,
                        principalTable: "Employees",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SellingOrders_Portfolios_PortfolioID",
                        column: x => x.PortfolioID,
                        principalTable: "Portfolios",
                        principalColumn: "PortfolioID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SellingOrderDetails",
                columns: table => new
                {
                    SellOrderDetailID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    StockCount = table.Column<int>(nullable: false),
                    SellingPrice = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    SelingValue = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    BankCommissionRate = table.Column<float>(nullable: false),
                    BankCommission = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    TaxRateOnCommission = table.Column<float>(nullable: false),
                    TaxOnCommission = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    NetAmmount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    SellingOrderID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SellingOrderDetails", x => x.SellOrderDetailID);
                    table.ForeignKey(
                        name: "FK_SellingOrderDetails_SellingOrders_SellingOrderID",
                        column: x => x.SellingOrderID,
                        principalTable: "SellingOrders",
                        principalColumn: "SellingOrderID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptExchangeDetails_ReceiptID",
                table: "ReceiptExchangeDetails",
                column: "ReceiptID");

            migrationBuilder.CreateIndex(
                name: "IX_EntryDetails_EntryID",
                table: "EntryDetails",
                column: "EntryID");

            migrationBuilder.CreateIndex(
                name: "IX_Entries_NoticeID",
                table: "Entries",
                column: "NoticeID",
                unique: true,
                filter: "[NoticeID] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Entries_PurchaseOrderID",
                table: "Entries",
                column: "PurchaseOrderID",
                unique: true,
                filter: "[PurchaseOrderID] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Entries_ReceiptID",
                table: "Entries",
                column: "ReceiptID",
                unique: true,
                filter: "[ReceiptID] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Entries_SellingOrderID",
                table: "Entries",
                column: "SellingOrderID",
                unique: true,
                filter: "[SellingOrderID] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderDetails_PurchaseID",
                table: "PurchaseOrderDetails",
                column: "PurchaseID");

            migrationBuilder.CreateIndex(
                name: "IX_SellingOrderDetails_SellingOrderID",
                table: "SellingOrderDetails",
                column: "SellingOrderID");

            migrationBuilder.CreateIndex(
                name: "IX_SellingOrders_EmployeeID",
                table: "SellingOrders",
                column: "EmployeeID");

            migrationBuilder.CreateIndex(
                name: "IX_SellingOrders_PortfolioID",
                table: "SellingOrders",
                column: "PortfolioID");

            migrationBuilder.AddForeignKey(
                name: "FK_Entries_Notices_NoticeID",
                table: "Entries",
                column: "NoticeID",
                principalTable: "Notices",
                principalColumn: "NoticeID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Entries_PurchaseOrders_PurchaseOrderID",
                table: "Entries",
                column: "PurchaseOrderID",
                principalTable: "PurchaseOrders",
                principalColumn: "PurchaseOrderID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Entries_ReceiptExchanges_ReceiptID",
                table: "Entries",
                column: "ReceiptID",
                principalTable: "ReceiptExchanges",
                principalColumn: "ReceiptID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Entries_SellingOrders_SellingOrderID",
                table: "Entries",
                column: "SellingOrderID",
                principalTable: "SellingOrders",
                principalColumn: "SellingOrderID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EntryDetails_Entries_EntryID",
                table: "EntryDetails",
                column: "EntryID",
                principalTable: "Entries",
                principalColumn: "EntryID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PortfolioShareHolders_Partners_PartnerID",
                table: "PortfolioShareHolders",
                column: "PartnerID",
                principalTable: "Partners",
                principalColumn: "PartnerID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PortfolioShareHolders_Portfolios_PortfolioID",
                table: "PortfolioShareHolders",
                column: "PortfolioID",
                principalTable: "Portfolios",
                principalColumn: "PortfolioID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ReceiptExchangeDetails_ReceiptExchanges_ReceiptID",
                table: "ReceiptExchangeDetails",
                column: "ReceiptID",
                principalTable: "ReceiptExchanges",
                principalColumn: "ReceiptID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ReceiptExchanges_Currencies_CurrencyID",
                table: "ReceiptExchanges",
                column: "CurrencyID",
                principalTable: "Currencies",
                principalColumn: "CurrencyID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Entries_Notices_NoticeID",
                table: "Entries");

            migrationBuilder.DropForeignKey(
                name: "FK_Entries_PurchaseOrders_PurchaseOrderID",
                table: "Entries");

            migrationBuilder.DropForeignKey(
                name: "FK_Entries_ReceiptExchanges_ReceiptID",
                table: "Entries");

            migrationBuilder.DropForeignKey(
                name: "FK_Entries_SellingOrders_SellingOrderID",
                table: "Entries");

            migrationBuilder.DropForeignKey(
                name: "FK_EntryDetails_Entries_EntryID",
                table: "EntryDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_PortfolioShareHolders_Partners_PartnerID",
                table: "PortfolioShareHolders");

            migrationBuilder.DropForeignKey(
                name: "FK_PortfolioShareHolders_Portfolios_PortfolioID",
                table: "PortfolioShareHolders");

            migrationBuilder.DropForeignKey(
                name: "FK_ReceiptExchangeDetails_ReceiptExchanges_ReceiptID",
                table: "ReceiptExchangeDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_ReceiptExchanges_Currencies_CurrencyID",
                table: "ReceiptExchanges");

            migrationBuilder.DropTable(
                name: "PurchaseOrderDetails");

            migrationBuilder.DropTable(
                name: "SellingOrderDetails");

            migrationBuilder.DropTable(
                name: "SellingOrders");

            migrationBuilder.DropIndex(
                name: "IX_ReceiptExchangeDetails_ReceiptID",
                table: "ReceiptExchangeDetails");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PortfolioShareHolders",
                table: "PortfolioShareHolders");

            migrationBuilder.DropIndex(
                name: "IX_EntryDetails_EntryID",
                table: "EntryDetails");

            migrationBuilder.DropIndex(
                name: "IX_Entries_NoticeID",
                table: "Entries");

            migrationBuilder.DropIndex(
                name: "IX_Entries_PurchaseOrderID",
                table: "Entries");

            migrationBuilder.DropIndex(
                name: "IX_Entries_ReceiptID",
                table: "Entries");

            migrationBuilder.DropIndex(
                name: "IX_Entries_SellingOrderID",
                table: "Entries");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Currencies",
                table: "Currencies");

            migrationBuilder.DropColumn(
                name: "ReceiptID",
                table: "ReceiptExchangeDetails");

            migrationBuilder.DropColumn(
                name: "EntryID",
                table: "EntryDetails");

            migrationBuilder.DropColumn(
                name: "NoticeID",
                table: "Entries");

            migrationBuilder.DropColumn(
                name: "PurchaseOrderID",
                table: "Entries");

            migrationBuilder.DropColumn(
                name: "ReceiptID",
                table: "Entries");

            migrationBuilder.DropColumn(
                name: "SellingOrderID",
                table: "Entries");

            migrationBuilder.DropColumn(
                name: "AccountRefrence",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "Credit",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "CreditOpenningBalance",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "Debit",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "DebitOpenningBalance",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Accounts");

            migrationBuilder.RenameTable(
                name: "PortfolioShareHolders",
                newName: "Portfolioshareholders");

            migrationBuilder.RenameTable(
                name: "Currencies",
                newName: "Currency");

            migrationBuilder.RenameColumn(
                name: "PayWay",
                table: "PurchaseOrders",
                newName: "PurchaseWay");

            migrationBuilder.RenameColumn(
                name: "PurchaseOrderID",
                table: "PurchaseOrders",
                newName: "PurchaseID");

            migrationBuilder.RenameIndex(
                name: "IX_PortfolioShareHolders_PortfolioID",
                table: "Portfolioshareholders",
                newName: "IX_Portfolioshareholders_PortfolioID");

            migrationBuilder.RenameIndex(
                name: "IX_PortfolioShareHolders_PartnerID",
                table: "Portfolioshareholders",
                newName: "IX_Portfolioshareholders_PartnerID");

            migrationBuilder.RenameColumn(
                name: "NoticeDetailID",
                table: "NoticeDetails",
                newName: "NoticeCredDepID");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Settings",
                type: "nvarchar(100)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)");

            migrationBuilder.AddColumn<int>(
                name: "AccountID",
                table: "ReceiptExchanges",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<decimal>(
                name: "ReceiptExchangeAmount",
                table: "ReceiptExchangeDetails",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)");

            migrationBuilder.AddColumn<int>(
                name: "PartnerID",
                table: "NoticeDetails",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<decimal>(
                name: "DebitLimit",
                table: "Accounts",
                type: "decimal(10,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "CreditLimit",
                table: "Accounts",
                type: "decimal(10,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Currency",
                type: "nvarchar(100)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Portfolioshareholders",
                table: "Portfolioshareholders",
                column: "PortShareID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Currency",
                table: "Currency",
                column: "CurrencyID");

            migrationBuilder.CreateTable(
                name: "PurchaseOrderStocks",
                columns: table => new
                {
                    PurchaseStockID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BankCommittion = table.Column<decimal>(nullable: false),
                    BankCommittionRate = table.Column<float>(nullable: false),
                    PartnerID = table.Column<int>(nullable: false),
                    PurchaseID = table.Column<int>(nullable: false),
                    PurchaseValue = table.Column<decimal>(nullable: false),
                    StockCount = table.Column<int>(nullable: false),
                    StockPrice = table.Column<decimal>(nullable: false),
                    TaxOnCommission = table.Column<int>(nullable: false),
                    TaxRateOnCommission = table.Column<float>(nullable: false),
                    TotalCost = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseOrderStocks", x => x.PurchaseStockID);
                    table.ForeignKey(
                        name: "FK_PurchaseOrderStocks_Partners_PartnerID",
                        column: x => x.PartnerID,
                        principalTable: "Partners",
                        principalColumn: "PartnerID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PurchaseOrderStocks_PurchaseOrders_PurchaseID",
                        column: x => x.PurchaseID,
                        principalTable: "PurchaseOrders",
                        principalColumn: "PurchaseID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SellOrders",
                columns: table => new
                {
                    SellID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    Date = table.Column<DateTime>(nullable: true),
                    EmployeeID = table.Column<int>(nullable: false),
                    PortfolioID = table.Column<int>(nullable: false),
                    PurchaseWay = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SellOrders", x => x.SellID);
                    table.ForeignKey(
                        name: "FK_SellOrders_Employees_EmployeeID",
                        column: x => x.EmployeeID,
                        principalTable: "Employees",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SellOrders_Portfolios_PortfolioID",
                        column: x => x.PortfolioID,
                        principalTable: "Portfolios",
                        principalColumn: "PortfolioID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SellOrderStocks",
                columns: table => new
                {
                    SellStockID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BankCommittion = table.Column<decimal>(nullable: false),
                    BankCommittionRate = table.Column<float>(nullable: false),
                    PartnerID = table.Column<int>(nullable: false),
                    PurchaseValue = table.Column<decimal>(nullable: false),
                    SellID = table.Column<int>(nullable: false),
                    StockCount = table.Column<int>(nullable: false),
                    StockPrice = table.Column<decimal>(nullable: false),
                    TaxOnCommission = table.Column<int>(nullable: false),
                    TaxRateOnCommission = table.Column<float>(nullable: false),
                    TotalCost = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SellOrderStocks", x => x.SellStockID);
                    table.ForeignKey(
                        name: "FK_SellOrderStocks_Partners_PartnerID",
                        column: x => x.PartnerID,
                        principalTable: "Partners",
                        principalColumn: "PartnerID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SellOrderStocks_SellOrders_SellID",
                        column: x => x.SellID,
                        principalTable: "SellOrders",
                        principalColumn: "SellID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptExchanges_AccountID",
                table: "ReceiptExchanges",
                column: "AccountID");

            migrationBuilder.CreateIndex(
                name: "IX_NoticeDetails_PartnerID",
                table: "NoticeDetails",
                column: "PartnerID");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderStocks_PartnerID",
                table: "PurchaseOrderStocks",
                column: "PartnerID");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderStocks_PurchaseID",
                table: "PurchaseOrderStocks",
                column: "PurchaseID");

            migrationBuilder.CreateIndex(
                name: "IX_SellOrders_EmployeeID",
                table: "SellOrders",
                column: "EmployeeID");

            migrationBuilder.CreateIndex(
                name: "IX_SellOrders_PortfolioID",
                table: "SellOrders",
                column: "PortfolioID");

            migrationBuilder.CreateIndex(
                name: "IX_SellOrderStocks_PartnerID",
                table: "SellOrderStocks",
                column: "PartnerID");

            migrationBuilder.CreateIndex(
                name: "IX_SellOrderStocks_SellID",
                table: "SellOrderStocks",
                column: "SellID");

            migrationBuilder.AddForeignKey(
                name: "FK_NoticeDetails_Partners_PartnerID",
                table: "NoticeDetails",
                column: "PartnerID",
                principalTable: "Partners",
                principalColumn: "PartnerID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Portfolioshareholders_Partners_PartnerID",
                table: "Portfolioshareholders",
                column: "PartnerID",
                principalTable: "Partners",
                principalColumn: "PartnerID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Portfolioshareholders_Portfolios_PortfolioID",
                table: "Portfolioshareholders",
                column: "PortfolioID",
                principalTable: "Portfolios",
                principalColumn: "PortfolioID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ReceiptExchanges_Accounts_AccountID",
                table: "ReceiptExchanges",
                column: "AccountID",
                principalTable: "Accounts",
                principalColumn: "AccountID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ReceiptExchanges_Currency_CurrencyID",
                table: "ReceiptExchanges",
                column: "CurrencyID",
                principalTable: "Currency",
                principalColumn: "CurrencyID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

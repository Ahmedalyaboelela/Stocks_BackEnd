using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class addResetTablesAsSettingTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "Countries",
                type: "nvarchar(100)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Currency",
                columns: table => new
                {
                    CurrencyID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    NameAR = table.Column<string>(type: "nvarchar(150)", nullable: false),
                    NameEN = table.Column<string>(type: "nvarchar(150)", nullable: true),
                    PartValue = table.Column<float>(nullable: false),
                    PartName = table.Column<string>(type: "nvarchar(150)", nullable: true),
                    CurrencyValue = table.Column<float>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Currency", x => x.CurrencyID);
                });

            migrationBuilder.CreateTable(
                name: "Entries",
                columns: table => new
                {
                    EntryID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    Date = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Entries", x => x.EntryID);
                });

            migrationBuilder.CreateTable(
                name: "EntryDetails",
                columns: table => new
                {
                    EntryDetailID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Debit = table.Column<decimal>(nullable: false),
                    Credit = table.Column<decimal>(nullable: false),
                    AccountID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntryDetails", x => x.EntryDetailID);
                    table.ForeignKey(
                        name: "FK_EntryDetails_Accounts_AccountID",
                        column: x => x.AccountID,
                        principalTable: "Accounts",
                        principalColumn: "AccountID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Notices",
                columns: table => new
                {
                    NoticeID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    NoticeDate = table.Column<DateTime>(nullable: true),
                    Type = table.Column<bool>(nullable: false),
                    PortfolioID = table.Column<int>(nullable: false),
                    EmployeeID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notices", x => x.NoticeID);
                    table.ForeignKey(
                        name: "FK_Notices_Employees_EmployeeID",
                        column: x => x.EmployeeID,
                        principalTable: "Employees",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Notices_Portfolios_PortfolioID",
                        column: x => x.PortfolioID,
                        principalTable: "Portfolios",
                        principalColumn: "PortfolioID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseOrders",
                columns: table => new
                {
                    PurchaseID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    PurchaseWay = table.Column<int>(nullable: false),
                    Date = table.Column<DateTime>(nullable: true),
                    EmployeeID = table.Column<int>(nullable: false),
                    PortfolioID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseOrders", x => x.PurchaseID);
                    table.ForeignKey(
                        name: "FK_PurchaseOrders_Employees_EmployeeID",
                        column: x => x.EmployeeID,
                        principalTable: "Employees",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PurchaseOrders_Portfolios_PortfolioID",
                        column: x => x.PortfolioID,
                        principalTable: "Portfolios",
                        principalColumn: "PortfolioID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReceiptExchangeDetails",
                columns: table => new
                {
                    ReceiptExchangeID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ReceiptExchangeAmount = table.Column<decimal>(nullable: false),
                    AccountID = table.Column<int>(nullable: false),
                    ChiqueNumber = table.Column<int>(nullable: false),
                    Type = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReceiptExchangeDetails", x => x.ReceiptExchangeID);
                    table.ForeignKey(
                        name: "FK_ReceiptExchangeDetails_Accounts_AccountID",
                        column: x => x.AccountID,
                        principalTable: "Accounts",
                        principalColumn: "AccountID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SellOrders",
                columns: table => new
                {
                    SellID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    PurchaseWay = table.Column<int>(nullable: false),
                    Date = table.Column<DateTime>(nullable: true),
                    EmployeeID = table.Column<int>(nullable: false),
                    PortfolioID = table.Column<int>(nullable: false)
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
                name: "Settings",
                columns: table => new
                {
                    SettingID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    DoNotGenerateEntry = table.Column<bool>(nullable: false),
                    GenerateEntry = table.Column<bool>(nullable: false),
                    AutoGenerateEntry = table.Column<bool>(nullable: false),
                    VoucherType = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settings", x => x.SettingID);
                });

            migrationBuilder.CreateTable(
                name: "ReceiptExchanges",
                columns: table => new
                {
                    ReceiptID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    Date = table.Column<DateTime>(nullable: true),
                    CurrencyID = table.Column<int>(nullable: false),
                    AccountID = table.Column<int>(nullable: false),
                    Type = table.Column<bool>(nullable: false),
                    ChiqueNumber = table.Column<int>(nullable: true),
                    ChiqueDate = table.Column<DateTime>(nullable: true),
                    Description = table.Column<string>(type: "nvarchar(MAX)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReceiptExchanges", x => x.ReceiptID);
                    table.ForeignKey(
                        name: "FK_ReceiptExchanges_Accounts_AccountID",
                        column: x => x.AccountID,
                        principalTable: "Accounts",
                        principalColumn: "AccountID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReceiptExchanges_Currency_CurrencyID",
                        column: x => x.CurrencyID,
                        principalTable: "Currency",
                        principalColumn: "CurrencyID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NoticeDetails",
                columns: table => new
                {
                    NoticeCredDepID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    NoticeID = table.Column<int>(nullable: false),
                    CreditDebitMoney = table.Column<decimal>(nullable: false),
                    CreditorDebitStocks = table.Column<float>(nullable: false),
                    PartnerID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NoticeDetails", x => x.NoticeCredDepID);
                    table.ForeignKey(
                        name: "FK_NoticeDetails_Notices_NoticeID",
                        column: x => x.NoticeID,
                        principalTable: "Notices",
                        principalColumn: "NoticeID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NoticeDetails_Partners_PartnerID",
                        column: x => x.PartnerID,
                        principalTable: "Partners",
                        principalColumn: "PartnerID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseOrderStocks",
                columns: table => new
                {
                    PurchaseStockID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PurchaseID = table.Column<int>(nullable: false),
                    BankCommittion = table.Column<decimal>(nullable: false),
                    BankCommittionRate = table.Column<float>(nullable: false),
                    PartnerID = table.Column<int>(nullable: false),
                    PurchaseValue = table.Column<decimal>(nullable: false),
                    StockPrice = table.Column<decimal>(nullable: false),
                    StockCount = table.Column<int>(nullable: false),
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
                name: "SellOrderStocks",
                columns: table => new
                {
                    SellStockID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    SellID = table.Column<int>(nullable: false),
                    BankCommittion = table.Column<decimal>(nullable: false),
                    BankCommittionRate = table.Column<float>(nullable: false),
                    PartnerID = table.Column<int>(nullable: false),
                    PurchaseValue = table.Column<decimal>(nullable: false),
                    StockPrice = table.Column<decimal>(nullable: false),
                    StockCount = table.Column<int>(nullable: false),
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

            migrationBuilder.CreateTable(
                name: "SettingAccounts",
                columns: table => new
                {
                    SettingAccountID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    SettingID = table.Column<int>(nullable: false),
                    AccountID = table.Column<int>(nullable: false),
                    AccountType = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SettingAccounts", x => x.SettingAccountID);
                    table.ForeignKey(
                        name: "FK_SettingAccounts_Accounts_AccountID",
                        column: x => x.AccountID,
                        principalTable: "Accounts",
                        principalColumn: "AccountID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SettingAccounts_Settings_SettingID",
                        column: x => x.SettingID,
                        principalTable: "Settings",
                        principalColumn: "SettingID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EntryDetails_AccountID",
                table: "EntryDetails",
                column: "AccountID");

            migrationBuilder.CreateIndex(
                name: "IX_NoticeDetails_NoticeID",
                table: "NoticeDetails",
                column: "NoticeID");

            migrationBuilder.CreateIndex(
                name: "IX_NoticeDetails_PartnerID",
                table: "NoticeDetails",
                column: "PartnerID");

            migrationBuilder.CreateIndex(
                name: "IX_Notices_EmployeeID",
                table: "Notices",
                column: "EmployeeID");

            migrationBuilder.CreateIndex(
                name: "IX_Notices_PortfolioID",
                table: "Notices",
                column: "PortfolioID");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrders_EmployeeID",
                table: "PurchaseOrders",
                column: "EmployeeID");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrders_PortfolioID",
                table: "PurchaseOrders",
                column: "PortfolioID");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderStocks_PartnerID",
                table: "PurchaseOrderStocks",
                column: "PartnerID");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderStocks_PurchaseID",
                table: "PurchaseOrderStocks",
                column: "PurchaseID");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptExchangeDetails_AccountID",
                table: "ReceiptExchangeDetails",
                column: "AccountID");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptExchanges_AccountID",
                table: "ReceiptExchanges",
                column: "AccountID");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptExchanges_CurrencyID",
                table: "ReceiptExchanges",
                column: "CurrencyID");

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

            migrationBuilder.CreateIndex(
                name: "IX_SettingAccounts_AccountID",
                table: "SettingAccounts",
                column: "AccountID");

            migrationBuilder.CreateIndex(
                name: "IX_SettingAccounts_SettingID",
                table: "SettingAccounts",
                column: "SettingID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Entries");

            migrationBuilder.DropTable(
                name: "EntryDetails");

            migrationBuilder.DropTable(
                name: "NoticeDetails");

            migrationBuilder.DropTable(
                name: "PurchaseOrderStocks");

            migrationBuilder.DropTable(
                name: "ReceiptExchangeDetails");

            migrationBuilder.DropTable(
                name: "ReceiptExchanges");

            migrationBuilder.DropTable(
                name: "SellOrderStocks");

            migrationBuilder.DropTable(
                name: "SettingAccounts");

            migrationBuilder.DropTable(
                name: "Notices");

            migrationBuilder.DropTable(
                name: "PurchaseOrders");

            migrationBuilder.DropTable(
                name: "Currency");

            migrationBuilder.DropTable(
                name: "SellOrders");

            migrationBuilder.DropTable(
                name: "Settings");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "Countries");
        }
    }
}

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
                name: "EntryDetail",
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
                    table.PrimaryKey("PK_EntryDetail", x => x.EntryDetailID);
                    table.ForeignKey(
                        name: "FK_EntryDetail_Accounts_AccountID",
                        column: x => x.AccountID,
                        principalTable: "Accounts",
                        principalColumn: "AccountID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Notice",
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
                    table.PrimaryKey("PK_Notice", x => x.NoticeID);
                    table.ForeignKey(
                        name: "FK_Notice_Employees_EmployeeID",
                        column: x => x.EmployeeID,
                        principalTable: "Employees",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Notice_Portfolios_PortfolioID",
                        column: x => x.PortfolioID,
                        principalTable: "Portfolios",
                        principalColumn: "PortfolioID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseOrder",
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
                    table.PrimaryKey("PK_PurchaseOrder", x => x.PurchaseID);
                    table.ForeignKey(
                        name: "FK_PurchaseOrder_Employees_EmployeeID",
                        column: x => x.EmployeeID,
                        principalTable: "Employees",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PurchaseOrder_Portfolios_PortfolioID",
                        column: x => x.PortfolioID,
                        principalTable: "Portfolios",
                        principalColumn: "PortfolioID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SellOrder",
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
                    table.PrimaryKey("PK_SellOrder", x => x.SellID);
                    table.ForeignKey(
                        name: "FK_SellOrder_Employees_EmployeeID",
                        column: x => x.EmployeeID,
                        principalTable: "Employees",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SellOrder_Portfolios_PortfolioID",
                        column: x => x.PortfolioID,
                        principalTable: "Portfolios",
                        principalColumn: "PortfolioID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Setting",
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
                    table.PrimaryKey("PK_Setting", x => x.SettingID);
                });

            migrationBuilder.CreateTable(
                name: "ReceiptExchange",
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
                    table.PrimaryKey("PK_ReceiptExchange", x => x.ReceiptID);
                    table.ForeignKey(
                        name: "FK_ReceiptExchange_Accounts_AccountID",
                        column: x => x.AccountID,
                        principalTable: "Accounts",
                        principalColumn: "AccountID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReceiptExchange_Currency_CurrencyID",
                        column: x => x.CurrencyID,
                        principalTable: "Currency",
                        principalColumn: "CurrencyID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NoticeCreditorDeptor",
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
                    table.PrimaryKey("PK_NoticeCreditorDeptor", x => x.NoticeCredDepID);
                    table.ForeignKey(
                        name: "FK_NoticeCreditorDeptor_Notice_NoticeID",
                        column: x => x.NoticeID,
                        principalTable: "Notice",
                        principalColumn: "NoticeID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NoticeCreditorDeptor_Partners_PartnerID",
                        column: x => x.PartnerID,
                        principalTable: "Partners",
                        principalColumn: "PartnerID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseOrderStock",
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
                    table.PrimaryKey("PK_PurchaseOrderStock", x => x.PurchaseStockID);
                    table.ForeignKey(
                        name: "FK_PurchaseOrderStock_Partners_PartnerID",
                        column: x => x.PartnerID,
                        principalTable: "Partners",
                        principalColumn: "PartnerID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PurchaseOrderStock_PurchaseOrder_PurchaseID",
                        column: x => x.PurchaseID,
                        principalTable: "PurchaseOrder",
                        principalColumn: "PurchaseID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SellOrderStock",
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
                    table.PrimaryKey("PK_SellOrderStock", x => x.SellStockID);
                    table.ForeignKey(
                        name: "FK_SellOrderStock_Partners_PartnerID",
                        column: x => x.PartnerID,
                        principalTable: "Partners",
                        principalColumn: "PartnerID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SellOrderStock_SellOrder_SellID",
                        column: x => x.SellID,
                        principalTable: "SellOrder",
                        principalColumn: "SellID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SettingAccount",
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
                    table.PrimaryKey("PK_SettingAccount", x => x.SettingAccountID);
                    table.ForeignKey(
                        name: "FK_SettingAccount_Accounts_AccountID",
                        column: x => x.AccountID,
                        principalTable: "Accounts",
                        principalColumn: "AccountID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SettingAccount_Setting_SettingID",
                        column: x => x.SettingID,
                        principalTable: "Setting",
                        principalColumn: "SettingID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Entry",
                columns: table => new
                {
                    EntryID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    Date = table.Column<DateTime>(nullable: true),
                    SellID = table.Column<int>(nullable: false),
                    PurchaseID = table.Column<int>(nullable: false),
                    ReceiptExchangeID = table.Column<int>(nullable: false),
                    NoticeID = table.Column<int>(nullable: false),
                    CurrencyID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Entry", x => x.EntryID);
                    table.ForeignKey(
                        name: "FK_Entry_Currency_CurrencyID",
                        column: x => x.CurrencyID,
                        principalTable: "Currency",
                        principalColumn: "CurrencyID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Entry_Notice_NoticeID",
                        column: x => x.NoticeID,
                        principalTable: "Notice",
                        principalColumn: "NoticeID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Entry_PurchaseOrder_PurchaseID",
                        column: x => x.PurchaseID,
                        principalTable: "PurchaseOrder",
                        principalColumn: "PurchaseID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Entry_ReceiptExchange_ReceiptExchangeID",
                        column: x => x.ReceiptExchangeID,
                        principalTable: "ReceiptExchange",
                        principalColumn: "ReceiptID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Entry_SellOrder_SellID",
                        column: x => x.SellID,
                        principalTable: "SellOrder",
                        principalColumn: "SellID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReceiptExchangeDetail",
                columns: table => new
                {
                    ReceiptExchangeID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ReceiptID = table.Column<int>(nullable: false),
                    ReceiptExchangeAmount = table.Column<decimal>(nullable: false),
                    AccountID = table.Column<int>(nullable: false),
                    ChiqueNumber = table.Column<int>(nullable: false),
                    Type = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReceiptExchangeDetail", x => x.ReceiptExchangeID);
                    table.ForeignKey(
                        name: "FK_ReceiptExchangeDetail_Accounts_AccountID",
                        column: x => x.AccountID,
                        principalTable: "Accounts",
                        principalColumn: "AccountID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReceiptExchangeDetail_ReceiptExchange_ReceiptID",
                        column: x => x.ReceiptID,
                        principalTable: "ReceiptExchange",
                        principalColumn: "ReceiptID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Entry_CurrencyID",
                table: "Entry",
                column: "CurrencyID");

            migrationBuilder.CreateIndex(
                name: "IX_Entry_NoticeID",
                table: "Entry",
                column: "NoticeID");

            migrationBuilder.CreateIndex(
                name: "IX_Entry_PurchaseID",
                table: "Entry",
                column: "PurchaseID");

            migrationBuilder.CreateIndex(
                name: "IX_Entry_ReceiptExchangeID",
                table: "Entry",
                column: "ReceiptExchangeID");

            migrationBuilder.CreateIndex(
                name: "IX_Entry_SellID",
                table: "Entry",
                column: "SellID");

            migrationBuilder.CreateIndex(
                name: "IX_EntryDetail_AccountID",
                table: "EntryDetail",
                column: "AccountID");

            migrationBuilder.CreateIndex(
                name: "IX_Notice_EmployeeID",
                table: "Notice",
                column: "EmployeeID");

            migrationBuilder.CreateIndex(
                name: "IX_Notice_PortfolioID",
                table: "Notice",
                column: "PortfolioID");

            migrationBuilder.CreateIndex(
                name: "IX_NoticeCreditorDeptor_NoticeID",
                table: "NoticeCreditorDeptor",
                column: "NoticeID");

            migrationBuilder.CreateIndex(
                name: "IX_NoticeCreditorDeptor_PartnerID",
                table: "NoticeCreditorDeptor",
                column: "PartnerID");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrder_EmployeeID",
                table: "PurchaseOrder",
                column: "EmployeeID");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrder_PortfolioID",
                table: "PurchaseOrder",
                column: "PortfolioID");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderStock_PartnerID",
                table: "PurchaseOrderStock",
                column: "PartnerID");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderStock_PurchaseID",
                table: "PurchaseOrderStock",
                column: "PurchaseID");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptExchange_AccountID",
                table: "ReceiptExchange",
                column: "AccountID");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptExchange_CurrencyID",
                table: "ReceiptExchange",
                column: "CurrencyID");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptExchangeDetail_AccountID",
                table: "ReceiptExchangeDetail",
                column: "AccountID");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptExchangeDetail_ReceiptID",
                table: "ReceiptExchangeDetail",
                column: "ReceiptID");

            migrationBuilder.CreateIndex(
                name: "IX_SellOrder_EmployeeID",
                table: "SellOrder",
                column: "EmployeeID");

            migrationBuilder.CreateIndex(
                name: "IX_SellOrder_PortfolioID",
                table: "SellOrder",
                column: "PortfolioID");

            migrationBuilder.CreateIndex(
                name: "IX_SellOrderStock_PartnerID",
                table: "SellOrderStock",
                column: "PartnerID");

            migrationBuilder.CreateIndex(
                name: "IX_SellOrderStock_SellID",
                table: "SellOrderStock",
                column: "SellID");

            migrationBuilder.CreateIndex(
                name: "IX_SettingAccount_AccountID",
                table: "SettingAccount",
                column: "AccountID");

            migrationBuilder.CreateIndex(
                name: "IX_SettingAccount_SettingID",
                table: "SettingAccount",
                column: "SettingID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Entry");

            migrationBuilder.DropTable(
                name: "EntryDetail");

            migrationBuilder.DropTable(
                name: "NoticeCreditorDeptor");

            migrationBuilder.DropTable(
                name: "PurchaseOrderStock");

            migrationBuilder.DropTable(
                name: "ReceiptExchangeDetail");

            migrationBuilder.DropTable(
                name: "SellOrderStock");

            migrationBuilder.DropTable(
                name: "SettingAccount");

            migrationBuilder.DropTable(
                name: "Notice");

            migrationBuilder.DropTable(
                name: "PurchaseOrder");

            migrationBuilder.DropTable(
                name: "ReceiptExchange");

            migrationBuilder.DropTable(
                name: "SellOrder");

            migrationBuilder.DropTable(
                name: "Setting");

            migrationBuilder.DropTable(
                name: "Currency");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "Countries");
        }
    }
}

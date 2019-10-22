using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class AddNewSellOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Entries_SellingOrders_SellingOrderID",
                table: "Entries");

            migrationBuilder.DropForeignKey(
                name: "FK_SellingOrders_Employees_EmployeeID",
                table: "SellingOrders");

            migrationBuilder.DropIndex(
                name: "IX_SellingOrders_EmployeeID",
                table: "SellingOrders");

            migrationBuilder.DropIndex(
                name: "IX_Entries_SellingOrderID",
                table: "Entries");

            migrationBuilder.DropColumn(
                name: "Date",
                table: "SellingOrders");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "SellingOrders");

            migrationBuilder.DropColumn(
                name: "EmployeeID",
                table: "SellingOrders");

            migrationBuilder.DropColumn(
                name: "PayWay",
                table: "SellingOrders");

            migrationBuilder.DropColumn(
                name: "BankCommission",
                table: "SellingOrderDetails");

            migrationBuilder.DropColumn(
                name: "BankCommissionRate",
                table: "SellingOrderDetails");

            migrationBuilder.DropColumn(
                name: "NetAmmount",
                table: "SellingOrderDetails");

            migrationBuilder.DropColumn(
                name: "SelingValue",
                table: "SellingOrderDetails");

            migrationBuilder.DropColumn(
                name: "SellingPrice",
                table: "SellingOrderDetails");

            migrationBuilder.DropColumn(
                name: "TaxOnCommission",
                table: "SellingOrderDetails");

            migrationBuilder.DropColumn(
                name: "TaxRateOnCommission",
                table: "SellingOrderDetails");

            migrationBuilder.RenameColumn(
                name: "SellingOrderID",
                table: "Entries",
                newName: "SellingInvoiceID");

            migrationBuilder.AddColumn<DateTime>(
                name: "OrderDate",
                table: "SellingOrders",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "OrderType",
                table: "SellingOrders",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "PriceType",
                table: "SellingOrderDetails",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "SellingInvoices",
                columns: table => new
                {
                    SellingInvoiceID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    Date = table.Column<DateTime>(nullable: true),
                    Description = table.Column<string>(type: "nvarchar(MAX)", nullable: true),
                    EmployeeID = table.Column<int>(nullable: false),
                    SellingOrderID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SellingInvoices", x => x.SellingInvoiceID);
                    table.ForeignKey(
                        name: "FK_SellingInvoices_Employees_EmployeeID",
                        column: x => x.EmployeeID,
                        principalTable: "Employees",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SellingInvoices_SellingOrders_SellingOrderID",
                        column: x => x.SellingOrderID,
                        principalTable: "SellingOrders",
                        principalColumn: "SellingOrderID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SellingInvoiceDetails",
                columns: table => new
                {
                    SellInvoiceDetailID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    StockCount = table.Column<float>(nullable: false),
                    SellingPrice = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    SelingValue = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    BankCommissionRate = table.Column<float>(nullable: false),
                    BankCommission = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    TaxRateOnCommission = table.Column<float>(nullable: false),
                    TaxOnCommission = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    NetAmmount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    SellingInvoiceID = table.Column<int>(nullable: false),
                    PartnerID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SellingInvoiceDetails", x => x.SellInvoiceDetailID);
                    table.ForeignKey(
                        name: "FK_SellingInvoiceDetails_Partners_PartnerID",
                        column: x => x.PartnerID,
                        principalTable: "Partners",
                        principalColumn: "PartnerID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SellingInvoiceDetails_SellingInvoices_SellingInvoiceID",
                        column: x => x.SellingInvoiceID,
                        principalTable: "SellingInvoices",
                        principalColumn: "SellingInvoiceID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Entries_SellingInvoiceID",
                table: "Entries",
                column: "SellingInvoiceID",
                unique: true,
                filter: "[SellingInvoiceID] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_SellingInvoiceDetails_PartnerID",
                table: "SellingInvoiceDetails",
                column: "PartnerID");

            migrationBuilder.CreateIndex(
                name: "IX_SellingInvoiceDetails_SellingInvoiceID",
                table: "SellingInvoiceDetails",
                column: "SellingInvoiceID");

            migrationBuilder.CreateIndex(
                name: "IX_SellingInvoices_EmployeeID",
                table: "SellingInvoices",
                column: "EmployeeID");

            migrationBuilder.CreateIndex(
                name: "IX_SellingInvoices_SellingOrderID",
                table: "SellingInvoices",
                column: "SellingOrderID");

            migrationBuilder.AddForeignKey(
                name: "FK_Entries_SellingInvoices_SellingInvoiceID",
                table: "Entries",
                column: "SellingInvoiceID",
                principalTable: "SellingInvoices",
                principalColumn: "SellingInvoiceID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Entries_SellingInvoices_SellingInvoiceID",
                table: "Entries");

            migrationBuilder.DropTable(
                name: "SellingInvoiceDetails");

            migrationBuilder.DropTable(
                name: "SellingInvoices");

            migrationBuilder.DropIndex(
                name: "IX_Entries_SellingInvoiceID",
                table: "Entries");

            migrationBuilder.DropColumn(
                name: "OrderDate",
                table: "SellingOrders");

            migrationBuilder.DropColumn(
                name: "OrderType",
                table: "SellingOrders");

            migrationBuilder.DropColumn(
                name: "PriceType",
                table: "SellingOrderDetails");

            migrationBuilder.RenameColumn(
                name: "SellingInvoiceID",
                table: "Entries",
                newName: "SellingOrderID");

            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "SellingOrders",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "SellingOrders",
                type: "nvarchar(MAX)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EmployeeID",
                table: "SellingOrders",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PayWay",
                table: "SellingOrders",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "BankCommission",
                table: "SellingOrderDetails",
                type: "decimal(10,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<float>(
                name: "BankCommissionRate",
                table: "SellingOrderDetails",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<decimal>(
                name: "NetAmmount",
                table: "SellingOrderDetails",
                type: "decimal(10,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "SelingValue",
                table: "SellingOrderDetails",
                type: "decimal(10,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "SellingPrice",
                table: "SellingOrderDetails",
                type: "decimal(10,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TaxOnCommission",
                table: "SellingOrderDetails",
                type: "decimal(10,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<float>(
                name: "TaxRateOnCommission",
                table: "SellingOrderDetails",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.CreateIndex(
                name: "IX_SellingOrders_EmployeeID",
                table: "SellingOrders",
                column: "EmployeeID");

            migrationBuilder.CreateIndex(
                name: "IX_Entries_SellingOrderID",
                table: "Entries",
                column: "SellingOrderID",
                unique: true,
                filter: "[SellingOrderID] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Entries_SellingOrders_SellingOrderID",
                table: "Entries",
                column: "SellingOrderID",
                principalTable: "SellingOrders",
                principalColumn: "SellingOrderID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SellingOrders_Employees_EmployeeID",
                table: "SellingOrders",
                column: "EmployeeID",
                principalTable: "Employees",
                principalColumn: "EmployeeID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

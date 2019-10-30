using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class AddNewPurchaseInvoice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Entries_PurchaseOrders_PurchaseOrderID",
                table: "Entries");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseOrderDetails_PurchaseOrders_PurchaseID",
                table: "PurchaseOrderDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseOrders_Employees_EmployeeID",
                table: "PurchaseOrders");

            migrationBuilder.DropIndex(
                name: "IX_Entries_PurchaseOrderID",
                table: "Entries");

            migrationBuilder.DropColumn(
                name: "Date",
                table: "PurchaseOrders");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "PurchaseOrders");

            migrationBuilder.DropColumn(
                name: "PayWay",
                table: "PurchaseOrders");

            migrationBuilder.DropColumn(
                name: "BankCommission",
                table: "PurchaseOrderDetails");

            migrationBuilder.DropColumn(
                name: "BankCommissionRate",
                table: "PurchaseOrderDetails");

            migrationBuilder.DropColumn(
                name: "NetAmmount",
                table: "PurchaseOrderDetails");

            migrationBuilder.DropColumn(
                name: "PurchasePrice",
                table: "PurchaseOrderDetails");

            migrationBuilder.DropColumn(
                name: "PurchaseValue",
                table: "PurchaseOrderDetails");

            migrationBuilder.DropColumn(
                name: "TaxOnCommission",
                table: "PurchaseOrderDetails");

            migrationBuilder.DropColumn(
                name: "TaxRateOnCommission",
                table: "PurchaseOrderDetails");

            migrationBuilder.RenameColumn(
                name: "PurchaseID",
                table: "PurchaseOrderDetails",
                newName: "PurchaseOrderID");

            migrationBuilder.RenameIndex(
                name: "IX_PurchaseOrderDetails_PurchaseID",
                table: "PurchaseOrderDetails",
                newName: "IX_PurchaseOrderDetails_PurchaseOrderID");

            migrationBuilder.RenameColumn(
                name: "PurchaseOrderID",
                table: "Entries",
                newName: "PurchaseInvoiceID");

            migrationBuilder.AddColumn<int>(
                name: "PurchaseOrderID",
                table: "SellingOrderDetails",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "EmployeeID",
                table: "PurchaseOrders",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<DateTime>(
                name: "OrderDate",
                table: "PurchaseOrders",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "OrderType",
                table: "PurchaseOrders",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "PriceType",
                table: "PurchaseOrderDetails",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "PurchaseInvoices",
                columns: table => new
                {
                    PurchaseInvoiceID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    Date = table.Column<DateTime>(nullable: true),
                    Description = table.Column<string>(type: "nvarchar(MAX)", nullable: true),
                    EmployeeID = table.Column<int>(nullable: false),
                    PurchaseOrderID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseInvoices", x => x.PurchaseInvoiceID);
                    table.ForeignKey(
                        name: "FK_PurchaseInvoices_Employees_EmployeeID",
                        column: x => x.EmployeeID,
                        principalTable: "Employees",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PurchaseInvoices_PurchaseOrders_PurchaseOrderID",
                        column: x => x.PurchaseOrderID,
                        principalTable: "PurchaseOrders",
                        principalColumn: "PurchaseOrderID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseInvoiceDetails",
                columns: table => new
                {
                    PurchaseInvoiceDetailID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    StockCount = table.Column<float>(nullable: false),
                    PurchasePrice = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    PurchaseValue = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    BankCommissionRate = table.Column<float>(nullable: false),
                    BankCommission = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    TaxRateOnCommission = table.Column<float>(nullable: false),
                    TaxOnCommission = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    NetAmmount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    PurchaseInvoiceID = table.Column<int>(nullable: false),
                    PartnerID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseInvoiceDetails", x => x.PurchaseInvoiceDetailID);
                    table.ForeignKey(
                        name: "FK_PurchaseInvoiceDetails_Partners_PartnerID",
                        column: x => x.PartnerID,
                        principalTable: "Partners",
                        principalColumn: "PartnerID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PurchaseInvoiceDetails_PurchaseInvoices_PurchaseInvoiceID",
                        column: x => x.PurchaseInvoiceID,
                        principalTable: "PurchaseInvoices",
                        principalColumn: "PurchaseInvoiceID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SellingOrderDetails_PurchaseOrderID",
                table: "SellingOrderDetails",
                column: "PurchaseOrderID");

            migrationBuilder.CreateIndex(
                name: "IX_Entries_PurchaseInvoiceID",
                table: "Entries",
                column: "PurchaseInvoiceID",
                unique: true,
                filter: "[PurchaseInvoiceID] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseInvoiceDetails_PartnerID",
                table: "PurchaseInvoiceDetails",
                column: "PartnerID");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseInvoiceDetails_PurchaseInvoiceID",
                table: "PurchaseInvoiceDetails",
                column: "PurchaseInvoiceID");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseInvoices_EmployeeID",
                table: "PurchaseInvoices",
                column: "EmployeeID");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseInvoices_PurchaseOrderID",
                table: "PurchaseInvoices",
                column: "PurchaseOrderID");

            migrationBuilder.AddForeignKey(
                name: "FK_Entries_PurchaseInvoices_PurchaseInvoiceID",
                table: "Entries",
                column: "PurchaseInvoiceID",
                principalTable: "PurchaseInvoices",
                principalColumn: "PurchaseInvoiceID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseOrderDetails_PurchaseOrders_PurchaseOrderID",
                table: "PurchaseOrderDetails",
                column: "PurchaseOrderID",
                principalTable: "PurchaseOrders",
                principalColumn: "PurchaseOrderID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseOrders_Employees_EmployeeID",
                table: "PurchaseOrders",
                column: "EmployeeID",
                principalTable: "Employees",
                principalColumn: "EmployeeID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SellingOrderDetails_PurchaseOrders_PurchaseOrderID",
                table: "SellingOrderDetails",
                column: "PurchaseOrderID",
                principalTable: "PurchaseOrders",
                principalColumn: "PurchaseOrderID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Entries_PurchaseInvoices_PurchaseInvoiceID",
                table: "Entries");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseOrderDetails_PurchaseOrders_PurchaseOrderID",
                table: "PurchaseOrderDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseOrders_Employees_EmployeeID",
                table: "PurchaseOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_SellingOrderDetails_PurchaseOrders_PurchaseOrderID",
                table: "SellingOrderDetails");

            migrationBuilder.DropTable(
                name: "PurchaseInvoiceDetails");

            migrationBuilder.DropTable(
                name: "PurchaseInvoices");

            migrationBuilder.DropIndex(
                name: "IX_SellingOrderDetails_PurchaseOrderID",
                table: "SellingOrderDetails");

            migrationBuilder.DropIndex(
                name: "IX_Entries_PurchaseInvoiceID",
                table: "Entries");

            migrationBuilder.DropColumn(
                name: "PurchaseOrderID",
                table: "SellingOrderDetails");

            migrationBuilder.DropColumn(
                name: "OrderDate",
                table: "PurchaseOrders");

            migrationBuilder.DropColumn(
                name: "OrderType",
                table: "PurchaseOrders");

            migrationBuilder.DropColumn(
                name: "PriceType",
                table: "PurchaseOrderDetails");

            migrationBuilder.RenameColumn(
                name: "PurchaseOrderID",
                table: "PurchaseOrderDetails",
                newName: "PurchaseID");

            migrationBuilder.RenameIndex(
                name: "IX_PurchaseOrderDetails_PurchaseOrderID",
                table: "PurchaseOrderDetails",
                newName: "IX_PurchaseOrderDetails_PurchaseID");

            migrationBuilder.RenameColumn(
                name: "PurchaseInvoiceID",
                table: "Entries",
                newName: "PurchaseOrderID");

            migrationBuilder.AlterColumn<int>(
                name: "EmployeeID",
                table: "PurchaseOrders",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "PurchaseOrders",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "PurchaseOrders",
                type: "nvarchar(MAX)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PayWay",
                table: "PurchaseOrders",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "BankCommission",
                table: "PurchaseOrderDetails",
                type: "decimal(10,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<float>(
                name: "BankCommissionRate",
                table: "PurchaseOrderDetails",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<decimal>(
                name: "NetAmmount",
                table: "PurchaseOrderDetails",
                type: "decimal(10,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PurchasePrice",
                table: "PurchaseOrderDetails",
                type: "decimal(10,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PurchaseValue",
                table: "PurchaseOrderDetails",
                type: "decimal(10,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TaxOnCommission",
                table: "PurchaseOrderDetails",
                type: "decimal(10,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<float>(
                name: "TaxRateOnCommission",
                table: "PurchaseOrderDetails",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.CreateIndex(
                name: "IX_Entries_PurchaseOrderID",
                table: "Entries",
                column: "PurchaseOrderID",
                unique: true,
                filter: "[PurchaseOrderID] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Entries_PurchaseOrders_PurchaseOrderID",
                table: "Entries",
                column: "PurchaseOrderID",
                principalTable: "PurchaseOrders",
                principalColumn: "PurchaseOrderID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseOrderDetails_PurchaseOrders_PurchaseID",
                table: "PurchaseOrderDetails",
                column: "PurchaseID",
                principalTable: "PurchaseOrders",
                principalColumn: "PurchaseOrderID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseOrders_Employees_EmployeeID",
                table: "PurchaseOrders",
                column: "EmployeeID",
                principalTable: "Employees",
                principalColumn: "EmployeeID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

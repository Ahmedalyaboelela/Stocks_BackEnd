using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class SellAndPurchaseOrderModification : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FromDate",
                table: "SellingOrders",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "OrderPeriod",
                table: "SellingOrders",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrderRemarks",
                table: "SellingOrders",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ToDate",
                table: "SellingOrders",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Remarks",
                table: "SellingOrderDetails",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FromDate",
                table: "PurchaseOrders",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "OrderPeriod",
                table: "PurchaseOrders",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrderRemarks",
                table: "PurchaseOrders",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ToDate",
                table: "PurchaseOrders",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Remarks",
                table: "PurchaseOrderDetails",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FromDate",
                table: "SellingOrders");

            migrationBuilder.DropColumn(
                name: "OrderPeriod",
                table: "SellingOrders");

            migrationBuilder.DropColumn(
                name: "OrderRemarks",
                table: "SellingOrders");

            migrationBuilder.DropColumn(
                name: "ToDate",
                table: "SellingOrders");

            migrationBuilder.DropColumn(
                name: "Remarks",
                table: "SellingOrderDetails");

            migrationBuilder.DropColumn(
                name: "FromDate",
                table: "PurchaseOrders");

            migrationBuilder.DropColumn(
                name: "OrderPeriod",
                table: "PurchaseOrders");

            migrationBuilder.DropColumn(
                name: "OrderRemarks",
                table: "PurchaseOrders");

            migrationBuilder.DropColumn(
                name: "ToDate",
                table: "PurchaseOrders");

            migrationBuilder.DropColumn(
                name: "Remarks",
                table: "PurchaseOrderDetails");
        }
    }
}

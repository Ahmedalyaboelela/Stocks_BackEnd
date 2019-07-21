using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class porttransaction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OpeningStocksCount",
                table: "Portfolios");

            migrationBuilder.RenameColumn(
                name: "StocksCount",
                table: "Portfolios",
                newName: "TotalStocksCount");

            migrationBuilder.RenameColumn(
                name: "StocksCount",
                table: "PortfolioOpeningStocks",
                newName: "OpeningStocksCount");

            migrationBuilder.RenameColumn(
                name: "StockValue",
                table: "PortfolioOpeningStocks",
                newName: "OpeningStockValue");

            migrationBuilder.AddColumn<int>(
                name: "PartnerID",
                table: "SellingOrderDetails",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PartnerID",
                table: "PurchaseOrderDetails",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "Creationdate",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PortfolioTransactions",
                columns: table => new
                {
                    PortTransID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CurrentStocksCount = table.Column<float>(nullable: false),
                    CurrentStockValue = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    PortfolioID = table.Column<int>(nullable: false),
                    PartnerID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PortfolioTransactions", x => x.PortTransID);
                    table.ForeignKey(
                        name: "FK_PortfolioTransactions_Partners_PartnerID",
                        column: x => x.PartnerID,
                        principalTable: "Partners",
                        principalColumn: "PartnerID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PortfolioTransactions_Portfolios_PortfolioID",
                        column: x => x.PortfolioID,
                        principalTable: "Portfolios",
                        principalColumn: "PortfolioID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SellingOrderDetails_PartnerID",
                table: "SellingOrderDetails",
                column: "PartnerID");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderDetails_PartnerID",
                table: "PurchaseOrderDetails",
                column: "PartnerID");

            migrationBuilder.CreateIndex(
                name: "IX_PortfolioTransactions_PartnerID",
                table: "PortfolioTransactions",
                column: "PartnerID");

            migrationBuilder.CreateIndex(
                name: "IX_PortfolioTransactions_PortfolioID",
                table: "PortfolioTransactions",
                column: "PortfolioID");

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseOrderDetails_Partners_PartnerID",
                table: "PurchaseOrderDetails",
                column: "PartnerID",
                principalTable: "Partners",
                principalColumn: "PartnerID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SellingOrderDetails_Partners_PartnerID",
                table: "SellingOrderDetails",
                column: "PartnerID",
                principalTable: "Partners",
                principalColumn: "PartnerID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseOrderDetails_Partners_PartnerID",
                table: "PurchaseOrderDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_SellingOrderDetails_Partners_PartnerID",
                table: "SellingOrderDetails");

            migrationBuilder.DropTable(
                name: "PortfolioTransactions");

            migrationBuilder.DropIndex(
                name: "IX_SellingOrderDetails_PartnerID",
                table: "SellingOrderDetails");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseOrderDetails_PartnerID",
                table: "PurchaseOrderDetails");

            migrationBuilder.DropColumn(
                name: "PartnerID",
                table: "SellingOrderDetails");

            migrationBuilder.DropColumn(
                name: "PartnerID",
                table: "PurchaseOrderDetails");

            migrationBuilder.DropColumn(
                name: "Creationdate",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "TotalStocksCount",
                table: "Portfolios",
                newName: "StocksCount");

            migrationBuilder.RenameColumn(
                name: "OpeningStocksCount",
                table: "PortfolioOpeningStocks",
                newName: "StocksCount");

            migrationBuilder.RenameColumn(
                name: "OpeningStockValue",
                table: "PortfolioOpeningStocks",
                newName: "StockValue");

            migrationBuilder.AddColumn<float>(
                name: "OpeningStocksCount",
                table: "Portfolios",
                nullable: true);
        }
    }
}

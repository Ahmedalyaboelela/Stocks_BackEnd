using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class UpdatePortofolio : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PortfolioShareHolders");

            migrationBuilder.DropColumn(
                name: "PartnerCount",
                table: "Portfolios");

            migrationBuilder.DropColumn(
                name: "PortfolioCapital",
                table: "Portfolios");

            migrationBuilder.DropColumn(
                name: "StockValue",
                table: "Portfolios");

            migrationBuilder.AddColumn<float>(
                name: "OpeningStocksCount",
                table: "Portfolios",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PortfolioOpeningStocks",
                columns: table => new
                {
                    PortOPenStockID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    StocksCount = table.Column<float>(nullable: false),
                    StockValue = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    PortfolioID = table.Column<int>(nullable: false),
                    PartnerID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PortfolioOpeningStocks", x => x.PortOPenStockID);
                    table.ForeignKey(
                        name: "FK_PortfolioOpeningStocks_Partners_PartnerID",
                        column: x => x.PartnerID,
                        principalTable: "Partners",
                        principalColumn: "PartnerID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PortfolioOpeningStocks_Portfolios_PortfolioID",
                        column: x => x.PortfolioID,
                        principalTable: "Portfolios",
                        principalColumn: "PortfolioID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PortfolioOpeningStocks_PartnerID",
                table: "PortfolioOpeningStocks",
                column: "PartnerID");

            migrationBuilder.CreateIndex(
                name: "IX_PortfolioOpeningStocks_PortfolioID",
                table: "PortfolioOpeningStocks",
                column: "PortfolioID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PortfolioOpeningStocks");

            migrationBuilder.DropColumn(
                name: "OpeningStocksCount",
                table: "Portfolios");

            migrationBuilder.AddColumn<int>(
                name: "PartnerCount",
                table: "Portfolios",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PortfolioCapital",
                table: "Portfolios",
                type: "decimal(10,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "StockValue",
                table: "Portfolios",
                type: "decimal(10,2)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PortfolioShareHolders",
                columns: table => new
                {
                    PortShareID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Amount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(MAX)", nullable: true),
                    PartnerID = table.Column<int>(nullable: false),
                    Percentage = table.Column<float>(nullable: false),
                    PortfolioID = table.Column<int>(nullable: false),
                    StocksCount = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PortfolioShareHolders", x => x.PortShareID);
                    table.ForeignKey(
                        name: "FK_PortfolioShareHolders_Partners_PartnerID",
                        column: x => x.PartnerID,
                        principalTable: "Partners",
                        principalColumn: "PartnerID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PortfolioShareHolders_Portfolios_PortfolioID",
                        column: x => x.PortfolioID,
                        principalTable: "Portfolios",
                        principalColumn: "PortfolioID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PortfolioShareHolders_PartnerID",
                table: "PortfolioShareHolders",
                column: "PartnerID");

            migrationBuilder.CreateIndex(
                name: "IX_PortfolioShareHolders_PortfolioID",
                table: "PortfolioShareHolders",
                column: "PortfolioID");
        }
    }
}

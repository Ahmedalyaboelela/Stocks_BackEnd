using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class ReportSettingTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasTransaction",
                table: "PortfolioTransactions");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Accounts");

            migrationBuilder.CreateTable(
                name: "ReportSettings",
                columns: table => new
                {
                    ReportSettingID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PortfolioID = table.Column<int>(nullable: false),
                    PartnerID = table.Column<int>(nullable: false),
                    CurrentDate = table.Column<DateTime>(nullable: false),
                    DailyStockValue = table.Column<decimal>(type: "decimal(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportSettings", x => x.ReportSettingID);
                    table.ForeignKey(
                        name: "FK_ReportSettings_Partners_PartnerID",
                        column: x => x.PartnerID,
                        principalTable: "Partners",
                        principalColumn: "PartnerID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReportSettings_Portfolios_PortfolioID",
                        column: x => x.PortfolioID,
                        principalTable: "Portfolios",
                        principalColumn: "PortfolioID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReportSettings_PartnerID",
                table: "ReportSettings",
                column: "PartnerID");

            migrationBuilder.CreateIndex(
                name: "IX_ReportSettings_PortfolioID",
                table: "ReportSettings",
                column: "PortfolioID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReportSettings");

            migrationBuilder.AddColumn<bool>(
                name: "HasTransaction",
                table: "PortfolioTransactions",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Accounts",
                nullable: false,
                defaultValue: false);
        }
    }
}

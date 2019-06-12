using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class AddEMpPortofolioTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    EmployeeID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    NameAR = table.Column<string>(type: "nvarchar(150)", nullable: false),
                    NameEN = table.Column<string>(type: "nvarchar(150)", nullable: true),
                    ImagePath = table.Column<string>(type: "nvarchar(150)", nullable: true),
                    Nationality = table.Column<string>(type: "nvarchar(150)", nullable: true),
                    Religion = table.Column<int>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    IsInternal = table.Column<bool>(nullable: false),
                    Profession = table.Column<string>(type: "nvarchar(150)", nullable: true),
                    PassportProfession = table.Column<string>(type: "nvarchar(150)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(150)", nullable: true),
                    Mobile = table.Column<string>(type: "nvarchar(150)", nullable: true),
                    BirthDate = table.Column<DateTime>(nullable: true),
                    Age = table.Column<int>(nullable: false),
                    BankAccNum = table.Column<string>(type: "nvarchar(100)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.EmployeeID);
                });

            migrationBuilder.CreateTable(
                name: "Portfolios",
                columns: table => new
                {
                    PortfolioID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    NameAR = table.Column<string>(type: "nvarchar(150)", nullable: false),
                    NameEN = table.Column<string>(type: "nvarchar(150)", nullable: true),
                    EstablishDate = table.Column<DateTime>(nullable: true),
                    Description = table.Column<string>(type: "nvarchar(MAX)", nullable: true),
                    StockValue = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    PortfolioCapital = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    PartnerCount = table.Column<int>(nullable: true),
                    StocksCount = table.Column<float>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Portfolios", x => x.PortfolioID);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeCards",
                columns: table => new
                {
                    EmpCardId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CardType = table.Column<int>(nullable: false),
                    Code = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    IssuePlace = table.Column<string>(type: "nvarchar(150)", nullable: true),
                    IssueDate = table.Column<DateTime>(nullable: true),
                    EndDate = table.Column<DateTime>(nullable: true),
                    RenewalDate = table.Column<DateTime>(nullable: true),
                    Fees = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(MAX)", nullable: true),
                    EmployeeID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeCards", x => x.EmpCardId);
                    table.ForeignKey(
                        name: "FK_EmployeeCards_Employees_EmployeeID",
                        column: x => x.EmployeeID,
                        principalTable: "Employees",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PortfolioAccounts",
                columns: table => new
                {
                    PortfolioAccountID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AccountID = table.Column<int>(nullable: false),
                    PortfolioID = table.Column<int>(nullable: false),
                    Type = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PortfolioAccounts", x => x.PortfolioAccountID);
                    table.ForeignKey(
                        name: "FK_PortfolioAccounts_Accounts_AccountID",
                        column: x => x.AccountID,
                        principalTable: "Accounts",
                        principalColumn: "AccountID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PortfolioAccounts_Portfolios_PortfolioID",
                        column: x => x.PortfolioID,
                        principalTable: "Portfolios",
                        principalColumn: "PortfolioID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Portfolioshareholders",
                columns: table => new
                {
                    PortShareID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Amount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Percentage = table.Column<float>(nullable: false),
                    StocksCount = table.Column<int>(nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(MAX)", nullable: true),
                    PortfolioID = table.Column<int>(nullable: false),
                    PartnerID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Portfolioshareholders", x => x.PortShareID);
                    table.ForeignKey(
                        name: "FK_Portfolioshareholders_Partners_PartnerID",
                        column: x => x.PartnerID,
                        principalTable: "Partners",
                        principalColumn: "PartnerID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Portfolioshareholders_Portfolios_PortfolioID",
                        column: x => x.PortfolioID,
                        principalTable: "Portfolios",
                        principalColumn: "PortfolioID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeCards_EmployeeID",
                table: "EmployeeCards",
                column: "EmployeeID");

            migrationBuilder.CreateIndex(
                name: "IX_PortfolioAccounts_AccountID",
                table: "PortfolioAccounts",
                column: "AccountID");

            migrationBuilder.CreateIndex(
                name: "IX_PortfolioAccounts_PortfolioID",
                table: "PortfolioAccounts",
                column: "PortfolioID");

            migrationBuilder.CreateIndex(
                name: "IX_Portfolioshareholders_PartnerID",
                table: "Portfolioshareholders",
                column: "PartnerID");

            migrationBuilder.CreateIndex(
                name: "IX_Portfolioshareholders_PortfolioID",
                table: "Portfolioshareholders",
                column: "PortfolioID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmployeeCards");

            migrationBuilder.DropTable(
                name: "PortfolioAccounts");

            migrationBuilder.DropTable(
                name: "Portfolioshareholders");

            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "Portfolios");
        }
    }
}

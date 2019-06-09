using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    AccountID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    NameAR = table.Column<string>(type: "nvarchar(150)", nullable: false),
                    NameEN = table.Column<string>(type: "nvarchar(150)", nullable: true),
                    AccountType = table.Column<bool>(nullable: false),
                    AccountCategory = table.Column<int>(nullable: false),
                    DebitLimit = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    CreditLimit = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(MAX)", nullable: true),
                    Phone1 = table.Column<string>(type: "nvarchar(150)", nullable: true),
                    Phone2 = table.Column<string>(type: "nvarchar(150)", nullable: true),
                    Fax = table.Column<string>(type: "nvarchar(150)", nullable: true),
                    Telex = table.Column<string>(type: "nvarchar(150)", nullable: true),
                    Mobile = table.Column<string>(type: "nvarchar(150)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(150)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(MAX)", nullable: true),
                    TaxNum = table.Column<string>(type: "nvarchar(250)", nullable: true),
                    AccoutnParentID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.AccountID);
                    table.ForeignKey(
                        name: "FK_Accounts_Accounts_AccoutnParentID",
                        column: x => x.AccoutnParentID,
                        principalTable: "Accounts",
                        principalColumn: "AccountID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    CountryID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    NameAR = table.Column<string>(type: "nvarchar(150)", nullable: false),
                    NameEN = table.Column<string>(type: "nvarchar(150)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.CountryID);
                });

            migrationBuilder.CreateTable(
                name: "Partners",
                columns: table => new
                {
                    PartnerID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    NameAR = table.Column<string>(type: "nvarchar(150)", nullable: false),
                    NameEN = table.Column<string>(type: "nvarchar(150)", nullable: true),
                    IdentityType = table.Column<int>(nullable: false),
                    IdentityNumber = table.Column<string>(type: "nvarchar(150)", nullable: true),
                    IssuePlace = table.Column<string>(type: "nvarchar(150)", nullable: true),
                    IssueDate = table.Column<DateTime>(nullable: true),
                    Address = table.Column<string>(type: "nvarchar(MAX)", nullable: true),
                    Mobile = table.Column<string>(type: "nvarchar(150)", nullable: true),
                    Phone1 = table.Column<string>(type: "nvarchar(150)", nullable: true),
                    Phone2 = table.Column<string>(type: "nvarchar(150)", nullable: true),
                    Fax = table.Column<string>(type: "nvarchar(150)", nullable: true),
                    ConvertNumber = table.Column<string>(type: "nvarchar(150)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(150)", nullable: true),
                    AccountID = table.Column<int>(nullable: false),
                    CountryID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Partners", x => x.PartnerID);
                    table.ForeignKey(
                        name: "FK_Partners_Accounts_AccountID",
                        column: x => x.AccountID,
                        principalTable: "Accounts",
                        principalColumn: "AccountID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Partners_Countries_CountryID",
                        column: x => x.CountryID,
                        principalTable: "Countries",
                        principalColumn: "CountryID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_AccoutnParentID",
                table: "Accounts",
                column: "AccoutnParentID");

            migrationBuilder.CreateIndex(
                name: "IX_Partners_AccountID",
                table: "Partners",
                column: "AccountID");

            migrationBuilder.CreateIndex(
                name: "IX_Partners_CountryID",
                table: "Partners",
                column: "CountryID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Partners");

            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropTable(
                name: "Countries");
        }
    }
}

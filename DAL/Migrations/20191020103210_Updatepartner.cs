using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class Updatepartner : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Capital",
                table: "Partners",
                type: "decimal(10,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CommercialRegNo",
                table: "Partners",
                type: "nvarchar(150)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "Partners",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "Partners",
                type: "nvarchar(MAX)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TaxNo",
                table: "Partners",
                type: "nvarchar(150)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PartnerAttachments",
                columns: table => new
                {
                    PartnerAttachID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    FilePath = table.Column<string>(type: "nvarchar(250)", nullable: false),
                    PartnerID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartnerAttachments", x => x.PartnerAttachID);
                    table.ForeignKey(
                        name: "FK_PartnerAttachments_Partners_PartnerID",
                        column: x => x.PartnerID,
                        principalTable: "Partners",
                        principalColumn: "PartnerID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PartnerAttachments_PartnerID",
                table: "PartnerAttachments",
                column: "PartnerID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PartnerAttachments");

            migrationBuilder.DropColumn(
                name: "Capital",
                table: "Partners");

            migrationBuilder.DropColumn(
                name: "CommercialRegNo",
                table: "Partners");

            migrationBuilder.DropColumn(
                name: "Date",
                table: "Partners");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "Partners");

            migrationBuilder.DropColumn(
                name: "TaxNo",
                table: "Partners");
        }
    }
}

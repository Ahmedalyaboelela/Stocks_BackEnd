using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class updateNoticeDetails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "SellingOrders",
                type: "nvarchar(MAX)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "PurchaseOrders",
                type: "nvarchar(MAX)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Notices",
                type: "nvarchar(MAX)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PartnerID",
                table: "NoticeDetails",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "StocksCredit",
                table: "Accounts",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "StocksDebit",
                table: "Accounts",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_NoticeDetails_PartnerID",
                table: "NoticeDetails",
                column: "PartnerID");

            migrationBuilder.AddForeignKey(
                name: "FK_NoticeDetails_Partners_PartnerID",
                table: "NoticeDetails",
                column: "PartnerID",
                principalTable: "Partners",
                principalColumn: "PartnerID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NoticeDetails_Partners_PartnerID",
                table: "NoticeDetails");

            migrationBuilder.DropIndex(
                name: "IX_NoticeDetails_PartnerID",
                table: "NoticeDetails");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "SellingOrders");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "PurchaseOrders");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Notices");

            migrationBuilder.DropColumn(
                name: "PartnerID",
                table: "NoticeDetails");

            migrationBuilder.DropColumn(
                name: "StocksCredit",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "StocksDebit",
                table: "Accounts");
        }
    }
}

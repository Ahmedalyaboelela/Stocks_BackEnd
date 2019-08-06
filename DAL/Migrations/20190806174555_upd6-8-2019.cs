using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class upd682019 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notices_Currencies_CurrencyID",
                table: "Notices");

            migrationBuilder.DropIndex(
                name: "IX_Notices_CurrencyID",
                table: "Notices");

            migrationBuilder.DropColumn(
                name: "CurrencyID",
                table: "Notices");

            migrationBuilder.AlterColumn<float>(
                name: "StockCount",
                table: "SellingOrderDetails",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<float>(
                name: "StockCount",
                table: "PurchaseOrderDetails",
                nullable: false,
                oldClrType: typeof(int));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "StockCount",
                table: "SellingOrderDetails",
                nullable: false,
                oldClrType: typeof(float));

            migrationBuilder.AlterColumn<int>(
                name: "StockCount",
                table: "PurchaseOrderDetails",
                nullable: false,
                oldClrType: typeof(float));

            migrationBuilder.AddColumn<int>(
                name: "CurrencyID",
                table: "Notices",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notices_CurrencyID",
                table: "Notices",
                column: "CurrencyID");

            migrationBuilder.AddForeignKey(
                name: "FK_Notices_Currencies_CurrencyID",
                table: "Notices",
                column: "CurrencyID",
                principalTable: "Currencies",
                principalColumn: "CurrencyID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

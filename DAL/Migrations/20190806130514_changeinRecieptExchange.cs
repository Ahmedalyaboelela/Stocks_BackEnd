using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class changeinRecieptExchange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReceiptExchanges_Currencies_CurrencyID",
                table: "ReceiptExchanges");

            migrationBuilder.DropIndex(
                name: "IX_ReceiptExchanges_CurrencyID",
                table: "ReceiptExchanges");

            migrationBuilder.DropColumn(
                name: "CurrencyID",
                table: "ReceiptExchanges");

            migrationBuilder.DropColumn(
                name: "ChiqueNumber",
                table: "ReceiptExchangeDetails");

            migrationBuilder.AddColumn<string>(
                name: "BankName",
                table: "ReceiptExchanges",
                type: "nvarchar(150)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Handling",
                table: "ReceiptExchanges",
                type: "nvarchar(150)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ReceiptExchangeType",
                table: "ReceiptExchanges",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "RecieptValue",
                table: "ReceiptExchanges",
                type: "decimal(10,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TaxNumber",
                table: "ReceiptExchanges",
                type: "nvarchar(150)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "DetailType",
                table: "ReceiptExchangeDetails",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TaxNumber",
                table: "Notices",
                type: "nvarchar(150)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BankName",
                table: "ReceiptExchanges");

            migrationBuilder.DropColumn(
                name: "Handling",
                table: "ReceiptExchanges");

            migrationBuilder.DropColumn(
                name: "ReceiptExchangeType",
                table: "ReceiptExchanges");

            migrationBuilder.DropColumn(
                name: "RecieptValue",
                table: "ReceiptExchanges");

            migrationBuilder.DropColumn(
                name: "TaxNumber",
                table: "ReceiptExchanges");

            migrationBuilder.DropColumn(
                name: "DetailType",
                table: "ReceiptExchangeDetails");

            migrationBuilder.DropColumn(
                name: "TaxNumber",
                table: "Notices");

            migrationBuilder.AddColumn<int>(
                name: "CurrencyID",
                table: "ReceiptExchanges",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ChiqueNumber",
                table: "ReceiptExchangeDetails",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptExchanges_CurrencyID",
                table: "ReceiptExchanges",
                column: "CurrencyID");

            migrationBuilder.AddForeignKey(
                name: "FK_ReceiptExchanges_Currencies_CurrencyID",
                table: "ReceiptExchanges",
                column: "CurrencyID",
                principalTable: "Currencies",
                principalColumn: "CurrencyID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

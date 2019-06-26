using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class updateinentriesandothertables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReceiptExchanges_Currencies_CurrencyID",
                table: "ReceiptExchanges");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "ReceiptExchangeDetails");

            migrationBuilder.DropColumn(
                name: "CreditDebitMoney",
                table: "NoticeDetails");

            migrationBuilder.DropColumn(
                name: "CreditorDebitStocks",
                table: "NoticeDetails");

            migrationBuilder.AddColumn<bool>(
                name: "TransferToAccounts",
                table: "Settings",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<int>(
                name: "CurrencyID",
                table: "ReceiptExchanges",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "ChiqueNumber",
                table: "ReceiptExchangeDetails",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<int>(
                name: "CurrencyID",
                table: "Notices",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AccountID",
                table: "NoticeDetails",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "Credit",
                table: "NoticeDetails",
                type: "decimal(10,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Debit",
                table: "NoticeDetails",
                type: "decimal(10,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<float>(
                name: "StocksCredit",
                table: "NoticeDetails",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "StocksDebit",
                table: "NoticeDetails",
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Debit",
                table: "EntryDetails",
                type: "decimal(10,2)",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<decimal>(
                name: "Credit",
                table: "EntryDetails",
                type: "decimal(10,2)",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AddColumn<float>(
                name: "StocksCredit",
                table: "EntryDetails",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "StocksDebit",
                table: "EntryDetails",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notices_CurrencyID",
                table: "Notices",
                column: "CurrencyID");

            migrationBuilder.CreateIndex(
                name: "IX_NoticeDetails_AccountID",
                table: "NoticeDetails",
                column: "AccountID");

            migrationBuilder.AddForeignKey(
                name: "FK_NoticeDetails_Accounts_AccountID",
                table: "NoticeDetails",
                column: "AccountID",
                principalTable: "Accounts",
                principalColumn: "AccountID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Notices_Currencies_CurrencyID",
                table: "Notices",
                column: "CurrencyID",
                principalTable: "Currencies",
                principalColumn: "CurrencyID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ReceiptExchanges_Currencies_CurrencyID",
                table: "ReceiptExchanges",
                column: "CurrencyID",
                principalTable: "Currencies",
                principalColumn: "CurrencyID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NoticeDetails_Accounts_AccountID",
                table: "NoticeDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_Notices_Currencies_CurrencyID",
                table: "Notices");

            migrationBuilder.DropForeignKey(
                name: "FK_ReceiptExchanges_Currencies_CurrencyID",
                table: "ReceiptExchanges");

            migrationBuilder.DropIndex(
                name: "IX_Notices_CurrencyID",
                table: "Notices");

            migrationBuilder.DropIndex(
                name: "IX_NoticeDetails_AccountID",
                table: "NoticeDetails");

            migrationBuilder.DropColumn(
                name: "TransferToAccounts",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "CurrencyID",
                table: "Notices");

            migrationBuilder.DropColumn(
                name: "AccountID",
                table: "NoticeDetails");

            migrationBuilder.DropColumn(
                name: "Credit",
                table: "NoticeDetails");

            migrationBuilder.DropColumn(
                name: "Debit",
                table: "NoticeDetails");

            migrationBuilder.DropColumn(
                name: "StocksCredit",
                table: "NoticeDetails");

            migrationBuilder.DropColumn(
                name: "StocksDebit",
                table: "NoticeDetails");

            migrationBuilder.DropColumn(
                name: "StocksCredit",
                table: "EntryDetails");

            migrationBuilder.DropColumn(
                name: "StocksDebit",
                table: "EntryDetails");

            migrationBuilder.AlterColumn<int>(
                name: "CurrencyID",
                table: "ReceiptExchanges",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ChiqueNumber",
                table: "ReceiptExchangeDetails",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Type",
                table: "ReceiptExchangeDetails",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "CreditDebitMoney",
                table: "NoticeDetails",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<float>(
                name: "CreditorDebitStocks",
                table: "NoticeDetails",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AlterColumn<decimal>(
                name: "Debit",
                table: "EntryDetails",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Credit",
                table: "EntryDetails",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)");

            migrationBuilder.AddForeignKey(
                name: "FK_ReceiptExchanges_Currencies_CurrencyID",
                table: "ReceiptExchanges",
                column: "CurrencyID",
                principalTable: "Currencies",
                principalColumn: "CurrencyID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

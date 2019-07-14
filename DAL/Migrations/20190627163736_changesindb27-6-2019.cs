using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class changesindb2762019 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReceiptExchangeAmount",
                table: "ReceiptExchangeDetails");

            migrationBuilder.AlterColumn<bool>(
                name: "DoNotGenerateEntry",
                table: "Settings",
                nullable: true,
                oldClrType: typeof(bool));

            migrationBuilder.AddColumn<decimal>(
                name: "Credit",
                table: "ReceiptExchangeDetails",
                type: "decimal(10,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Debit",
                table: "ReceiptExchangeDetails",
                type: "decimal(10,2)",
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Debit",
                table: "NoticeDetails",
                type: "decimal(10,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Credit",
                table: "NoticeDetails",
                type: "decimal(10,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Debit",
                table: "EntryDetails",
                type: "decimal(10,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Credit",
                table: "EntryDetails",
                type: "decimal(10,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)");

            migrationBuilder.AddColumn<bool>(
                name: "TransferedToAccounts",
                table: "Entries",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Credit",
                table: "ReceiptExchangeDetails");

            migrationBuilder.DropColumn(
                name: "Debit",
                table: "ReceiptExchangeDetails");

            migrationBuilder.DropColumn(
                name: "TransferedToAccounts",
                table: "Entries");

            migrationBuilder.AlterColumn<bool>(
                name: "DoNotGenerateEntry",
                table: "Settings",
                nullable: false,
                oldClrType: typeof(bool),
                oldNullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ReceiptExchangeAmount",
                table: "ReceiptExchangeDetails",
                type: "decimal(10,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<decimal>(
                name: "Debit",
                table: "NoticeDetails",
                type: "decimal(10,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Credit",
                table: "NoticeDetails",
                type: "decimal(10,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Debit",
                table: "EntryDetails",
                type: "decimal(10,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Credit",
                table: "EntryDetails",
                type: "decimal(10,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldNullable: true);
        }
    }
}

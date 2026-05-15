using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CryptocurrencyExchange.Migrations
{
    /// <inheritdoc />
    public partial class ConstrainWalletItemAndOutboxSymbolLength : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_WalletItems",
                table: "WalletItems");

            migrationBuilder.AlterColumn<string>(
                name: "Symbol",
                table: "WalletItems",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WalletItems",
                table: "WalletItems",
                columns: new[] { "UserId", "Symbol" });

            migrationBuilder.AlterColumn<string>(
                name: "Symbol",
                table: "TransferCompletedOutbox",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(16)",
                oldMaxLength: 16);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_WalletItems",
                table: "WalletItems");

            migrationBuilder.AlterColumn<string>(
                name: "Symbol",
                table: "WalletItems",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10);

            migrationBuilder.AddPrimaryKey(
                name: "PK_WalletItems",
                table: "WalletItems",
                columns: new[] { "UserId", "Symbol" });

            migrationBuilder.AlterColumn<string>(
                name: "Symbol",
                table: "TransferCompletedOutbox",
                type: "nvarchar(16)",
                maxLength: 16,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10);
        }
    }
}

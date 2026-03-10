using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CryptocurrencyExchange.Migrations
{
    /// <inheritdoc />
    public partial class ReplaceWalletItemPkWithCompositeKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_WalletItems",
                table: "WalletItems");

            migrationBuilder.DropIndex(
                name: "IX_WalletItems_UserId",
                table: "WalletItems");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "WalletItems");

            migrationBuilder.AlterColumn<string>(
                name: "Symbol",
                table: "WalletItems",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WalletItems",
                table: "WalletItems",
                columns: new[] { "UserId", "Symbol" });
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
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "WalletItems",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WalletItems",
                table: "WalletItems",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_WalletItems_UserId",
                table: "WalletItems",
                column: "UserId");
        }
    }
}

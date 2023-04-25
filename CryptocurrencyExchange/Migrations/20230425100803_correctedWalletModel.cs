using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CryptocurrencyExchange.Migrations
{
    /// <inheritdoc />
    public partial class correctedWalletModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WalletItems_Users_UsersId",
                table: "WalletItems");

            migrationBuilder.RenameColumn(
                name: "UsersId",
                table: "WalletItems",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_WalletItems_UsersId",
                table: "WalletItems",
                newName: "IX_WalletItems_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_WalletItems_Users_UserId",
                table: "WalletItems",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WalletItems_Users_UserId",
                table: "WalletItems");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "WalletItems",
                newName: "UsersId");

            migrationBuilder.RenameIndex(
                name: "IX_WalletItems_UserId",
                table: "WalletItems",
                newName: "IX_WalletItems_UsersId");

            migrationBuilder.AddForeignKey(
                name: "FK_WalletItems_Users_UsersId",
                table: "WalletItems",
                column: "UsersId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

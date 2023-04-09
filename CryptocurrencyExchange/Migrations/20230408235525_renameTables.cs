using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CryptocurrencyExchange.Migrations
{
    /// <inheritdoc />
    public partial class renameTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WalletItem_User_UsersId",
                table: "WalletItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WalletItem",
                table: "WalletItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_User",
                table: "User");

            migrationBuilder.RenameTable(
                name: "WalletItem",
                newName: "WalletItems");

            migrationBuilder.RenameTable(
                name: "User",
                newName: "Users");

            migrationBuilder.RenameIndex(
                name: "IX_WalletItem_UsersId",
                table: "WalletItems",
                newName: "IX_WalletItems_UsersId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WalletItems",
                table: "WalletItems",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                table: "Users",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WalletItems_Users_UsersId",
                table: "WalletItems",
                column: "UsersId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WalletItems_Users_UsersId",
                table: "WalletItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WalletItems",
                table: "WalletItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                table: "Users");

            migrationBuilder.RenameTable(
                name: "WalletItems",
                newName: "WalletItem");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "User");

            migrationBuilder.RenameIndex(
                name: "IX_WalletItems_UsersId",
                table: "WalletItem",
                newName: "IX_WalletItem_UsersId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WalletItem",
                table: "WalletItem",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_User",
                table: "User",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WalletItem_User_UsersId",
                table: "WalletItem",
                column: "UsersId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

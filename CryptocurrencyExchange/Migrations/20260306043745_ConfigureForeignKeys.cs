using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CryptocurrencyExchange.Migrations
{
    public partial class ConfigureForeignKeys : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Transfers_ReceiverId",
                table: "Transfers",
                column: "ReceiverId");

            migrationBuilder.CreateIndex(
                name: "IX_Transfers_SenderId",
                table: "Transfers",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_Staking_UserId",
                table: "Staking",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Futures_UserId",
                table: "Futures",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_FutureHistory_FutureId",
                table: "FutureHistory",
                column: "FutureId");

            migrationBuilder.AddForeignKey(
                name: "FK_FutureHistory_Futures_FutureId",
                table: "FutureHistory",
                column: "FutureId",
                principalTable: "Futures",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Futures_Users_UserId",
                table: "Futures",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Staking_Users_UserId",
                table: "Staking",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Transfers_Users_ReceiverId",
                table: "Transfers",
                column: "ReceiverId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Transfers_Users_SenderId",
                table: "Transfers",
                column: "SenderId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FutureHistory_Futures_FutureId",
                table: "FutureHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_Futures_Users_UserId",
                table: "Futures");

            migrationBuilder.DropForeignKey(
                name: "FK_Staking_Users_UserId",
                table: "Staking");

            migrationBuilder.DropForeignKey(
                name: "FK_Transfers_Users_ReceiverId",
                table: "Transfers");

            migrationBuilder.DropForeignKey(
                name: "FK_Transfers_Users_SenderId",
                table: "Transfers");

            migrationBuilder.DropIndex(
                name: "IX_Transfers_ReceiverId",
                table: "Transfers");

            migrationBuilder.DropIndex(
                name: "IX_Transfers_SenderId",
                table: "Transfers");

            migrationBuilder.DropIndex(
                name: "IX_Staking_UserId",
                table: "Staking");

            migrationBuilder.DropIndex(
                name: "IX_Futures_UserId",
                table: "Futures");

            migrationBuilder.DropIndex(
                name: "IX_FutureHistory_FutureId",
                table: "FutureHistory");
        }
    }
}

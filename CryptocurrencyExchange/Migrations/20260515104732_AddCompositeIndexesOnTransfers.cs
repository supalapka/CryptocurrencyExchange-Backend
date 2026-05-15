using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CryptocurrencyExchange.Migrations
{
    /// <inheritdoc />
    public partial class AddCompositeIndexesOnTransfers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Transfers_ReceiverId",
                table: "Transfers");

            migrationBuilder.DropIndex(
                name: "IX_Transfers_SenderId",
                table: "Transfers");

            migrationBuilder.CreateIndex(
                name: "IX_Transfers_ReceiverId_CreatedAt",
                table: "Transfers",
                columns: new[] { "ReceiverId", "CreatedAt" },
                descending: new[] { false, true });

            migrationBuilder.CreateIndex(
                name: "IX_Transfers_SenderId_CreatedAt",
                table: "Transfers",
                columns: new[] { "SenderId", "CreatedAt" },
                descending: new[] { false, true });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Transfers_ReceiverId_CreatedAt",
                table: "Transfers");

            migrationBuilder.DropIndex(
                name: "IX_Transfers_SenderId_CreatedAt",
                table: "Transfers");

            migrationBuilder.CreateIndex(
                name: "IX_Transfers_ReceiverId",
                table: "Transfers",
                column: "ReceiverId");

            migrationBuilder.CreateIndex(
                name: "IX_Transfers_SenderId",
                table: "Transfers",
                column: "SenderId");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CryptocurrencyExchange.Migrations
{
    /// <inheritdoc />
    public partial class addedFuturesHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsLiquidated",
                table: "Futures");

            migrationBuilder.CreateTable(
                name: "FutureHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    FutureId = table.Column<int>(type: "int", nullable: false),
                    MarkPrice = table.Column<double>(type: "float", nullable: false),
                    IsLiquidated = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FutureHistory", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FutureHistory");

            migrationBuilder.AddColumn<bool>(
                name: "IsLiquidated",
                table: "Futures",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}

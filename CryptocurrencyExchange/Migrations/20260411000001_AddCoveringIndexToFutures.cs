using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CryptocurrencyExchange.Migrations
{
    /// <inheritdoc />
    public partial class AddCoveringIndexToFutures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "DROP INDEX IX_Futures_UserId ON Futures",
                suppressTransaction: true);

            migrationBuilder.Sql(
                "CREATE INDEX IX_Futures_UserId ON Futures(UserId) INCLUDE (Symbol, Margin)",
                suppressTransaction: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "DROP INDEX IX_Futures_UserId ON Futures",
                suppressTransaction: true);

            migrationBuilder.Sql(
                "CREATE INDEX IX_Futures_UserId ON Futures(UserId)",
                suppressTransaction: true);
        }
    }
}

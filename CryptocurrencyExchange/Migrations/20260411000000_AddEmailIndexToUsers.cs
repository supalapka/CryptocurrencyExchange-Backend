using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CryptocurrencyExchange.Migrations
{
    /// <inheritdoc />
    public partial class AddEmailIndexToUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "ALTER TABLE Users ALTER COLUMN Email NVARCHAR(256) NOT NULL",
                suppressTransaction: true);

            migrationBuilder.Sql(
                "CREATE INDEX IX_Users_Email ON Users(Email)",
                suppressTransaction: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "DROP INDEX IX_Users_Email ON Users",
                suppressTransaction: true);

            migrationBuilder.Sql(
                "ALTER TABLE Users ALTER COLUMN Email NVARCHAR(MAX) NOT NULL",
                suppressTransaction: true);
        }
    }
}

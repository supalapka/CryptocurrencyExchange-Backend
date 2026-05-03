using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CryptocurrencyExchange.EmailService.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProcessedNotifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OutboxEntryId = table.Column<int>(type: "int", nullable: false),
                    ProcessedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcessedNotifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SentEmails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmailAddress = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EmailType = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SentAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SentEmails", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProcessedNotifications_OutboxEntryId",
                table: "ProcessedNotifications",
                column: "OutboxEntryId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SentEmails_EmailAddress_EmailType",
                table: "SentEmails",
                columns: new[] { "EmailAddress", "EmailType" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProcessedNotifications");

            migrationBuilder.DropTable(
                name: "SentEmails");
        }
    }
}

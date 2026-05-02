using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CryptocurrencyExchange.Migrations
{
    /// <inheritdoc />
    public partial class AddUserRegistrationOutbox : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserRegistrationOutbox",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProcessedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRegistrationOutbox", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserRegistrationOutbox_ProcessedAt",
                table: "UserRegistrationOutbox",
                column: "ProcessedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserRegistrationOutbox");
        }
    }
}

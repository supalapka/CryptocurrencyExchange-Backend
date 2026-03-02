using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CryptocurrencyExchange.Migrations
{
    public partial class AddLogEntries : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Futures_Users_UserId",
                table: "Futures");

            migrationBuilder.DropIndex(
                name: "IX_Futures_UserId",
                table: "Futures");

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "WalletItems",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "Staking",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<decimal>(
                name: "Margin",
                table: "Futures",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.CreateTable(
                name: "LogEntries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Level = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Exception = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TimestampUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogEntries", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LogEntries_Level",
                table: "LogEntries",
                column: "Level");

            migrationBuilder.CreateIndex(
                name: "IX_LogEntries_TimestampUtc",
                table: "LogEntries",
                column: "TimestampUtc");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LogEntries");

            migrationBuilder.AlterColumn<double>(
                name: "Amount",
                table: "WalletItems",
                type: "float",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<double>(
                name: "Amount",
                table: "Staking",
                type: "float",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<double>(
                name: "Margin",
                table: "Futures",
                type: "float",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.CreateIndex(
                name: "IX_Futures_UserId",
                table: "Futures",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Futures_Users_UserId",
                table: "Futures",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

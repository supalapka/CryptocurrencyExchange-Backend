using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CryptocurrencyExchange.Migrations
{
    /// <inheritdoc />
    public partial class completed_staking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Symbol",
                table: "Staking");

            migrationBuilder.AddColumn<float>(
                name: "Amount",
                table: "Staking",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<int>(
                name: "StakingCoinId",
                table: "Staking",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "StakingCoins",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Symbol = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RatePerMonth = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StakingCoins", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Staking_StakingCoinId",
                table: "Staking",
                column: "StakingCoinId");

            migrationBuilder.AddForeignKey(
                name: "FK_Staking_StakingCoins_StakingCoinId",
                table: "Staking",
                column: "StakingCoinId",
                principalTable: "StakingCoins",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Staking_StakingCoins_StakingCoinId",
                table: "Staking");

            migrationBuilder.DropTable(
                name: "StakingCoins");

            migrationBuilder.DropIndex(
                name: "IX_Staking_StakingCoinId",
                table: "Staking");

            migrationBuilder.DropColumn(
                name: "Amount",
                table: "Staking");

            migrationBuilder.DropColumn(
                name: "StakingCoinId",
                table: "Staking");

            migrationBuilder.AddColumn<string>(
                name: "Symbol",
                table: "Staking",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}

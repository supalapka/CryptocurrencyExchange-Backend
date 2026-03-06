using CryptocurrencyExchange.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Security.Cryptography;
using System.Text;

namespace CryptocurrencyExchange.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeedController : ControllerBase
    {
        private readonly DataContext _context;
        private const int UserCount = 700_000;
        private const int BatchSize = 10_000;

        private static readonly string[] CoinSymbols = { "BTC", "ETH", "SOL", "DOGE", "XRP", "ADA", "AVAX", "DOT", "MATIC", "LINK", "UNI", "ATOM", "LTC", "FIL", "NEAR" };
        private static readonly string[] FutureSymbols = { "BTCUSDT", "ETHUSDT", "SOLUSDT", "DOGEUSDT" };
        private static readonly int[] Leverages = { 2, 5, 10, 20, 50 };
        private static readonly string[] LogLevels = { "Information", "Warning", "Error", "Debug" };

        public SeedController(DataContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Seed()
        {
            var connection = (SqlConnection)_context.Database.GetDbConnection();
            await connection.OpenAsync();

            await SeedUsers(connection);
            await SeedStakingCoins(connection);
            await SeedWalletItems(connection);
            await SeedFutures(connection);
            await SeedFutureHistory(connection);
            await SeedStaking(connection);
            await SeedNotifications(connection);
            await SeedTransfers(connection);
            await SeedCryptoNews(connection);
            await SeedLogEntries(connection);

            await connection.CloseAsync();

            return Ok($"Seeded {UserCount} users with related data.");
        }

        private static async Task SeedUsers(SqlConnection connection)
        {
            var table = new DataTable();
            table.Columns.Add("Email", typeof(string));
            table.Columns.Add("PasswordHash", typeof(byte[]));
            table.Columns.Add("PasswordSalt", typeof(byte[]));

            using var hmac = new HMACSHA512();
            var salt = hmac.Key;
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes("qwe"));

            for (int i = 0; i < UserCount; i++)
            {
                table.Rows.Add($"user{i}@test.com", hash, salt);

                if (table.Rows.Count >= BatchSize)
                {
                    await BulkCopy(connection, "Users", table);
                    table.Clear();
                }
            }
            if (table.Rows.Count > 0)
                await BulkCopy(connection, "Users", table);
        }

        private static async Task SeedWalletItems(SqlConnection connection)
        {
            var table = new DataTable();
            table.Columns.Add("UserId", typeof(int));
            table.Columns.Add("Symbol", typeof(string));
            table.Columns.Add("Amount", typeof(decimal));

            for (int userId = 1; userId <= UserCount; userId++)
            {
                int count = Random.Shared.Next(5, 16);
                var usedSymbols = CoinSymbols.OrderBy(_ => Random.Shared.Next()).Take(count);

                foreach (var symbol in usedSymbols)
                {
                    table.Rows.Add(
                        userId,
                        symbol,
                        Math.Round((decimal)(Random.Shared.NextDouble() * 100), 8));

                    if (table.Rows.Count >= BatchSize)
                    {
                        await BulkCopy(connection, "WalletItems", table);
                        table.Clear();
                    }
                }
            }
            if (table.Rows.Count > 0)
                await BulkCopy(connection, "WalletItems", table);
        }

        private static async Task SeedFutures(SqlConnection connection)
        {
            var table = new DataTable();
            table.Columns.Add("Symbol", typeof(string));
            table.Columns.Add("Margin", typeof(decimal));
            table.Columns.Add("EntryPrice", typeof(double));
            table.Columns.Add("UserId", typeof(int));
            table.Columns.Add("IsCompleted", typeof(bool));
            table.Columns.Add("Leverage", typeof(int));
            table.Columns.Add("Position", typeof(int));

            for (int userId = 1; userId <= UserCount; userId++)
            {
                int count = Random.Shared.Next(4, 11);

                for (int j = 0; j < count; j++)
                {
                    table.Rows.Add(
                        FutureSymbols[Random.Shared.Next(FutureSymbols.Length)],
                        Math.Round((decimal)(Random.Shared.NextDouble() * 5000), 2),
                        Random.Shared.NextDouble() * 50000,
                        userId,
                        Random.Shared.Next(2) == 0,
                        Leverages[Random.Shared.Next(Leverages.Length)],
                        Random.Shared.Next(2));

                    if (table.Rows.Count >= BatchSize)
                    {
                        await BulkCopy(connection, "Futures", table);
                        table.Clear();
                    }
                }
            }
            if (table.Rows.Count > 0)
                await BulkCopy(connection, "Futures", table);
        }

        private static async Task SeedFutureHistory(SqlConnection connection)
        {
            var futureCount = await GetRowCount(connection, "Futures");

            var table = new DataTable();
            table.Columns.Add("Id", typeof(int));
            table.Columns.Add("FutureId", typeof(int));
            table.Columns.Add("MarkPrice", typeof(double));
            table.Columns.Add("IsLiquidated", typeof(bool));

            for (int i = 0; i < futureCount; i++)
            {
                table.Rows.Add(
                    i + 1,
                    i + 1,
                    Random.Shared.NextDouble() * 50000,
                    Random.Shared.Next(10) == 0);

                if (table.Rows.Count >= BatchSize)
                {
                    await BulkCopy(connection, "FutureHistory", table);
                    table.Clear();
                }
            }
            if (table.Rows.Count > 0)
                await BulkCopy(connection, "FutureHistory", table);
        }

        private static async Task SeedStakingCoins(SqlConnection connection)
        {
            var table = new DataTable();
            table.Columns.Add("Symbol", typeof(string));
            table.Columns.Add("RatePerMonth", typeof(float));
            table.Columns.Add("Description", typeof(string));

            for (int i = 0; i < CoinSymbols.Length; i++)
            {
                table.Rows.Add(
                    CoinSymbols[i],
                    (float)Math.Round(Random.Shared.NextDouble() * 5, 2),
                    $"Staking {CoinSymbols[i]}");
            }

            await BulkCopy(connection, "StakingCoins", table);
        }

        private static async Task SeedStaking(SqlConnection connection)
        {
            var table = new DataTable();
            table.Columns.Add("UserId", typeof(int));
            table.Columns.Add("StakingCoinId", typeof(int));
            table.Columns.Add("Amount", typeof(decimal));
            table.Columns.Add("StartDate", typeof(DateTime));
            table.Columns.Add("DurationInMonth", typeof(int));
            table.Columns.Add("IsCompleted", typeof(bool));

            for (int userId = 1; userId <= UserCount; userId++)
            {
                int count = Random.Shared.Next(0, 4);

                for (int j = 0; j < count; j++)
                {
                    table.Rows.Add(
                        userId,
                        Random.Shared.Next(1, CoinSymbols.Length + 1),
                        Math.Round((decimal)(Random.Shared.NextDouble() * 1000), 2),
                        DateTime.UtcNow.AddDays(-Random.Shared.Next(1, 365)),
                        Random.Shared.Next(1, 13),
                        Random.Shared.Next(3) == 0);

                    if (table.Rows.Count >= BatchSize)
                    {
                        await BulkCopy(connection, "Staking", table);
                        table.Clear();
                    }
                }
            }
            if (table.Rows.Count > 0)
                await BulkCopy(connection, "Staking", table);
        }

        private static async Task SeedNotifications(SqlConnection connection)
        {
            var table = new DataTable();
            table.Columns.Add("UserId", typeof(int));
            table.Columns.Add("Message", typeof(string));
            table.Columns.Add("IsRead", typeof(bool));
            table.Columns.Add("CreatedAt", typeof(DateTime));

            for (int userId = 1; userId <= UserCount; userId++)
            {
                int count = Random.Shared.Next(1, 6);

                for (int j = 0; j < count; j++)
                {
                    table.Rows.Add(
                        userId,
                        $"Notification #{j} for user {userId}",
                        Random.Shared.Next(2) == 0,
                        DateTime.UtcNow.AddMinutes(-Random.Shared.Next(1, 100000)));

                    if (table.Rows.Count >= BatchSize)
                    {
                        await BulkCopy(connection, "Notifications", table);
                        table.Clear();
                    }
                }
            }
            if (table.Rows.Count > 0)
                await BulkCopy(connection, "Notifications", table);
        }

        private static async Task SeedTransfers(SqlConnection connection)
        {
            var table = new DataTable();
            table.Columns.Add("SenderId", typeof(int));
            table.Columns.Add("ReceiverId", typeof(int));
            table.Columns.Add("Symbol", typeof(string));
            table.Columns.Add("Amount", typeof(decimal));
            table.Columns.Add("Code", typeof(string));
            table.Columns.Add("Status", typeof(string));
            table.Columns.Add("CreatedAt", typeof(DateTime));

            for (int userId = 1; userId <= UserCount; userId++)
            {
                int count = Random.Shared.Next(0, 4);

                for (int j = 0; j < count; j++)
                {
                    int receiver = Random.Shared.Next(1, UserCount + 1);
                    if (receiver == userId) receiver = (userId % UserCount) + 1;

                    table.Rows.Add(
                        userId,
                        receiver,
                        CoinSymbols[Random.Shared.Next(CoinSymbols.Length)],
                        Math.Round((decimal)(Random.Shared.NextDouble() * 10), 8),
                        Random.Shared.Next(100000, 999999).ToString(),
                        Random.Shared.Next(2) == 0 ? "Pending" : "Completed",
                        DateTime.UtcNow.AddMinutes(-Random.Shared.Next(1, 100000)));

                    if (table.Rows.Count >= BatchSize)
                    {
                        await BulkCopy(connection, "Transfers", table);
                        table.Clear();
                    }
                }
            }
            if (table.Rows.Count > 0)
                await BulkCopy(connection, "Transfers", table);
        }

        private static async Task SeedCryptoNews(SqlConnection connection)
        {
            var table = new DataTable();
            table.Columns.Add("Id", typeof(Guid));
            table.Columns.Add("Coin", typeof(string));
            table.Columns.Add("Title", typeof(string));
            table.Columns.Add("Url", typeof(string));

            for (int i = 0; i < 10_000; i++)
            {
                table.Rows.Add(
                    Guid.NewGuid(),
                    CoinSymbols[i % CoinSymbols.Length],
                    $"Crypto news title #{i}",
                    $"https://news.example.com/article/{i}");

                if (table.Rows.Count >= BatchSize)
                {
                    await BulkCopy(connection, "CryptoNews", table);
                    table.Clear();
                }
            }
            if (table.Rows.Count > 0)
                await BulkCopy(connection, "CryptoNews", table);
        }

        private static async Task SeedLogEntries(SqlConnection connection)
        {
            var table = new DataTable();
            table.Columns.Add("Level", typeof(string));
            table.Columns.Add("Category", typeof(string));
            table.Columns.Add("Message", typeof(string));
            table.Columns.Add("Exception", typeof(string));
            table.Columns.Add("TimestampUtc", typeof(DateTime));

            for (int i = 0; i < 100_000; i++)
            {
                table.Rows.Add(
                    LogLevels[i % LogLevels.Length],
                    $"Category.{i % 50}",
                    $"Log message #{i}",
                    i % 5 == 0 ? $"System.Exception: Error {i}" : (object)DBNull.Value,
                    DateTime.UtcNow.AddSeconds(-i));

                if (table.Rows.Count >= BatchSize)
                {
                    await BulkCopy(connection, "LogEntries", table);
                    table.Clear();
                }
            }
            if (table.Rows.Count > 0)
                await BulkCopy(connection, "LogEntries", table);
        }

        private static async Task BulkCopy(SqlConnection connection, string tableName, DataTable table)
        {
            using var bulk = new SqlBulkCopy(connection);
            bulk.DestinationTableName = tableName;
            bulk.BatchSize = BatchSize;

            foreach (DataColumn col in table.Columns)
                bulk.ColumnMappings.Add(col.ColumnName, col.ColumnName);

            await bulk.WriteToServerAsync(table);
        }

        private static async Task<int> GetRowCount(SqlConnection connection, string tableName)
        {
            using var cmd = new SqlCommand($"SELECT COUNT(*) FROM [{tableName}]", connection);
            return (int)(await cmd.ExecuteScalarAsync())!;
        }
    }
}

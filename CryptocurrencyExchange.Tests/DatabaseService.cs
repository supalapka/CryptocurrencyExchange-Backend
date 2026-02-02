using CryptocurrencyExchange.Data;
using CryptocurrencyExchange.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Text;

namespace CryptocurrencyExchange.Tests
{
    public static class DatabaseService
    {

        public static DataContext CreateInMemoryDbContext(string databaseName)
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName)
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;

            return new DataContext(options);
        }

        public static DataContext CreateSqliteInMemoryContext(SqliteConnection _connection = null)
        {
            if (_connection == null)
            {
                _connection = new SqliteConnection("DataSource=:memory:");
                _connection.Open();
            }

            var options = new DbContextOptionsBuilder<DataContext>()
                 .UseSqlite(_connection)
                 .Options;

            var ctx = new DataContext(options);
            ctx.Database.EnsureCreated();
            return ctx;
        }

        public static User CreateUser(int userId)
        {
            return new User
            {
                Id = userId,
                PasswordHash = Encoding.UTF8.GetBytes("qwe"),
                PasswordSalt = Encoding.UTF8.GetBytes("qwe"),
            };
        }

        public static WalletItem CreateWalletItem(int userId, string symbol, decimal amount)
        {
            return new WalletItem
            {
                UserId = userId,
                Symbol = symbol,
                Amount = amount
            };
        }

    }
}

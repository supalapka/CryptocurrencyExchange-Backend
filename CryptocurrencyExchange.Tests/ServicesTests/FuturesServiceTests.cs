using CryptocurrencyExchange.Exceptions;
using CryptocurrencyExchange.Models;
using CryptocurrencyExchange.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace CryptocurrencyExchange.Tests.ServicesTests
{
    [TestFixture]
    public class FuturesServiceTests
    {
        const int userUsdtBalance = 1000;
        private int _testUserId;

        private static SqliteConnection _connection;

        [Test]
        public void EnsureSufficientBalance_WhenBalanceIsLow_Throws()
        {
            int balance = 100;
            Assert.Throws<InsufficientFundsException>(() =>
                FuturesService.EnsureSufficientBalance(balance, balance+1)
            );
        }

        [Test]
        public void CalculateBalanceAfterClose_ReturnsCorrectValue()
        {
            var result = FuturesService.CalculateBalanceAfterClose(
                currentBalance: 1000,
                margin: 1000,
                pnl: 200
            );

            Assert.AreEqual(2200, result);
        }

        [Test]
        public async Task CreateFutureAsync_WithSufficientBalance()
        {
            // Arrange
            var futureDto = new FutureDto
            {
                Symbol = "BTC",
                EntryPrice = 50000,
                Margin = 500,
                Leverage = 10,
                Position = PositionType.Long,
            };

            using var ctx = DatabaseService.CreateSqliteInMemoryContext(_connection);
            var futuresService = new FuturesService(ctx);

            // Act
            await futuresService.CreateFutureAsync(futureDto, _testUserId);

            // Assert
            var future = await ctx.Futures
                .SingleAsync(f => f.UserId == _testUserId && f.Symbol == futureDto.Symbol);
            Assert.AreEqual(500, future.Margin);
        }


        [Test]
        public async Task CreateFutureAsync_WithInsufficientBalance()
        {
            // Arrange
            var futureDto = new FutureDto
            {
                Symbol = "BTC",
                EntryPrice = 50000,
                Margin = userUsdtBalance * 2,
                Leverage = 10,
                Position = PositionType.Long,
            };

            using var _ctx = DatabaseService.CreateSqliteInMemoryContext(_connection);

            var userUsdt = DatabaseService.CreateWalletItem(_testUserId, "usdt", userUsdtBalance);
            _ctx.WalletItems.Add(userUsdt);
            await _ctx.SaveChangesAsync();

            var futuresService = new FuturesService(_ctx);

            // Act & Assert
            Assert.ThrowsAsync<InsufficientFundsException>(async () => await futuresService.CreateFutureAsync(futureDto, _testUserId));
        }


        [Test]
        public async Task ClosePosition_Success_RightBalance()
        {
            using var _ctx = DatabaseService.CreateSqliteInMemoryContext(_connection);
            var service = new FuturesService(_ctx);

            // Arrange
            int margin = 1000;
            int PnL = 200;
            const int expectedBalance = 2200;
            var future = new Future { UserId = _testUserId, Margin = margin, Position = PositionType.Long };
            _ctx.Futures.Add(future);
            await _ctx.SaveChangesAsync();

            // Act
            await service.ClosePosition(future.Id, PnL, 9000);

            // Assert
            var wallet = _ctx.WalletItems.Single(x => x.UserId == _testUserId && x.Symbol == "usdt");
            Assert.AreEqual(expectedBalance, wallet.Amount);
        }



        [OneTimeSetUp]
        public async Task OneTimeSetup()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            using var ctx = DatabaseService.CreateSqliteInMemoryContext(_connection);
            await ctx.Database.EnsureCreatedAsync();
        }

        [SetUp]
        public async Task Setup()
        {
            using var ctx = DatabaseService.CreateSqliteInMemoryContext(_connection);

            ctx.Futures.RemoveRange(ctx.Futures);
            ctx.WalletItems.RemoveRange(ctx.WalletItems);
            ctx.Users.RemoveRange(ctx.Users);
            await ctx.SaveChangesAsync();

            var user = DatabaseService.CreateUser(0);
            ctx.Users.Add(user);
            await ctx.SaveChangesAsync();
            _testUserId = user.Id;

            var wallet = DatabaseService.CreateWalletItem(_testUserId, "usdt", userUsdtBalance);
            ctx.WalletItems.Add(wallet);
            await ctx.SaveChangesAsync();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            _connection.Close();
        }
    }

}

using CryptocurrencyExchange.Models;
using CryptocurrencyExchange.Services;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace CryptocurrencyExchange.Tests.ServicesTests
{
    [TestFixture]
    public class FuturesServiceTests
    {
        int userUsdtBalance = 1000;
        private int _testUserId;

        [Test]
        public async Task CreateFutureAsync_WithSufficientBalance()
        {
            // Arrange
            using var ctx = DatabaseService.CreateDbContext();

            var futureDto = new FutureDto
            {
                Symbol = "BTC",
                EntryPrice = 50000,
                Margin = 500,
                Leverage = 10,
                Position = PositionType.Long,
            };

            var futuresService = new FuturesService(ctx);

            // Act
            await futuresService.CreateFutureAsync(futureDto, _testUserId);

            // Assert
            var future = await ctx.Futures.FirstOrDefaultAsync(f => f.UserId == _testUserId && f.Symbol == "BTC");
            Assert.IsNotNull(future);
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
            var ctx = DatabaseService.CreateDbContext();

            var userUsdt = DatabaseService.CreateWalletItem(_testUserId, "usdt", userUsdtBalance);
            ctx.WalletItems.Add(userUsdt);
            await ctx.SaveChangesAsync();

            var futuresService = new FuturesService(ctx);

            // Act & Assert
            Assert.ThrowsAsync<Exception>(async () => await futuresService.CreateFutureAsync(futureDto, _testUserId));
        }


        [Test]
        public async Task ClosePosition_Success_RightBalance()
        {
            // Arrange
            int futureId;
            int PnL = 200;
            int margin = 1000;
            int finalUserBalanceShouldBe = userUsdtBalance + PnL + margin;

            using (var setupCtx = DatabaseService.CreateDbContext())
            {
                var future = new Future
                {
                    UserId = _testUserId,
                    Margin = margin,
                    Leverage = 10,
                    Position = PositionType.Long
                };
                setupCtx.Futures.Add(future);
                await setupCtx.SaveChangesAsync();
                futureId = future.Id;
            }

            // Act
            using (var actCtx = DatabaseService.CreateDbContext())
            {
                var service = new FuturesService(actCtx);
                await service.ClosePosition(futureId, PnL, 9000);
            }

            // Assert
            using (var assertCtx = DatabaseService.CreateDbContext())
            {
                var wallet = assertCtx.WalletItems.Single(x => x.UserId == _testUserId && x.Symbol == "usdt");
                Assert.AreEqual(finalUserBalanceShouldBe, wallet.Amount);
            }
        }


        [TearDown]
        public async Task CleanDatabase()
        {
            var ctx = DatabaseService.CreateDbContext();
            var future = ctx.Futures.FirstOrDefault();
            if (future != null)
                ctx.Futures.Remove(future);

            var walletItem = ctx.WalletItems.FirstOrDefault();
            if (walletItem != null)
                ctx.WalletItems.Remove(walletItem);

            await ctx.SaveChangesAsync();
        }


        [SetUp]
        public async Task Setup()
        {
            var ctx = DatabaseService.CreateDbContext();

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
    }
}

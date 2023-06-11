using CryptocurrencyExchange.Models;
using CryptocurrencyExchange.Services;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace CryptocurrencyExchange.Tests.ServicesTests
{
    [TestFixture]
    public class FuturesServiceTests
    {
        [Test]
        public async Task CreateFutureAsync_WithSufficientBalance()
        {
            // Arrange
            int userUsdtBalance = 1000;
            var futureDto = new FutureDto
            {
                Symbol = "BTC",
                EntryPrice = 50000,
                Margin = 900, // less then userUsdtBalance
                Leverage = 10,
                Position = PositionType.Long,
            };
            var ctx = DatabaseService.CreateDbContext();

            var user = ctx.Users.First();
            if (user == null)
            {
                user = DatabaseService.CreateUser(0);
                ctx.Users.Add(user);
                await ctx.SaveChangesAsync(); //save for get user id
            }

            var userUsdt = DatabaseService.CreateWalletItem(user.Id, "usdt", userUsdtBalance);
            ctx.WalletItems.Add(userUsdt);
            await ctx.SaveChangesAsync();

            var futuresService = new FuturesService(ctx);

            await futuresService.CreateFutureAsync(futureDto, user.Id);

            // Assert
            var future = await ctx.Futures.FirstOrDefaultAsync(f => f.UserId == user.Id);
            Assert.IsNotNull(future);
        }


        [Test]
        public async Task CreateFutureAsync_WithInsufficientBalance()
        {
            // Arrange
            int userUsdtBalance = 1000;
            var futureDto = new FutureDto
            {
                Symbol = "BTC",
                EntryPrice = 50000,
                Margin = userUsdtBalance * 2,
                Leverage = 10,
                Position = PositionType.Long,
            };
            var ctx = DatabaseService.CreateDbContext();

            var user = ctx.Users.First();
            if (user == null)
            {
                user = DatabaseService.CreateUser(0);
                ctx.Users.Add(user);
                await ctx.SaveChangesAsync(); //save for get user id
            }

            var userUsdt = DatabaseService.CreateWalletItem(user.Id, "usdt", userUsdtBalance);
            ctx.WalletItems.Add(userUsdt);
            await ctx.SaveChangesAsync();

            var futuresService = new FuturesService(ctx);

            // Act & Assert
            Assert.ThrowsAsync<Exception>(async () => await futuresService.CreateFutureAsync(futureDto, user.Id));
        }


        [Test]
        public async Task ClosePosition_Success_RightBalance()
        {
            // Arrange
            var userStartedBalance = 0;
            var futureMargin = 1000;
            var futurePnL = 200;
            var ctx = DatabaseService.CreateDbContext();

            var user = ctx.Users.First();
            if (user == null)
            {
                user = DatabaseService.CreateUser(0);
                ctx.Users.Add(user);
                await ctx.SaveChangesAsync(); //save for get user id
            }

            var future = new Future
            {
                Symbol = "BTC",
                EntryPrice = 50000,
                Margin = futureMargin,
                Leverage = 10,
                Position = PositionType.Long,
                IsCompleted = false,
                UserId = user.Id,

            };

            var userUsdt = DatabaseService.CreateWalletItem(user.Id, "usdt", userStartedBalance);
            ctx.WalletItems.Add(userUsdt);
            ctx.Futures.Add(future);
            await ctx.SaveChangesAsync();

            var futuresService = new FuturesService(ctx);

            //Act
            await futuresService.ClosePosition(future.Id, (double)futurePnL, 9000);

            //Assert
            Assert.IsTrue(future.IsCompleted);
            Assert.AreEqual(userStartedBalance + futureMargin + futurePnL, (int)userUsdt.Amount);
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

    }
}

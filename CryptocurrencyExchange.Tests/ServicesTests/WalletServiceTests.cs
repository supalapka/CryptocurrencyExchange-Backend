using CryptocurrencyExchange.Data;
using CryptocurrencyExchange.Models;
using CryptocurrencyExchange.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using System.Text;

namespace CryptocurrencyExchange.Tests.ServicesTests
{
    [TestFixture]
    public class WalletServiceTests
    {
        [Test]
        public async Task BuyAsync_WithEnoughBalance()
        {
            // Arrange
            int coinPrice = 50;
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "WithEnoughBalance")
                .Options;

            using (var dbContext = new DataContext(options))
            {
                var userId = 1;
                var coinSymbol = "btc";
                var usdToBuy = 100;

                var usdtWalletItem = new WalletItem
                {
                    UserId = userId,
                    Symbol = "usdt",
                    Amount = 5000
                };

                var coinToBuy = new WalletItem
                {
                    UserId = userId,
                    Symbol = coinSymbol,
                    Amount = 0
                };

                dbContext.WalletItems.Add(usdtWalletItem);
                dbContext.WalletItems.Add(coinToBuy);
                await dbContext.SaveChangesAsync();

                var marketServiceMock = new Mock<IMarketService>();
                marketServiceMock.Setup(x => x.GetPrice(coinSymbol)).ReturnsAsync(coinPrice);

                var walletService = new WalletService(dbContext, marketServiceMock.Object);

                // Act
                await walletService.BuyAsync(userId, coinSymbol, usdToBuy);

                // Assert
                Assert.AreEqual(4900, usdtWalletItem.Amount);
                Assert.AreEqual(usdToBuy / coinPrice, coinToBuy.Amount);
            }
        }


        [Test]
        public async Task BuyAsync_NotEnoughBalance_ThrowsException()
        {
            // Arrange
            int coinPrice = 50;
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "NotEnoughBalance")
                .Options;

            using (var dbContext = new DataContext(options))
            {
                var userId = 1;
                var coinSymbol = "btc";
                var usdToBuy = 100;

                var usdtWalletItem = new WalletItem
                {
                    UserId = userId,
                    Symbol = "usdt",
                    Amount = 0 //set zero user balance
                };

                var coinToBuy = new WalletItem
                {
                    UserId = userId,
                    Symbol = coinSymbol,
                    Amount = 0
                };

                dbContext.WalletItems.Add(usdtWalletItem);
                dbContext.WalletItems.Add(coinToBuy);
                await dbContext.SaveChangesAsync();

                var marketServiceMock = new Mock<IMarketService>();
                marketServiceMock.Setup(x => x.GetPrice(coinSymbol)).ReturnsAsync(coinPrice);

                var walletService = new WalletService(dbContext, marketServiceMock.Object);


                // Act & Assert
                Assert.ThrowsAsync<Exception>(async () => await walletService.BuyAsync(userId, coinSymbol, usdToBuy));
            }
        }


        [Test]
        public async Task SellAsync_CoinSold()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "CoinSold")
                .Options;

            using (var dbContext = new DataContext(options))
            {
                int coinPrice = 100;
                var userId = 1;
                var coinSymbol = "btc";
                var amount = 1.0;

                var coinToSell = new WalletItem
                {
                    UserId = userId,
                    Symbol = coinSymbol,
                    Amount = amount
                };

                var usdtWalletItem = new WalletItem
                {
                    UserId = userId,
                    Symbol = "usdt",
                    Amount = 0,
                };

                dbContext.WalletItems.Add(usdtWalletItem);
                dbContext.WalletItems.Add(coinToSell);
                await dbContext.SaveChangesAsync();

                var marketServiceMock = new Mock<IMarketService>();
                marketServiceMock.Setup(x => x.GetPrice(coinSymbol)).ReturnsAsync(coinPrice);

                var walletService = new WalletService(dbContext, marketServiceMock.Object);

                // Act
                await walletService.SellAsync(userId, coinSymbol, coinToSell.Amount);

                // Assert
                Assert.AreEqual(0, coinToSell.Amount);
                Assert.AreEqual(usdtWalletItem.Amount, coinPrice * amount);
            }
        }

        [Test]
        public async Task SendCryptoAsync_SuccessfulSent()
        {
            // Arrange
            var senderId = 1;
            var receiverId = 2;
            var symbol = "btc";
            var amount = 1.0;

            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "SuccessfulSent")
                .Options;

            using (var dbContext = new DataContext(options))
            {
                var sender = new User
                {
                    Id = senderId,
                    PasswordHash = Encoding.UTF8.GetBytes("qwe"),
                    PasswordSalt = Encoding.UTF8.GetBytes("qwe"),
                };
                var receiver = new User
                {
                    Id = receiverId,
                    PasswordHash = Encoding.UTF8.GetBytes("qwe"),
                    PasswordSalt = Encoding.UTF8.GetBytes("qwe"),
                };

                var senderWalletItem = new WalletItem
                {
                    UserId = senderId,
                    Symbol = symbol,
                    Amount = amount
                };

                var receiverWalletItem = new WalletItem
                {
                    UserId = receiverId,
                    Symbol = symbol,
                    Amount = 0
                };

                dbContext.Users.Add(sender);
                dbContext.Users.Add(receiver);
                dbContext.WalletItems.Add(senderWalletItem);
                dbContext.WalletItems.Add(receiverWalletItem);
                await dbContext.SaveChangesAsync();

                var marketServiceMock = new Mock<IMarketService>();
                var notificationServiceMock = new Mock<INotificationService>();

                var walletService = new WalletService(dbContext, notificationServiceMock.Object, marketServiceMock.Object);

                // Act
                await walletService.SendCryptoAsync(senderId, symbol, amount, receiverId);

                // Assert
                Assert.AreEqual(amount, receiverWalletItem.Amount);
                Assert.AreEqual(0, senderWalletItem.Amount);
            }
        }

        [Test]
        public async Task SendCryptoAsync_NotEnoughBalanceToSend() //fix code reapiting later
        {
            // Arrange
            var senderId = 1;
            var receiverId = 2;
            var symbol = "btc";
            var amount = 1.0;

            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "NotEnoughBalanceToSend")
                .Options;

            using (var dbContext = new DataContext(options))
            {
                var sender = new User
                {
                    Id = senderId,
                    PasswordHash = Encoding.UTF8.GetBytes("qwe"),
                    PasswordSalt = Encoding.UTF8.GetBytes("qwe"),
                };
                var receiver = new User
                {
                    Id = receiverId,
                    PasswordHash = Encoding.UTF8.GetBytes("qwe"),
                    PasswordSalt = Encoding.UTF8.GetBytes("qwe"),
                };

                var senderWalletItem = new WalletItem
                {
                    UserId = senderId,
                    Symbol = symbol,
                    Amount = amount
                };

                var receiverWalletItem = new WalletItem
                {
                    UserId = receiverId,
                    Symbol = symbol,
                    Amount = 0
                };

                dbContext.Users.Add(sender);
                dbContext.Users.Add(receiver);
                dbContext.WalletItems.Add(senderWalletItem);
                dbContext.WalletItems.Add(receiverWalletItem);
                await dbContext.SaveChangesAsync();

                var marketServiceMock = new Mock<IMarketService>();
                var notificationServiceMock = new Mock<INotificationService>();

                var walletService = new WalletService(dbContext, notificationServiceMock.Object, marketServiceMock.Object);

                // Act & Assert
                Assert.ThrowsAsync<Exception>(async () => await walletService.SendCryptoAsync(senderId, symbol, amount * 2, receiverId));
            }
        }


        private static DbSet<T> MockDbSet<T>(IQueryable<T> data) where T : class
        {
            var mockSet = new Mock<DbSet<T>>();
            mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());
            return mockSet.Object;
        }
    }
}

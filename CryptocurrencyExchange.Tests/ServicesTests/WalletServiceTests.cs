using CryptocurrencyExchange.Services;
using Moq;
using NUnit.Framework;

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
            var userId = 1;
            var coinSymbol = "btc";
            var usdToBuy = 100;

            var ctx = DatabaseService.CreateDbContext("WithEnoughBalance");

            var coinToBuy = DatabaseService.CreateWalletItem(userId, coinSymbol, 0);
            var usdtWalletItem = DatabaseService.CreateWalletItem(userId, "usdt", 5000);

            ctx.WalletItems.Add(usdtWalletItem);
            ctx.WalletItems.Add(coinToBuy);
            await ctx.SaveChangesAsync();

            var marketServiceMock = new Mock<IMarketService>();
            marketServiceMock.Setup(x => x.GetPrice(coinSymbol)).ReturnsAsync(coinPrice);

            var walletService = new WalletService(ctx, marketServiceMock.Object);

            // Act
            await walletService.BuyAsync(userId, coinSymbol, usdToBuy);

            // Assert
            Assert.AreEqual(4900, usdtWalletItem.Amount);
            Assert.AreEqual(usdToBuy / coinPrice, coinToBuy.Amount);
        }


        [Test]
        public async Task BuyAsync_NotEnoughBalance_ThrowsException()
        {
            // Arrange
            var coinPrice = 50;
            var userId = 1;
            var coinSymbol = "btc";
            var usdToBuy = 100;

            var ctx = DatabaseService.CreateDbContext("NotEnoughBalance");

            var coinToBuy = DatabaseService.CreateWalletItem(userId, coinSymbol, 0);
            var usdtWalletItem = DatabaseService.CreateWalletItem(userId, "usdt", 0);  //set balance less then usdToBuy

            ctx.WalletItems.Add(usdtWalletItem);
            ctx.WalletItems.Add(coinToBuy);
            await ctx.SaveChangesAsync();

            var marketServiceMock = new Mock<IMarketService>();
            marketServiceMock.Setup(x => x.GetPrice(coinSymbol)).ReturnsAsync(coinPrice);

            var walletService = new WalletService(ctx, marketServiceMock.Object);

            // Act & Assert
            Assert.ThrowsAsync<Exception>(async () => await walletService.BuyAsync(userId, coinSymbol, usdToBuy));
        }


        [Test]
        public async Task SellAsync_CoinSold()
        {
            // Arrange
            int coinPrice = 100;
            var userId = 1;
            var coinSymbol = "btc";
            var amount = 1.0;

            var ctx = DatabaseService.CreateDbContext("CoinSold");

            var coinToSell = DatabaseService.CreateWalletItem(userId, coinSymbol, amount);
            var usdtWalletItem = DatabaseService.CreateWalletItem(userId, "usdt", 0);

            ctx.WalletItems.Add(usdtWalletItem);
            ctx.WalletItems.Add(coinToSell);
            await ctx.SaveChangesAsync();

            var marketServiceMock = new Mock<IMarketService>();
            marketServiceMock.Setup(x => x.GetPrice(coinSymbol)).ReturnsAsync(coinPrice);

            var walletService = new WalletService(ctx, marketServiceMock.Object);

            // Act
            await walletService.SellAsync(userId, coinSymbol, coinToSell.Amount);

            // Assert
            Assert.AreEqual(0, coinToSell.Amount);
            Assert.AreEqual(usdtWalletItem.Amount, coinPrice * amount);
        }

        [Test]
        public async Task SendCryptoAsync_SuccessfulSent()
        {
            // Arrange
            var senderId = 1;
            var receiverId = 2;
            var symbol = "btc";
            var amount = 1.0;

            var ctx = DatabaseService.CreateDbContext("SuccessfulSent");

            var sender = DatabaseService.CreateUser(senderId);
            var receiver = DatabaseService.CreateUser(receiverId);

            var senderWalletItem = DatabaseService.CreateWalletItem(senderId, symbol, amount);
            var receiverWalletItem = DatabaseService.CreateWalletItem(senderId, symbol, amount);

            ctx.Users.Add(sender);
            ctx.Users.Add(receiver);
            ctx.WalletItems.Add(senderWalletItem);
            ctx.WalletItems.Add(receiverWalletItem);
            await ctx.SaveChangesAsync();

            var marketServiceMock = new Mock<IMarketService>();
            var notificationServiceMock = new Mock<INotificationService>();

            var walletService = new WalletService(ctx, notificationServiceMock.Object, marketServiceMock.Object);

            // Act
            await walletService.SendCryptoAsync(senderId, symbol, amount, receiverId);

            // Assert
            Assert.AreEqual(amount, receiverWalletItem.Amount);
            Assert.AreEqual(0, senderWalletItem.Amount);
        }

        [Test]
        public async Task SendCryptoAsync_NotEnoughBalanceToSend()
        {
            // Arrange
            var senderId = 1;
            var receiverId = 2;
            var symbol = "btc";
            var amount = 1.0;

            var ctx = DatabaseService.CreateDbContext("NotEnoughBalanceToSend");

            var sender = DatabaseService.CreateUser(senderId);
            var receiver = DatabaseService.CreateUser(receiverId);

            var senderWalletItem = DatabaseService.CreateWalletItem(senderId, symbol, amount);
            var receiverWalletItem = DatabaseService.CreateWalletItem(senderId, symbol, amount);

            ctx.Users.Add(sender);
            ctx.Users.Add(receiver);
            ctx.WalletItems.Add(senderWalletItem);
            ctx.WalletItems.Add(receiverWalletItem);
            await ctx.SaveChangesAsync();

            var marketServiceMock = new Mock<IMarketService>();
            var notificationServiceMock = new Mock<INotificationService>();

            var walletService = new WalletService(ctx, notificationServiceMock.Object, marketServiceMock.Object);

            // Act & Assert
            Assert.ThrowsAsync<Exception>(async () => await walletService.SendCryptoAsync(senderId, symbol, amount * 2, receiverId));
        }
    }
}

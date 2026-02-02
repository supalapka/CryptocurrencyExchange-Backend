using CryptocurrencyExchange.Exceptions;
using CryptocurrencyExchange.Models;
using CryptocurrencyExchange.Services;
using CryptocurrencyExchange.Services.Interfaces;
using CryptocurrencyExchange.Services.Wallet;
using Moq;
using NUnit.Framework;

namespace CryptocurrencyExchange.Tests.ServicesTests
{
    [TestFixture]
    public class WalletServiceTests
    {
        int userId = 1;
        int initialBalance = 500;

        private Mock<IUnitOfWork> unitOfWorkMock;
        private Mock<IWalletItemRepository> walletItemRepoMock;
        private Mock<IMarketService> marketServiceMock;
        private Mock<INotificationService> notificationServiceMock;
        private IWalletDomainService walletDomainService;

        private WalletService walletService;

        [SetUp]
        public void Setup()
        {
            unitOfWorkMock = new Mock<IUnitOfWork>();
            walletItemRepoMock = new Mock<IWalletItemRepository>();
            marketServiceMock = new Mock<IMarketService>();
            notificationServiceMock = new Mock<INotificationService>();
            walletDomainService = new WalletDomainService();

            walletService = new WalletService(
                notificationServiceMock.Object,
                marketServiceMock.Object,
                walletItemRepoMock.Object,
                unitOfWorkMock.Object,
                walletDomainService
            );
        }

        [Test]
        public async Task BuyAsync_WithEnoughBalance()
        {
            // Arrange
            var coinSymbol = "btc";
            var usdToBuy = 100;
            var coinPrice = 50;
            var expectedUsdtBalance = initialBalance - usdToBuy; // expect 400
            var expectedCoinAmount = usdToBuy / coinPrice; // expect 100 / 50 = 2 coins

            var usdtWalletItem = new WalletItem { UserId = userId, Symbol = "usdt", Amount = initialBalance };
            var coinToBuy = new WalletItem { UserId = userId, Symbol = coinSymbol, Amount = 0 };

            walletItemRepoMock.Setup(x => x.GetAsync(userId, "usdt")).ReturnsAsync(usdtWalletItem);
            walletItemRepoMock.Setup(x => x.GetAsync(userId, coinSymbol)).ReturnsAsync(coinToBuy);
            marketServiceMock.Setup(x => x.GetPrice(coinSymbol)).ReturnsAsync(coinPrice);

            // Act
            await walletService.BuyAsync(userId, coinSymbol, usdToBuy);

            // Assert
            Assert.AreEqual(expectedUsdtBalance, usdtWalletItem.Amount);
            Assert.AreEqual(expectedCoinAmount, coinToBuy.Amount);
        }


        [Test]
        public async Task BuyAsync_NotEnoughBalance_InsufficientBalanceException()
        {
            // Arrange
            var coinSymbol = "btc";
            var usdToBuy = initialBalance + 100;

            var usdtWalletItem = new WalletItem { UserId = userId, Symbol = "usdt", Amount = initialBalance };

            walletItemRepoMock.Setup(x => x.GetAsync(userId, "usdt")).ReturnsAsync(usdtWalletItem);

            // Act + Assert
            Assert.ThrowsAsync<InsufficientFundsException>(
               () => walletService.BuyAsync(userId, coinSymbol, usdToBuy)
            );
        }


        [Test]
        public async Task SellAsync_CoinSold()
        {
            // Arrange
            int coinPrice = 100;
            var userId = 1;
            var coinSymbol = "btc";
            var amount = 1.0;

            var ctx = DatabaseService.CreateInMemoryDbContext("CoinSold");

            var coinToSell = DatabaseService.CreateWalletItem(userId, coinSymbol, amount);
            var usdtWalletItem = DatabaseService.CreateWalletItem(userId, "usdt", 0);

            ctx.WalletItems.Add(usdtWalletItem);
            ctx.WalletItems.Add(coinToSell);
            await ctx.SaveChangesAsync();

            var marketServiceMock = new Mock<IMarketService>();
            marketServiceMock.Setup(x => x.GetPrice(coinSymbol)).ReturnsAsync(coinPrice);

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

            var walletItemRepoMock = new Mock<IWalletItemRepository>();
            walletItemRepoMock
                .Setup(x => x.GetAsync(senderId, symbol))
                .ReturnsAsync(senderWalletItem);

            walletItemRepoMock
                .Setup(x => x.GetAsync(receiverId, symbol))
                .ReturnsAsync(receiverWalletItem);


            // Act
            await walletService.SendCryptoAsync(senderId, symbol, amount, receiverId);

            // Assert
            Assert.AreEqual(0, senderWalletItem.Amount);
            Assert.AreEqual(amount, receiverWalletItem.Amount);

            unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Once);
        }

        [Test]
        public async Task SendCryptoAsync_NotEnoughBalanceToSend()
        {
            // Arrange
            var senderId = 1;
            var receiverId = 2;
            var symbol = "btc";
            var amount = 1.0;

            var ctx = DatabaseService.CreateInMemoryDbContext("NotEnoughBalanceToSend");

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

            //    var walletService = new WalletService(ctx, notificationServiceMock.Object, marketServiceMock.Object);

            // Act & Assert
            // Assert.ThrowsAsync<Exception>(async () => await walletService.SendCryptoAsync(senderId, symbol, amount * 2, receiverId));
        }
    }
}

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

            unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Once);  // mean CommitAsync should be called 
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

            unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Never); // mean CommitAsync should not be called
        }


        [Test]
        public async Task SellAsync_CoinSold()
        {
            // Arrange
            var coinSymbol = "btc";
            var coinsAmount = 1;
            var coinsToSell = 1;
            var coinPrice = 50;
            var expectedUsdtBalance = initialBalance + coinsToSell * coinPrice; // expect 550
            var expectedCoinAmount = 0;

            var usdtWalletItem = new WalletItem { UserId = userId, Symbol = "usdt", Amount = initialBalance };
            var coinToSell = new WalletItem { UserId = userId, Symbol = coinSymbol, Amount = coinsAmount };

            walletItemRepoMock.Setup(x => x.GetAsync(userId, "usdt")).ReturnsAsync(usdtWalletItem);
            walletItemRepoMock.Setup(x => x.GetAsync(userId, coinSymbol)).ReturnsAsync(coinToSell);
            marketServiceMock.Setup(x => x.GetPrice(coinSymbol)).ReturnsAsync(coinPrice);

            // Act
            await walletService.SellAsync(userId, coinSymbol, coinsToSell);

            // Assert
            Assert.AreEqual(expectedUsdtBalance, usdtWalletItem.Amount);
            Assert.AreEqual(expectedCoinAmount, coinToSell.Amount);

            unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Once);  // mean CommitAsync should be called 
        }
    }
}

using CryptocurrencyExchange.Core.Interfaces;
using CryptocurrencyExchange.Core.Interfaces.Services;
using CryptocurrencyExchange.Core.Models;
using CryptocurrencyExchange.Core.ValueObject;
using CryptocurrencyExchange.Exceptions;
using CryptocurrencyExchange.Services.Wallet;
using Moq;
using NUnit.Framework;

namespace CryptocurrencyExchange.Tests.ServicesTests
{
    [TestFixture]
    public class WalletServiceTests
    {
        int initialBalance = 500;

        private Mock<IUnitOfWork> unitOfWorkMock = null!;
        private Mock<IWalletItemRepository> walletItemRepoMock = null!;
        private Mock<IMarketService> marketServiceMock = null!;
        private Mock<IWalletDomainService> walletDomainService = null!;


        [SetUp]
        public void Setup()
        {
            unitOfWorkMock = new Mock<IUnitOfWork>();
            walletItemRepoMock = new Mock<IWalletItemRepository>();
            marketServiceMock = new Mock<IMarketService>();
            walletDomainService = new Mock<IWalletDomainService>();
        }

        [Test]
        public async Task BuyAsync_WithEnoughBalance()
        {
            // Arrange
            var coinSymbol = "btc";
            var usdToBuy = 100;

            var usdt = WalletItemMother.CreateUsdt(amount: 1000);
            var btc = WalletItemMother.CreateBtc(amount: 0);

            walletItemRepoMock
                .Setup(x => x.GetCoinsDataForTradeAsync(TestUser.DefaultId, coinSymbol))
                .ReturnsAsync(new TradeWalletItems(usdt, btc));

            unitOfWorkMock
                .Setup(x => x.ExecuteInTransactionAsync(It.IsAny<Func<Task>>()))
                .Returns<Func<Task>>(action => action());

            var walletService = new WalletService(
                marketServiceMock.Object,
                walletItemRepoMock.Object,
                unitOfWorkMock.Object,
                walletDomainService.Object
            );

            // Act
            await walletService.BuyAsync(TestUser.DefaultId, coinSymbol, usdToBuy);

            // Assert
            unitOfWorkMock.Verify(x => x.ExecuteInTransactionAsync(It.IsAny<Func<Task>>()),
                Times.Once);

            walletItemRepoMock.Verify
                (x => x.GetCoinsDataForTradeAsync(TestUser.DefaultId, coinSymbol), Times.Once);

            walletDomainService.Verify(service => service.Buy(usdt, btc,
                It.IsAny<decimal>(), It.IsAny<decimal>()), Times.Once);
        }


        [Test]
        public void BuyAsync_NotEnoughBalance_ThrowsInsufficientFundsException()
        {
            // Arrange
            walletItemRepoMock.Setup(x => x.GetCoinsDataForTradeAsync(TestUser.DefaultId, "btc"))
                .ReturnsAsync(new TradeWalletItems(It.IsAny<WalletItem>(), It.IsAny<WalletItem>()));

            walletDomainService.Setup(x => x.Buy(
                It.IsAny<WalletItem>(),
                It.IsAny<WalletItem>(),
                It.IsAny<decimal>(),
                It.IsAny<decimal>()))
                .Throws<InsufficientFundsException>();

            unitOfWorkMock
                .Setup(x => x.ExecuteInTransactionAsync(It.IsAny<Func<Task>>()))
                .Returns<Func<Task>>(action => action());

            var walletService = new WalletService(
                marketServiceMock.Object,
                walletItemRepoMock.Object,
                unitOfWorkMock.Object,
                walletDomainService.Object
            );

            // Act + Assert
            Assert.ThrowsAsync<InsufficientFundsException>(async () =>
            await walletService.BuyAsync(TestUser.DefaultId, "btc", 100));

            unitOfWorkMock.Verify(
                x => x.ExecuteInTransactionAsync(It.IsAny<Func<Task>>()),
                Times.Once);

            walletDomainService.Verify(x => x.Buy(
                It.IsAny<WalletItem>(),
                It.IsAny<WalletItem>(),
                100,
                It.IsAny<decimal>()),
                Times.Once);
        }


        [Test]
        public async Task SellAsync_CoinSold()
        {
            // Arrange
            var coinSymbol = "btc";
            decimal coinsToSell = 1;

            var usdtWalletItem = WalletItemMother.CreateUsdt(amount: initialBalance);
            var coinToSell = WalletItemMother.CreateBtc(amount: coinsToSell);

            walletItemRepoMock.Setup(x => x.GetCoinsDataForTradeAsync(TestUser.DefaultId, coinSymbol))
              .ReturnsAsync(new TradeWalletItems(usdtWalletItem, coinToSell));

            unitOfWorkMock
               .Setup(x => x.ExecuteInTransactionAsync(It.IsAny<Func<Task>>()))
               .Returns<Func<Task>>(action => action());

            var walletService = new WalletService(
                marketServiceMock.Object,
                walletItemRepoMock.Object,
                unitOfWorkMock.Object,
                walletDomainService.Object
            );

            // Act
            await walletService.SellAsync(TestUser.DefaultId, coinSymbol, coinsToSell);

            // Assert
            unitOfWorkMock.Verify(
                x => x.ExecuteInTransactionAsync(It.IsAny<Func<Task>>()),
                Times.Once);

            walletDomainService.Verify(x => x.Sell(usdtWalletItem, coinToSell,
                    coinsToSell, It.IsAny<decimal>()), Times.Once);
        }
    }
}

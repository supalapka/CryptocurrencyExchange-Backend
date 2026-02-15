using CryptocurrencyExchange.Core.Interfaces;
using CryptocurrencyExchange.Core.Interfaces.Repositories;
using CryptocurrencyExchange.Core.Interfaces.Services;
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


        [SetUp]
        public void Setup()
        {
            unitOfWorkMock = new Mock<IUnitOfWork>();
            walletItemRepoMock = new Mock<IWalletItemRepository>();
            marketServiceMock = new Mock<IMarketService>();
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

            marketServiceMock
                .Setup(x => x.GetPrice(coinSymbol))
                .ReturnsAsync(50000m);

            var walletService = new WalletService(
                marketServiceMock.Object,
                walletItemRepoMock.Object,
                unitOfWorkMock.Object
            );


            // Act
            await walletService.BuyAsync(TestUser.DefaultId, coinSymbol, usdToBuy);

            // Assert
            unitOfWorkMock.Verify(x => x.ExecuteInTransactionAsync(It.IsAny<Func<Task>>()),
                Times.Once);

            walletItemRepoMock.Verify
                (x => x.GetCoinsDataForTradeAsync(TestUser.DefaultId, coinSymbol), Times.Once);
        }


        [Test]
        public void BuyAsync_NotEnoughBalance_ThrowsInsufficientFundsException()
        {
            // Arrange
            var usdt = WalletItemMother.CreateUsdt(amount: 50);
            var btc = WalletItemMother.CreateBtc(amount: 0);

            walletItemRepoMock.Setup(x => x.GetCoinsDataForTradeAsync(TestUser.DefaultId, "btc"))
                .ReturnsAsync(new TradeWalletItems(usdt, btc));

            unitOfWorkMock
                .Setup(x => x.ExecuteInTransactionAsync(It.IsAny<Func<Task>>()))
                .Returns<Func<Task>>(action => action());

            var walletService = new WalletService(
                marketServiceMock.Object,
                walletItemRepoMock.Object,
                unitOfWorkMock.Object
            );

            // Act + Assert
            Assert.ThrowsAsync<InsufficientFundsException>(async () =>
            await walletService.BuyAsync(TestUser.DefaultId, "btc", 100));

            unitOfWorkMock.Verify(
                x => x.ExecuteInTransactionAsync(It.IsAny<Func<Task>>()),
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
         
            marketServiceMock
               .Setup(x => x.GetPrice(coinSymbol))
               .ReturnsAsync(50000m);

            unitOfWorkMock
               .Setup(x => x.ExecuteInTransactionAsync(It.IsAny<Func<Task>>()))
               .Returns<Func<Task>>(action => action());

            var walletService = new WalletService(
                marketServiceMock.Object,
                walletItemRepoMock.Object,
                unitOfWorkMock.Object
            );

            // Act
            await walletService.SellAsync(TestUser.DefaultId, coinSymbol, coinsToSell);

            // Assert
            unitOfWorkMock.Verify(
                x => x.ExecuteInTransactionAsync(It.IsAny<Func<Task>>()),
                Times.Once);
        }
    }
}

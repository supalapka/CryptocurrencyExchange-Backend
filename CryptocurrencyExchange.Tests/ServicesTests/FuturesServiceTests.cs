using CryptocurrencyExchange.Core.Interfaces;
using CryptocurrencyExchange.Core.Interfaces.Repositories;
using CryptocurrencyExchange.Core.Models;
using CryptocurrencyExchange.Core.ValueObject;
using CryptocurrencyExchange.Exceptions;
using CryptocurrencyExchange.Services;
using CryptocurrencyExchange.Services.Futures;
using Moq;
using NUnit.Framework;

namespace CryptocurrencyExchange.Tests.ServicesTests
{
    [TestFixture]
    public class FuturesServiceTests
    {
        const int userUsdtBalance = 1000;

        private Mock<IUnitOfWork> unitOfWorkMock = null!;
        private Mock<IWalletItemRepository> walletRepositoryMock = null!;
        private Mock<IFutureRepository> futureRepositoryMock = null!;
        private Mock<IFuturesDomainService> futuresDomainServiceMock = null!;

        [Test]
        public async Task CreateFutureAsync_WithSufficientBalance_ReturnsFutureId()
        {
            // Arrange
            var futureDto = new FutureDto
            {
                Symbol = "BTC",
                EntryPrice = 50000,
                Margin = 500,
                Leverage = 10,
                Position = PositionType.Long
            };

            var expectedFuture = new Future
            {
                Id = 1,
                UserId = TestUser.DefaultId,
                Margin = 500,
                Symbol = "BTC"
            };

            walletRepositoryMock
                .Setup(x => x.GetAsync(TestUser.DefaultId, "usdt"))
                .ReturnsAsync(WalletItemMother.CreateUsdt(amount: userUsdtBalance));

            futuresDomainServiceMock
                .Setup(x => x.OpenPosition(futureDto, It.IsAny<WalletItem>()))
                .Returns(expectedFuture);

            unitOfWorkMock
                .Setup(x => x.ExecuteInTransactionAsync(It.IsAny<Func<Task>>()))
                .Returns<Func<Task>>(action => action());

            var service = new FuturesService(
                futuresDomainServiceMock.Object,
                unitOfWorkMock.Object,
                walletRepositoryMock.Object,
                futureRepositoryMock.Object);

            // Act
            var resultId = await service.CreateFutureAsync(futureDto, TestUser.DefaultId);

            // Assert
            Assert.AreEqual(1, resultId);

            futureRepositoryMock.Verify(x => x.AddAsync(expectedFuture), Times.Once);
        }


        [Test]
        public void CreateFutureAsync_WithInsufficientBalance_ThrowExceptionAndDoNotSave()
        {
            // Arrange
            var futureDto = new FutureDto
            {
                Symbol = "BTC",
                EntryPrice = 50000,
                Margin = 500,
                Leverage = 10,
                Position = PositionType.Long
            };


            walletRepositoryMock
                .Setup(x => x.GetAsync(TestUser.DefaultId, "usdt"))
                .ReturnsAsync(WalletItemMother.CreateUsdt(amount: 1));

            futuresDomainServiceMock
                .Setup(x => x.OpenPosition(It.IsAny<FutureDto>(), It.IsAny<WalletItem>()))
                .Throws<InsufficientFundsException>();

            unitOfWorkMock
                .Setup(x => x.ExecuteInTransactionAsync(It.IsAny<Func<Task>>()))
                .Returns<Func<Task>>(action => action());

            var service = new FuturesService(
                futuresDomainServiceMock.Object,
                unitOfWorkMock.Object,
                walletRepositoryMock.Object,
                futureRepositoryMock.Object);

            // Act & Assert
            Assert.ThrowsAsync<InsufficientFundsException>(async () =>
              await service.CreateFutureAsync(futureDto, TestUser.DefaultId));

            futureRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Future>()), Times.Never);
        }



        [Test]
        public async Task ClosePosition_WhenCalled_Saved()
        {
            // Arrange
            const int margin = 1000;
            const decimal pnl = 200;

            var position = new Future
            {
                Id = 1,
                UserId = TestUser.DefaultId,
                Margin = margin,
                Position = PositionType.Long,
                IsCompleted = false
            };

            var usdtWallet = WalletItemMother.CreateUsdt(amount: userUsdtBalance);


            futureRepositoryMock.Setup(x => x.GetByIdAsync(position.Id))
                .ReturnsAsync(position);

            walletRepositoryMock.Setup(x => x.GetAsync(TestUser.DefaultId, "usdt"))
                .ReturnsAsync(usdtWallet);

            unitOfWorkMock.Setup(x => x.ExecuteInTransactionAsync(It.IsAny<Func<Task>>()))
                .Returns<Func<Task>>(action => action());

            futuresDomainServiceMock.Setup(x => x.ClosePosition(position, usdtWallet, pnl))
                .Callback<Future, WalletItem, decimal>((pos, wallet, profitAndLoss) =>
                {
                    pos.IsCompleted = true;
                });

            var service = new FuturesService(
                futuresDomainServiceMock.Object,
                unitOfWorkMock.Object,
                walletRepositoryMock.Object,
                futureRepositoryMock.Object);

            // Act
            await service.ClosePosition(position.Id, pnl, 9000);

            // Assert
            Assert.IsTrue(position.IsCompleted);

            futureRepositoryMock.Verify(x => x.AddPositionToHistoryAsync(It.IsAny<Future>(), It.IsAny<double>()), Times.Once);
        }



        [SetUp]
        public void Setup()
        {
            unitOfWorkMock = new Mock<IUnitOfWork>();
            walletRepositoryMock = new Mock<IWalletItemRepository>();
            futureRepositoryMock = new Mock<IFutureRepository>();
            futuresDomainServiceMock = new Mock<IFuturesDomainService>();
        }

    }

}

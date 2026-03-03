using CryptocurrencyExchange.Application.Wallet;
using CryptocurrencyExchange.Application.Wallets;
using CryptocurrencyExchange.Core.Interfaces;
using CryptocurrencyExchange.Core.Interfaces.Repositories;
using CryptocurrencyExchange.Core.Interfaces.Services;
using CryptocurrencyExchange.Core.Models;
using CryptocurrencyExchange.Core.ValueObject;
using CryptocurrencyExchange.Exceptions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;

namespace CryptocurrencyExchange.Tests.ServicesTests
{
    [TestFixture]
    public class WalletServiceTests
    {
        private Mock<IMarketService> _marketService;
        private Mock<IWalletItemRepository> _walletRepo;
        private Mock<IUnitOfWork> _uow;

        private int _btcPrice = 500;

        private WalletService _service;

        [SetUp]
        public void SetUp()
        {
            _marketService = new Mock<IMarketService>();
            _walletRepo = new Mock<IWalletItemRepository>();
            _uow = new Mock<IUnitOfWork>();

            _uow.Setup(x => x.ExecuteInTransactionAsync(It.IsAny<Func<Task>>()))
                .Returns<Func<Task>>(f => f());

            _service = new WalletService(
                _marketService.Object,
                _walletRepo.Object,
                _uow.Object,
                NullLogger<WalletService>.Instance
            );
        }

        [Test]
        public async Task BuyAsync_ShouldExecuteWalletBuyFlow()
        {
            // Arrange
            var usdt = WalletItemMother.CreateUsdt(amount: 1000);
            var btc = WalletItemMother.CreateBtc(amount: 0);

            _marketService
                .Setup(x => x.GetPrice(CoinSymbol.Btc.Value))
                .ReturnsAsync(_btcPrice);

            _walletRepo
                .Setup(x => x.GetCoinsDataForTradeAsync(TestUser.DefaultId, new CoinSymbol(CoinSymbol.Btc.Value)))
                .ReturnsAsync(new TradeWalletItems(usdt, btc));

            CoinTradeDto coinTradeDto = new CoinTradeDto
            (
                CoinSymbol: new CoinSymbol(CoinSymbol.Btc.Value),
                CoinAmount: 1
            );

            // Act
            await _service.BuyAsync(TestUser.DefaultId, coinTradeDto);

            // Assert
            _uow.Verify(x => x.ExecuteInTransactionAsync(It.IsAny<Func<Task>>()), Times.Once);
            _marketService.Verify(x => x.GetPrice(CoinSymbol.Btc.Value), Times.Once);

            _walletRepo.Verify(
                x => x.GetCoinsDataForTradeAsync(TestUser.DefaultId, new CoinSymbol(CoinSymbol.Btc.Value)),
                Times.Once);
        }

        [Test]
        public async Task SellAsync_ShouldExecuteWalletSellFlow()
        {
            // Arrange
            var usdt = WalletItemMother.CreateUsdt(amount: 0);
            var btc = WalletItemMother.CreateBtc(amount: 1);

            _marketService
                .Setup(x => x.GetPrice(CoinSymbol.Btc.Value))
                .ReturnsAsync(_btcPrice);

            _walletRepo
                .Setup(x => x.GetCoinsDataForTradeAsync(TestUser.DefaultId, new CoinSymbol(CoinSymbol.Btc.Value)))
                .ReturnsAsync(new TradeWalletItems(usdt, btc));

            CoinTradeDto coinTradeDto = new CoinTradeDto(CoinSymbol.Btc, CoinAmount: 1);

            // Act
            await _service.SellAsync(TestUser.DefaultId, coinTradeDto);

            // Assert
            _uow.Verify(x => x.ExecuteInTransactionAsync(It.IsAny<Func<Task>>()), Times.Once);
            _marketService.Verify(x => x.GetPrice(CoinSymbol.Btc.Value), Times.Once);

            _walletRepo.Verify(
                x => x.GetCoinsDataForTradeAsync(TestUser.DefaultId, new CoinSymbol(CoinSymbol.Btc.Value)),
                Times.Once);
        }

        [Test]
        public void Buy_WhenNotEnoughBalance_ShouldThrow_InsufficientFundsException()
        {
            // Arrange
            var usdt = WalletItemMother.CreateUsdt(amount: 0);
            var btc = WalletItemMother.CreateBtc(amount: 0);

            _marketService
                .Setup(x => x.GetPrice(CoinSymbol.Btc.Value))
                .ReturnsAsync(_btcPrice);

            _walletRepo
                .Setup(x => x.GetCoinsDataForTradeAsync(TestUser.DefaultId, new CoinSymbol(CoinSymbol.Btc.Value)))
                .ReturnsAsync(new TradeWalletItems(usdt, btc));

            CoinTradeDto coinTradeDto = new CoinTradeDto(CoinSymbol.Btc, CoinAmount: 100);

            // Act & Assert
            Assert.ThrowsAsync<InsufficientFundsException>(async () =>
                await _service.BuyAsync(TestUser.DefaultId, coinTradeDto));
        }

        [Test]
        public void Sell_WhenNotEnoughBalance_ShouldThrow_InsufficientFundsException()
        {
            // Arrange
            var usdt = WalletItemMother.CreateUsdt(amount: 0);
            var btc = WalletItemMother.CreateBtc(amount: 0);

            _marketService
                .Setup(x => x.GetPrice(CoinSymbol.Btc.Value))
                .ReturnsAsync(_btcPrice);

            _walletRepo
                .Setup(x => x.GetCoinsDataForTradeAsync(TestUser.DefaultId, new CoinSymbol(CoinSymbol.Btc.Value)))
                .ReturnsAsync(new TradeWalletItems(usdt, btc));

            CoinTradeDto coinTradeDto = new CoinTradeDto(CoinSymbol.Btc, CoinAmount: 100);

            // Act & Assert
            Assert.ThrowsAsync<InsufficientFundsException>(async () =>
                await _service.SellAsync(TestUser.DefaultId, coinTradeDto));
        }

        [Test]
        public async Task GetCoinAmountAsync_WhenItemNotExists_ShouldReturnZero()
        {
            _walletRepo
                .Setup(x => x.GetAsync(TestUser.DefaultId, new CoinSymbol(CoinSymbol.Btc.Value)))
                .ReturnsAsync((WalletItem?)null);

            var amount = await _service.GetCoinAmountAsync(TestUser.DefaultId, new CoinSymbol(CoinSymbol.Btc.Value));

            Assert.That(amount, Is.EqualTo(0));
        }
    }
}

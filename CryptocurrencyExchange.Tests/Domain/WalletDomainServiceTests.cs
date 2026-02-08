using CryptocurrencyExchange.Services.Wallet;
using NUnit.Framework;

namespace CryptocurrencyExchange.Tests.Domain
{
    [TestFixture]
    public class WalletDomainServiceTests
    {
        private WalletDomainService walletDomainService = null!;

        private const decimal BTC_PRICE = 20m;

        [SetUp]
        public void SetUp()
        {
            walletDomainService = new WalletDomainService();
        }

        [Test]
        public void Buy_WhenEnoughUsdt_ShouldDecreaseUsdtAndIncreaseCoin()
        {
            // Arrange
            decimal initialUsdtAmount = 100;
            decimal usdtToSpend = 20;

            decimal expectedUsdtAfterBuy = 80;
            decimal expectedCoinAmount = 1;

            var usdt = WalletItemMother.CreateUsdt(amount: initialUsdtAmount);
            var btc = WalletItemMother.CreateBtc(amount: 0);

            // Act
            walletDomainService.Buy(usdt, btc, usdtToSpend, BTC_PRICE);

            // Assert
            Assert.AreEqual(expectedUsdtAfterBuy, usdt.Amount);
            Assert.AreEqual(expectedCoinAmount, btc.Amount);
        }


        [Test]
        public void Buy_WhenUsdtIsInsufficient_ShouldThrowInsufficientFundsException()
        {
            // Arrange
            decimal usdtToSpend = 100;

            var usdt = WalletItemMother.CreateUsdt(amount: 50);
            var btc = WalletItemMother.CreateBtc(amount: 0);

            // Act and Assert
            Assert.Throws<Exceptions.InsufficientFundsException>(() =>
            walletDomainService.Buy(usdt, btc, usdtToSpend, BTC_PRICE));
        }


        [Test]
        public void Sell_WhenEnoughCoin_ShouldDecreaseCoinAndIncreaseUsdt()
        {
            // Arrange
            decimal usdtAmount = 0;

            decimal expectedUsdtAfterSell = 20;
            decimal expectedCoinAmount = 0;

            var usdt = WalletItemMother.CreateUsdt(amount: usdtAmount);
            var btc = WalletItemMother.CreateBtc(amount: 1);

            // Act
            walletDomainService.Sell(usdt, btc, 1, BTC_PRICE);

            // Assert
            Assert.AreEqual(expectedUsdtAfterSell, usdt.Amount);
            Assert.AreEqual(expectedCoinAmount, btc.Amount);
        }


        [Test]
        public void Sell_WhenBtcIsInsufficient_ShouldThrowInsufficientFundsException()
        {
            // Arrange
            decimal btcToSell = 1;

            var usdt = WalletItemMother.CreateUsdt(amount: 100);
            var btc = WalletItemMother.CreateBtc(amount: 0);

            // Act and Assert
            Assert.Throws<Exceptions.InsufficientFundsException>(() =>
            walletDomainService.Sell(usdt, btc, btcToSell, BTC_PRICE));
        }

    }
}

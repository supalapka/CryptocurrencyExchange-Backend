using CryptocurrencyExchange.Services.Wallet;
using NUnit.Framework;

namespace CryptocurrencyExchange.Tests.Domain
{
    [TestFixture]
    public class WalletDomainServiceTests
    {
        private WalletDomainService walletDomainService = null!;

        private const int USER_ID = 1;
        private const decimal BTC_PRICE = 20m;
        private const string BTC_SYMBOL = "btc";

        [SetUp]
        public void SetUp()
        {
            walletDomainService = new WalletDomainService();
        }

        [Test]
        public void Buy_WhenEnoughUsdt_ShouldDecreaseUsdtAndIncreaseCoin()
        {
            // Arrange
            decimal usdtAmount = 100;

            decimal usdtToSpend = 20;

            decimal expectedUsdtAfterBuy = 80;
            decimal expectedCoinAmount = 1;

            var usdt = WalletItemMother.CreateUsdt(USER_ID, usdtAmount);
            var btc = WalletItemMother.CreateItem(USER_ID, BTC_SYMBOL, 0);

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
            decimal usdtAmount = 50;
            decimal usdtToSpend = 100;

            var usdt = WalletItemMother.CreateUsdt(USER_ID, usdtAmount);
            var btc = WalletItemMother.CreateItem(USER_ID, BTC_SYMBOL, 0);

            // Act and Assert
            Assert.Throws<Exceptions.InsufficientFundsException>(() =>
            walletDomainService.Buy(usdt, btc, usdtToSpend, BTC_PRICE));
        }


        [Test]
        public void Sell_WhenEnoughCoin_ShouldDecreaseCoinAndIncreaseUsdt()
        {
            // Arrange
            decimal usdtAmount = 0;
            decimal coinsToSell = 1;

            decimal expectedUsdtAfterSell = 20;
            decimal expectedCoinAmount = 0;

            var usdt = WalletItemMother.CreateUsdt(USER_ID, usdtAmount);
            var btc = WalletItemMother.CreateItem(USER_ID, BTC_SYMBOL, coinsToSell);

            // Act
            walletDomainService.Sell(usdt, btc, coinsToSell, BTC_PRICE);

            // Assert
            Assert.AreEqual(expectedUsdtAfterSell, usdt.Amount);
            Assert.AreEqual(expectedCoinAmount, btc.Amount);
        }


        [Test]
        public void Sell_WhenBtcIsInsufficient_ShouldThrowInsufficientFundsException()
        {
            // Arrange
            decimal btcAmount = 0;
            decimal btcToSell = 1;

            var usdt = WalletItemMother.CreateUsdt(USER_ID, 100);
            var btc = WalletItemMother.CreateItem(USER_ID, BTC_SYMBOL, btcAmount);

            // Act and Assert
            Assert.Throws<Exceptions.InsufficientFundsException>(() =>
            walletDomainService.Sell(usdt, btc, btcToSell, BTC_PRICE));
        }

    }
}

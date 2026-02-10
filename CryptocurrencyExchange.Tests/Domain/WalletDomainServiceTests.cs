using CryptocurrencyExchange.Core.Domain;
using NUnit.Framework;

namespace CryptocurrencyExchange.Tests.Domain
{
    [TestFixture]
    public class WalletDomainServiceTests
    {
        private WalletDomainService walletDomainService = null!;

        [SetUp]
        public void SetUp()
        {
            walletDomainService = new WalletDomainService();
        }


        [Test]
        public void Buy_WhenEnoughUsdt_ShouldDecreaseUsdtAndIncreaseCoin()
        {
            // Arrange
            var usdt = WalletItemMother.CreateUsdt(amount: 100);
            var btc = WalletItemMother.CreateBtc(amount: 0);

            // Act
            walletDomainService.Buy(usdt, btc, usd: 20, coinPrice: 20);

            // Assert
            Assert.AreEqual(80, usdt.Amount);
            Assert.AreEqual(1, btc.Amount);
        }


        [Test]
        public void Buy_WhenUsdtIsInsufficient_ShouldThrowInsufficientFundsException()
        {
            // Arrange
            var usdt = WalletItemMother.CreateUsdt(amount: 0);
            var btc = WalletItemMother.CreateBtc(amount: 0);

            // Act and Assert
            Assert.Throws<Exceptions.InsufficientFundsException>(() =>
            walletDomainService.Buy(usdt, btc, usd: 9999, coinPrice: 20));
        }


        [Test]
        public void Sell_WhenEnoughBtc_DecreasesBtcAndIncreasesUsdt()
        {
            var usdt = WalletItemMother.CreateUsdt(amount: 0);
            var btc = WalletItemMother.CreateBtc(amount: 1);

            walletDomainService.Sell(usdt, btc, amount: 1, coinPrice: 20);

            Assert.AreEqual(20, usdt.Amount);
            Assert.AreEqual(0, btc.Amount);
        }



        [Test]
        public void Sell_WhenBtcIsInsufficient_ShouldThrowInsufficientFundsException()
        {
            // Arrange
            var usdt = WalletItemMother.CreateUsdt(amount: 0);
            var btc = WalletItemMother.CreateBtc(amount: 0);

            // Act and Assert
            Assert.Throws<Exceptions.InsufficientFundsException>(() =>
            walletDomainService.Sell(usdt, btc, amount: 999, coinPrice: 20));
        }

    }
}

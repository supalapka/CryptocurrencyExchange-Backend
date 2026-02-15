using CryptocurrencyExchange.Core.Domain.Wallets;
using CryptocurrencyExchange.Exceptions;
using NUnit.Framework;

namespace CryptocurrencyExchange.Tests.Domain
{
    [TestFixture]
    internal class WalletTests
    {
        [Test]
        public void GetBalance_ShouldReturnCorrectAmount()
        {
            // Arrange
            var usdt = WalletItemMother.CreateUsdt(amount: 1000);

            // Act
            Wallet wallet = new Wallet(TestUser.DefaultId, new[] { usdt });

            // Assert
            Assert.That(wallet.GetBalance("usdt"), Is.EqualTo(1000));
        }

        [Test]
        public void Buy_WhenEnoughBalance_ShouldDecreaseUsdtAndIncreaseBtc()
        {
            // Arrange
            var usdt = WalletItemMother.CreateUsdt(amount: 1000);
            var btc = WalletItemMother.CreateBtc(amount: 0);

            Wallet wallet = new Wallet(TestUser.DefaultId, new[] { usdt, btc });

            // Act
            wallet.Buy("btc", usd: 500, coinPrice: 500);

            // Assert
            Assert.That(usdt.Amount, Is.EqualTo(500));
            Assert.That(btc.Amount, Is.EqualTo(1));
        }

        [Test]
        public void Buy_NotEnoughBalance_ShouldThrowException()
        {
            // Arrange
            var usdt = WalletItemMother.CreateUsdt(amount: 0);
            var btc = WalletItemMother.CreateBtc(amount: 0);

            Wallet wallet = new Wallet(TestUser.DefaultId, new[] { usdt, btc });

            // Act & Assert
            Assert.Throws<InsufficientFundsException>(() =>
                wallet.Buy("btc", usd: 500, coinPrice: 500));
        }

        [Test]
        public void Sell_WhenEnoughBalance_ShouldEcreaseUsdtAndDecreaseBtc()
        {
            // Arrange
            var usdt = WalletItemMother.CreateUsdt(amount: 0);
            var btc = WalletItemMother.CreateBtc(amount: 1);

            Wallet wallet = new Wallet(TestUser.DefaultId, new[] { usdt, btc });

            // Act
            wallet.Sell("btc", amount: 1, coinPrice: 500);

            // Assert
            Assert.That(wallet.GetBalance("usdt"), Is.EqualTo(500));
            Assert.That(wallet.GetBalance("btc"), Is.EqualTo(0));
        }

        [Test]
        public void Sell_NotEnoughBalance_ShouldThrowException()
        {
            // Arrange
            var usdt = WalletItemMother.CreateUsdt(amount: 0);
            var btc = WalletItemMother.CreateBtc(amount: 0);

            Wallet wallet = new Wallet(TestUser.DefaultId, new[] { usdt, btc });

            // Act & Assert
            Assert.Throws<InsufficientFundsException>(() =>
                wallet.Sell("btc", amount: 1, coinPrice: 500));
        }
    }
}

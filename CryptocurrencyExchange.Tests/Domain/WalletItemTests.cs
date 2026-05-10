using CryptocurrencyExchange.Core.Models;
using NUnit.Framework;

namespace CryptocurrencyExchange.Tests.Domain
{
    [TestFixture]
    internal class WalletItemTests
    {
        [Test]
        public void HasSufficientBalance_WhenAmountEqualsBalance_ReturnsTrue()
        {
            var item = WalletItemMother.CreateUsdt(100m);

            Assert.That(item.HasSufficientBalance(100m), Is.True);
        }

        [Test]
        public void HasSufficientBalance_WhenAmountBelowBalance_ReturnsTrue()
        {
            var item = WalletItemMother.CreateUsdt(100m);

            Assert.That(item.HasSufficientBalance(50m), Is.True);
        }

        [Test]
        public void HasSufficientBalance_WhenAmountExceedsBalance_ReturnsFalse()
        {
            var item = WalletItemMother.CreateUsdt(50m);

            Assert.That(item.HasSufficientBalance(100m), Is.False);
        }
    }
}

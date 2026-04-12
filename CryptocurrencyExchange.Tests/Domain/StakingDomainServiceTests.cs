using CryptocurrencyExchange.Core.Domain;
using CryptocurrencyExchange.Core.Models;
using NUnit.Framework;

namespace CryptocurrencyExchange.Tests.Domain
{
    [TestFixture]
    public class StakingDomainServiceTests
    {
        [Test]
        public void CreateStaking_SetsEndDateToStartDatePlusDurationInMonthTimesThirtyDays()
        {
            var walletItem = WalletItemMother.CreateItem("BTC", 100m);
            var stakingCoin = new StakingCoin { Id = 1, Symbol = "BTC", RatePerMonth = 1.5f };
            var service = new StakingDomainService();

            var result = service.CreateStaking(walletItem, stakingCoin, 50m, 3);

            Assert.That(result.EndDate, Is.EqualTo(DateTime.Today.AddDays(3 * 30)));
        }
    }
}

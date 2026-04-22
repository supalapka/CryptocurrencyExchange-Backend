using CryptocurrencyExchange.Core.Interfaces;
using CryptocurrencyExchange.Infrastructure.Market;
using Moq;
using NUnit.Framework;

namespace CryptocurrencyExchange.Tests.Infrastructure.Market
{
    [TestFixture]
    internal class RoutingApiMarketPriceProviderTests
    {
        [Test]
        public async Task GetPriceInUsdt_WhenFirstClientSucceeds_ReturnsPrice()
        {
            var client1 = new Mock<IMarketApiClient>();
            var client2 = new Mock<IMarketApiClient>();
            client1.Setup(c => c.GetUsdtPriceAsync("BTC")).ReturnsAsync(50000m);

            var provider = new RoutingApiMarketPriceProvider(new[] { client1.Object, client2.Object });

            var result = await provider.GetPriceInUsdt("BTC");

            Assert.That(result, Is.EqualTo(50000m));
            client2.Verify(c => c.GetUsdtPriceAsync(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task GetPriceInUsdt_WhenFirstClientThrows_FallsBackToSecond()
        {
            var client1 = new Mock<IMarketApiClient>();
            var client2 = new Mock<IMarketApiClient>();
            client1.Setup(c => c.GetUsdtPriceAsync("BTC")).ThrowsAsync(new HttpRequestException("binance down"));
            client2.Setup(c => c.GetUsdtPriceAsync("BTC")).ReturnsAsync(50000m);

            var provider = new RoutingApiMarketPriceProvider(new[] { client1.Object, client2.Object });

            var result = await provider.GetPriceInUsdt("BTC");

            Assert.That(result, Is.EqualTo(50000m));
            client2.Verify(c => c.GetUsdtPriceAsync("BTC"), Times.Once);
        }

        [Test]
        public void GetPriceInUsdt_WhenAllClientsFail_ThrowsLastException()
        {
            var client1 = new Mock<IMarketApiClient>();
            var client2 = new Mock<IMarketApiClient>();
            var lastException = new InvalidOperationException("last error");
            client1.Setup(c => c.GetUsdtPriceAsync("BTC")).ThrowsAsync(new HttpRequestException("first"));
            client2.Setup(c => c.GetUsdtPriceAsync("BTC")).ThrowsAsync(lastException);

            var provider = new RoutingApiMarketPriceProvider(new[] { client1.Object, client2.Object });

            var ex = Assert.ThrowsAsync<InvalidOperationException>(() => provider.GetPriceInUsdt("BTC"));
            Assert.That(ex!.Message, Is.EqualTo("last error"));
        }

        [Test]
        public void Constructor_WhenNoClientsRegistered_ThrowsInvalidOperationException()
        {
            Assert.Throws<InvalidOperationException>(() =>
                new RoutingApiMarketPriceProvider(Enumerable.Empty<IMarketApiClient>()));
        }
    }
}

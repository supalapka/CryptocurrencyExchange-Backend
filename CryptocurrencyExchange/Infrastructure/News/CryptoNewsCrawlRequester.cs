using Crawler.Contracts;
using CryptocurrencyExchange.Core.Interfaces.Services;
using MassTransit;

namespace CryptocurrencyExchange.Infrastructure.News
{
    public class CryptoNewsCrawlRequester : ICryptoNewsUpdateRequester
    {
        private readonly ISendEndpointProvider sendEndpointProvider;

        public CryptoNewsCrawlRequester(ISendEndpointProvider sendEndpointProvider)
        {
            this.sendEndpointProvider = sendEndpointProvider;
        }

        public async Task TriggerUpdate(string coin)
        {
            var endpoint = await sendEndpointProvider.GetSendEndpoint(
                new Uri("queue:crawler.start")
            );

            await endpoint.Send(new StartCrawl(coin));
        }
    }
}

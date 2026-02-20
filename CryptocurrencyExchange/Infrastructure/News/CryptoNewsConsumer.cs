using Crawler.Contracts;
using CryptocurrencyExchange.Core.Entities;
using CryptocurrencyExchange.Core.Interfaces.Repositories;
using MassTransit;

namespace CryptocurrencyExchange.Infrastructure.News
{
    public class CryptoNewsConsumer : IConsumer<UrlMatched>
    {
        private readonly ICryptoNewsRepository repository;

        public CryptoNewsConsumer(ICryptoNewsRepository repository)
        {
            this.repository = repository;
        }

        public async Task Consume(ConsumeContext<UrlMatched> context)
        {
            var message = context.Message;

            var news = new CryptoNews
            {
                Coin = message.Coin,
                Title = message.Title,
                Url = message.Url,
            };

            await repository.AddAsync(news);
        }
    }
}

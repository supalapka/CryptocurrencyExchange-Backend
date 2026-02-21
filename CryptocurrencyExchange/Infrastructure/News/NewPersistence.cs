using CryptocurrencyExchange.Core.Entities;
using CryptocurrencyExchange.Core.Interfaces.Repositories;

namespace CryptocurrencyExchange.Infrastructure.News
{
    public class NewPersistence : ICryptoNewsRepository
    {
        public Task AddAsync(CryptoNews news)
        {
            CryptoNewsInMemoryStore.News.TryAdd(news.Url, news);
            return Task.CompletedTask;
        }
    }
}

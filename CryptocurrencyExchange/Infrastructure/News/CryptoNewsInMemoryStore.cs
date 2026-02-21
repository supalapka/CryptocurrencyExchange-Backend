using CryptocurrencyExchange.Core.Entities;
using System.Collections.Concurrent;

namespace CryptocurrencyExchange.Infrastructure.News
{
    public static class CryptoNewsInMemoryStore
    {
        // key = Url
        public static readonly ConcurrentDictionary<string, CryptoNews> News = new();
    }
}

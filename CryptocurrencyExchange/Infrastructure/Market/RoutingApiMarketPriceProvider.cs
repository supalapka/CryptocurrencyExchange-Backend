using CryptocurrencyExchange.Core.Interfaces;

namespace CryptocurrencyExchange.Infrastructure.Market
{
    public class RoutingApiMarketPriceProvider : IMarketPriceProvider
    {
        private readonly IReadOnlyList<IMarketApiClient> _clients;
        private int _index = -1;

        public RoutingApiMarketPriceProvider(IEnumerable<IMarketApiClient> clients)
        {
            _clients = clients.ToList();
            if (_clients.Count == 0)
                throw new InvalidOperationException("No market api clients registered");
        }

        public async Task<decimal> GetPriceInUsdt(string coinSymbol)
        {
            int startIndex = Interlocked.Increment(ref _index);
            Exception? lastException = null;

            for (int i = 0; i < _clients.Count; i++)
            {
                var client = _clients[((startIndex + i) & int.MaxValue) % _clients.Count];
                try
                {
                    return await client.GetUsdtPriceAsync(coinSymbol);
                }
                catch (Exception ex)
                {
                    lastException = ex;
                }
            }

            throw lastException!;
        }
    }
}

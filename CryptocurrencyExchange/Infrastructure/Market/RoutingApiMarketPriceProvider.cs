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

        public Task<decimal> GetPriceInUsdt(string coinSymbol)
        {
            int next = GetNextClientIndexThreadSafe();
            var client = _clients[GetRoundRobinClientIndex(next)];

            return client.GetUsdtPriceAsync(coinSymbol);
        }

        private int GetNextClientIndexThreadSafe()
        {
            return Interlocked.Increment(ref _index);
        }

        private int GetRoundRobinClientIndex(int counter)
        {
            return counter % _clients.Count;
        }
    }
}

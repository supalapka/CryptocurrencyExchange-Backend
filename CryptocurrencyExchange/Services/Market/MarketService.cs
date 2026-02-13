using CryptocurrencyExchange.Infrastructure.Market;
using CryptocurrencyExchange.Services.Interfaces;

namespace CryptocurrencyExchange.Services.Market
{
    public class MarketService : IMarketService
    {

        private readonly IMarketApiClient _marketApiClient;

        public MarketService(IMarketApiClient marketApiClient)
        {
            _marketApiClient = marketApiClient;
        }

        public async Task<decimal> GetPrice(string coinSymbol)
        {
            return await _marketApiClient.GetUsdtPriceAsync(coinSymbol);
        }

    }
}

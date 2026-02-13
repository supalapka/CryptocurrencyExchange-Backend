using CryptocurrencyExchange.Core.Interfaces;
using CryptocurrencyExchange.Core.Interfaces.Services;

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

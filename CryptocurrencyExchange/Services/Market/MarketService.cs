using CryptocurrencyExchange.Core.Interfaces;
using CryptocurrencyExchange.Core.Interfaces.Services;

namespace CryptocurrencyExchange.Services.Market
{
    public class MarketService : IMarketService
    {
        private readonly IMarketPriceProvider _marketPriceProvider;

        public MarketService(IMarketPriceProvider marketPriceProvider)
        {
            _marketPriceProvider = marketPriceProvider;
        }

        public async Task<decimal> GetPrice(string coinSymbol)
        {
            return await _marketPriceProvider.GetPriceInUsdt(coinSymbol);
        }
    }
}

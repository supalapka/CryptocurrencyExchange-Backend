using CryptocurrencyExchange.Core.Interfaces;

namespace CryptocurrencyExchange.Infrastructure.Market
{
    public class ApiMarketPriceProvider : IMarketPriceProvider
    {
        private readonly IMarketApiClient _marketApiClient;

        public ApiMarketPriceProvider(IMarketApiClient marketApiClient)
        {
            _marketApiClient = marketApiClient;
        }

        public async Task<decimal> GetPriceInUsdt(string coinSymbol)
        {
            return await _marketApiClient.GetUsdtPriceAsync(coinSymbol);
        }
    }
}

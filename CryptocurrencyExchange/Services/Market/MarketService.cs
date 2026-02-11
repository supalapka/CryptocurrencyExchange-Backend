using CryptocurrencyExchange.InfrastructureProject.Market;
using CryptocurrencyExchange.Services.Interfaces;
using Newtonsoft.Json;

namespace CryptocurrencyExchange.Services.Market
{
    public class MarketService : IMarketService
    {
        private const string USDT_SYMBOL = "USDT";

        private readonly IMarketApiClient _marketApiClient;

        public MarketService(IMarketApiClient marketApiClient)
        {
            _marketApiClient = marketApiClient;
        }

        public async Task<decimal> GetPrice(string coinSymbol)
        {
            var normalizedSymbol = NormalizeSymbol(coinSymbol);

            var content = await _marketApiClient.GetPriceRawAsync(normalizedSymbol);

            return ParsePrice(content);
        }

        private string NormalizeSymbol(string coinSymbol)
        {
            var normalizedSymbol = coinSymbol.ToUpper();
            if (!normalizedSymbol.EndsWith(USDT_SYMBOL))
                normalizedSymbol += USDT_SYMBOL;
            return normalizedSymbol;
        }

        private decimal ParsePrice(string content)
        {
            var dto = JsonConvert.DeserializeObject<PriceResponseDto>(content)
              ?? throw new InvalidOperationException("Empty price response");

            if (dto.Price is null)
                throw new InvalidOperationException("Price not found in response");

            return (decimal)dto.Price;
        }

    }
}

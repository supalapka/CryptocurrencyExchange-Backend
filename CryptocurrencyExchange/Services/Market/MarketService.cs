using CryptocurrencyExchange.Services.Interfaces;
using Newtonsoft.Json;

namespace CryptocurrencyExchange.Services.Market
{
    public class MarketService : IMarketService
    {
        private const string USDT_SYMBOL = "USDT";

        private readonly HttpClient httpClient;

        public MarketService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<decimal> GetPrice(string coinSymbol)
        {
            var normalizedSymbol = NormalizeSymbol(coinSymbol);

            HttpResponseMessage response = await httpClient.GetAsync($"ticker/price?symbol={normalizedSymbol}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

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

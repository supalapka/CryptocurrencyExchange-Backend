using CryptocurrencyExchange.Services.Interfaces;
using Newtonsoft.Json.Linq;

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

            var jObject = JObject.Parse(content);

            return (decimal)jObject["price"];
        }

        private string NormalizeSymbol(string coinSymbol)
        {
            var normalizedSymbol = coinSymbol.ToUpper();
            if (!normalizedSymbol.EndsWith(USDT_SYMBOL))
                normalizedSymbol += USDT_SYMBOL;
            return normalizedSymbol;
        }

    }
}

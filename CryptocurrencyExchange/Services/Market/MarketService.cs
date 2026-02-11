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
            coinSymbol = coinSymbol.ToUpper();

            if (!coinSymbol.EndsWith(USDT_SYMBOL))
                coinSymbol += USDT_SYMBOL;

            HttpResponseMessage response = await httpClient.GetAsync($"ticker/price?symbol={coinSymbol}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            var jObject = JObject.Parse(content);

            return (decimal)jObject["price"];
        }

    }
}

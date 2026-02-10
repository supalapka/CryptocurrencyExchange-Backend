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

        public async Task<List<string>> GetSymbolsByPage()
        {
            HttpResponseMessage response = await httpClient.GetAsync("ticker/24hr");
            response.EnsureSuccessStatusCode();

            string responseBody = await response.Content.ReadAsStringAsync();

            List<string> symbols = new List<string>();

            JArray marketData = JArray.Parse(responseBody);
            foreach (JObject market in marketData)
            {
                symbols.Add(market["symbol"].ToString());
            }

            symbols = symbols.Where(x => x.Contains(USDT_SYMBOL)).Take(100).ToList();
            return symbols;
        }

    }
}

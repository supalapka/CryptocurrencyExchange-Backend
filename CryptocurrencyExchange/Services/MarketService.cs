using Newtonsoft.Json.Linq;

namespace CryptocurrencyExchange.Services
{
    public class MarketService : IMarketService
    {
        public async Task<decimal> GetPrice(string coinSymbol)
        {
            var baseUrl = "https://api.binance.com";
            var httpClient = new HttpClient { BaseAddress = new Uri(baseUrl) };
            coinSymbol = coinSymbol.ToUpper();
            if (!coinSymbol.EndsWith("USDT"))
                coinSymbol += "USDT";
            var endpoint = $"/api/v3/ticker/price?symbol={coinSymbol}";
            var response = await httpClient.GetAsync(endpoint);
            var content = await response.Content.ReadAsStringAsync();
            var jObject = JObject.Parse(content);

            return (decimal)jObject["price"];
        }

        public List<string> GetSymbolsByPage()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://api.binance.com/api/v3/");
            HttpResponseMessage response = client.GetAsync("ticker/24hr").Result;
            string responseBody = response.Content.ReadAsStringAsync().Result;

            List<string> symbols = new List<string>();

            JArray marketData = JArray.Parse(responseBody);
            foreach (JObject market in marketData)
            {
                symbols.Add(market["symbol"].ToString());
            }

            symbols = symbols.Where(x => x.Contains("USDT")).Take(100).ToList();
            return symbols;
        }

    }
}

using CryptocurrencyExchange.Core.Interfaces;
using Newtonsoft.Json.Linq;
using System.Globalization;

namespace CryptocurrencyExchange.Infrastructure.Market
{
    public class BybitMarketApiClient : IMarketApiClient
    {
        private const string UsdtSymbol = "USDT";
        private readonly HttpClient _httpClient;

        public BybitMarketApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://api.bybit.com/");
        }

        public async Task<decimal> GetUsdtPriceAsync(string symbol)
        {
            var normalizedSymbol = NormalizeSymbol(symbol);
            var coinData = await GetCoinDataResponse(normalizedSymbol);

            return ParsePrice(coinData);
        }

        private string NormalizeSymbol(string coinSymbol)
        {
            var normalizedSymbol = coinSymbol.ToUpper();
            if (!normalizedSymbol.EndsWith(UsdtSymbol))
                normalizedSymbol += UsdtSymbol;

            return normalizedSymbol;
        }

        private async Task<string> GetCoinDataResponse(string symbol)
        {
            using var response = await _httpClient.GetAsync($"/v5/market/tickers?category=spot&symbol={symbol}");
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }

        private decimal ParsePrice(string content)
        {
            var json = JObject.Parse(content);
            var lastPrice = json["result"]?["list"]?.First?["lastPrice"]?.Value<string>()
                ?? throw new InvalidOperationException("LastPrice not found in Bybit response");

            return decimal.Parse(lastPrice, CultureInfo.InvariantCulture);
        }
    }
}

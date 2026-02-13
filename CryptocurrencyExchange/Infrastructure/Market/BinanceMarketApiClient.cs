using CryptocurrencyExchange.Core.Interfaces;
using Newtonsoft.Json;
using System.Globalization;

namespace CryptocurrencyExchange.Infrastructure.Market
{
    public sealed class BinanceMarketApiClient : IMarketApiClient
    {
        private record PriceResponseDto
        {
            public string? Price { get; init; }
        }

        private const string UsdtSymbol = "USDT";
        private readonly HttpClient _httpClient;

        public BinanceMarketApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
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
            using var response = await _httpClient.GetAsync($"ticker/price?symbol={symbol}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        private decimal ParsePrice(string content)
        {
            var dto = JsonConvert.DeserializeObject<PriceResponseDto>(content)
              ?? throw new InvalidOperationException("Empty price response");

            if (dto.Price is null)
                throw new InvalidOperationException("Price not found in response");

            return decimal.Parse(dto.Price, CultureInfo.InvariantCulture);
        }

    }

}

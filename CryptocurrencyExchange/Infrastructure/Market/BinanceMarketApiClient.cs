using CryptocurrencyExchange.Core.Interfaces;
using CryptocurrencyExchange.Core.ValueObject;
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

        private readonly HttpClient _httpClient;

        public BinanceMarketApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://api.binance.com/api/v3/");
        }

        public async Task<decimal> GetUsdtPriceAsync(string symbol)
        {
            var normalizedSymbol = NormalizeSymbol(symbol);
            var coinData = await GetCoinDataResponse(normalizedSymbol);

            return ParsePrice(coinData);
        }

        private string NormalizeSymbol(string coinSymbol)
        {
            string normalizedSymbol = coinSymbol;
            if (!normalizedSymbol.EndsWith(CoinSymbol.Usdt.Value))
                normalizedSymbol += CoinSymbol.Usdt.Value;

            return normalizedSymbol.ToUpper();
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

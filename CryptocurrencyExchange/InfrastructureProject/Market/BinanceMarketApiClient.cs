using Newtonsoft.Json;

namespace CryptocurrencyExchange.InfrastructureProject.Market
{
    public sealed class BinanceMarketApiClient : IMarketApiClient
    {
        private const string USDT_SYMBOL = "USDT";

        private readonly HttpClient httpClient;

        public BinanceMarketApiClient(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<decimal> GetUsdtPriceAsync(string symbol)
        {
            var normalizedSymbol = NormalizeSymbol(symbol);


            using var response = await httpClient.GetAsync($"ticker/price?symbol={normalizedSymbol}");
            response.EnsureSuccessStatusCode();

            return ParsePrice(await response.Content.ReadAsStringAsync());
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

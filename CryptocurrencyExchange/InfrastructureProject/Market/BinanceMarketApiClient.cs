namespace CryptocurrencyExchange.InfrastructureProject.Market
{
    public sealed class BinanceMarketApiClient : IMarketApiClient
    {
        private readonly HttpClient httpClient;

        public BinanceMarketApiClient(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<string> GetPriceRawAsync(string symbol)
        {
            using var response = await httpClient.GetAsync($"ticker/price?symbol={symbol}");
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }
    }

}

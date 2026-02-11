namespace CryptocurrencyExchange.InfrastructureProject.Market
{
    public interface IMarketApiClient
    {
        Task<string> GetPriceRawAsync(string symbol);
    }
}

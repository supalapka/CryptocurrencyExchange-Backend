namespace CryptocurrencyExchange.InfrastructureProject.Market
{
    public interface IMarketApiClient
    {
        Task<decimal> GetUsdtPriceAsync(string symbol);
    }
}

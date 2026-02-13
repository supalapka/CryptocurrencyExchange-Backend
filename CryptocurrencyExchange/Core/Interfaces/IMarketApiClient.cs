namespace CryptocurrencyExchange.Core.Interfaces
{
    public interface IMarketApiClient
    {
        Task<decimal> GetUsdtPriceAsync(string symbol);
    }
}

namespace CryptocurrencyExchange.Core.Interfaces
{
    public interface IMarketPriceProvider
    {
        Task<decimal> GetPriceInUsdt(string coinSymbol);
    }
}

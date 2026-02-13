namespace CryptocurrencyExchange.Core.Interfaces.Services
{
    public interface IMarketService
    {
        Task<decimal> GetPrice(string coinSymbol);
    }
}

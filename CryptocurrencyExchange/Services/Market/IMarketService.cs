namespace CryptocurrencyExchange.Services.Interfaces
{
    public interface IMarketService
    {
        Task<decimal> GetPrice(string coinSymbol);
    }
}

namespace CryptocurrencyExchange.Services
{
    public interface IMarketService
    {
        Task<decimal> GetPrice(string coinSymbol);
        Task<List<string>> GetSymbolsByPage();
    }
}

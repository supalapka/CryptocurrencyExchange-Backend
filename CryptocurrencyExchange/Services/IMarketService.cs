namespace CryptocurrencyExchange.Services
{
    public interface IMarketService
    {
        Task<decimal> GetPrice(string coinSymbol);
        List<string> GetSymbolsByPage();
    }
}

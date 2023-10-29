namespace CryptocurrencyExchange.Services
{
    public interface IMarketService
    {
        Task<double> GetPrice(string coinSymbol);
        List<string> GetSymbolsByPage();
    }
}

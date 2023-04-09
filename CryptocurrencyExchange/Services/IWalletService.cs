using CryptocurrencyExchange.Models;

namespace CryptocurrencyExchange.Services
{
    public interface IWalletService
    {
        Task BuyAsync(int userId, string coinSymbol, double usd);
        Task SellAsync(int userId, string coinSymbol, double amount);
        Task<double> GetCoinAmountAsync(int userId, string symbol);
        Task<List<WalletItem>> GetFullWalletAsync(int userId);
        Task<WalletItem> GeWalletItemAsync(int userId, string symbol);
    }
}

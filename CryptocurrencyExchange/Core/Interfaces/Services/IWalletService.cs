using CryptocurrencyExchange.Core.Models;

namespace CryptocurrencyExchange.Core.Interfaces.Services
{
    public interface IWalletService
    {
        Task BuyAsync(int userId, string coinSymbol, decimal usd);
        Task SellAsync(int userId, string coinSymbol, decimal amount);
        Task<decimal> GetCoinAmountAsync(int userId, string symbol);
        Task<List<WalletItem>> GetFullWalletAsync(int userId);
        Task<WalletItem> GetOrCreateWalletItem(int userId, string symbol);
    }
}

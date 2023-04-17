using CryptocurrencyExchange.Models;

namespace CryptocurrencyExchange.Services
{
    public interface IWalletService
    {
        Task BuyAsync(int userId, string coinSymbol, double usd);
        Task SellAsync(int userId, string coinSymbol, double amount);
        Task<double> GetCoinAmountAsync(int userId, string symbol);
        Task<List<WalletItem>> GetFullWalletAsync(int userId);
        WalletItem GeWalletItem(int userId, string symbol);
        Task SendCryptoAsync(int senderId, string symbol, double amount, int receiver);
        Task<decimal> GetPrice(string coinSymbol);
    }
}

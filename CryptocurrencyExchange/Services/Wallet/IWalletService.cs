using CryptocurrencyExchange.Models;

namespace CryptocurrencyExchange.Services.Wallet
{
    public interface IWalletService
    {
        Task BuyAsync(int userId, string coinSymbol, decimal usd);
        Task SellAsync(int userId, string coinSymbol, double amount);
        Task<double> GetCoinAmountAsync(int userId, string symbol);
        Task<List<WalletItem>> GetFullWalletAsync(int userId);
        Task<WalletItem> GeWalletItem(int userId, string symbol);
        Task SendCryptoAsync(int senderId, string symbol, double amount, int receiver);
    }
}

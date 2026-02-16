using CryptocurrencyExchange.Core.Models;
using CryptocurrencyExchange.Services.WalletTrade;

namespace CryptocurrencyExchange.Core.Interfaces.Services
{
    public interface IWalletService
    {
        Task BuyAsync(CoinTradeDto coinTradeDto);
        Task SellAsync(CoinTradeDto coinTradeDto);
        Task<decimal> GetCoinAmountAsync(int userId, string symbol);
        Task<List<WalletItem>> GetFullWalletAsync(int userId);
        Task<WalletItem> GetOrCreateWalletItem(int userId, string symbol);
    }
}

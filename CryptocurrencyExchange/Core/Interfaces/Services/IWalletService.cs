using CryptocurrencyExchange.Core.Models;
using CryptocurrencyExchange.Core.ValueObject;
using CryptocurrencyExchange.Application.Wallet;

namespace CryptocurrencyExchange.Core.Interfaces.Services
{
    public interface IWalletService
    {
        Task BuyAsync(int userId, CoinTradeDto coinTradeDto);
        Task SellAsync(int userId, CoinTradeDto coinTradeDto);
        Task<decimal> GetCoinAmountAsync(int userId, CoinSymbol symbol);
        Task<List<WalletItem>> GetFullWalletAsync(int userId);
        Task<WalletItem> GetOrCreateWalletItem(int userId, CoinSymbol symbol);
    }
}

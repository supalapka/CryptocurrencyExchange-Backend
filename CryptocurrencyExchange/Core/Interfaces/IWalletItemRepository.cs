using CryptocurrencyExchange.Core.Models;
using CryptocurrencyExchange.Core.ValueObject;

namespace CryptocurrencyExchange.Core.Interfaces
{
    public interface IWalletItemRepository
    {
        Task<List<WalletItem>> GetNonEmptyByUserAsync(int userId);

        Task<WalletItem?> GetAsync(int userId, string symbol);
        Task AddAsync(WalletItem item);
        Task<TradeWalletItems> GetCoinsDataForTradeAsync(int userId, string coinSymbol);
    }
}

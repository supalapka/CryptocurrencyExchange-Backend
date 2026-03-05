using CryptocurrencyExchange.Core.Models;
using CryptocurrencyExchange.Core.ValueObject;

namespace CryptocurrencyExchange.Core.Interfaces.Repositories
{
    public interface IWalletItemRepository
    {
        Task<List<WalletItem>> GetNonEmptyByUserAsync(int userId);

        Task<WalletItem?> GetAsync(int userId, CoinSymbol symbol);
        Task<WalletItem?> GetWithUserAsync(int userId, CoinSymbol symbol);
        Task AddAsync(WalletItem item);
        Task<TradeWalletItems> GetCoinsDataForTradeAsync(int userId, CoinSymbol coinSymbol);
        Task<(WalletItem? Sender, WalletItem? Receiver)> GetForTransferAsync(int senderId, int receiverId, CoinSymbol symbol);
    }
}

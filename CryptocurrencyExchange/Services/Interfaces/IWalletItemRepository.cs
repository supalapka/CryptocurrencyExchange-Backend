using CryptocurrencyExchange.Models;

namespace CryptocurrencyExchange.Services.Interfaces
{
    public interface IWalletItemRepository
    {
        Task<WalletItem?> GetAsync(int userId, string symbol);
        Task AddAsync(WalletItem item);
    }
}

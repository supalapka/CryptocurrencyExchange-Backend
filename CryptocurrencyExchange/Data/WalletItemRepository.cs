using CryptocurrencyExchange.Models;
using CryptocurrencyExchange.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CryptocurrencyExchange.Data
{
    public class WalletItemRepository : IWalletItemRepository
    {
        private readonly DataContext _context;

        public WalletItemRepository(DataContext context)
        {
            _context = context;
        }

        public Task<WalletItem?> GetAsync(int userId, string symbol) =>
            _context.WalletItems.FirstOrDefaultAsync(x => x.UserId == userId && x.Symbol == symbol);

        public async Task AddAsync(WalletItem item)
        {
            await _context.WalletItems.AddAsync(item);
        }
    }

}

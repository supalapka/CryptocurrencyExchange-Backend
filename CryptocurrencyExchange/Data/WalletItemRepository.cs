using CryptocurrencyExchange.Exceptions;
using CryptocurrencyExchange.Models;
using CryptocurrencyExchange.Services.Interfaces;
using CryptocurrencyExchange.Services.Wallet;
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

        public Task<List<WalletItem>> GetNonEmptyByUserAsync(int userId)
        {
            return _context.WalletItems.Where(x => x.UserId == userId && x.Amount > 0).ToListAsync();
        }

        public async Task<TradeWalletItems> GetCoinsDataForTradeAsync(int userId, string coinSymbol)
        {
            List<WalletItem> coins = await _context.WalletItems.Where(x => x.UserId == userId
            && (x.Symbol == "usdt" || x.Symbol == coinSymbol)).ToListAsync();

            var usdt = coins.FirstOrDefault(x => x.Symbol == "usdt")
                ?? throw new WalletItemNotFoundException("USDT");

            var coin = coins.FirstOrDefault(x => x.Symbol == coinSymbol)
                ?? throw new WalletItemNotFoundException(coinSymbol);

            return new TradeWalletItems(usdt, coin);
        }
    }

}

using CryptocurrencyExchange.Core.Interfaces.Repositories;
using CryptocurrencyExchange.Core.Models;
using CryptocurrencyExchange.Core.ValueObject;
using CryptocurrencyExchange.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace CryptocurrencyExchange.Infrastructure.Persistence.Repositories
{
    public class WalletItemRepository : IWalletItemRepository
    {
        private readonly DataContext _context;

        public WalletItemRepository(DataContext context)
        {
            _context = context;
        }

        public Task<WalletItem?> GetAsync(int userId, CoinSymbol symbol) =>
            _context.WalletItems.FirstOrDefaultAsync(x => x.UserId == userId && x.Symbol == symbol);

        public Task<WalletItem?> GetWithUserAsync(int userId, CoinSymbol symbol) =>
            _context.WalletItems.Include(x => x.User)
                .FirstOrDefaultAsync(x => x.UserId == userId && x.Symbol == symbol);

        public async Task AddAsync(WalletItem item)
        {
            await _context.WalletItems.AddAsync(item);
        }

        public Task<List<WalletItem>> GetNonEmptyByUserAsync(int userId)
        {
            return _context.WalletItems.Where(x => x.UserId == userId && x.Amount > 0).ToListAsync();
        }

        public async Task<(WalletItem? Sender, WalletItem? Receiver)> GetForTransferAsync(
            int senderId, int receiverId, CoinSymbol symbol)
        {
            var items = await _context.WalletItems
                .Where(x => (x.UserId == senderId || x.UserId == receiverId) && x.Symbol == symbol)
                .ToListAsync();

            return (
                items.FirstOrDefault(x => x.UserId == senderId),
                items.FirstOrDefault(x => x.UserId == receiverId)
            );
        }

        public async Task<TradeWalletItems> GetCoinsDataForTradeAsync(int userId, CoinSymbol coinSymbol)
        {
            List<WalletItem> coins = await _context.WalletItems.Where(x => x.UserId == userId
            && (x.Symbol.Value == CoinSymbol.Usdt.Value || x.Symbol == coinSymbol)).ToListAsync();

            var usdt = coins.FirstOrDefault(x => x.Symbol.Value == CoinSymbol.Usdt.Value)
                ?? throw new WalletItemNotFoundException(CoinSymbol.Usdt.Value);

            var coin = coins.FirstOrDefault(x => x.Symbol == coinSymbol)
                ?? throw new WalletItemNotFoundException(coinSymbol.Value);

            return new TradeWalletItems(usdt, coin);
        }
    }

}

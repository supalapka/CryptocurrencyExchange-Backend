using CryptocurrencyExchange.NotificationService.Entities;
using CryptocurrencyExchange.NotificationService.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CryptocurrencyExchange.NotificationService.Persistence
{
    public class NotificationOutboxRepository : INotificationOutboxRepository
    {
        private readonly NotificationDbContext _context;

        public NotificationOutboxRepository(NotificationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(NotificationOutbox entry)
        {
            await _context.NotificationOutboxEntries.AddAsync(entry);
        }

        public Task<List<NotificationOutbox>> GetPendingAsync() =>
            _context.NotificationOutboxEntries
                .Where(x => x.ProcessedAt == null)
                .ToListAsync();
    }
}

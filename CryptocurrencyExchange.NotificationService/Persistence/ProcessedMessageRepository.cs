using CryptocurrencyExchange.NotificationService.Entities;
using CryptocurrencyExchange.NotificationService.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CryptocurrencyExchange.NotificationService.Persistence
{
    public class ProcessedMessageRepository : IProcessedMessageRepository
    {
        private readonly NotificationDbContext _context;

        public ProcessedMessageRepository(NotificationDbContext context)
        {
            _context = context;
        }

        public Task<bool> ExistsAsync(string key) =>
            _context.ProcessedMessages.AnyAsync(x => x.Key == key);

        public async Task AddAsync(ProcessedMessage message)
        {
            await _context.ProcessedMessages.AddAsync(message);
        }
    }
}

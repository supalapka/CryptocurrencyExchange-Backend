using CryptocurrencyExchange.Core.Interfaces.Repositories;
using CryptocurrencyExchange.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace CryptocurrencyExchange.Infrastructure.Persistence.Repositories
{
    public class EfTransferCompletedOutboxRepository : ITransferCompletedOutboxRepository
    {
        private readonly DataContext _context;

        public EfTransferCompletedOutboxRepository(DataContext context)
            => _context = context;

        public async Task AddAsync(TransferCompletedOutbox entry)
            => await _context.TransferCompletedOutbox.AddAsync(entry);

        public async Task<List<TransferCompletedOutbox>> GetPendingAsync()
            => await _context.TransferCompletedOutbox
                .Where(e => e.ProcessedAt == null)
                .ToListAsync();
    }
}

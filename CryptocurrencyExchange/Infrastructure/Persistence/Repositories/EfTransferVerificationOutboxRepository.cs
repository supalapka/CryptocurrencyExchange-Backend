using CryptocurrencyExchange.Core.Interfaces.Repositories;
using CryptocurrencyExchange.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace CryptocurrencyExchange.Infrastructure.Persistence.Repositories
{
    public class EfTransferVerificationOutboxRepository : ITransferVerificationOutboxRepository
    {
        private readonly DataContext _context;

        public EfTransferVerificationOutboxRepository(DataContext context)
            => _context = context;

        public async Task AddAsync(TransferVerificationOutbox entry)
            => await _context.TransferVerificationOutbox.AddAsync(entry);

        public async Task<List<TransferVerificationOutbox>> GetPendingAsync()
            => await _context.TransferVerificationOutbox
                .Where(e => e.ProcessedAt == null)
                .ToListAsync();
    }
}

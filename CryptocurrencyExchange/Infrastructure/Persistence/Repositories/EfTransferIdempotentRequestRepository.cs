using CryptocurrencyExchange.Core.Interfaces.Repositories;
using CryptocurrencyExchange.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace CryptocurrencyExchange.Infrastructure.Persistence.Repositories
{
    public class EfTransferIdempotentRequestRepository : ITransferIdempotentRequestRepository
    {
        private readonly DataContext _context;

        public EfTransferIdempotentRequestRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<TransferIdempotentRequest?> FindAsync(string key, int userId)
        {
            return await _context.TransferIdempotentRequests
                .FirstOrDefaultAsync(r => r.Key == key && r.UserId == userId);
        }

        public async Task AddAsync(TransferIdempotentRequest request)
        {
            await _context.TransferIdempotentRequests.AddAsync(request);
        }
    }
}

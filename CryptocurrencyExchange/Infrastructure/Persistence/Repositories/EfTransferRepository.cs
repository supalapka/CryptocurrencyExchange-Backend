using CryptocurrencyExchange.Core.Interfaces.Repositories;
using CryptocurrencyExchange.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace CryptocurrencyExchange.Infrastructure.Persistence.Repositories
{
    public class EfTransferRepository : ITransferRepository
    {
        private readonly DataContext _context;

        public EfTransferRepository(DataContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Transfer transfer)
        {
            await _context.Transfers.AddAsync(transfer);
        }

        public async Task<Transfer?> GetPendingByIdAndSenderAsync(int transferId, int senderId)
        {
            return await _context.Transfers
                .FirstOrDefaultAsync(t =>
                    t.Id == transferId &&
                    t.SenderId == senderId &&
                    t.Status == TransferStatus.Pending);
        }
    }
}

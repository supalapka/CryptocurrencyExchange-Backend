using CryptocurrencyExchange.Core.Models;

namespace CryptocurrencyExchange.Core.Interfaces.Repositories
{
    public interface ITransferIdempotentRequestRepository
    {
        Task<TransferIdempotentRequest?> FindAsync(string key, int userId);
        Task AddAsync(TransferIdempotentRequest request);
    }
}

using CryptocurrencyExchange.Core.Models;

namespace CryptocurrencyExchange.Core.Interfaces.Repositories
{
    public interface ITransferVerificationOutboxRepository
    {
        Task AddAsync(TransferVerificationOutbox entry);
        Task<List<TransferVerificationOutbox>> GetPendingAsync();
    }
}

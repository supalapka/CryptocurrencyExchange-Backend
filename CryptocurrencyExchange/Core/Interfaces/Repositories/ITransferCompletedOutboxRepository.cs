using CryptocurrencyExchange.Core.Models;

namespace CryptocurrencyExchange.Core.Interfaces.Repositories
{
    public interface ITransferCompletedOutboxRepository
    {
        Task AddAsync(TransferCompletedOutbox entry);
        Task<List<TransferCompletedOutbox>> GetPendingAsync();
    }
}

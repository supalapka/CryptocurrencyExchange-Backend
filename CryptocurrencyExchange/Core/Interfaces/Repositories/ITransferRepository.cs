using CryptocurrencyExchange.Core.Models;

namespace CryptocurrencyExchange.Core.Interfaces.Repositories
{
    public interface ITransferRepository
    {
        Task AddAsync(Transfer transfer);
        Task<Transfer?> GetPendingByIdAndSenderAsync(int transferId, int senderId);
    }
}

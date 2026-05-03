using CryptocurrencyExchange.NotificationService.Entities;

namespace CryptocurrencyExchange.NotificationService.Interfaces
{
    public interface IProcessedMessageRepository
    {
        Task<bool> ExistsAsync(string key);
        Task AddAsync(ProcessedMessage message);
    }
}

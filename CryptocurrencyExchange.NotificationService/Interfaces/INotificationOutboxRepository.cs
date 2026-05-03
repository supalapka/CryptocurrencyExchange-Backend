using CryptocurrencyExchange.NotificationService.Entities;

namespace CryptocurrencyExchange.NotificationService.Interfaces
{
    public interface INotificationOutboxRepository
    {
        Task AddAsync(NotificationOutbox entry);
        Task<List<NotificationOutbox>> GetPendingAsync();
    }
}

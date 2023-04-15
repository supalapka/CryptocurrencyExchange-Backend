using CryptocurrencyExchange.Models;

namespace CryptocurrencyExchange.Services
{
    public interface INotificationService
    {
        public Task CreateNotification(string message, int receiverId);
        public Notification GetLastNotification(int userId);
        public Task MarkAsRead(int notificationId);
    }
}

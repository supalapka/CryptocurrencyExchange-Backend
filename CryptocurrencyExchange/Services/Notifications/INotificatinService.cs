using CryptocurrencyExchange.Core.Models;

namespace CryptocurrencyExchange.Services.Notifications
{
    public interface INotificationService
    {
        public Task CreateNotification(string message, int receiverId);
        public Notification GetLastNotification(int userId);
        public Task MarkAsRead(int notificationId);
    }
}

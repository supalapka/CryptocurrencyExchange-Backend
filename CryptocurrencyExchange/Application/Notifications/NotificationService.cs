using CryptocurrencyExchange.Core.Interfaces.Services;
using CryptocurrencyExchange.Core.Models;
using CryptocurrencyExchange.Infrastructure.Persistence;

namespace CryptocurrencyExchange.Application.Notifications
{
    public class NotificationService : INotificationService
    {
        private readonly DataContext _dataContext;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(DataContext context, ILogger<NotificationService> logger)
        {
            _dataContext = context;
            _logger = logger;
        }

        public async Task CreateNotification(string message, int receiverId)
        {
            var notification = new Notification()
            {
                CreatedAt = DateTime.Now,
                Message = message,
                IsRead = false,
                UserId = receiverId
            };

            _dataContext.Notifications.Add(notification);
            await _dataContext.SaveChangesAsync();

            _logger.LogInformation("Notification created for user {UserId}: {Message}", receiverId, message);
        }


        public Notification GetLastNotification(int userId)
        {
            var last = _dataContext.Notifications.Where(x => x.UserId == userId
            && x.IsRead == false)
                 .OrderByDescending(n => n.Id).FirstOrDefault();
            return last;
        }


        public async Task MarkAsRead(int notificationId)
        {
            var notification = _dataContext.Notifications.Find(notificationId);
            notification.IsRead = true;
            await _dataContext.SaveChangesAsync();
        }
    }
}

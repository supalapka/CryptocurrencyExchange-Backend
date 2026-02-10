using CryptocurrencyExchange.Core.Models;
using CryptocurrencyExchange.Data;

namespace CryptocurrencyExchange.Services.Notifications
{
    public class NotificationService : INotificationService
    {
        private readonly DataContext _dataContext;

        public NotificationService(DataContext context)
        {
            _dataContext = context;
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

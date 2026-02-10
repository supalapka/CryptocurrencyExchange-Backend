using CryptocurrencyExchange.Core.Models;
using CryptocurrencyExchange.Services.Notifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CryptocurrencyExchange.Controllers
{
    public class NotificationController : Controller
    {
        private readonly INotificationService _notificatinService;

        public NotificationController(INotificationService notificatinService)
        {
            _notificatinService = notificatinService;
        }

        public INotificationService NotificatinService => _notificatinService;

        [Authorize]
        [HttpGet("auth/notifications/last")]
        public Notification GetLastNotification()
        {
            int userId = Convert.ToInt32(HttpContext.Items["UserId"]);
            return NotificatinService.GetLastNotification(userId);
        }


        [Authorize]
        [HttpGet("auth/notifications/read/{id}")]
        public async Task Read(int id) => await NotificatinService.MarkAsRead(id);

    }
}

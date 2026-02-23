using CryptocurrencyExchange.Core.Interfaces.Services;
using CryptocurrencyExchange.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CryptocurrencyExchange.Controllers
{
    [ApiController]
    [Authorize]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificatinService;

        public NotificationController(INotificationService notificatinService)
        {
            _notificatinService = notificatinService;
        }

        [HttpGet("auth/notifications/last")]
        public Notification GetLastNotification()
        {
            int userId = Convert.ToInt32(HttpContext.Items["UserId"]);
            return _notificatinService.GetLastNotification(userId);
        }

        [HttpGet("auth/notifications/read/{id}")]
        public async Task Read(int id) => await _notificatinService.MarkAsRead(id);
    }
}

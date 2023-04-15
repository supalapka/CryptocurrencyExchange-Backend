using CryptocurrencyExchange.Data;
using CryptocurrencyExchange.Models;
using CryptocurrencyExchange.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CryptocurrencyExchange.Controllers
{
    public class NotificationController : Controller
    {
        private readonly INotificationService _notificatinService;

        public NotificationController(INotificationService notificatinService)
        {
            _notificatinService = notificatinService;
        }

        [Authorize]
        [HttpGet("auth/notifications/last")]
        public  Notification GetLastNotification()
        {
            string userIdClaimValue = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int userId = int.Parse(userIdClaimValue);

            return _notificatinService.GetLastNotification(userId);
        }


        [Authorize]
        [HttpGet("auth/notifications/read/{id}")]
        public async Task<ActionResult> Read(int id)
        {
           await _notificatinService.MarkAsRead(id);
            return Ok(); 
        }
    }
}

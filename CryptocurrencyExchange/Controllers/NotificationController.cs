﻿using CryptocurrencyExchange.Models;
using CryptocurrencyExchange.Services;
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

        [Authorize]
        [HttpGet("auth/notifications/last")]
        public Notification GetLastNotification()
        {
            int userId = Convert.ToInt32(HttpContext.Items["UserId"]);
            return _notificatinService.GetLastNotification(userId);
        }


        [Authorize]
        [HttpGet("auth/notifications/read/{id}")]
        public async Task Read(int id) => await _notificatinService.MarkAsRead(id);
      
    }
}

using Microsoft.AspNetCore.SignalR;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.DTOs.Enums;
using PortEval.Application.Services.Hubs;
using PortEval.Application.Services.Interfaces;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace PortEval.Application.Services
{
    public class NotificationService : INotificationService
    {
        private readonly ILogger _logger;
        private readonly IHubContext<NotificationHub, INotificationHubClient> _notificationHub;

        public NotificationService(ILoggerFactory loggerFactory, IHubContext<NotificationHub, INotificationHubClient> notificationHub)
        {
            _logger = loggerFactory.CreateLogger<NotificationService>();
            _notificationHub = notificationHub;
        }

        public async Task SendNotificationAsync(NotificationType type, string message = null)
        {
            var notification = new NotificationDto
            {
                Time = DateTime.UtcNow,
                Type = type,
                Message = message
            };

            _logger.LogInformation($"Sending notification: {type}, \"{message}\".");
            await _notificationHub.Clients.All.ReceiveNotification(notification);
        }
    }
}

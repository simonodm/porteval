using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using PortEval.Application.Features.Hubs;
using PortEval.Application.Features.Interfaces.Services;
using PortEval.Application.Models.DTOs;
using PortEval.Application.Models.DTOs.Enums;
using System;
using System.Threading.Tasks;

namespace PortEval.Application.Features.Services
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

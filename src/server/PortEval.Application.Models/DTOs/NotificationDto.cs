using System;
using PortEval.Application.Models.DTOs.Enums;
using Swashbuckle.AspNetCore.Annotations;

namespace PortEval.Application.Models.DTOs;

[SwaggerSchema("Represents a notification emitted by the application.")]
public class NotificationDto
{
    [SwaggerSchema("Time of the notification.")]
    public DateTime Time { get; set; }

    [SwaggerSchema("Type of the notification.")]
    public NotificationType Type { get; set; }

    [SwaggerSchema("Optional notification message.")]
    public string Message { get; set; }
}
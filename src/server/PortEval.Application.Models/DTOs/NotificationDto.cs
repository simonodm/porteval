using PortEval.Application.Models.DTOs.Enums;
using System;

namespace PortEval.Application.Models.DTOs
{
    public class NotificationDto
    {
        public DateTime Time { get; set; }
        public NotificationType Type { get; set; }
        public string Message { get; set; }
    }
}

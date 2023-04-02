using System.Threading.Tasks;
using PortEval.Application.Models.DTOs;

namespace PortEval.Application.Core.Hubs
{
    public interface INotificationHubClient
    {
        public Task ReceiveNotification(NotificationDto notification);
    }
}

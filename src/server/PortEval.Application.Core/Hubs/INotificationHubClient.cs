using PortEval.Application.Models.DTOs;
using System.Threading.Tasks;

namespace PortEval.Application.Core.Hubs
{
    public interface INotificationHubClient
    {
        public Task ReceiveNotification(NotificationDto notification);
    }
}

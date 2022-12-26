using PortEval.Application.Models.DTOs;
using System.Threading.Tasks;

namespace PortEval.Application.Features.Hubs
{
    public interface INotificationHubClient
    {
        public Task ReceiveNotification(NotificationDto notification);
    }
}

using PortEval.Application.Models.DTOs.Enums;
using System.Threading.Tasks;

namespace PortEval.Application.Features.Interfaces.Services
{
    public interface INotificationService
    {
        public Task SendNotificationAsync(NotificationType type, string message = null);
    }
}

using System.Threading.Tasks;
using PortEval.Application.Models.DTOs.Enums;

namespace PortEval.Application.Services.Interfaces
{
    public interface INotificationService
    {
        public Task SendNotificationAsync(NotificationType type, string message = null);
    }
}

using PortEval.Application.Models.DTOs.Enums;
using System.Threading.Tasks;

namespace PortEval.Application.Core.Interfaces.Services
{
    /// <summary>
    /// Handles emission of real-time notifications to clients.
    /// </summary>
    public interface INotificationService
    {
        /// <summary>
        /// Emits a notification to registered clients.
        /// </summary>
        /// <param name="type">Type of the notification.</param>
        /// <param name="message">Notification message.</param>
        /// <returns>
        /// A task representing the asynchronous emission operation.
        /// </returns>
        public Task<OperationResponse> SendNotificationAsync(NotificationType type, string message = null);
    }
}

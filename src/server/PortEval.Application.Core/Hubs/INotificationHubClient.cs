using System.Threading.Tasks;
using PortEval.Application.Models.DTOs;

namespace PortEval.Application.Core.Hubs;

/// <summary>
///     Defines the client methods of the <see cref="NotificationHub" /> SignalR hub.
/// </summary>
public interface INotificationHubClient
{
    /// <summary>
    ///     Receives a notification emitted by the hub.
    /// </summary>
    /// <param name="notification">Notification to receive.</param>
    /// <returns>A task representing the asynchronous receive operation.</returns>
    public Task ReceiveNotification(NotificationDto notification);
}
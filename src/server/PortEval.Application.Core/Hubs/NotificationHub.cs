using Microsoft.AspNetCore.SignalR;

namespace PortEval.Application.Core.Hubs;

/// <summary>
///     Represents a SignalR hub for PortEval notifications.
/// </summary>
public class NotificationHub : Hub<INotificationHubClient>
{
}
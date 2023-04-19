namespace PortEval.Application.Models.DTOs.Enums;

/// <summary>
///     Represents the type of a notification emitted by the application.
/// </summary>
public enum NotificationType
{
    /// <summary>
    ///     A purely informative notification, no action needed.
    /// </summary>
    Info,

    /// <summary>
    ///     A notification indicating that new data is available in the application and the client should refresh their data.
    /// </summary>
    NewDataAvailable
}
using Hangfire.Dashboard;

namespace PortEval.Application.Filters;

/// <summary>
///     A simple pass-through filter for Hangfire dashboard. This filter should only be used in development environment.
/// </summary>
public class HangfireDevAuthorizationFilter : IDashboardAuthorizationFilter
{
    /// <summary>
    ///     Authorizes the request.
    /// </summary>
    public bool Authorize(DashboardContext context)
    {
        return true;
    }
}
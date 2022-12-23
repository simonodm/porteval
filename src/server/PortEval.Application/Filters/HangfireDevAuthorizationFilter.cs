using Hangfire.Dashboard;

namespace PortEval.Application.Filters
{
    public class HangfireDevAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            return true;
        }
    }
}

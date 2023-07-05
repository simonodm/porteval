using Microsoft.AspNetCore.Builder;
using PortEval.Application.Middleware.Exceptions;

namespace PortEval.Application.Extensions;

/// <summary>
///     Contains extension methods for PortEval's middleware configuration.
/// </summary>
public static class ExceptionMiddlewareExtensions
{
    /// <summary>
    ///     Configures the custom exception middleware.
    /// </summary>
    /// <param name="app">ASP.NET application builder.</param>
    public static void UseCustomExceptionMiddleware(this IApplicationBuilder app)
    {
        app.UseMiddleware<ExceptionMiddleware>();
    }
}
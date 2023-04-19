using System;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Win32;

namespace PortEval.Application.Extensions;

/// <summary>
///     Implements extension methods on <see cref="IConfiguration" /> configuration object.
/// </summary>
public static class IConfigurationExtensions
{
    /// <summary>
    ///     Retrieves a text configuration value by key, searching all possible sources of the configuration value.
    /// </summary>
    /// <param name="configuration">A <see cref="IConfiguration" /> instance.</param>
    /// <param name="key">Key to retrieve configuration value of.</param>
    /// <returns>A configuration value corresponding to the provided key if such was found, <c>null</c> otherwise.</returns>
    /// <remarks>
    ///     This method first searches the <see cref="IConfiguration" /> object on which this method is called, then it checks
    ///     the environment variables, and then it checks the appropriate Windows registry key if the application is running on
    ///     a Windows system.
    /// </remarks>
    public static string GetConfigurationValue(this IConfiguration configuration, string key)
    {
        var configValue = configuration.GetValue(typeof(string), key);
        if (configValue is string configString && !string.IsNullOrWhiteSpace(configString)) return configString;

        var envVarValue = Environment.GetEnvironmentVariable(key);
        if (!string.IsNullOrWhiteSpace(envVarValue)) return envVarValue;

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            var registryValue = Registry.GetValue(GetConfigurationRegistryKey(), key, null);
            if (registryValue is string registryString && !string.IsNullOrWhiteSpace(registryString))
                return registryString;
        }

        return null;
    }

    private static string GetConfigurationRegistryKey()
    {
        return "HKEY_LOCAL_MACHINE\\SOFTWARE\\PortEval\\Configuration";
    }
}
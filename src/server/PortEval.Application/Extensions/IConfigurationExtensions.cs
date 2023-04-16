using Microsoft.Extensions.Configuration;
using Microsoft.Win32;
using System;
using System.Runtime.InteropServices;

namespace PortEval.Application.Extensions
{
    public static class IConfigurationExtensions
    {
        public static string GetConfigurationValue(this IConfiguration configuration, string key)
        {
            var configValue = configuration.GetValue(typeof(string), key);
            if (configValue is string configString && !string.IsNullOrWhiteSpace(configString))
            {
                return configString;
            }
            
            var envVarValue = Environment.GetEnvironmentVariable(key);
            if (!string.IsNullOrWhiteSpace(envVarValue))
            {
                return envVarValue;
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var registryValue = Registry.GetValue(GetConfigurationRegistryKey(), key, null);
                if (registryValue is string registryString && !string.IsNullOrWhiteSpace(registryString))
                {
                    return registryString;
                }
            }
            
            return null;
        }

        private static string GetConfigurationRegistryKey()
        {
            return "HKEY_LOCAL_MACHINE\\SOFTWARE\\PortEval\\Configuration";
        }
    }
}

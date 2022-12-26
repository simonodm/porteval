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
            var envVarValue = Environment.GetEnvironmentVariable(key);
            if (envVarValue != null)
            {
                return envVarValue;
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var registryValue = Registry.GetValue(GetConfigurationRegistryKey(), key, null);
                if (registryValue != null)
                {
                    return registryValue as string;
                }
            }

            var configValue = configuration.GetValue(typeof(string), key);
            return configValue as string;
        }

        private static string GetConfigurationRegistryKey()
        {
            return "HKEY_LOCAL_MACHINE\\SOFTWARE\\PortEval\\Configuration";
        }
    }
}

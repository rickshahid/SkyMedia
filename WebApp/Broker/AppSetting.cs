using System;
using System.Collections.Generic;

using Microsoft.Extensions.Configuration;

namespace SkyMedia.ServiceBroker
{
    public static class AppSetting
    {
        public static IConfigurationRoot ConfigRoot { get; set; }

        public static string GetValue(string settingKey)
        {
            return GetValue(settingKey, false)[0];
        }

        public static string[] GetValue(string settingKey, bool parseValue)
        {
            string configValue = ConfigRoot[settingKey];
            if (configValue == null) configValue = string.Empty;
            return parseValue ? ParseConnection(configValue) : new string[] { configValue };
        }

        public static string[] ParseConnection(string accountConnection)
        {
            List<string> parsedConnection = new List<string>();
            string[] accountSettings = accountConnection.Split(Constants.MultiItemsSeparator);
            foreach (string accountSetting in accountSettings)
            {
                if (accountSetting.StartsWith(Constants.ConnectionStrings.AccountNamePrefix, StringComparison.InvariantCultureIgnoreCase))
                {
                    parsedConnection.Add(accountSetting.Remove(0, Constants.ConnectionStrings.AccountNamePrefix.Length));
                }
                else if (accountSetting.StartsWith(Constants.ConnectionStrings.AccountKeyPrefix, StringComparison.InvariantCultureIgnoreCase))
                {
                    parsedConnection.Add(accountSetting.Remove(0, Constants.ConnectionStrings.AccountKeyPrefix.Length));
                }
            }
            return parsedConnection.ToArray();
        }
    }
}

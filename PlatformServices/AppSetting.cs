using System;
using System.Collections.Generic;

using Microsoft.Extensions.Configuration;

namespace AzureSkyMedia.PlatformServices
{
    public static class AppSetting
    {
        public static IConfigurationRoot ConfigRoot;

        private static string[] ParseConnection(string accountConnection)
        {
            List<string> parsedConnection = new List<string>();
            StringComparison comparisonType = StringComparison.OrdinalIgnoreCase;
            string[] accountSettings = accountConnection.Split(Constant.TextDelimiter.Connection);
            foreach (string accountSetting in accountSettings)
            {
                if (accountSetting.StartsWith(Constant.AppSettingKey.AccountNamePrefix, comparisonType))
                {
                    parsedConnection.Add(accountSetting.Remove(0, Constant.AppSettingKey.AccountNamePrefix.Length));
                }
                else if (accountSetting.StartsWith(Constant.AppSettingKey.AccountKeyPrefix, comparisonType))
                {
                    parsedConnection.Add(accountSetting.Remove(0, Constant.AppSettingKey.AccountKeyPrefix.Length));
                }
                else if (accountSetting.StartsWith(Constant.AppSettingKey.DatabaseIdPrefix, comparisonType))
                {
                    parsedConnection.Add(accountSetting.Remove(0, Constant.AppSettingKey.DatabaseIdPrefix.Length));
                }
            }
            return parsedConnection.ToArray();
        }

        public static string[] GetValue(string settingKey, bool parseValue)
        {
            string settingValue = ConfigRoot == null ? Environment.GetEnvironmentVariable(settingKey) : ConfigRoot[settingKey];
            if (settingValue == null)
            {
                settingValue = string.Empty;
            }
            return parseValue ? ParseConnection(settingValue) : new string[] { settingValue };
        }

        public static string GetValue(string settingKey)
        {
            string[] settingValue = GetValue(settingKey, false);
            return settingValue.Length == 0 ? string.Empty : settingValue[0];
        }
    }
}
using System;
using System.Collections.Generic;

using Microsoft.Extensions.Configuration;

namespace AzureSkyMedia.PlatformServices
{
    internal static class AppSetting
    {
        public static IConfiguration Configuration;

        private static string[] ParseValue(string accountConnection)
        {
            List<string> parsedValue = new List<string>();
            StringComparison comparisonType = StringComparison.OrdinalIgnoreCase;
            string[] accountSettings = accountConnection.Split(Constant.TextDelimiter.Connection);
            foreach (string accountSetting in accountSettings)
            {
                if (accountSetting.StartsWith(Constant.AppSettingKey.AccountNamePrefix, comparisonType))
                {
                    parsedValue.Add(accountSetting.Remove(0, Constant.AppSettingKey.AccountNamePrefix.Length));
                }
                else if (accountSetting.StartsWith(Constant.AppSettingKey.AccountKeyPrefix, comparisonType))
                {
                    parsedValue.Add(accountSetting.Remove(0, Constant.AppSettingKey.AccountKeyPrefix.Length));
                }
                else if (accountSetting.StartsWith(Constant.AppSettingKey.AccountEndpointPrefix, comparisonType))
                {
                    parsedValue.Add(accountSetting.Remove(0, Constant.AppSettingKey.AccountEndpointPrefix.Length));
                }
                else if (accountSetting.StartsWith(Constant.AppSettingKey.DatabaseIdPrefix, comparisonType))
                {
                    parsedValue.Add(accountSetting.Remove(0, Constant.AppSettingKey.DatabaseIdPrefix.Length));
                }
                else
                {
                    parsedValue.Add(accountSetting);
                }
            }
            return parsedValue.ToArray();
        }

        public static string[] GetValue(string settingKey, bool parseValue)
        {
            string settingValue;
            if (Configuration == null)
            {
                settingValue = Environment.GetEnvironmentVariable(settingKey);
            }
            else
            {
                settingValue = Configuration[settingKey];
            }
            if (settingValue == null)
            {
                settingValue = string.Empty;
            }
            return parseValue ? ParseValue(settingValue) : new string[] { settingValue };
        }

        public static string GetValue(string settingKey)
        {
            string[] settingValue = GetValue(settingKey, false);
            return settingValue.Length == 0 ? string.Empty : settingValue[0];
        }
    }
}
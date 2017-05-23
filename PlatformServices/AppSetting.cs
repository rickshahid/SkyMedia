using System;
//using System.Threading.Tasks;
using System.Collections.Generic;

//using Microsoft.Azure.KeyVault;
//using Microsoft.Azure.KeyVault.Models;
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
            else if (settingValue.Contains(Constant.KeyVaultDomain))
            {
                //CacheClient cacheClient = new CacheClient();
                //settingValue = cacheClient.GetValue<string>(settingKey);
                //if (string.IsNullOrEmpty(settingValue))
                //{
                //    KeyVaultClient.AuthenticationCallback vaultAuth = new KeyVaultClient.AuthenticationCallback(AuthToken.GetVaultToken);
                //    KeyVaultClient vaultClient = new KeyVaultClient(vaultAuth);
                //    Task<SecretBundle> vaultTask = vaultClient.GetSecretAsync(settingValue);
                //    SecretBundle vaultSecret = vaultTask.Result;
                //    settingValue = vaultSecret.Value;
                //}
                //cacheClient.SetValue<string>(settingKey, settingValue);
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

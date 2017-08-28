using System;
using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using StackExchange.Redis;

namespace AzureSkyMedia.PlatformServices
{
    internal class CacheClient
    {
        private static Lazy<ConnectionMultiplexer> _service = new Lazy<ConnectionMultiplexer>(() =>
        {
            string settingKey = Constant.AppSettingKey.AzureCache;
            string[] accountCredentials = AppSetting.GetValue(settingKey, true);
            string accountName = accountCredentials[0];
            string accountKey = accountCredentials[1];

            ConfigurationOptions serviceOptions = new ConfigurationOptions();
            serviceOptions.EndPoints.Add(accountName);
            serviceOptions.Password = accountKey;
            serviceOptions.AbortOnConnectFail = false;
            serviceOptions.Ssl = true;

            return ConnectionMultiplexer.Connect(serviceOptions);
        });
        private string _partitionId;

        public CacheClient(string authToken)
        {
            _partitionId = AuthToken.GetClaimValue(authToken, Constant.UserAttribute.MediaAccountName);
        }

        private IDatabase GetCache()
        {
            return _service.Value.GetDatabase();
        }

        private string MapItemKey(string itemKey)
        {
            return string.Concat(_partitionId, Constant.TextDelimiter.Identifier, itemKey);
        }

        public T GetValue<T>(string itemKey)
        {
            T value = default(T);
            IDatabase cache = GetCache();
            itemKey = MapItemKey(itemKey);
            string itemValue = cache.StringGet(itemKey);
            if (typeof(T) == typeof(string))
            {
                value = (T)Convert.ChangeType(itemValue, typeof(T));
            }
            else if (itemValue != null)
            {
                value = JsonConvert.DeserializeObject<T>(itemValue);
            }
            return value;
        }

        public T[] GetValues<T>(string itemKey)
        {
            List<T> values = new List<T>();
            IDatabase cache = GetCache();
            itemKey = MapItemKey(itemKey);
            string itemValue = cache.StringGet(itemKey);
            if (!string.IsNullOrEmpty(itemValue))
            {
                JArray itemValues = JArray.Parse(itemValue);
                foreach (JToken itemToken in itemValues)
                {
                    T value = JsonConvert.DeserializeObject<T>(itemToken.ToString());
                    values.Add(value);
                }
            }
            return values.ToArray();
        }

        public void SetValue<T>(string itemKey, T itemValue)
        {
            SetValue<T>(itemKey, itemValue, null);
        }

        public void SetValue<T>(string itemKey, T itemValue, TimeSpan? itemExpiration)
        {
            IDatabase cache = GetCache();
            itemKey = MapItemKey(itemKey);
            if (itemValue == null)
            {
                cache.KeyDelete(itemKey);
            }
            else if (typeof(T) == typeof(string))
            {
                cache.StringSet(itemKey, itemValue.ToString(), itemExpiration);
            }
            else
            {
                string itemSerialized = JsonConvert.SerializeObject(itemValue);
                cache.StringSet(itemKey, itemSerialized, itemExpiration);
            }
        }
    }
}
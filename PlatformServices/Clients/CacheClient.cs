using System;
using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using StackExchange.Redis;

namespace AzureSkyMedia.PlatformServices
{
    internal class CacheClient
    {
        private static Lazy<ConnectionMultiplexer> _cache = new Lazy<ConnectionMultiplexer>(() =>
        {
            string settingKey = Constant.AppSettingKey.AzureCache;
            string accountConnection = AppSetting.GetValue(settingKey);
            return ConnectionMultiplexer.Connect(accountConnection);
        });
        private string _partitionId;

        public CacheClient(string authToken)
        {
            User authUser = new User(authToken);
            _partitionId = authUser.Id;
        }

        private IDatabase GetDatabase()
        {
            return _cache.Value.GetDatabase();
        }

        private string MapItemKey(string itemKey)
        {
            return string.Concat(_partitionId, Constant.TextDelimiter.Identifier, itemKey);
        }

        public T GetValue<T>(string itemKey)
        {
            T value = default(T);
            IDatabase database = GetDatabase();
            itemKey = MapItemKey(itemKey);
            string itemValue = database.StringGet(itemKey);
            if (itemValue != null)
            {
                value = JsonConvert.DeserializeObject<T>(itemValue);
            }
            return value;
        }

        public T[] GetValues<T>(string itemKey)
        {
            List<T> values = new List<T>();
            IDatabase database = GetDatabase();
            itemKey = MapItemKey(itemKey);
            string itemValue = database.StringGet(itemKey);
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
            IDatabase database = GetDatabase();
            itemKey = MapItemKey(itemKey);
            if (itemValue == null)
            {
                database.KeyDelete(itemKey);
            }
            else if (typeof(T) == typeof(string))
            {
                database.StringSet(itemKey, itemValue.ToString(), itemExpiration);
            }
            else
            {
                string itemSerialized = JsonConvert.SerializeObject(itemValue);
                database.StringSet(itemKey, itemSerialized, itemExpiration);
            }
        }
    }
}
using System;
using System.Collections.Generic;

using Microsoft.WindowsAzure.MediaServices.Client;

using Newtonsoft.Json;

using StackExchange.Redis;

namespace AzureSkyMedia.PlatformServices
{
    public class CacheClient
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

        public CacheClient()
        {
            _partitionId = Constant.AppSettingKey.AzureCache;
        }

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

        public MediaProcessor[] Initialize(string authToken)
        {
            MediaClient mediaClient = new MediaClient(authToken);
            IMediaProcessor[] mediaProcessors = mediaClient.GetEntities(MediaEntity.Processor) as IMediaProcessor[];
            List<MediaProcessor> processorList = new List<MediaProcessor>();
            foreach (IMediaProcessor mediaProcessor in mediaProcessors)
            {
                MediaProcessor processorType = Processor.GetProcessorType(mediaProcessor.Id);
                processorList.Add(processorType);
            }
            MediaProcessor[] processorTypes = processorList.ToArray();
            SetValue<MediaProcessor[]>(Constant.Cache.ItemKey.MediaProcessors, processorTypes);
            return processorTypes;
        }

        public T GetValue<T>(string itemKey)
        {
            IDatabase cache = GetCache();
            itemKey = MapItemKey(itemKey);
            string itemValue = cache.StringGet(itemKey);
            if (typeof(T) == typeof(string))
            {
                return (T)Convert.ChangeType(itemValue, typeof(T));
            }
            return JsonConvert.DeserializeObject<T>(itemValue);
        }

        public bool SetValue<T>(string itemKey, T itemValue)
        {
            return SetValue<T>(itemKey, itemValue, null);
        }

        public bool SetValue<T>(string itemKey, T itemValue, TimeSpan? itemExpiration)
        {
            IDatabase cache = GetCache();
            itemKey = MapItemKey(itemKey);
            if (typeof(T) == typeof(string))
            {
                return cache.StringSet(itemKey, itemValue.ToString(), itemExpiration);
            }
            string itemSerialized = JsonConvert.SerializeObject(itemValue);
            return cache.StringSet(itemKey, itemSerialized, itemExpiration);
        }
    }
}

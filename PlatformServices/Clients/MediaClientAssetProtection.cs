using System;
using System.Linq;
using System.Collections.Generic;

using Microsoft.WindowsAzure.MediaServices.Client;
using Microsoft.WindowsAzure.MediaServices.Client.DynamicEncryption;
using Microsoft.WindowsAzure.MediaServices.Client.ContentKeyAuthorization;

namespace AzureSkyMedia.PlatformServices
{
    internal partial class MediaClient
    {
        private static ContentProtection GetContentProtection(string jobId, string taskId)
        {
            TableClient tableClient = new TableClient();
            string tableName = Constant.Storage.Table.ContentProtection;
            return tableClient.GetEntity<ContentProtection>(tableName, jobId, taskId);
        }

        public static ContentProtection[] GetContentProtections(string directoryId, IJob job, MediaJobTask[] jobTasks)
        {
            List<ContentProtection> contentProtections = new List<ContentProtection>();
            string settingKey = Constant.AppSettingKey.DirectoryClientId;
            settingKey = string.Format(settingKey, directoryId);
            string directoryClientId = AppSetting.GetValue(settingKey);
            for (int i = 0; i < job.Tasks.Count; i++)
            {
                if (jobTasks[i].ContentProtection != null)
                {
                    ContentProtection contentProtection = jobTasks[i].ContentProtection;
                    contentProtection.RowKey = job.Tasks[i].Id;
                    contentProtection.DirectoryId = directoryId;
                    contentProtection.Audience = directoryClientId;
                    contentProtections.Add(contentProtection);
                }
            }
            return contentProtections.ToArray();
        }

        public static void DeleteContentProtections(TableClient tableClient, string jobId)
        {
            string tableName = Constant.Storage.Table.ContentProtection;
            ContentProtection[] contentProtections = tableClient.GetEntities<ContentProtection>(tableName, jobId);
            tableClient.DeleteEntities(tableName, contentProtections);
        }

        public static StreamProtection[] GetStreamProtections(string authToken, string protectionTypes)
        {
            List<StreamProtection> streamProtections = new List<StreamProtection>();
            if (!string.IsNullOrEmpty(protectionTypes))
            {
                authToken = string.Concat("Bearer=", authToken);
                string[] streamProtectionTypes = protectionTypes.Split(Constant.TextDelimiter.Application);
                foreach (string streamProtectionType in streamProtectionTypes)
                {
                    StreamProtection streamProtection = new StreamProtection()
                    {
                        Type = (MediaProtection)Enum.Parse(typeof(MediaProtection), streamProtectionType),
                        AuthenticationToken = authToken
                    };
                    streamProtections.Add(streamProtection);
                }
            }
            return streamProtections.ToArray();
        }

        public StreamProtection[] GetStreamProtections(string authToken, IAsset asset)
        {
            List<StreamProtection> streamProtections = new List<StreamProtection>();
            authToken = string.Concat("Bearer=", authToken);

            IAssetDeliveryPolicy[] deliveryPolicies = asset.DeliveryPolicies.Where(dp => dp.AssetDeliveryPolicyType == AssetDeliveryPolicyType.DynamicEnvelopeEncryption).ToArray();
            if (deliveryPolicies.Length > 0)
            {
                StreamProtection streamProtection = new StreamProtection()
                {
                    Type = MediaProtection.AES,
                    AuthenticationToken = authToken
                };
                streamProtections.Add(streamProtection);
            }

            deliveryPolicies = asset.DeliveryPolicies.Where(dp => dp.AssetDeliveryPolicyType == AssetDeliveryPolicyType.DynamicCommonEncryption).ToArray();
            if (deliveryPolicies.Length > 0)
            {
                IContentKey[] contentKeys = asset.ContentKeys.Where(ck => ck.ContentKeyType == ContentKeyType.CommonEncryption).ToArray();
                foreach (IContentKey contentKey in contentKeys)
                {
                    IContentKeyAuthorizationPolicy authPolicy = GetEntityById(MediaEntity.ContentKeyAuthPolicy, contentKey.AuthorizationPolicyId) as IContentKeyAuthorizationPolicy;
                    if (authPolicy != null)
                    {
                        IContentKeyAuthorizationPolicyOption[] policyOptions = authPolicy.Options.Where(po => po.KeyDeliveryType == ContentKeyDeliveryType.PlayReadyLicense).ToArray();
                        if (policyOptions.Length > 0)
                        {
                            StreamProtection streamProtection = new StreamProtection()
                            {
                                Type = MediaProtection.PlayReady,
                                AuthenticationToken = authToken
                            };
                            streamProtections.Add(streamProtection);
                        }
                        policyOptions = authPolicy.Options.Where(po => po.KeyDeliveryType == ContentKeyDeliveryType.Widevine).ToArray();
                        if (policyOptions.Length > 0)
                        {
                            StreamProtection streamProtection = new StreamProtection()
                            {
                                Type = MediaProtection.Widevine,
                                AuthenticationToken = authToken
                            };
                            streamProtections.Add(streamProtection);
                        }
                    }
                }
            }

            deliveryPolicies = asset.DeliveryPolicies.Where(p => p.AssetDeliveryPolicyType == AssetDeliveryPolicyType.DynamicCommonEncryptionCbcs).ToArray();
            if (deliveryPolicies.Length > 0)
            {
                StreamProtection streamProtection = new StreamProtection()
                {
                    Type = MediaProtection.FairPlay,
                    AuthenticationToken = authToken
                };
                streamProtections.Add(streamProtection);
            }

            return streamProtections.ToArray();
        }
    }
}
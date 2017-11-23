using System.Linq;
using System.Collections.Generic;

using Microsoft.WindowsAzure.MediaServices.Client;
using Microsoft.WindowsAzure.MediaServices.Client.DynamicEncryption;
using Microsoft.WindowsAzure.MediaServices.Client.ContentKeyAuthorization;

namespace AzureSkyMedia.PlatformServices
{
    internal partial class MediaClient
    {
        private static ContentProtection GetContentProtection(MediaContentPublish contentPublish)
        {
            TableClient tableClient = new TableClient();
            string tableName = Constant.Storage.TableName.ContentProtection;
            return tableClient.GetEntity<ContentProtection>(tableName, contentPublish.PartitionKey, contentPublish.RowKey);
        }

        public MediaProtection[] GetContentProtection(IAsset asset)
        {
            List<MediaProtection> contentProtection = new List<MediaProtection>();

            IAssetDeliveryPolicy[] deliveryPolicies = asset.DeliveryPolicies.Where(dp => dp.AssetDeliveryPolicyType == AssetDeliveryPolicyType.DynamicEnvelopeEncryption).ToArray();
            if (deliveryPolicies.Length > 0)
            {
                contentProtection.Add(MediaProtection.AES);
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
                        if (policyOptions.Length > 0 && !contentProtection.Contains(MediaProtection.PlayReady))
                        {
                            contentProtection.Add(MediaProtection.PlayReady);
                        }
                        policyOptions = authPolicy.Options.Where(po => po.KeyDeliveryType == ContentKeyDeliveryType.Widevine).ToArray();
                        if (policyOptions.Length > 0 && !contentProtection.Contains(MediaProtection.Widevine))
                        {
                            contentProtection.Add(MediaProtection.Widevine);
                        }
                    }
                }
            }

            deliveryPolicies = asset.DeliveryPolicies.Where(p => p.AssetDeliveryPolicyType == AssetDeliveryPolicyType.DynamicCommonEncryptionCbcs).ToArray();
            if (deliveryPolicies.Length > 0)
            {
                contentProtection.Add(MediaProtection.FairPlay);
            }

            return contentProtection.ToArray();
        }
    }
}
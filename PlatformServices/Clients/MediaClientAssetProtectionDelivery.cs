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
        private AssetDeliveryProtocol GetDeliveryProtocols(ContentKeyDeliveryType[] deliveryTypes)
        {
            AssetDeliveryProtocol deliveryProtocols = AssetDeliveryProtocol.None;
            foreach (ContentKeyDeliveryType deliveryType in deliveryTypes)
            {
                switch (deliveryType)
                {
                    case ContentKeyDeliveryType.BaselineHttp:
                        deliveryProtocols = deliveryProtocols | Constant.Media.DeliveryProtocol.Aes;
                        break;
                    case ContentKeyDeliveryType.PlayReadyLicense:
                        deliveryProtocols = deliveryProtocols | Constant.Media.DeliveryProtocol.DrmPlayReady;
                        break;
                    case ContentKeyDeliveryType.Widevine:
                        deliveryProtocols = deliveryProtocols | Constant.Media.DeliveryProtocol.DrmWidevine;
                        break;
                    case ContentKeyDeliveryType.FairPlay:
                        deliveryProtocols = deliveryProtocols | Constant.Media.DeliveryProtocol.DrmFairPlay;
                        break;
                }
            }
            return deliveryProtocols;
        }

        private IAssetDeliveryPolicy GetDeliveryPolicy(string policyName, AssetDeliveryPolicyType policyType,
                                                       Dictionary<AssetDeliveryPolicyConfigurationKey, string> policyConfig,
                                                       ContentKeyDeliveryType[] deliveryTypes)
        {
            IAssetDeliveryPolicy deliveryPolicy = GetEntityByName(MediaEntity.DeliveryPolicy, policyName) as IAssetDeliveryPolicy;
            if (deliveryPolicy == null)
            {
                AssetDeliveryProtocol policyProtocols = GetDeliveryProtocols(deliveryTypes);
                deliveryPolicy = _media2.AssetDeliveryPolicies.Create(policyName, policyType, policyProtocols, policyConfig);
            }
            return deliveryPolicy;
        }

        private IAssetDeliveryPolicy[] GetDeliveryPolicies(ContentProtection contentProtection)
        {
            List<IAssetDeliveryPolicy> deliveryPolicies = new List<IAssetDeliveryPolicy>();
            AssetDeliveryPolicyType policyType = AssetDeliveryPolicyType.NoDynamicEncryption;
            Dictionary<AssetDeliveryPolicyConfigurationKey, string> policyConfig = null;
            string policyName = Constant.Media.DeliveryPolicy.DecryptionStorage;
            if (contentProtection.Aes)
            {
                policyType = AssetDeliveryPolicyType.DynamicEnvelopeEncryption;
                policyName = Constant.Media.DeliveryPolicy.EncryptionAes;
                ContentKeyType keyType = ContentKeyType.EnvelopeEncryption;
                string keyName = Constant.Media.ContentProtection.ContentKeyNameAes;
                IContentKey contentKey = GetContentKey(keyType, keyName, contentProtection);
                Uri keyDeliveryUrl = contentKey.GetKeyDeliveryUrl(ContentKeyDeliveryType.BaselineHttp);
                byte[] encryptionIV = CreateEncryptionKey();
                policyConfig = new Dictionary<AssetDeliveryPolicyConfigurationKey, string>();
                policyConfig.Add(AssetDeliveryPolicyConfigurationKey.EnvelopeKeyAcquisitionUrl, keyDeliveryUrl.ToString());
                policyConfig.Add(AssetDeliveryPolicyConfigurationKey.EnvelopeEncryptionIVAsBase64, Convert.ToBase64String(encryptionIV));
                ContentKeyDeliveryType[] deliveryTypes = new ContentKeyDeliveryType[] { ContentKeyDeliveryType.BaselineHttp };
                IAssetDeliveryPolicy deliveryPolicy = GetDeliveryPolicy(policyName, policyType, policyConfig, deliveryTypes);
                deliveryPolicies.Add(deliveryPolicy);
            }
            if (contentProtection.DrmPlayReady && contentProtection.DrmWidevine)
            {
                policyType = AssetDeliveryPolicyType.DynamicCommonEncryption;
                policyName = Constant.Media.DeliveryPolicy.EncryptionDrmPlayReadyWidevine;
                ContentKeyType keyType = ContentKeyType.CommonEncryption;
                string keyName = Constant.Media.ContentProtection.ContentKeyNameDrmPlayReadyWidevine;
                IContentKey contentKey = GetContentKey(keyType, keyName, contentProtection);
                policyConfig = new Dictionary<AssetDeliveryPolicyConfigurationKey, string>();
                Uri keyDeliveryUrl = contentKey.GetKeyDeliveryUrl(ContentKeyDeliveryType.PlayReadyLicense);
                policyConfig.Add(AssetDeliveryPolicyConfigurationKey.PlayReadyLicenseAcquisitionUrl, keyDeliveryUrl.ToString());
                keyDeliveryUrl = contentKey.GetKeyDeliveryUrl(ContentKeyDeliveryType.Widevine);
                policyConfig.Add(AssetDeliveryPolicyConfigurationKey.WidevineLicenseAcquisitionUrl, keyDeliveryUrl.ToString());
                ContentKeyDeliveryType[] deliveryTypes = new ContentKeyDeliveryType[] { ContentKeyDeliveryType.PlayReadyLicense, ContentKeyDeliveryType.Widevine };
                IAssetDeliveryPolicy deliveryPolicy = GetDeliveryPolicy(policyName, policyType, policyConfig, deliveryTypes);
                deliveryPolicies.Add(deliveryPolicy);
            }
            else if (contentProtection.DrmPlayReady)
            {
                policyType = AssetDeliveryPolicyType.DynamicCommonEncryption;
                policyName = Constant.Media.DeliveryPolicy.EncryptionDrmPlayReady;
                ContentKeyType keyType = ContentKeyType.CommonEncryption;
                string keyName = Constant.Media.ContentProtection.ContentKeyNameDrmPlayReady;
                IContentKey contentKey = GetContentKey(keyType, keyName, contentProtection);
                Uri keyDeliveryUrl = contentKey.GetKeyDeliveryUrl(ContentKeyDeliveryType.PlayReadyLicense);
                policyConfig = new Dictionary<AssetDeliveryPolicyConfigurationKey, string>();
                policyConfig.Add(AssetDeliveryPolicyConfigurationKey.PlayReadyLicenseAcquisitionUrl, keyDeliveryUrl.ToString());
                ContentKeyDeliveryType[] deliveryTypes = new ContentKeyDeliveryType[] { ContentKeyDeliveryType.PlayReadyLicense };
                IAssetDeliveryPolicy deliveryPolicy = GetDeliveryPolicy(policyName, policyType, policyConfig, deliveryTypes);
                deliveryPolicies.Add(deliveryPolicy);
            }
            else if (contentProtection.DrmWidevine)
            {
                policyType = AssetDeliveryPolicyType.DynamicCommonEncryption;
                policyName = Constant.Media.DeliveryPolicy.EncryptionDrmWidevine;
                ContentKeyType keyType = ContentKeyType.CommonEncryption;
                string keyName = Constant.Media.ContentProtection.ContentKeyNameDrmWidevine;
                IContentKey contentKey = GetContentKey(keyType, keyName, contentProtection);
                Uri keyDeliveryUrl = contentKey.GetKeyDeliveryUrl(ContentKeyDeliveryType.Widevine);
                policyConfig = new Dictionary<AssetDeliveryPolicyConfigurationKey, string>();
                policyConfig.Add(AssetDeliveryPolicyConfigurationKey.WidevineLicenseAcquisitionUrl, keyDeliveryUrl.ToString());
                ContentKeyDeliveryType[] deliveryTypes = new ContentKeyDeliveryType[] { ContentKeyDeliveryType.Widevine };
                IAssetDeliveryPolicy deliveryPolicy = GetDeliveryPolicy(policyName, policyType, policyConfig, deliveryTypes);
                deliveryPolicies.Add(deliveryPolicy);
            }
            return deliveryPolicies.ToArray();
        }

        public void SetDeliveryPolicies(IAsset asset, ContentProtection contentProtection)
        {
            IContentKey[] contentKeys = GetContentKeys(contentProtection);
            foreach (IContentKey contentKey in contentKeys)
            {
                IContentKey[] assetKeys = asset.ContentKeys.Where(k => k.ContentKeyType == contentKey.ContentKeyType).ToArray();
                foreach (IContentKey assetKey in assetKeys)
                {
                    asset.ContentKeys.Remove(assetKey);
                }
                asset.ContentKeys.Add(contentKey);
            }
            IAssetDeliveryPolicy[] deliveryPolicies = asset.DeliveryPolicies.ToArray();
            foreach (IAssetDeliveryPolicy deliveryPolicy in deliveryPolicies)
            {
                asset.DeliveryPolicies.Remove(deliveryPolicy);
            }
            deliveryPolicies = GetDeliveryPolicies(contentProtection);
            foreach (IAssetDeliveryPolicy deliveryPolicy in deliveryPolicies)
            {
                asset.DeliveryPolicies.Add(deliveryPolicy);
            }
        }
    }
}
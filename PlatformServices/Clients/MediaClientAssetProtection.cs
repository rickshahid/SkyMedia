using System;
using System.Linq;
using System.Collections.Generic;
using System.Security.Cryptography;

using Microsoft.WindowsAzure.MediaServices.Client;
using Microsoft.WindowsAzure.MediaServices.Client.Widevine;
using Microsoft.WindowsAzure.MediaServices.Client.DynamicEncryption;
using Microsoft.WindowsAzure.MediaServices.Client.ContentKeyAuthorization;

using Newtonsoft.Json;

namespace AzureSkyMedia.PlatformServices
{
    public partial class MediaClient
    {
        private byte[] CreateEncryptionKey()
        {
            byte[] encryptionKey = new byte[Constant.Media.ContentProtection.EncryptionKeyByteCount];
            using (RNGCryptoServiceProvider encryptionProvider = new RNGCryptoServiceProvider())
            {
                encryptionProvider.GetBytes(encryptionKey);
            }
            return encryptionKey;
        }

        private IContentKey GetContentKey(ContentKeyType keyType, string keyName, ContentProtection contentProtection)
        {
            IContentKey contentKey = GetEntityByName(MediaEntity.ContentKey, keyName, true) as IContentKey;
            if (contentKey == null)
            {
                Guid keyId = Guid.NewGuid();
                byte[] encryptionKey = CreateEncryptionKey();
                contentKey = _media.ContentKeys.Create(keyId, encryptionKey, keyName, keyType);
                SetContentKeyAuthPolicy(contentKey, contentProtection);
            }
            return contentKey;
        }

        private IContentKey[] GetContentKeys(ContentProtection contentProtection)
        {
            List<IContentKey> contentKeys = new List<IContentKey>();
            if (contentProtection.Aes)
            {
                ContentKeyType keyType = ContentKeyType.EnvelopeEncryption;
                string keyName = Constant.Media.ContentProtection.ContentKeyNameAes;
                IContentKey contentKey = GetContentKey(keyType, keyName, contentProtection);
                contentKeys.Add(contentKey);
            }
            if (contentProtection.DrmPlayReady && contentProtection.DrmWidevine)
            {
                ContentKeyType keyType = ContentKeyType.CommonEncryption;
                string keyName = Constant.Media.ContentProtection.ContentKeyNameDrmPlayReadyWidevine;
                IContentKey contentKey = GetContentKey(keyType, keyName, contentProtection);
                contentKeys.Add(contentKey);
            }
            else if (contentProtection.DrmPlayReady)
            {
                ContentKeyType keyType = ContentKeyType.CommonEncryption;
                string keyName = Constant.Media.ContentProtection.ContentKeyNameDrmPlayReady;
                IContentKey contentKey = GetContentKey(keyType, keyName, contentProtection);
                contentKeys.Add(contentKey);
            }
            else if (contentProtection.DrmWidevine)
            {
                ContentKeyType keyType = ContentKeyType.CommonEncryption;
                string keyName = Constant.Media.ContentProtection.ContentKeyNameDrmWidevine;
                IContentKey contentKey = GetContentKey(keyType, keyName, contentProtection);
                contentKeys.Add(contentKey);
            }
            return contentKeys.ToArray();
        }

        private string GetAddressRangeXml(string contentAuthAddressRange)
        {
            string startAddress = contentAuthAddressRange.Trim();
            string endAddress = startAddress;
            if (contentAuthAddressRange.Contains(Constant.TextDelimiter.Application))
            {
                string[] addressRange = contentAuthAddressRange.Split(Constant.TextDelimiter.Application);
                startAddress = addressRange[0].Trim();
                endAddress = addressRange[1].Trim();
            }
            return string.Format(Constant.Media.ContentProtection.AuthAddressRangeXml, startAddress, endAddress);
        }

        private List<ContentKeyAuthorizationPolicyRestriction> CreateContentKeyAuthPolicyRestrictions(string policyName, ContentProtection contentProtection)
        {
            List<ContentKeyAuthorizationPolicyRestriction> policyRestrictions = new List<ContentKeyAuthorizationPolicyRestriction>();
            if (contentProtection.ContentAuthTypeAddress)
            {
                ContentKeyAuthorizationPolicyRestriction policyRestriction = new ContentKeyAuthorizationPolicyRestriction();
                policyRestriction.Name = string.Concat(policyName, Constant.Media.ContentProtection.AuthPolicyAddressRestrictionName);
                policyRestriction.SetKeyRestrictionTypeValue(ContentKeyRestrictionType.IPRestricted);
                policyRestriction.Requirements = GetAddressRangeXml(contentProtection.ContentAuthAddressRange);
                policyRestrictions.Add(policyRestriction);
            }
            if (contentProtection.ContentAuthTypeToken)
            {
                string settingKey = Constant.AppSettingKey.DirectoryDomainName;
                string domainName = AppSetting.GetValue(settingKey);

                settingKey = Constant.AppSettingKey.DirectoryDiscoveryUrl;
                string discoveryUrl = AppSetting.GetValue(settingKey);
                discoveryUrl = string.Format(discoveryUrl, domainName);

                settingKey = Constant.AppSettingKey.DirectoryIssuerUrl;
                string issuerUrl = AppSetting.GetValue(settingKey);

                settingKey = Constant.AppSettingKey.DirectoryClientId;
                string clientId = AppSetting.GetValue(settingKey);

                TokenRestrictionTemplate tokenTemplate = new TokenRestrictionTemplate(TokenType.JWT);
                tokenTemplate.OpenIdConnectDiscoveryDocument = new OpenIdConnectDiscoveryDocument(discoveryUrl);
                tokenTemplate.Issuer = issuerUrl;
                tokenTemplate.Audience = clientId;

                ContentKeyAuthorizationPolicyRestriction policyRestriction = new ContentKeyAuthorizationPolicyRestriction();
                policyRestriction.Name = string.Concat(policyName, Constant.Media.ContentProtection.AuthPolicyTokenRestrictionName);
                policyRestriction.SetKeyRestrictionTypeValue(ContentKeyRestrictionType.TokenRestricted);
                policyRestriction.Requirements = TokenRestrictionTemplateSerializer.Serialize(tokenTemplate);
                policyRestrictions.Add(policyRestriction);
            }
            if (policyRestrictions.Count == 0)
            {
                ContentKeyAuthorizationPolicyRestriction policyRestriction = new ContentKeyAuthorizationPolicyRestriction();
                policyRestriction.Name = string.Concat(policyName, Constant.Media.ContentProtection.AuthPolicyOpenRestrictionName);
                policyRestriction.SetKeyRestrictionTypeValue(ContentKeyRestrictionType.Open);
                policyRestrictions.Add(policyRestriction);
            }
            return policyRestrictions;
        }

        private IContentKeyAuthorizationPolicy CreateContentKeyAuthPolicy(string policyName, IContentKey contentKey, ContentProtection contentProtection)
        {
            IContentKeyAuthorizationPolicy authPolicy = _media.ContentKeyAuthorizationPolicies.CreateAsync(policyName).Result;
            List<ContentKeyAuthorizationPolicyRestriction> policyRestrictions = CreateContentKeyAuthPolicyRestrictions(policyName, contentProtection);
            switch (contentKey.ContentKeyType)
            {
                case ContentKeyType.EnvelopeEncryption:
                    string policyOptionName = string.Concat(policyName, Constant.Media.ContentProtection.AuthPolicyOptionNameAes);
                    IContentKeyAuthorizationPolicyOption policyOption = GetEntityByName(MediaEntity.ContentKeyAuthPolicyOption, policyOptionName, true) as IContentKeyAuthorizationPolicyOption;
                    if (policyOption == null)
                    {
                        ContentKeyDeliveryType deliveryType = ContentKeyDeliveryType.BaselineHttp;
                        string deliveryConfig = string.Empty;
                        policyOption = _media.ContentKeyAuthorizationPolicyOptions.Create(policyOptionName, deliveryType, policyRestrictions, deliveryConfig);
                    }
                    authPolicy.Options.Add(policyOption);
                    break;

                case ContentKeyType.CommonEncryption:
                    if (contentProtection.DrmPlayReady)
                    {
                        policyOptionName = string.Concat(policyName, Constant.Media.ContentProtection.AuthPolicyOptionNameDrmPlayReady);
                        policyOption = GetEntityByName(MediaEntity.ContentKeyAuthPolicyOption, policyOptionName, true) as IContentKeyAuthorizationPolicyOption;
                        if (policyOption == null)
                        {
                            PlayReadyLicenseResponseTemplate responseTemplate = new PlayReadyLicenseResponseTemplate();
                            PlayReadyLicenseTemplate licenseTemplate = new PlayReadyLicenseTemplate();
                            licenseTemplate.PlayRight.AllowPassingVideoContentToUnknownOutput = UnknownOutputPassingOption.NotAllowed;
                            licenseTemplate.LicenseType = PlayReadyLicenseType.Nonpersistent;
                            licenseTemplate.AllowTestDevices = true;
                            responseTemplate.LicenseTemplates.Add(licenseTemplate);

                            ContentKeyDeliveryType deliveryType = ContentKeyDeliveryType.PlayReadyLicense;
                            string deliveryConfig = MediaServicesLicenseTemplateSerializer.Serialize(responseTemplate);
                            policyOption = _media.ContentKeyAuthorizationPolicyOptions.Create(policyOptionName, deliveryType, policyRestrictions, deliveryConfig);
                        }
                        authPolicy.Options.Add(policyOption);
                    }
                    if (contentProtection.DrmWidevine)
                    {
                        policyOptionName = string.Concat(policyName, Constant.Media.ContentProtection.AuthPolicyOptionNameDrmWidevine);
                        policyOption = GetEntityByName(MediaEntity.ContentKeyAuthPolicyOption, policyOptionName, true) as IContentKeyAuthorizationPolicyOption;
                        if (policyOption == null)
                        {
                            ContentKeySpecs contentKeySpecs = new ContentKeySpecs();
                            contentKeySpecs.required_output_protection = new RequiredOutputProtection();
                            contentKeySpecs.required_output_protection.hdcp = Hdcp.HDCP_NONE;
                            contentKeySpecs.security_level = 1;
                            contentKeySpecs.track_type = "SD";

                            WidevineMessage widevineMessage = new WidevineMessage();
                            widevineMessage.allowed_track_types = AllowedTrackTypes.SD_HD;
                            widevineMessage.content_key_specs = new ContentKeySpecs[] { contentKeySpecs };
                            widevineMessage.policy_overrides = new
                            {
                                can_play = true,
                                can_renew = true,
                                can_persist = false
                            };

                            ContentKeyDeliveryType deliveryType = ContentKeyDeliveryType.Widevine;
                            string deliveryConfig = JsonConvert.SerializeObject(widevineMessage);
                            policyOption = _media.ContentKeyAuthorizationPolicyOptions.Create(policyOptionName, deliveryType, policyRestrictions, deliveryConfig);
                        }
                        authPolicy.Options.Add(policyOption);
                    }
                    break;
            }
            return authPolicy;
        }

        private void SetContentKeyAuthPolicy(IContentKey contentKey, ContentProtection contentProtection)
        {
            string policyName = string.Concat(contentKey.Name, Constant.Media.ContentProtection.AuthPolicyName);
            IContentKeyAuthorizationPolicy authPolicy = GetEntityByName(MediaEntity.ContentKeyAuthPolicy, policyName, true) as IContentKeyAuthorizationPolicy;
            if (authPolicy == null)
            {
                authPolicy = CreateContentKeyAuthPolicy(policyName, contentKey, contentProtection);
            }
            if (contentKey.AuthorizationPolicyId != authPolicy.Id)
            {
                contentKey.AuthorizationPolicyId = authPolicy.Id;
                contentKey.Update();
            }
        }

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
                }
            }
            return deliveryProtocols;
        }

        private IAssetDeliveryPolicy GetDeliveryPolicy(AssetDeliveryPolicyType policyType, Dictionary<AssetDeliveryPolicyConfigurationKey,
                                                       string> policyConfig, string policyName, ContentKeyDeliveryType[] deliveryTypes)
        {
            IAssetDeliveryPolicy deliveryPolicy = GetEntityByName(MediaEntity.DeliveryPolicy, policyName, true) as IAssetDeliveryPolicy;
            if (deliveryPolicy == null)
            {
                AssetDeliveryProtocol policyProtocols = GetDeliveryProtocols(deliveryTypes);
                deliveryPolicy = _media.AssetDeliveryPolicies.Create(policyName, policyType, policyProtocols, policyConfig);
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
                IAssetDeliveryPolicy deliveryPolicy = GetDeliveryPolicy(policyType, policyConfig, policyName, deliveryTypes);
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
                IAssetDeliveryPolicy deliveryPolicy = GetDeliveryPolicy(policyType, policyConfig, policyName, deliveryTypes);
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
                IAssetDeliveryPolicy deliveryPolicy = GetDeliveryPolicy(policyType, policyConfig, policyName, deliveryTypes);
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
                IAssetDeliveryPolicy deliveryPolicy = GetDeliveryPolicy(policyType, policyConfig, policyName, deliveryTypes);
                deliveryPolicies.Add(deliveryPolicy);
            }
            return deliveryPolicies.ToArray();
        }

        public void AddDeliveryPolicies(IAsset asset, ContentProtection contentProtection)
        {
            IContentKey[] contentKeys = GetContentKeys(contentProtection);
            foreach (IContentKey contentKey in contentKeys)
            {
                asset.ContentKeys.Add(contentKey);
            }
            IAssetDeliveryPolicy[] deliveryPolicies = GetDeliveryPolicies(contentProtection);
            foreach (IAssetDeliveryPolicy deliveryPolicy in deliveryPolicies)
            {
                asset.DeliveryPolicies.Add(deliveryPolicy);
            }
        }

        public MediaProtection[] GetProtectionTypes(IAsset asset)
        {
            List<MediaProtection> protectionTypes = new List<MediaProtection>();
            IAssetDeliveryPolicy deliveryPolicy = asset.DeliveryPolicies.Where(p => p.AssetDeliveryPolicyType == AssetDeliveryPolicyType.DynamicEnvelopeEncryption).SingleOrDefault();
            if (deliveryPolicy != null)
            {
                protectionTypes.Add(MediaProtection.AES);
            }
            deliveryPolicy = asset.DeliveryPolicies.Where(p => p.AssetDeliveryPolicyType == AssetDeliveryPolicyType.DynamicCommonEncryption).SingleOrDefault();
            if (deliveryPolicy != null)
            {
                IContentKey contentKey = asset.ContentKeys.Where(k => k.ContentKeyType == ContentKeyType.CommonEncryption).SingleOrDefault();
                if (contentKey != null)
                {
                    IContentKeyAuthorizationPolicy authPolicy = GetEntityById(MediaEntity.ContentKeyAuthPolicy, contentKey.AuthorizationPolicyId) as IContentKeyAuthorizationPolicy;
                    if (authPolicy != null)
                    {
                        IContentKeyAuthorizationPolicyOption policyOption = authPolicy.Options.Where(o => o.KeyDeliveryType == ContentKeyDeliveryType.PlayReadyLicense).SingleOrDefault();
                        if (policyOption != null)
                        {
                            protectionTypes.Add(MediaProtection.PlayReady);
                        }
                        policyOption = authPolicy.Options.Where(o => o.KeyDeliveryType == ContentKeyDeliveryType.Widevine).SingleOrDefault();
                        if (policyOption != null)
                        {
                            protectionTypes.Add(MediaProtection.Widevine);
                        }
                    }
                }
            }
            return protectionTypes.ToArray();
        }
    }
}

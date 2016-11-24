using System;
using System.Linq;
using System.Collections.Generic;
using System.Security.Cryptography;

using Microsoft.WindowsAzure.MediaServices.Client;
using Microsoft.WindowsAzure.MediaServices.Client.Widevine;
using Microsoft.WindowsAzure.MediaServices.Client.DynamicEncryption;
using Microsoft.WindowsAzure.MediaServices.Client.ContentKeyAuthorization;

using Newtonsoft.Json;

using SkyMedia.WebApp.Models;

namespace SkyMedia.ServiceBroker
{
    internal partial class MediaClient
    {
        private byte[] CreateEncryptionKey()
        {
            byte[] encryptionKey = new byte[Constants.Media.ContentProtection.EncryptionKeyByteCount];
            using (RNGCryptoServiceProvider encryptionProvider = new RNGCryptoServiceProvider())
            {
                encryptionProvider.GetBytes(encryptionKey);
            }
            return encryptionKey;
        }

        private IContentKey GetContentKey(ContentKeyType keyType, string keyName, ContentProtection contentProtection)
        {
            IContentKey contentKey = GetEntityByName(EntityType.ContentKey, keyName, true) as IContentKey;
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
            if (contentProtection.AES)
            {
                ContentKeyType keyType = ContentKeyType.EnvelopeEncryption;
                string keyName = Constants.Media.ContentProtection.ContentKeyNameAES;
                IContentKey contentKey = GetContentKey(keyType, keyName, contentProtection);
                contentKeys.Add(contentKey);
            }
            if (contentProtection.DRMPlayReady && contentProtection.DRMWidevine)
            {
                ContentKeyType keyType = ContentKeyType.CommonEncryption;
                string keyName = Constants.Media.ContentProtection.ContentKeyNameDRMPlayReadyWidevine;
                IContentKey contentKey = GetContentKey(keyType, keyName, contentProtection);
                contentKeys.Add(contentKey);
            }
            else if (contentProtection.DRMPlayReady)
            {
                ContentKeyType keyType = ContentKeyType.CommonEncryption;
                string keyName = Constants.Media.ContentProtection.ContentKeyNameDRMPlayReady;
                IContentKey contentKey = GetContentKey(keyType, keyName, contentProtection);
                contentKeys.Add(contentKey);
            }
            else if (contentProtection.DRMWidevine)
            {
                ContentKeyType keyType = ContentKeyType.CommonEncryption;
                string keyName = Constants.Media.ContentProtection.ContentKeyNameDRMWidevine;
                IContentKey contentKey = GetContentKey(keyType, keyName, contentProtection);
                contentKeys.Add(contentKey);
            }
            return contentKeys.ToArray();
        }

        private string GetAddressRangeXml(string contentAuthAddressRange)
        {
            string startAddress = contentAuthAddressRange.Trim();
            string endAddress = startAddress;
            if (contentAuthAddressRange.Contains(Constants.MultiItemSeparator))
            {
                string[] addressRange = contentAuthAddressRange.Split(Constants.MultiItemSeparator);
                startAddress = addressRange[0].Trim();
                endAddress = addressRange[1].Trim();
            }
            return string.Format(Constants.Media.ContentProtection.AuthAddressRangeXML, startAddress, endAddress);
        }

        private List<ContentKeyAuthorizationPolicyRestriction> CreateContentKeyAuthPolicyRestrictions(string policyName, ContentProtection contentProtection)
        {
            List<ContentKeyAuthorizationPolicyRestriction> policyRestrictions = new List<ContentKeyAuthorizationPolicyRestriction>();
            if (contentProtection.ContentAuthTypeAddress)
            {
                ContentKeyAuthorizationPolicyRestriction policyRestriction = new ContentKeyAuthorizationPolicyRestriction();
                policyRestriction.Name = string.Concat(policyName, Constants.Media.ContentProtection.AuthPolicyAddressRestrictionName);
                policyRestriction.SetKeyRestrictionTypeValue(ContentKeyRestrictionType.IPRestricted);
                policyRestriction.Requirements = GetAddressRangeXml(contentProtection.ContentAuthAddressRange);
                policyRestrictions.Add(policyRestriction);
            }
            if (contentProtection.ContentAuthTypeToken)
            {
                string settingKey = Constants.AppSettings.DirectoryDiscoveryUrl;
                string discoveryUrl = AppSetting.GetValue(settingKey);

                settingKey = Constants.AppSettings.DirectoryIssuerUrl;
                string issuerUrl = AppSetting.GetValue(settingKey);

                settingKey = Constants.AppSettings.DirectoryClientId;
                string clientId = AppSetting.GetValue(settingKey);

                TokenRestrictionTemplate tokenTemplate = new TokenRestrictionTemplate(TokenType.JWT);
                tokenTemplate.OpenIdConnectDiscoveryDocument = new OpenIdConnectDiscoveryDocument(discoveryUrl);
                tokenTemplate.Issuer = issuerUrl;
                tokenTemplate.Audience = clientId;

                ContentKeyAuthorizationPolicyRestriction policyRestriction = new ContentKeyAuthorizationPolicyRestriction();
                policyRestriction.Name = string.Concat(policyName, Constants.Media.ContentProtection.AuthPolicyTokenRestrictionName);
                policyRestriction.SetKeyRestrictionTypeValue(ContentKeyRestrictionType.TokenRestricted);
                policyRestriction.Requirements = TokenRestrictionTemplateSerializer.Serialize(tokenTemplate);
                policyRestrictions.Add(policyRestriction);
            }
            if (policyRestrictions.Count == 0)
            {
                ContentKeyAuthorizationPolicyRestriction policyRestriction = new ContentKeyAuthorizationPolicyRestriction();
                policyRestriction.Name = string.Concat(policyName, Constants.Media.ContentProtection.AuthPolicyOpenRestrictionName);
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
                    string policyOptionName = string.Concat(policyName, Constants.Media.ContentProtection.AuthPolicyOptionNameAES);
                    IContentKeyAuthorizationPolicyOption policyOption = GetEntityByName(EntityType.ContentKeyAuthPolicyOption, policyOptionName, true) as IContentKeyAuthorizationPolicyOption;
                    if (policyOption == null)
                    {
                        ContentKeyDeliveryType deliveryType = ContentKeyDeliveryType.BaselineHttp;
                        string deliveryConfig = string.Empty;
                        policyOption = _media.ContentKeyAuthorizationPolicyOptions.Create(policyOptionName, deliveryType, policyRestrictions, deliveryConfig);
                    }
                    authPolicy.Options.Add(policyOption);
                    break;

                case ContentKeyType.CommonEncryption:
                    if (contentProtection.DRMPlayReady)
                    {
                        policyOptionName = string.Concat(policyName, Constants.Media.ContentProtection.AuthPolicyOptionNameDRMPlayReady);
                        policyOption = GetEntityByName(EntityType.ContentKeyAuthPolicyOption, policyOptionName, true) as IContentKeyAuthorizationPolicyOption;
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
                    if (contentProtection.DRMWidevine)
                    {
                        policyOptionName = string.Concat(policyName, Constants.Media.ContentProtection.AuthPolicyOptionNameDRMWidevine);
                        policyOption = GetEntityByName(EntityType.ContentKeyAuthPolicyOption, policyOptionName, true) as IContentKeyAuthorizationPolicyOption;
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
            string policyName = string.Concat(contentKey.Name, Constants.Media.ContentProtection.AuthPolicyName);
            IContentKeyAuthorizationPolicy authPolicy = GetEntityByName(EntityType.ContentKeyAuthPolicy, policyName, true) as IContentKeyAuthorizationPolicy;
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

        private AssetDeliveryProtocol GetDeliveryProtocols(ContentKeyDeliveryType deliveryType)
        {
            AssetDeliveryProtocol deliveryProtocols = AssetDeliveryProtocol.None;
            switch (deliveryType)
            {
                case ContentKeyDeliveryType.BaselineHttp:
                    deliveryProtocols = Constants.Media.DeliveryProtocol.AES;
                    break;
                case ContentKeyDeliveryType.PlayReadyLicense:
                    deliveryProtocols = Constants.Media.DeliveryProtocol.DRMPlayReady;
                    break;
                case ContentKeyDeliveryType.Widevine:
                    deliveryProtocols = Constants.Media.DeliveryProtocol.DRMWidevine;
                    break;
            }
            return deliveryProtocols;
        }

        private IAssetDeliveryPolicy GetDeliveryPolicy(AssetDeliveryPolicyType policyType, Dictionary<AssetDeliveryPolicyConfigurationKey,
                                                       string> policyConfig, string policyName, ContentKeyDeliveryType deliveryType)
        {
            IAssetDeliveryPolicy deliveryPolicy = GetEntityByName(EntityType.DeliveryPolicy, policyName, true) as IAssetDeliveryPolicy;
            if (deliveryPolicy == null)
            {
                AssetDeliveryProtocol policyProtocols = GetDeliveryProtocols(deliveryType);
                deliveryPolicy = _media.AssetDeliveryPolicies.Create(policyName, policyType, policyProtocols, policyConfig);
            }
            return deliveryPolicy;
        }

        private IAssetDeliveryPolicy[] GetDeliveryPolicies(ContentProtection contentProtection)
        {
            List<IAssetDeliveryPolicy> deliveryPolicies = new List<IAssetDeliveryPolicy>();
            AssetDeliveryPolicyType policyType = AssetDeliveryPolicyType.NoDynamicEncryption;
            Dictionary<AssetDeliveryPolicyConfigurationKey, string> policyConfig = null;
            string policyName = Constants.Media.DeliveryPolicy.DecryptionStorage;
            if (contentProtection.AES)
            {
                policyType = AssetDeliveryPolicyType.DynamicEnvelopeEncryption;
                policyName = Constants.Media.DeliveryPolicy.EncryptionAES;
                ContentKeyType keyType = ContentKeyType.EnvelopeEncryption;
                string keyName = Constants.Media.ContentProtection.ContentKeyNameAES;
                IContentKey contentKey = GetContentKey(keyType, keyName, contentProtection);
                Uri keyDeliveryUrl = contentKey.GetKeyDeliveryUrl(ContentKeyDeliveryType.BaselineHttp);
                byte[] encryptionIV = CreateEncryptionKey();
                policyConfig = new Dictionary<AssetDeliveryPolicyConfigurationKey, string>();
                policyConfig.Add(AssetDeliveryPolicyConfigurationKey.EnvelopeKeyAcquisitionUrl, keyDeliveryUrl.ToString());
                policyConfig.Add(AssetDeliveryPolicyConfigurationKey.EnvelopeEncryptionIVAsBase64, Convert.ToBase64String(encryptionIV));
                IAssetDeliveryPolicy deliveryPolicy = GetDeliveryPolicy(policyType, policyConfig, policyName, ContentKeyDeliveryType.BaselineHttp);
                deliveryPolicies.Add(deliveryPolicy);
            }
            if (contentProtection.DRMPlayReady && contentProtection.DRMWidevine)
            {
                policyType = AssetDeliveryPolicyType.DynamicCommonEncryption;
                policyName = Constants.Media.DeliveryPolicy.EncryptionDRMPlayReadyWidevine;
                ContentKeyType keyType = ContentKeyType.CommonEncryption;
                string keyName = Constants.Media.ContentProtection.ContentKeyNameDRMPlayReadyWidevine;
                IContentKey contentKey = GetContentKey(keyType, keyName, contentProtection);
                policyConfig = new Dictionary<AssetDeliveryPolicyConfigurationKey, string>();
                Uri keyDeliveryUrl = contentKey.GetKeyDeliveryUrl(ContentKeyDeliveryType.PlayReadyLicense);
                policyConfig.Add(AssetDeliveryPolicyConfigurationKey.PlayReadyLicenseAcquisitionUrl, keyDeliveryUrl.ToString());
                keyDeliveryUrl = contentKey.GetKeyDeliveryUrl(ContentKeyDeliveryType.Widevine);
                policyConfig.Add(AssetDeliveryPolicyConfigurationKey.WidevineLicenseAcquisitionUrl, keyDeliveryUrl.ToString());
                IAssetDeliveryPolicy deliveryPolicy = GetDeliveryPolicy(policyType, policyConfig, policyName, ContentKeyDeliveryType.PlayReadyLicense);
                deliveryPolicies.Add(deliveryPolicy);
                deliveryPolicy = GetDeliveryPolicy(policyType, policyConfig, policyName, ContentKeyDeliveryType.Widevine);
                deliveryPolicies.Add(deliveryPolicy);
            }
            else if (contentProtection.DRMPlayReady)
            {
                policyType = AssetDeliveryPolicyType.DynamicCommonEncryption;
                policyName = Constants.Media.DeliveryPolicy.EncryptionDRMPlayReady;
                ContentKeyType keyType = ContentKeyType.CommonEncryption;
                string keyName = Constants.Media.ContentProtection.ContentKeyNameDRMPlayReady;
                IContentKey contentKey = GetContentKey(keyType, keyName, contentProtection);
                Uri keyDeliveryUrl = contentKey.GetKeyDeliveryUrl(ContentKeyDeliveryType.PlayReadyLicense);
                policyConfig = new Dictionary<AssetDeliveryPolicyConfigurationKey, string>();
                policyConfig.Add(AssetDeliveryPolicyConfigurationKey.PlayReadyLicenseAcquisitionUrl, keyDeliveryUrl.ToString());
                IAssetDeliveryPolicy deliveryPolicy = GetDeliveryPolicy(policyType, policyConfig, policyName, ContentKeyDeliveryType.PlayReadyLicense);
                deliveryPolicies.Add(deliveryPolicy);
            }
            else if (contentProtection.DRMWidevine)
            {
                policyType = AssetDeliveryPolicyType.DynamicCommonEncryption;
                policyName = Constants.Media.DeliveryPolicy.EncryptionDRMWidevine;
                ContentKeyType keyType = ContentKeyType.CommonEncryption;
                string keyName = Constants.Media.ContentProtection.ContentKeyNameDRMWidevine;
                IContentKey contentKey = GetContentKey(keyType, keyName, contentProtection);
                Uri keyDeliveryUrl = contentKey.GetKeyDeliveryUrl(ContentKeyDeliveryType.Widevine);
                policyConfig = new Dictionary<AssetDeliveryPolicyConfigurationKey, string>();
                policyConfig.Add(AssetDeliveryPolicyConfigurationKey.WidevineLicenseAcquisitionUrl, keyDeliveryUrl.ToString());
                IAssetDeliveryPolicy deliveryPolicy = GetDeliveryPolicy(policyType, policyConfig, policyName, ContentKeyDeliveryType.Widevine);
                deliveryPolicies.Add(deliveryPolicy);
            }
            return deliveryPolicies.ToArray();
        }

        public void AddDeliveryPolicies(IAsset asset, ContentProtection contentProtection)
        {
            if (asset.Options == AssetCreationOptions.StorageEncrypted)
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
        }

        public string[] GetProtectionTypes(IAsset asset)
        {
            List<string> protectionTypes = new List<string>();
            IAssetDeliveryPolicy deliveryPolicy = asset.DeliveryPolicies.Where(p => p.AssetDeliveryPolicyType == AssetDeliveryPolicyType.DynamicEnvelopeEncryption).SingleOrDefault();
            if (deliveryPolicy != null)
            {
                protectionTypes.Add(ProtectionType.AES.ToString());
            }
            deliveryPolicy = asset.DeliveryPolicies.Where(p => p.AssetDeliveryPolicyType == AssetDeliveryPolicyType.DynamicCommonEncryption).SingleOrDefault();
            if (deliveryPolicy != null)
            {
                IContentKey contentKey = asset.ContentKeys.Where(k => k.ContentKeyType == ContentKeyType.CommonEncryption).SingleOrDefault();
                if (contentKey != null)
                {
                    IContentKeyAuthorizationPolicy authPolicy = GetEntityById(EntityType.ContentKeyAuthPolicy, contentKey.AuthorizationPolicyId) as IContentKeyAuthorizationPolicy;
                    if (authPolicy != null)
                    {
                        IContentKeyAuthorizationPolicyOption policyOption = authPolicy.Options.Where(o => o.KeyDeliveryType == ContentKeyDeliveryType.PlayReadyLicense).SingleOrDefault();
                        if (policyOption != null)
                        {
                            protectionTypes.Add(ProtectionType.PlayReady.ToString());
                        }
                        policyOption = authPolicy.Options.Where(o => o.KeyDeliveryType == ContentKeyDeliveryType.Widevine).SingleOrDefault();
                        if (policyOption != null)
                        {
                            protectionTypes.Add(ProtectionType.Widevine.ToString());
                        }
                    }
                }
            }
            return protectionTypes.ToArray();
        }
    }
}

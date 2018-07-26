using System.Collections.Generic;

using Microsoft.Azure.Management.Media;
using Microsoft.Azure.Management.Media.Models;

namespace AzureSkyMedia.PlatformServices
{
    internal partial class MediaClient
    {
        public void CreateContentKeyPolicy(string policyName, ContentKeyPolicyConfiguration[] policyConfigurations)
        {
            ContentKeyPolicy keyPolicy = _media.ContentKeyPolicies.Get(MediaAccount.ResourceGroupName, MediaAccount.Name, policyName);
            if (keyPolicy == null)
            {
                string settingKey = Constant.AppSettingKey.DirectoryClientId;
                string clientId = AppSetting.GetValue(settingKey);

                settingKey = Constant.AppSettingKey.DirectoryTenantId;
                string tenantId = AppSetting.GetValue(settingKey);

                settingKey = Constant.AppSettingKey.DirectoryIssuerUrl;
                string issuerUrl = AppSetting.GetValue(settingKey);
                issuerUrl = string.Format(issuerUrl, tenantId);

                settingKey = Constant.AppSettingKey.DirectoryPolicyIdSignUpIn;
                string signUpInPolicyId = AppSetting.GetValue(settingKey);

                settingKey = Constant.AppSettingKey.DirectoryDiscoveryPath;
                string discoveryPath = AppSetting.GetValue(settingKey);
                discoveryPath = string.Concat(discoveryPath, "?p=", signUpInPolicyId);

                ContentKeyPolicyTokenRestriction policyRestriction = new ContentKeyPolicyTokenRestriction()
                {
                    OpenIdConnectDiscoveryDocument = string.Concat(issuerUrl, discoveryPath),
                    RestrictionTokenType = ContentKeyPolicyRestrictionTokenType.Jwt,
                    Audience = clientId,
                    Issuer = issuerUrl
                };

                List<ContentKeyPolicyOption> policyOptions = new List<ContentKeyPolicyOption>();
                foreach (ContentKeyPolicyConfiguration policyConfiguration in policyConfigurations)
                {
                    ContentKeyPolicyOption policyOption = new ContentKeyPolicyOption(policyConfiguration, policyRestriction);
                    policyOptions.Add(policyOption);
                }
                keyPolicy = _media.ContentKeyPolicies.CreateOrUpdate(MediaAccount.ResourceGroupName, MediaAccount.Name, policyName, policyOptions);
            }
        }

        public StreamProtection[] GetProtectionInfo(string authToken, MediaClient mediaClient, StreamingLocator locator)
        {
            authToken = string.Concat("Bearer=", authToken);
            List<StreamProtection> protectionInfo = new List<StreamProtection>();
            if (locator.StreamingPolicyName == PredefinedStreamingPolicy.ClearKey)
            {
                StreamProtection streamProtection = new StreamProtection()
                {
                    Type = MediaProtection.AES,
                    AuthenticationToken = authToken
                };
                protectionInfo.Add(streamProtection);
            }
            else if (locator.StreamingPolicyName == PredefinedStreamingPolicy.SecureStreaming)
            {
                ContentKeyPolicy contentKeyPolicy = mediaClient.GetEntity<ContentKeyPolicy>(MediaEntity.ContentKeyPolicy, locator.DefaultContentKeyPolicyName);
                foreach (ContentKeyPolicyOption contentKeyPolicyOption in contentKeyPolicy.Options)
                {
                    if (contentKeyPolicyOption.Configuration is ContentKeyPolicyPlayReadyConfiguration)
                    {
                        StreamProtection streamProtection = new StreamProtection()
                        {
                            Type = MediaProtection.PlayReady,
                            AuthenticationToken = authToken
                        };
                        protectionInfo.Add(streamProtection);
                    }
                    else if (contentKeyPolicyOption.Configuration is ContentKeyPolicyWidevineConfiguration)
                    {
                        StreamProtection streamProtection = new StreamProtection()
                        {
                            Type = MediaProtection.Widevine,
                            AuthenticationToken = authToken
                        };
                        protectionInfo.Add(streamProtection);
                    }
                }
            }
            return protectionInfo.ToArray();
        }
    }
}
using System.Collections.Generic;

using Microsoft.Azure.Management.Media;
using Microsoft.Azure.Management.Media.Models;

namespace AzureSkyMedia.PlatformServices
{
    internal partial class MediaClient
    {
        public ContentKeyPolicy CreateContentKeyPolicy(string policyName, ContentKeyPolicyConfiguration[] policyConfigurations)
        {
            ContentKeyPolicy contentKeyPolicy = _media.ContentKeyPolicies.Get(MediaAccount.ResourceGroupName, MediaAccount.Name, policyName);
            if (contentKeyPolicy == null)
            {
                string settingKey = Constant.AppSettingKey.DirectoryClientId;
                string clientId = AppSetting.GetValue(settingKey);

                settingKey = Constant.AppSettingKey.DirectoryTenantId;
                string tenantId = AppSetting.GetValue(settingKey);

                settingKey = Constant.AppSettingKey.DirectoryAuthorityUrl;
                string authorityUrl = AppSetting.GetValue(settingKey);
                authorityUrl = string.Format(authorityUrl, tenantId);

                settingKey = Constant.AppSettingKey.DirectoryPolicyIdSignUpIn;
                string signUpInPolicyId = AppSetting.GetValue(settingKey);

                settingKey = Constant.AppSettingKey.DirectoryDiscoveryPath;
                string discoveryPath = AppSetting.GetValue(settingKey);
                discoveryPath = string.Concat(discoveryPath, "?p=", signUpInPolicyId);

                ContentKeyPolicyTokenRestriction policyRestriction = new ContentKeyPolicyTokenRestriction()
                {
                    OpenIdConnectDiscoveryDocument = string.Concat(authorityUrl, discoveryPath),
                    RestrictionTokenType = ContentKeyPolicyRestrictionTokenType.Jwt,
                    Issuer = authorityUrl,
                    Audience = clientId
                };

                List<ContentKeyPolicyOption> policyOptions = new List<ContentKeyPolicyOption>();
                foreach (ContentKeyPolicyConfiguration policyConfiguration in policyConfigurations)
                {
                    ContentKeyPolicyOption policyOption = new ContentKeyPolicyOption(policyConfiguration, policyRestriction);
                    policyOptions.Add(policyOption);
                }
                contentKeyPolicy = _media.ContentKeyPolicies.CreateOrUpdate(MediaAccount.ResourceGroupName, MediaAccount.Name, policyName, policyOptions);
            }
            return contentKeyPolicy;
        }

        public MediaProtection[] GetStreamProtection(string authToken, MediaClient mediaClient, StreamingLocator locator)
        {
            List<MediaProtection> streamProtection = new List<MediaProtection>();
            if (locator.StreamingPolicyName == PredefinedStreamingPolicy.ClearKey)
            {
                streamProtection.Add(new MediaProtection()
                {
                    Type = MediaContentProtection.AES,
                    AuthenticationToken = authToken
                });
            }
            else if (locator.StreamingPolicyName == PredefinedStreamingPolicy.MultiDrmCencStreaming || locator.StreamingPolicyName == PredefinedStreamingPolicy.MultiDrmStreaming)
            {
                ContentKeyPolicy contentKeyPolicy = mediaClient.GetEntity<ContentKeyPolicy>(MediaEntity.ContentKeyPolicy, locator.DefaultContentKeyPolicyName);
                foreach (ContentKeyPolicyOption contentKeyPolicyOption in contentKeyPolicy.Options)
                {
                    if (contentKeyPolicyOption.Configuration is ContentKeyPolicyPlayReadyConfiguration)
                    {
                        streamProtection.Add(new MediaProtection()
                        {
                            Type = MediaContentProtection.PlayReady,
                            AuthenticationToken = authToken
                        });
                    }
                    else if (contentKeyPolicyOption.Configuration is ContentKeyPolicyWidevineConfiguration)
                    {
                        streamProtection.Add(new MediaProtection()
                        {
                            Type = MediaContentProtection.Widevine,
                            AuthenticationToken = authToken
                        });
                    }
                }
            }
            return streamProtection.ToArray();
        }
    }
}
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
                string settingKey = Constant.AppSettingKey.DirectoryTenantId;
                string tenantId = AppSetting.GetValue(settingKey);

                settingKey = Constant.AppSettingKey.DirectoryIssuerUrl;
                string issuerUrl = AppSetting.GetValue(settingKey);
                issuerUrl = string.Format(issuerUrl, tenantId);

                settingKey = Constant.AppSettingKey.DirectoryDiscoveryPath;
                string discoveryPath = AppSetting.GetValue(settingKey);

                settingKey = Constant.AppSettingKey.DirectoryClientId;
                string clientId = AppSetting.GetValue(settingKey);

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
    }
}
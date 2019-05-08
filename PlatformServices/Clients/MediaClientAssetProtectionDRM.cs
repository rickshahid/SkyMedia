using Microsoft.Azure.Management.Media.Models;

using Newtonsoft.Json;

namespace AzureSkyMedia.PlatformServices
{
    internal partial class MediaClient
    {
        public void CreateContentKeyPolicyDrm(string policyName, ContentProtection contentProtection)
        {
            ContentKeyPolicyPlayReadyLicenseType licenseType = ContentKeyPolicyPlayReadyLicenseType.NonPersistent;
            if (contentProtection != null && contentProtection.PersistentLicense)
            {
                licenseType = ContentKeyPolicyPlayReadyLicenseType.Persistent;
            }
            ContentKeyPolicyPlayReadyLicense playReadyLicense = new ContentKeyPolicyPlayReadyLicense()
            {
                LicenseType = licenseType,
                ContentType = ContentKeyPolicyPlayReadyContentType.Unspecified,
                ContentKeyLocation = new ContentKeyPolicyPlayReadyContentEncryptionKeyFromHeader()
            };
            ContentKeyPolicyPlayReadyConfiguration playReadyConfiguration = new ContentKeyPolicyPlayReadyConfiguration()
            {
                Licenses = new ContentKeyPolicyPlayReadyLicense[] { playReadyLicense }
            };
            WidevineTemplate widevineTemplate = new WidevineTemplate();
            if (contentProtection != null && contentProtection.PersistentLicense)
            {
                widevineTemplate.PolicyOverrides = new PolicyOverrides()
                {
                    CanPersist = true
                };
            }
            ContentKeyPolicyWidevineConfiguration widevineConfiguration = new ContentKeyPolicyWidevineConfiguration()
            {
                WidevineTemplate = JsonConvert.SerializeObject(widevineTemplate)
            };
            ContentKeyPolicyConfiguration[] policyConfigurations = new ContentKeyPolicyConfiguration[]
            {
                    playReadyConfiguration,
                    widevineConfiguration
            };
            CreateContentKeyPolicy(policyName, policyConfigurations);
        }
    }
}
using Microsoft.Azure.Management.Media.Models;

namespace AzureSkyMedia.PlatformServices
{
    internal partial class MediaClient
    {
        public void CreateContentKeyPolicyDRM()
        {
            ContentKeyPolicyPlayReadyLicense playReadyLicence = new ContentKeyPolicyPlayReadyLicense();
            ContentKeyPolicyPlayReadyConfiguration playReadyConfiguration = new ContentKeyPolicyPlayReadyConfiguration()
            {
                Licenses = new ContentKeyPolicyPlayReadyLicense[] { playReadyLicence }
            };
            ContentKeyPolicyWidevineConfiguration widevineConfiguration = new ContentKeyPolicyWidevineConfiguration();
            string policyName = Constant.Media.ContentKey.PolicyDRM;
            ContentKeyPolicyConfiguration[] policyConfigurations = new ContentKeyPolicyConfiguration[]
            {
                    playReadyConfiguration,
                    widevineConfiguration
            };
            CreateContentKeyPolicy(policyName, policyConfigurations);
        }
    }
}
using Microsoft.Azure.Management.Media.Models;

namespace AzureSkyMedia.PlatformServices
{
    internal partial class MediaClient
    {
        public void CreateContentKeyPolicyAes(string policyName)
        {
            ContentKeyPolicyConfiguration[] policyConfigurations = new ContentKeyPolicyConfiguration[]
            {
                new ContentKeyPolicyClearKeyConfiguration()
            };
            CreateContentKeyPolicy(policyName, policyConfigurations);
        }
    }
}
using Microsoft.Azure.Management.Media.Models;

namespace AzureSkyMedia.PlatformServices
{
    internal partial class MediaClient
    {
        public void CreateContentKeyPolicyAES()
        {
            string policyName = Constant.Media.ContentKey.PolicyAES;
            ContentKeyPolicyConfiguration[] policyConfigurations = new ContentKeyPolicyConfiguration[]
            {
                    new ContentKeyPolicyClearKeyConfiguration()
            };
            CreateContentKeyPolicy(policyName, policyConfigurations);
        }
    }
}
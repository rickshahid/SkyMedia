using Microsoft.Azure.Management.Media;
using Microsoft.Azure.Management.Media.Models;

namespace AzureSkyMedia.PlatformServices
{
    internal partial class MediaClient
    {
        private string GetDefaultUrl(string relativeUrl)
        {
            StreamingEndpoint defaultEndpoint = GetEntity<StreamingEndpoint>(MediaEntity.StreamingEndpoint, Constant.Media.Stream.DefaultEndpoint);
            return string.Concat("//", defaultEndpoint.HostName, relativeUrl);
        }

        public string GetPlayerUrl(StreamingLocator locator)
        {
            string playerUrl = string.Empty;
            ListPathsResponse paths = _media.StreamingLocators.ListPaths(MediaAccount.ResourceGroupName, MediaAccount.Name, locator.Name);
            foreach (StreamingPath streamingPath in paths.StreamingPaths)
            {
                if (streamingPath.StreamingProtocol == StreamingPolicyStreamingProtocol.SmoothStreaming && streamingPath.Paths.Count > 0)
                {
                    playerUrl = streamingPath.Paths[0];
                }
            }
            if (string.IsNullOrEmpty(playerUrl) && paths.DownloadPaths.Count > 0)
            {
                playerUrl = paths.DownloadPaths[0];
            }
            if (!string.IsNullOrEmpty(playerUrl))
            {
                playerUrl = GetDefaultUrl(playerUrl);
            }
            return playerUrl;
        }

        public StreamingLocator CreateLocator(string assetName, string streamingPolicyName, ContentProtection contentProtection)
        {
            string contentKeyPolicyName = null;
            if (streamingPolicyName == PredefinedStreamingPolicy.ClearKey)
            {
                CreateContentKeyPolicyAES();
                contentKeyPolicyName = Constant.Media.ContentKey.PolicyAES;
            }
            else if (streamingPolicyName == PredefinedStreamingPolicy.SecureStreaming)
            {
                CreateContentKeyPolicyDRM(contentProtection);
                contentKeyPolicyName = Constant.Media.ContentKey.PolicyDRM;
            }
            StreamingLocator locator = new StreamingLocator(assetName, streamingPolicyName)
            {
                DefaultContentKeyPolicyName = contentKeyPolicyName
            };
            _media.StreamingLocators.Delete(MediaAccount.ResourceGroupName, MediaAccount.Name, assetName);
            return _media.StreamingLocators.Create(MediaAccount.ResourceGroupName, MediaAccount.Name, assetName, locator);
        }
    }
}
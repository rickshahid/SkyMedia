using System.Threading.Tasks;

using Microsoft.Rest.Azure;
using Microsoft.Azure.Management.Media.Models;

namespace AzureSkyMedia.PlatformServices
{
    internal partial class MediaClient
    {
        private string GetStreamingPath(StreamingLocator locator, StreamingPolicyStreamingProtocol protocol)
        {
            string streamingPath = string.Empty;
            Task<AzureOperationResponse<ListPathsResponse>> task = _media.StreamingLocators.ListPathsWithHttpMessagesAsync(MediaAccount.ResourceGroupName, MediaAccount.Name, locator.Name);
            AzureOperationResponse<ListPathsResponse> response = task.Result;
            ListPathsResponse paths = response.Body;
            foreach (StreamingPath path in paths.StreamingPaths)
            {
                if (path.StreamingProtocol == protocol && path.Paths.Count > 0)
                {
                    streamingPath = path.Paths[0];
                }
            }
            return streamingPath;
        }

        public string GetStreamingUrl(StreamingLocator locator)
        {
            string streamingUrl = string.Empty;
            StreamingEndpoint defaultEndpoint = GetEntity<StreamingEndpoint>(MediaEntity.StreamingEndpoint, Constant.Media.Stream.DefaultEndpoint);
            string streamingPath = GetStreamingPath(locator, StreamingPolicyStreamingProtocol.SmoothStreaming);
            if (!string.IsNullOrEmpty(streamingPath))
            {
                streamingUrl = string.Concat("//", defaultEndpoint.HostName, streamingPath);
            }
            return streamingUrl;
        }

        public StreamingLocator CreateLocator(string assetName, string streamingPolicyName)
        {
            StreamingLocator locator = new StreamingLocator(assetName, streamingPolicyName);
            Task<AzureOperationResponse<StreamingLocator>> task = _media.StreamingLocators.CreateWithHttpMessagesAsync(MediaAccount.ResourceGroupName, MediaAccount.Name, assetName, locator);
            AzureOperationResponse<StreamingLocator> response = task.Result;
            return response.Body;
        }
    }
}
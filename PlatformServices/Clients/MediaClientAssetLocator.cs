using System;
using System.Linq;
using System.Collections.Generic;

using Microsoft.Azure.Management.Media;
using Microsoft.Azure.Management.Media.Models;

namespace AzureSkyMedia.PlatformServices
{
    internal partial class MediaClient
    {
        private static int SortStreamingLocators(StreamingLocator leftItem, StreamingLocator rightItem)
        {
            return DateTime.Compare(leftItem.Created, rightItem.Created);
        }

        private StreamingLocator GetStreamingLocator(string locatorName, string assetName, string streamingPolicyName, ContentProtection contentProtection)
        {
            StreamingLocator streamingLocator = _media.StreamingLocators.Get(MediaAccount.ResourceGroupName, MediaAccount.Name, locatorName);
            if (streamingLocator == null)
            {
                string contentKeyPolicyName = null;
                if (string.IsNullOrEmpty(streamingPolicyName))
                {
                    streamingPolicyName = PredefinedStreamingPolicy.ClearStreamingOnly;
                }
                else if (streamingPolicyName == PredefinedStreamingPolicy.ClearKey)
                {
                    contentKeyPolicyName = Constant.Media.ContentKeyPolicy.Aes;
                    CreateContentKeyPolicyAes(contentKeyPolicyName);
                }
                else if (streamingPolicyName == PredefinedStreamingPolicy.MultiDrmStreaming)
                {
                    contentKeyPolicyName = Constant.Media.ContentKeyPolicy.Drm;
                    CreateContentKeyPolicyDrm(contentKeyPolicyName, contentProtection);
                }
                else if (streamingPolicyName == PredefinedStreamingPolicy.MultiDrmCencStreaming)
                {
                    contentKeyPolicyName = Constant.Media.ContentKeyPolicy.DrmCenc;
                    CreateContentKeyPolicyDrm(contentKeyPolicyName, contentProtection);
                }
                streamingLocator = new StreamingLocator(assetName, streamingPolicyName)
                {
                    DefaultContentKeyPolicyName = contentKeyPolicyName
                };
                streamingLocator = _media.StreamingLocators.Create(MediaAccount.ResourceGroupName, MediaAccount.Name, locatorName, streamingLocator);
            }
            return streamingLocator;
        }

        private string GetStreamingHost(string streamingEndpointName)
        {
            string streamingHostName = null;
            if (string.IsNullOrEmpty(streamingEndpointName))
            {
                streamingEndpointName = Constant.Media.Stream.DefaultEndpointName;
            }
            StreamingEndpoint streamingEndpoint = GetEntity<StreamingEndpoint>(MediaEntity.StreamingEndpoint, streamingEndpointName);
            if (streamingEndpoint != null)
            {
                if (streamingEndpoint.ResourceState != StreamingEndpointResourceState.Starting &&
                    streamingEndpoint.ResourceState != StreamingEndpointResourceState.Running &&
                    streamingEndpoint.ResourceState != StreamingEndpointResourceState.Scaling)
                {
                    _media.StreamingEndpoints.Start(MediaAccount.ResourceGroupName, MediaAccount.Name, streamingEndpointName);
                }
                streamingHostName = streamingEndpoint.HostName;
            }
            return streamingHostName;
        }

        private StreamingLocator[] GetStreamingLocators(string assetName)
        {
            List<StreamingLocator> streamingLocators = new List<StreamingLocator>();
            ListStreamingLocatorsResponse streamingLocatorList = _media.Assets.ListStreamingLocators(MediaAccount.ResourceGroupName, MediaAccount.Name, assetName);
            foreach (AssetStreamingLocator assetStreamingLocator in streamingLocatorList.StreamingLocators)
            {
                StreamingLocator streamingLocator = GetEntity<StreamingLocator>(MediaEntity.StreamingLocator, assetStreamingLocator.Name);
                streamingLocators.Add(streamingLocator);
            }
            return streamingLocators.ToArray();
        }

        public IEnumerable<StreamingLocator> GetStreamingLocators()
        {
            StreamingLocator[] streamingLocators = GetEntities<StreamingLocator>(MediaEntity.StreamingLocator).ToArray();
            Array.Sort<StreamingLocator>(streamingLocators, SortStreamingLocators);
            return streamingLocators;
        }

        public string[] GetStreamingUrls(string assetName)
        {
            List<string> streamingUrls = new List<string>();
            IEnumerable<StreamingLocator> streamingLocators = GetStreamingLocators(assetName);
            foreach (StreamingLocator streamingLocator in streamingLocators)
            {
                string streamingUrl = GetLocatorUrl(streamingLocator, null);
                if (!string.IsNullOrEmpty(streamingUrl))
                {
                    streamingUrls.Add(streamingUrl);
                }
            }
            return streamingUrls.ToArray();
        }

        public string GetLocatorUrl(StreamingLocator streamingLocator, string fileName)
        {
            UriBuilder uriBuilder = new UriBuilder()
            {
                Scheme = Constant.Media.Stream.DefaultScheme,
                Host = GetStreamingHost(null)
            };
            ListPathsResponse paths = _media.StreamingLocators.ListPaths(MediaAccount.ResourceGroupName, MediaAccount.Name, streamingLocator.Name);
            if (!string.IsNullOrEmpty(fileName))
            {
                foreach (string downloadPath in paths.DownloadPaths)
                {
                    if (downloadPath.Contains(fileName))
                    {
                        uriBuilder.Path = downloadPath;
                    }
                }
            }
            else
            {
                foreach (StreamingPath streamingPath in paths.StreamingPaths)
                {
                    if (streamingPath.StreamingProtocol == StreamingPolicyStreamingProtocol.SmoothStreaming && streamingPath.Paths.Count == 1)
                    {
                        uriBuilder.Path = streamingPath.Paths[0];
                    }
                }
                uriBuilder.Path = string.Concat(uriBuilder.Path, Constant.Media.Stream.DefaultFormat);
            }
            return uriBuilder.ToString();
        }

        public string GetDownloadUrl(string assetName, string fileName)
        {
            string streamingPolicyName = PredefinedStreamingPolicy.DownloadOnly;
            StreamingLocator streamingLocator = GetStreamingLocator(assetName, assetName, streamingPolicyName, null);
            return GetLocatorUrl(streamingLocator, fileName);
        }

        public StreamingLocator GetStreamingLocator(string assetName, string streamingPolicyName, ContentProtection contentProtection)
        {
            return GetStreamingLocator(assetName, assetName, streamingPolicyName, contentProtection);
        }

        public void DeleteStreamingLocators(string assetName)
        {
            ListStreamingLocatorsResponse locatorList = _media.Assets.ListStreamingLocators(MediaAccount.ResourceGroupName, MediaAccount.Name, assetName);
            foreach (AssetStreamingLocator streamingLocator in locatorList.StreamingLocators)
            {
                DeleteEntity(MediaEntity.StreamingLocator, streamingLocator.Name);
            }
        }
    }
}
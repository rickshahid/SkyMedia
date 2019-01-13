using System;
using System.Linq;
using System.Collections.Generic;

using Microsoft.Azure.Management.Media;
using Microsoft.Azure.Management.Media.Models;

namespace AzureSkyMedia.PlatformServices
{
    internal partial class MediaClient
    {
        private static int OrderByCreated(StreamingLocator leftItem, StreamingLocator rightItem)
        {
            return DateTime.Compare(leftItem.Created, rightItem.Created);
        }

        private string GetDefaultUrl(string relativePath)
        {
            StreamingEndpoint defaultEndpoint = GetEntity<StreamingEndpoint>(MediaEntity.StreamingEndpoint, Constant.Media.Stream.DefaultEndpoint);
            return string.Concat("//", defaultEndpoint.HostName, relativePath);
        }

        public string GetPlayerUrl(StreamingLocator locator)
        {
            string playerUrl = string.Empty;
            ListPathsResponse paths = _media.StreamingLocators.ListPaths(MediaAccount.ResourceGroupName, MediaAccount.Name, locator.Name);
            foreach (StreamingPath streamingPath in paths.StreamingPaths)
            {
                if (streamingPath.StreamingProtocol == StreamingPolicyStreamingProtocol.SmoothStreaming && streamingPath.Paths.Count == 1)
                {
                    playerUrl = streamingPath.Paths[0];
                }
            }
            if (string.IsNullOrEmpty(playerUrl) && paths.DownloadPaths.Count == 1)
            {
                playerUrl = paths.DownloadPaths[0];
            }
            if (!string.IsNullOrEmpty(playerUrl))
            {
                playerUrl = GetDefaultUrl(playerUrl);
            }
            return playerUrl;
        }

        public string GetDownloadUrl(Asset asset, string fileName)
        {
            string streamingPolicyName = PredefinedStreamingPolicy.DownloadOnly;
            StreamingLocator locator = CreateLocator(streamingPolicyName, asset.Name, streamingPolicyName, null);
            string relativePath = string.Concat("/", locator.StreamingLocatorId, "/", fileName);
            return GetDefaultUrl(relativePath);
        }

        public IEnumerable<StreamingLocator> GetLocators()
        {
            StreamingLocator[] locators = GetEntities<StreamingLocator>(MediaEntity.StreamingLocator).ToArray();
            Array.Sort<StreamingLocator>(locators, OrderByCreated);
            return locators;
        }

        public StreamingLocator[] GetLocators(string assetName)
        {
            List<StreamingLocator> locators = new List<StreamingLocator>();
            ListStreamingLocatorsResponse locatorList = _media.Assets.ListStreamingLocators(MediaAccount.ResourceGroupName, MediaAccount.Name, assetName);
            foreach (AssetStreamingLocator streamingLocator in locatorList.StreamingLocators)
            {
                StreamingLocator locator = GetEntity<StreamingLocator>(MediaEntity.StreamingLocator, streamingLocator.Name);
                locators.Add(locator);
            }
            return locators.ToArray();
        }

        public string[] GetStreamingUrls(string assetName)
        {
            List<string> streamingUrls = new List<string>();
            IEnumerable<StreamingLocator> streamingLocators = GetLocators(assetName);
            foreach (StreamingLocator streamingLocator in streamingLocators)
            {
                string streamingUrl = GetPlayerUrl(streamingLocator);
                if (!string.IsNullOrEmpty(streamingUrl))
                {
                    streamingUrls.Add(streamingUrl);
                }
            }
            return streamingUrls.ToArray();
        }

        public void DeleteLocators(string assetName)
        {
            ListStreamingLocatorsResponse locatorList = _media.Assets.ListStreamingLocators(MediaAccount.ResourceGroupName, MediaAccount.Name, assetName);
            foreach (AssetStreamingLocator streamingLocator in locatorList.StreamingLocators)
            {
                DeleteEntity(MediaEntity.StreamingLocator, streamingLocator.Name);
            }
        }

        public StreamingLocator CreateLocator(string locatorName, string assetName, string streamingPolicyName, ContentProtection contentProtection)
        {
            StreamingLocator locator = _media.StreamingLocators.Get(MediaAccount.ResourceGroupName, MediaAccount.Name, locatorName);
            if (locator == null)
            {
                string contentKeyPolicyName = null;
                if (streamingPolicyName == PredefinedStreamingPolicy.ClearKey)
                {
                    CreateContentKeyPolicyAES();
                    contentKeyPolicyName = Constant.Media.ContentKey.PolicyAES;
                }
                else if (streamingPolicyName == PredefinedStreamingPolicy.MultiDrmCencStreaming)
                {
                    CreateContentKeyPolicyDRM(contentProtection);
                    contentKeyPolicyName = Constant.Media.ContentKey.PolicyCENC;
                }
                else if (streamingPolicyName == PredefinedStreamingPolicy.MultiDrmStreaming)
                {
                    CreateContentKeyPolicyDRM(contentProtection);
                    contentKeyPolicyName = Constant.Media.ContentKey.PolicyDRM;
                }
                locator = new StreamingLocator(assetName, streamingPolicyName)
                {
                    DefaultContentKeyPolicyName = contentKeyPolicyName
                };
                locator = _media.StreamingLocators.Create(MediaAccount.ResourceGroupName, MediaAccount.Name, locatorName, locator);
            }
            return locator;
        }
    }
}
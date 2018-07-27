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

        public string GetDownloadUrl(StreamingLocator locator, string fileName)
        {
            string assetUrl = string.Concat("/", locator.StreamingLocatorId.ToString(), "/", fileName);
            return GetDefaultUrl(assetUrl);
        }

        public static IEnumerable<StreamingLocator> GetLocators(MediaClient mediaClient)
        {
            StreamingLocator[] locators = mediaClient.GetEntities<StreamingLocator>(MediaEntity.StreamingLocator).ToArray();
            Array.Sort<StreamingLocator>(locators, OrderByCreated);
            return locators;
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
                else if (streamingPolicyName == PredefinedStreamingPolicy.SecureStreaming)
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
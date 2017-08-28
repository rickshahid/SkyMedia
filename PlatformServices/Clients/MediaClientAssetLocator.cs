using System;
using System.Linq;

using Microsoft.WindowsAzure.MediaServices.Client;

namespace AzureSkyMedia.PlatformServices
{
    internal partial class MediaClient
    {
        private IAccessPolicy GetAccessPolicy()
        {
            string policyName = Constant.Media.AccessPolicy.ReadPolicyName;
            IAccessPolicy accessPolicy = GetEntityByName(MediaEntity.AccessPolicy, policyName, true) as IAccessPolicy;
            if (accessPolicy == null)
            {
                string settingKey = Constant.AppSettingKey.MediaLocatorReadDurationDays;
                string readDurationDays = AppSetting.GetValue(settingKey);
                TimeSpan policyDuration = new TimeSpan(int.Parse(readDurationDays), 0, 0, 0);
                accessPolicy = _media.AccessPolicies.Create(policyName, policyDuration, AccessPermissions.Read);
            }
            return accessPolicy;
        }

        private string GetPrimaryUrl(ILocator locator, string fileName, bool excludeProtocol)
        {
            string primaryUrl = locator.BaseUri;
            if (excludeProtocol)
            {
                primaryUrl = primaryUrl.Split(':')[1];
            }
            if (string.IsNullOrEmpty(fileName))
            {
                fileName = GetPrimaryFile(locator.Asset);
            }
            switch (locator.Type)
            {
                case LocatorType.Sas:
                    primaryUrl = string.Concat(primaryUrl, "/", fileName);
                    primaryUrl = string.Concat(primaryUrl, locator.ContentAccessComponent);
                    break;
                case LocatorType.OnDemandOrigin:
                    primaryUrl = string.Concat(primaryUrl, "/", locator.ContentAccessComponent);
                    primaryUrl = string.Concat(primaryUrl, "/", fileName);
                    if (fileName.EndsWith(Constant.Media.FileExtension.Manifest, StringComparison.OrdinalIgnoreCase))
                    {
                        primaryUrl = string.Concat(primaryUrl, Constant.Media.Stream.LocatorManifestSuffix);
                    }
                    break;
            }
            return primaryUrl;
        }

        private static void SetPrimaryFile(IAsset asset)
        {
            if (asset.AssetFiles.Count() == 1)
            {
                IAssetFile assetFile = asset.AssetFiles.Single();
                assetFile.IsPrimary = true;
                assetFile.Update();
            }
            else
            {
                foreach (IAssetFile assetFile in asset.AssetFiles)
                {
                    if (assetFile.Name.EndsWith(Constant.Media.FileExtension.Manifest, StringComparison.OrdinalIgnoreCase))
                    {
                        assetFile.IsPrimary = true;
                        assetFile.Update();
                    }
                }
            }
        }

        internal static string GetPrimaryFile(IAsset asset)
        {
            string primaryFile = string.Empty;
            foreach (IAssetFile assetFile in asset.AssetFiles)
            {
                if (assetFile.IsPrimary)
                {
                    primaryFile = assetFile.Name;
                }
            }
            return primaryFile;
        }

        public string GetLocatorUrl(LocatorType locatorType, IAsset asset, string fileName, bool excludeProtocol)
        {
            ILocator locator = asset.Locators.Where(l => l.Type == locatorType).FirstOrDefault();
            if (locator == null)
            {
                locator = CreateLocator(locatorType, asset);
            }
            else
            {
                if (locator.ExpirationDateTime <= DateTime.UtcNow)
                {
                    IAccessPolicy accessPolicy = GetAccessPolicy();
                    DateTime accessExpiration = DateTime.UtcNow.Add(accessPolicy.Duration);
                    locator.Update(accessExpiration);
                }
            }
            return GetPrimaryUrl(locator, fileName, excludeProtocol);
        }

        public string GetLocatorUrl(IAsset asset, string fileName)
        {
            return GetLocatorUrl(LocatorType.OnDemandOrigin, asset, fileName, true);
        }

        public string GetLocatorUrl(IAsset asset)
        {
            return GetLocatorUrl(asset, null);
        }

        public string GetWebVttUrl(IAsset asset)
        {
            string webVttUrl = string.Empty;
            foreach (IAssetFile assetFile in asset.AssetFiles)
            {
                if (assetFile.Name.EndsWith(Constant.Media.FileExtension.WebVtt, StringComparison.OrdinalIgnoreCase))
                {
                    webVttUrl = GetLocatorUrl(asset, assetFile.Name);
                }
            }
            return webVttUrl;
        }

        public ILocator CreateLocator(LocatorType locatorType, IAsset asset)
        {
            ILocator locator = asset.Locators.Where(l => l.Type == locatorType).FirstOrDefault();
            if (locator == null)
            {
                IAccessPolicy accessPolicy = GetAccessPolicy();
                locator = _media.Locators.CreateLocator(locatorType, asset, accessPolicy, null);
            }
            return locator;
        }
    }
}
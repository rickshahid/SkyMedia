using System;
using System.Linq;

using Microsoft.WindowsAzure.MediaServices.Client;

namespace AzureSkyMedia.PlatformServices
{
    public partial class MediaClient
    {
        private IAccessPolicy GetAccessPolicy(bool writePolicy)
        {
            string readPolicyName = Constant.Media.AccessPolicy.ReadPolicyName;
            string writePolicyName = Constant.Media.AccessPolicy.WritePolicyName;
            string policyName = writePolicy ? writePolicyName : readPolicyName;
            IAccessPolicy accessPolicy = GetEntityByName(MediaEntity.AccessPolicy, policyName, true) as IAccessPolicy;
            if (accessPolicy == null)
            {
                string settingKey = Constant.AppSettingKey.MediaLocatorReadDurationDays;
                string durationDays = AppSetting.GetValue(settingKey);
                TimeSpan readPolicyDuration = new TimeSpan(int.Parse(durationDays), 0, 0, 0);
                TimeSpan writePolicyDuration = new TimeSpan(Constant.Storage.Blob.WriteDurationHours, 0, 0);
                TimeSpan accessDuration = writePolicy ? writePolicyDuration : readPolicyDuration;
                AccessPermissions accessPermissions = writePolicy ? AccessPermissions.Write : AccessPermissions.Read;
                accessPolicy = _media.AccessPolicies.Create(policyName, accessDuration, accessPermissions);
            }
            return accessPolicy;
        }

        private ILocator CreateLocator(LocatorType locatorType, IAsset asset, bool writePolicy)
        {
            ILocator locator = asset.Locators.Where(l => l.Type == locatorType).FirstOrDefault();
            if (locator == null)
            {
                IAccessPolicy accessPolicy = GetAccessPolicy(writePolicy);
                locator = _media.Locators.CreateLocator(locatorType, asset, accessPolicy, null);
            }
            return locator;
        }

        private ILocator CreateLocator(LocatorType locatorType, IAsset asset)
        {
            return CreateLocator(locatorType, asset, false);
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
            if (asset.AssetFiles.Count() == 1)
            {
                primaryFile = asset.AssetFiles.Single().Name;
            }
            else
            {
                foreach (IAssetFile assetFile in asset.AssetFiles)
                {
                    if (assetFile.IsPrimary)
                    {
                        primaryFile = assetFile.Name;
                    }
                }
            }
            return primaryFile;
        }

        private string GetLocatorUrl(ILocator locator, string fileName, bool includeProtocol)
        {
            string primaryUrl = locator.BaseUri;
            if (!includeProtocol)
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

        public string GetLocatorUrl(LocatorType locatorType, IAsset asset, string fileName, bool includeProtocol)
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
                    IAccessPolicy accessPolicy = GetAccessPolicy(false);
                    DateTime accessExpiration = DateTime.UtcNow.Add(accessPolicy.Duration);
                    locator.Update(accessExpiration);
                }
            }
            return GetLocatorUrl(locator, fileName, includeProtocol);
        }

        public string GetLocatorUrl(IAsset asset, string fileName)
        {
            return GetLocatorUrl(LocatorType.OnDemandOrigin, asset, fileName, false);
        }

        public string GetLocatorUrl(IAsset asset)
        {
            return GetLocatorUrl(asset, null);
        }
    }
}

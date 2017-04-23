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

                AccessPermissions accessPermissions = writePolicy ? AccessPermissions.Write : AccessPermissions.Read;
                TimeSpan accessDuration = writePolicy ? writePolicyDuration : readPolicyDuration;
                accessPolicy = _media.AccessPolicies.Create(policyName, accessDuration, accessPermissions);
            }
            return accessPolicy;
        }

        private ILocator CreateLocator(LocatorType locatorType, IAsset asset)
        {
            IAccessPolicy accessPolicy = GetAccessPolicy(false);
            return _media.Locators.CreateLocator(locatorType, asset, accessPolicy, null);
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
                    if (assetFile.Name.EndsWith(Constant.Media.FileExtension.Manifest, StringComparison.InvariantCultureIgnoreCase))
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

        private string GetLocatorUrl(ILocator locator, string fileName)
        {
            string primaryUrl = locator.BaseUri.Replace("http:", string.Empty);
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
                    if (fileName.EndsWith(Constant.Media.FileExtension.Manifest, StringComparison.InvariantCultureIgnoreCase))
                    {
                        primaryUrl = string.Concat(primaryUrl, Constant.Media.Stream.LocatorManifestSuffix);
                    }
                    break;
            }
            return primaryUrl;
        }

        public string GetLocatorUrl(IAsset asset, string fileName)
        {
            string locatorUrl = string.Empty;
            LocatorType locatorType = LocatorType.OnDemandOrigin;
            ILocator locator = asset.Locators.Where(l => l.Type == locatorType).FirstOrDefault();
            if (locator != null)
            {
                if (locator.ExpirationDateTime <= DateTime.UtcNow)
                {
                    IAccessPolicy accessPolicy = GetAccessPolicy(false);
                    DateTime accessExpiration = DateTime.UtcNow.Add(accessPolicy.Duration);
                    locator.Update(accessExpiration);
                }
                locatorUrl = GetLocatorUrl(locator, fileName);
            }
            return locatorUrl;
        }

        public string GetLocatorUrl(IAsset asset)
        {
            return GetLocatorUrl(asset, null);
        }
    }
}

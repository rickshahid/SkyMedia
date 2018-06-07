using System.Threading.Tasks;

using Microsoft.Rest.Azure;
using Microsoft.Azure.Management.Media.Models;

namespace AzureSkyMedia.PlatformServices
{
    internal partial class MediaClient
    {
        public StreamingLocator CreateLocator(string assetName, string policyName)
        {
            StreamingLocator locator = new StreamingLocator(assetName, policyName);
            Task<AzureOperationResponse<StreamingLocator>> createTask = _media.StreamingLocators.CreateWithHttpMessagesAsync(this.MediaAccount.ResourceGroupName, this.MediaAccount.Name, assetName, locator);
            AzureOperationResponse<StreamingLocator> createResponse = createTask.Result;
            return createResponse.Body;
        }

        //private bool IsManifestFile(string fileName)
        //{
        //    return fileName.EndsWith(Constant.Media.FileExtension.StreamManifest, StringComparison.OrdinalIgnoreCase);
        //}

        //private bool IsPrimaryFile(IAssetFile assetFile)
        //{
        //    return IsManifestFile(assetFile.Name) || assetFile.MimeType.Contains("video");
        //}

        //private IAccessPolicy GetAccessPolicy(bool readWrite)
        //{
        //    string policyName = readWrite ? Constant.Media.AccessPolicy.ReadWritePolicyName : Constant.Media.AccessPolicy.ReadOnlyPolicyName;
        //    IAccessPolicy accessPolicy = GetEntityByName(MediaEntity.AccessPolicy, policyName) as IAccessPolicy;
        //    if (accessPolicy == null)
        //    {
        //        TimeSpan policyDuration;
        //        AccessPermissions policyPermissions = AccessPermissions.Read;
        //        if (readWrite)
        //        {
        //            string settingKey = Constant.AppSettingKey.MediaLocatorWriteDurationMinutes;
        //            string durationMinutes = AppSetting.GetValue(settingKey);
        //            policyDuration = new TimeSpan(0, int.Parse(durationMinutes), 0);
        //            policyPermissions = policyPermissions | AccessPermissions.Write;
        //        }
        //        else
        //        {
        //            string settingKey = Constant.AppSettingKey.MediaLocatorReadDurationDays;
        //            string durationDays = AppSetting.GetValue(settingKey);
        //            policyDuration = new TimeSpan(int.Parse(durationDays), 0, 0, 0);
        //        }
        //        accessPolicy = _media2.AccessPolicies.Create(policyName, policyDuration, policyPermissions);
        //    }
        //    return accessPolicy;
        //}

        //private string GetPrimaryUrl(ILocator locator, string fileName, bool excludeProtocol)
        //{
        //    string primaryUrl = locator.BaseUri;
        //    if (excludeProtocol)
        //    {
        //        primaryUrl = primaryUrl.Split(':')[1];
        //    }
        //    if (string.IsNullOrEmpty(fileName))
        //    {
        //        fileName = GetPrimaryFile(locator.Asset);
        //    }
        //    switch (locator.Type)
        //    {
        //        case LocatorType.Sas:
        //            primaryUrl = string.Concat(primaryUrl, "/", fileName);
        //            primaryUrl = string.Concat(primaryUrl, locator.ContentAccessComponent);
        //            break;
        //        case LocatorType.OnDemandOrigin:
        //            primaryUrl = string.Concat(primaryUrl, "/", locator.ContentAccessComponent);
        //            primaryUrl = string.Concat(primaryUrl, "/", fileName);
        //            if (IsManifestFile(fileName))
        //            {
        //                primaryUrl = string.Concat(primaryUrl, Constant.Media.Stream.LocatorManifestSuffix);
        //            }
        //            break;
        //    }
        //    return primaryUrl;
        //}

        //private void SetPrimaryFile(IAsset asset)
        //{
        //    if (asset.AssetFiles.Count() == 1)
        //    {
        //        IAssetFile assetFile = asset.AssetFiles.Single();
        //        assetFile.IsPrimary = true;
        //        assetFile.Update();
        //    }
        //    else
        //    {
        //        foreach (IAssetFile assetFile in asset.AssetFiles)
        //        {
        //            if (IsPrimaryFile(assetFile))
        //            {
        //                assetFile.IsPrimary = true;
        //                assetFile.Update();
        //            }
        //        }
        //    }
        //}

        //public static string GetPrimaryFile(IAsset asset)
        //{
        //    string primaryFile = string.Empty;
        //    foreach (IAssetFile assetFile in asset.AssetFiles)
        //    {
        //        if (assetFile.IsPrimary)
        //        {
        //            primaryFile = assetFile.Name;
        //        }
        //    }
        //    return primaryFile;
        //}

        //public string GetLocatorUrl(LocatorType locatorType, IAsset asset, string fileName, bool excludeProtocol)
        //{
        //    ILocator locator = asset.Locators.Where(l => l.Type == locatorType).FirstOrDefault();
        //    if (locator == null)
        //    {
        //        locator = CreateLocator(locatorType, asset);
        //    }
        //    else if (locator.ExpirationDateTime <= DateTime.UtcNow)
        //    {
        //        string settingKey = Constant.AppSettingKey.MediaLocatorAutoRenewal;
        //        string autoRenewal = AppSetting.GetValue(settingKey);
        //        if (string.Equals(autoRenewal, "true", StringComparison.OrdinalIgnoreCase))
        //        {
        //            IAccessPolicy accessPolicy = GetAccessPolicy(false);
        //            DateTime policyExpiration = DateTime.UtcNow.Add(accessPolicy.Duration);
        //            locator.Update(policyExpiration);
        //        }
        //    }
        //    return GetPrimaryUrl(locator, fileName, excludeProtocol);
        //}

        //public string GetLocatorUrl(IAsset asset, string fileName)
        //{
        //    return GetLocatorUrl(LocatorType.OnDemandOrigin, asset, fileName, true);
        //}

        //public string GetLocatorUrl(IAsset asset)
        //{
        //    return GetLocatorUrl(asset, null);
        //}

        //public string[] GetWebVttUrls(IAsset asset)
        //{
        //    List<string> webVttUrls = new List<string>();
        //    foreach (IAssetFile assetFile in asset.AssetFiles)
        //    {
        //        if (assetFile.Name.EndsWith(Constant.Media.FileExtension.WebVtt, StringComparison.OrdinalIgnoreCase))
        //        {
        //            string webVttUrl = GetLocatorUrl(asset, assetFile.Name);
        //            webVttUrls.Add(webVttUrl);
        //        }
        //    }
        //    return webVttUrls.ToArray();
        //}

        //public ILocator CreateLocator(string locatorId, LocatorType locatorType, IAsset asset, bool readWrite)
        //{
        //    ILocator locator = asset.Locators.Where(l => l.Type == locatorType).FirstOrDefault();
        //    if (locator == null)
        //    {
        //        IAccessPolicy accessPolicy = GetAccessPolicy(readWrite);
        //        locator = _media2.Locators.CreateLocator(locatorId, locatorType, asset, accessPolicy, null);
        //    }
        //    return locator;
        //}

        //public ILocator CreateLocator(LocatorType locatorType, IAsset asset, bool readWrite)
        //{
        //    return CreateLocator(null, locatorType, asset, readWrite);
        //}

        //public ILocator CreateLocator(LocatorType locatorType, IAsset asset)
        //{
        //    return CreateLocator(null, locatorType, asset, false);
        //}
    }
}
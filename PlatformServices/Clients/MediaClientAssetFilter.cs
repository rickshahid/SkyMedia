using System.Collections.Generic;

using Microsoft.WindowsAzure.MediaServices.Client;

namespace AzureSkyMedia.PlatformServices
{
    public partial class MediaClient
    {
        private IAsset GetSourceAsset(string sourceUrl)
        {
            string locatorId = sourceUrl.Split('/')[3];
            locatorId = string.Concat(Constants.Media.Stream.LocatorIdPrefix, locatorId);
            ILocator locator = GetEntityById(MediaEntity.Locator, locatorId) as ILocator;
            return locator.Asset;
        }

        public object CreateFilter(string sourceUrl, string filterName, int markIn, int markOut)
        {
            ulong? timescale = 1;
            ulong? markInSeconds = (ulong?) markIn;
            ulong? markOutSeconds = markOut > 0 ? (ulong?)markOut : null;
            PresentationTimeRange timeRange = new PresentationTimeRange(timescale, markInSeconds, markOutSeconds);
            List<FilterTrackSelectStatement> trackConditions = new List<FilterTrackSelectStatement>();
            IAsset sourceAsset = GetSourceAsset(sourceUrl);
            return sourceAsset.AssetFilters.Create(filterName, timeRange, trackConditions);
        }

        public MediaAssetInput[] GetInputAssets(string sourceUrl)
        {
            IAsset sourceAsset = GetSourceAsset(sourceUrl);
            MediaAssetInput inputAsset = new MediaAssetInput();
            inputAsset.AssetId = sourceAsset.Id;
            return new MediaAssetInput[] { inputAsset };
        }
    }
}

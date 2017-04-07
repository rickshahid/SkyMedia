using System.Collections.Generic;

using Microsoft.WindowsAzure.MediaServices.Client;

namespace AzureSkyMedia.PlatformServices
{
    public partial class MediaClient
    {
        private IAsset GetSourceAsset(string sourceUrl)
        {
            string locatorId = sourceUrl.Split('/')[3];
            locatorId = string.Concat(Constant.Media.Stream.LocatorIdPrefix, locatorId);
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
    }
}

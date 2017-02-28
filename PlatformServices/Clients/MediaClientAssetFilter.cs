using System.Collections.Generic;

using Microsoft.WindowsAzure.MediaServices.Client;

namespace AzureSkyMedia.PlatformServices
{
    public partial class MediaClient
    {
        public object CreateClip(string sourceUrl, string filterName, int markIn, int markOut)
        {
            string locatorId = sourceUrl.Split('/')[3];
            locatorId = string.Concat(Constants.Media.Stream.LocatorIdPrefix, locatorId);
            ILocator locator = GetEntityById(MediaEntity.Locator, locatorId) as ILocator;

            ulong? timescale = 1;
            ulong? markInSeconds = (ulong?) markIn;
            ulong? markOutSeconds = markOut > 0 ? (ulong?)markOut : null;
            PresentationTimeRange timeRange = new PresentationTimeRange(timescale, markInSeconds, markOutSeconds);

            List<FilterTrackSelectStatement> trackConditions = new List<FilterTrackSelectStatement>();
            return locator.Asset.AssetFilters.Create(filterName, timeRange, trackConditions);
        }
    }
}

using System.Collections.Generic;

using Microsoft.WindowsAzure.MediaServices.Client;

namespace AzureSkyMedia.PlatformServices
{
    public partial class MediaClient
    {
        public IStreamingAssetFilter CreateFilter(IAsset asset, string filterName, int markIn, int markOut)
        {
            ulong? timescale = 1;
            ulong? markInSeconds = (ulong?)markIn;
            ulong? markOutSeconds = markOut > 0 ? (ulong?)markOut : null;
            PresentationTimeRange timeRange = new PresentationTimeRange(timescale, markInSeconds, markOutSeconds);
            List<FilterTrackSelectStatement> trackConditions = new List<FilterTrackSelectStatement>();
            return asset.AssetFilters.Create(filterName, timeRange, trackConditions);
        }
    }
}

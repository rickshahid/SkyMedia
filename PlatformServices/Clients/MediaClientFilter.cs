using Microsoft.Azure.Management.Media;
using Microsoft.Azure.Management.Media.Models;

namespace AzureSkyMedia.PlatformServices
{
    internal partial class MediaClient
    {
        private FilterTrackSelection[] GetTrackSelections(FilterTrackPropertyType trackType, string trackValue)
        {
            FilterTrackPropertyCondition trackCondition = new FilterTrackPropertyCondition()
            {
                Property = trackType,
                Operation = FilterTrackPropertyCompareOperation.Equal,
                Value = trackValue
            };
            FilterTrackSelection trackSelection = new FilterTrackSelection()
            {
                TrackSelections = new FilterTrackPropertyCondition[] { trackCondition }
            };
            return new FilterTrackSelection[] { trackSelection };
        }

        public AccountFilter CreateFilter(string filterName, long timescale, long startTimestamp, long endTimestamp)
        {
            AccountFilter accountFilter = new AccountFilter()
            {
                PresentationTimeRange = new PresentationTimeRange()
                {
                    Timescale = timescale,
                    StartTimestamp = startTimestamp,
                    EndTimestamp = endTimestamp,
                    PresentationWindowDuration = endTimestamp - startTimestamp
                }
            };
            return _media.AccountFilters.CreateOrUpdate(MediaAccount.ResourceGroupName, MediaAccount.Name, filterName, accountFilter);
        }

        public AccountFilter CreateFilter(string filterName, int bitrate)
        {
            AccountFilter accountFilter = new AccountFilter()
            {
                FirstQuality = new FirstQuality(bitrate)
            };
            return _media.AccountFilters.CreateOrUpdate(MediaAccount.ResourceGroupName, MediaAccount.Name, filterName, accountFilter);
        }

        public AccountFilter CreateFilter(string filterName, FilterTrackPropertyType trackType, string trackValue)
        {
            AccountFilter accountFilter = new AccountFilter()
            {
                Tracks = GetTrackSelections(trackType, trackValue)
            };
            return _media.AccountFilters.CreateOrUpdate(MediaAccount.ResourceGroupName, MediaAccount.Name, filterName, accountFilter);
        }

        public AssetFilter CreateFilter(string assetId, string filterName, long timescale, long startTimestamp, long endTimestamp)
        {
            string assetName = GetAssetName(assetId);
            AssetFilter assetFilter = new AssetFilter()
            {
                PresentationTimeRange = new PresentationTimeRange()
                {
                    Timescale = timescale,
                    StartTimestamp = startTimestamp,
                    EndTimestamp = endTimestamp,
                    PresentationWindowDuration = endTimestamp - startTimestamp
                }
            };
            return _media.AssetFilters.CreateOrUpdate(MediaAccount.ResourceGroupName, MediaAccount.Name, assetName, filterName, assetFilter);
        }

        public AssetFilter CreateFilter(string assetId, string filterName, int bitrate)
        {
            string assetName = GetAssetName(assetId);
            AssetFilter assetFilter = new AssetFilter()
            {
                FirstQuality = new FirstQuality(bitrate)
            };
            return _media.AssetFilters.CreateOrUpdate(MediaAccount.ResourceGroupName, MediaAccount.Name, assetName, filterName, assetFilter);
        }

        public AssetFilter CreateFilter(string assetId, string filterName, FilterTrackPropertyType trackType, string trackValue)
        {
            string assetName = GetAssetName(assetId);
            AssetFilter assetFilter = new AssetFilter()
            {
                Tracks = GetTrackSelections(trackType, trackValue)
            };
            return _media.AssetFilters.CreateOrUpdate(MediaAccount.ResourceGroupName, MediaAccount.Name, assetName, filterName, assetFilter);
        }
    }
}
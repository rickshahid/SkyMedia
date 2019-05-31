using System.Collections.Generic;

using Microsoft.Azure.Management.Media;
using Microsoft.Azure.Management.Media.Models;

namespace AzureSkyMedia.PlatformServices
{
    internal partial class MediaClient
    {
        private FilterTrackSelection[] GetTrackSelections(Dictionary<FilterTrackPropertyType, string> trackFilters)
        {
            List<FilterTrackPropertyCondition> trackConditions = new List<FilterTrackPropertyCondition>();
            foreach (KeyValuePair<FilterTrackPropertyType, string> trackFilter in trackFilters)
            {
                FilterTrackPropertyCondition trackCondition = new FilterTrackPropertyCondition()
                {
                    Property = trackFilter.Key,
                    Operation = FilterTrackPropertyCompareOperation.Equal,
                    Value = trackFilter.Value
                };
                trackConditions.Add(trackCondition);
            }
            FilterTrackSelection trackSelection = new FilterTrackSelection()
            {
                TrackSelections = trackConditions.ToArray()
            };
            return new FilterTrackSelection[] { trackSelection };
        }

        public AccountFilter CreateFilter(string filterName, int firstBitrate)
        {
            AccountFilter accountFilter = new AccountFilter()
            {
                FirstQuality = new FirstQuality(firstBitrate)
            };
            return _media.AccountFilters.CreateOrUpdate(MediaAccount.ResourceGroupName, MediaAccount.Name, filterName, accountFilter);
        }

        public AccountFilter CreateFilter(string filterName, long startSeconds, long endSeconds)
        {
            AccountFilter accountFilter = new AccountFilter()
            {
                PresentationTimeRange = new PresentationTimeRange()
                {
                    Timescale = Constant.Media.Filter.Timescale,
                    StartTimestamp = startSeconds,
                    EndTimestamp = endSeconds
                }
            };
            return _media.AccountFilters.CreateOrUpdate(MediaAccount.ResourceGroupName, MediaAccount.Name, filterName, accountFilter);
        }

        public AccountFilter CreateFilter(string filterName, Dictionary<FilterTrackPropertyType, string> trackFilters)
        {
            AccountFilter accountFilter = new AccountFilter()
            {
                Tracks = GetTrackSelections(trackFilters)
            };
            return _media.AccountFilters.CreateOrUpdate(MediaAccount.ResourceGroupName, MediaAccount.Name, filterName, accountFilter);
        }

        public AccountFilter CreateFilter(string filterName, long? dvrWindowSeconds, long? liveBackoffSeconds, bool? forceEndTimestamp)
        {
            AccountFilter accountFilter = new AccountFilter()
            {
                PresentationTimeRange = new PresentationTimeRange()
                {
                    Timescale = Constant.Media.Filter.Timescale,
                    PresentationWindowDuration = dvrWindowSeconds,
                    LiveBackoffDuration = liveBackoffSeconds,
                    ForceEndTimestamp = forceEndTimestamp
                }
            };
            return _media.AccountFilters.CreateOrUpdate(MediaAccount.ResourceGroupName, MediaAccount.Name, filterName, accountFilter);
        }

        public AssetFilter CreateFilter(string assetName, string filterName, int firstBitrate)
        {
            AssetFilter assetFilter = new AssetFilter()
            {
                FirstQuality = new FirstQuality(firstBitrate)
            };
            return _media.AssetFilters.CreateOrUpdate(MediaAccount.ResourceGroupName, MediaAccount.Name, assetName, filterName, assetFilter);
        }

        public AssetFilter CreateFilter(string assetName, string filterName, long startSeconds, long endSeconds)
        {
            AssetFilter assetFilter = new AssetFilter()
            {
                PresentationTimeRange = new PresentationTimeRange()
                {
                    Timescale = Constant.Media.Filter.Timescale,
                    StartTimestamp = startSeconds,
                    EndTimestamp = endSeconds
                }
            };
            return _media.AssetFilters.CreateOrUpdate(MediaAccount.ResourceGroupName, MediaAccount.Name, assetName, filterName, assetFilter);
        }

        public AssetFilter CreateFilter(string assetName, string filterName, Dictionary<FilterTrackPropertyType, string> trackFilters)
        {
            AssetFilter assetFilter = new AssetFilter()
            {
                Tracks = GetTrackSelections(trackFilters)
            };
            return _media.AssetFilters.CreateOrUpdate(MediaAccount.ResourceGroupName, MediaAccount.Name, assetName, filterName, assetFilter);
        }

        public AssetFilter CreateFilter(string assetName, string filterName, long? dvrWindowSeconds, long? liveBackoffSeconds, bool? forceEndTimestamp)
        {
            AssetFilter assetFilter = new AssetFilter()
            {
                PresentationTimeRange = new PresentationTimeRange()
                {
                    Timescale = Constant.Media.Filter.Timescale,
                    PresentationWindowDuration = dvrWindowSeconds,
                    LiveBackoffDuration = liveBackoffSeconds,
                    ForceEndTimestamp = forceEndTimestamp
                }
            };
            return _media.AssetFilters.CreateOrUpdate(MediaAccount.ResourceGroupName, MediaAccount.Name, assetName, filterName, assetFilter);
        }
    }
}
using Microsoft.Azure.Management.Media.Models;

namespace AzureSkyMedia.PlatformServices
{
    public class MediaLiveEvent : LiveEvent
    {
        internal MediaLiveEvent(MediaClient mediaClient, LiveEvent liveEvent) : base(liveEvent.Input, liveEvent.Id, liveEvent.Name, liveEvent.Type, liveEvent.Tags, liveEvent.Location, liveEvent.Description, liveEvent.Preview, liveEvent.Encoding, liveEvent.ProvisioningState, liveEvent.ResourceState, liveEvent.CrossSiteAccessPolicies, liveEvent.VanityUrl, liveEvent.StreamOptions, liveEvent.Created, liveEvent.LastModified)
        {
            Outputs = mediaClient.GetAllEntities<LiveOutput>(MediaEntity.LiveEventOutput, liveEvent.Name); 
        }

        public LiveOutput[] Outputs { get; }
    }
}
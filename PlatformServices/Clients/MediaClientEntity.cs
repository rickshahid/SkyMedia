using Microsoft.Rest.Azure;
using Microsoft.Azure.Management.Media;

namespace AzureSkyMedia.PlatformServices
{
    internal partial class MediaClient
    {
        public IPage<T> GetEntities<T>(MediaEntity entityType, string entityName = null)
        {
            IPage<T> entities = null;
            switch (entityType)
            {
                case MediaEntity.Asset:
                    entities = (IPage<T>)_media.Assets.List(MediaAccount.ResourceGroupName, MediaAccount.Name);
                    break;
                case MediaEntity.Transform:
                    entities = (IPage<T>)_media.Transforms.List(MediaAccount.ResourceGroupName, MediaAccount.Name);
                    break;
                case MediaEntity.TransformJob:
                    entities = (IPage<T>)_media.Jobs.List(MediaAccount.ResourceGroupName, MediaAccount.Name, entityName);
                    break;
                case MediaEntity.ContentKeyPolicy:
                    entities = (IPage<T>)_media.ContentKeyPolicies.List(MediaAccount.ResourceGroupName, MediaAccount.Name);
                    break;
                case MediaEntity.StreamingPolicy:
                    entities = (IPage<T>)_media.StreamingPolicies.List(MediaAccount.ResourceGroupName, MediaAccount.Name);
                    break;
                case MediaEntity.StreamingEndpoint:
                    entities = (IPage<T>)_media.StreamingEndpoints.List(MediaAccount.ResourceGroupName, MediaAccount.Name);
                    break;
                case MediaEntity.StreamingLocator:
                    entities = (IPage<T>)_media.StreamingLocators.List(MediaAccount.ResourceGroupName, MediaAccount.Name);
                    break;
                case MediaEntity.LiveEvent:
                    entities = (IPage<T>)_media.LiveEvents.List(MediaAccount.ResourceGroupName, MediaAccount.Name);
                    break;
                case MediaEntity.LiveOutput:
                    entities = (IPage<T>)_media.LiveOutputs.List(MediaAccount.ResourceGroupName, MediaAccount.Name, entityName);
                    break;
            }
            return entities;
        }

        public IPage<T> NextEntities<T>(MediaEntity entityType, IPage<T> currentPage)
        {
            IPage<T> entities = null;
            if (!string.IsNullOrEmpty(currentPage.NextPageLink))
            {
                switch (entityType)
                {
                    case MediaEntity.Asset:
                        entities = (IPage<T>)_media.Assets.ListNext(currentPage.NextPageLink);
                        break;
                    case MediaEntity.Transform:
                        entities = (IPage<T>)_media.Transforms.ListNext(currentPage.NextPageLink);
                        break;
                    case MediaEntity.TransformJob:
                        entities = (IPage<T>)_media.Jobs.ListNext(currentPage.NextPageLink);
                        break;
                    case MediaEntity.ContentKeyPolicy:
                        entities = (IPage<T>)_media.ContentKeyPolicies.ListNext(currentPage.NextPageLink);
                        break;
                    case MediaEntity.StreamingPolicy:
                        entities = (IPage<T>)_media.StreamingPolicies.ListNext(currentPage.NextPageLink);
                        break;
                    case MediaEntity.StreamingEndpoint:
                        entities = (IPage<T>)_media.StreamingEndpoints.ListNext(currentPage.NextPageLink);
                        break;
                    case MediaEntity.StreamingLocator:
                        entities = (IPage<T>)_media.StreamingLocators.ListNext(currentPage.NextPageLink);
                        break;
                    case MediaEntity.LiveEvent:
                        entities = (IPage<T>)_media.LiveEvents.ListNext(currentPage.NextPageLink);
                        break;
                    case MediaEntity.LiveOutput:
                        entities = (IPage<T>)_media.LiveOutputs.ListNext(currentPage.NextPageLink);
                        break;
                }
            }
            return entities;
        }

        public T GetEntity<T>(MediaEntity entityType, string entityName, string parentEntityName = null) where T : class
        {
            T entity = default(T);
            switch (entityType)
            {
                case MediaEntity.Asset:
                    entity = _media.Assets.Get(MediaAccount.ResourceGroupName, MediaAccount.Name, entityName) as T;
                    break;
                case MediaEntity.Transform:
                    entity = _media.Transforms.Get(MediaAccount.ResourceGroupName, MediaAccount.Name, entityName) as T;
                    break;
                case MediaEntity.TransformJob:
                    entity = _media.Jobs.Get(MediaAccount.ResourceGroupName, MediaAccount.Name, parentEntityName, entityName) as T;
                    break;
                case MediaEntity.ContentKeyPolicy:
                    entity = _media.ContentKeyPolicies.Get(MediaAccount.ResourceGroupName, MediaAccount.Name, entityName) as T;
                    break;
                case MediaEntity.StreamingPolicy:
                    entity = _media.StreamingPolicies.Get(MediaAccount.ResourceGroupName, MediaAccount.Name, entityName) as T;
                    break;
                case MediaEntity.StreamingEndpoint:
                    entity = _media.StreamingEndpoints.Get(MediaAccount.ResourceGroupName, MediaAccount.Name, entityName) as T;
                    break;
                case MediaEntity.StreamingLocator:
                    entity = _media.StreamingLocators.Get(MediaAccount.ResourceGroupName, MediaAccount.Name, entityName) as T;
                    break;
                case MediaEntity.LiveEvent:
                    entity = _media.LiveEvents.Get(MediaAccount.ResourceGroupName, MediaAccount.Name, entityName) as T;
                    break;
                case MediaEntity.LiveOutput:
                    entity = _media.LiveOutputs.Get(MediaAccount.ResourceGroupName, MediaAccount.Name, parentEntityName, entityName) as T;
                    break;
            }
            return entity;
        }

        public int GetEntityCount<T>(MediaEntity entityType, IPage<T> entities)
        {
            int entityCount = 0;
            do
            {
                foreach (T entity in entities)
                {
                    entityCount = entityCount + 1;
                }
                entities = NextEntities(entityType, entities);
            } while (entities != null);
            return entityCount;
        }
    }
}
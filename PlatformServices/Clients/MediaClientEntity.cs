using System.Collections.Generic;

using Microsoft.Rest.Azure;
using Microsoft.Rest.Azure.OData;
using Microsoft.Azure.Management.Media;
using Microsoft.Azure.Management.Media.Models;

namespace AzureSkyMedia.PlatformServices
{
    internal partial class MediaClient
    {
        public int GetEntityCount<T>(MediaEntity entityType) where T : Resource
        {
            T[] entities = GetAllEntities<T>(entityType);
            return entities.Length;
        }

        public int GetEntityCount<T1, T2>(MediaEntity entityType, MediaEntity parentEntityType) where T1 : Resource where T2 : Resource
        {
            T1[] entities = GetAllEntities<T1, T2>(entityType, parentEntityType);
            return entities.Length;
        }

        public T1[] GetAllEntities<T1, T2>(MediaEntity entityType, MediaEntity parentEntityType) where T1 : Resource where T2 : Resource
        {
            List<T1> allEntities = new List<T1>();
            IPage<T2> parentEntities = GetEntities<T2>(parentEntityType);
            foreach (T2 parentEntity in parentEntities)
            {
                T1[] entities = GetAllEntities<T1>(entityType, null, parentEntity.Name);
                allEntities.AddRange(entities);
            }
            return allEntities.ToArray();
        }

        public T[] GetAllEntities<T>(MediaEntity entityType, string queryFilter = null, string parentEntityName = null) where T : Resource
        {
            List<T> allEntities = new List<T>();
            IPage<T> entities = GetEntities<T>(entityType, queryFilter, parentEntityName);
            while (entities != null)
            {
                allEntities.AddRange(entities);
                entities = NextEntities(entityType, entities);
            }
            return allEntities.ToArray();
        }

        public IPage<T> GetEntities<T>(MediaEntity entityType, string queryFilter = null, string parentEntityName = null) where T : Resource
        {
            IPage<T> entities = null;
            switch (entityType)
            {
                case MediaEntity.Asset:
                    ODataQuery<Asset> assetQuery = new ODataQuery<Asset>()
                    {
                        Filter = queryFilter
                    };
                    entities = (IPage<T>)_media.Assets.List(MediaAccount.ResourceGroupName, MediaAccount.Name, assetQuery);
                    break;
                case MediaEntity.Transform:
                    ODataQuery<Transform> transformQuery = new ODataQuery<Transform>()
                    {
                        Filter = queryFilter
                    };
                    entities = (IPage<T>)_media.Transforms.List(MediaAccount.ResourceGroupName, MediaAccount.Name, transformQuery);
                    break;
                case MediaEntity.TransformJob:
                    ODataQuery<Job> transformJobQuery = new ODataQuery<Job>()
                    {
                        Filter = queryFilter
                    };
                    entities = (IPage<T>)_media.Jobs.List(MediaAccount.ResourceGroupName, MediaAccount.Name, parentEntityName, transformJobQuery);
                    break;
                case MediaEntity.ContentKeyPolicy:
                    ODataQuery<ContentKeyPolicy> contentKeyPolicyQuery = new ODataQuery<ContentKeyPolicy>()
                    {
                        Filter = queryFilter
                    };
                    entities = (IPage<T>)_media.ContentKeyPolicies.List(MediaAccount.ResourceGroupName, MediaAccount.Name, contentKeyPolicyQuery);
                    break;
                case MediaEntity.StreamingPolicy:
                    ODataQuery<StreamingPolicy> streamingPolicyQuery = new ODataQuery<StreamingPolicy>()
                    {
                        Filter = queryFilter
                    };
                    entities = (IPage<T>)_media.StreamingPolicies.List(MediaAccount.ResourceGroupName, MediaAccount.Name, streamingPolicyQuery);
                    break;
                case MediaEntity.StreamingEndpoint:
                    entities = (IPage<T>)_media.StreamingEndpoints.List(MediaAccount.ResourceGroupName, MediaAccount.Name);
                    break;
                case MediaEntity.StreamingLocator:
                    ODataQuery<StreamingLocator> streamingLocatorQuery = new ODataQuery<StreamingLocator>()
                    {
                        Filter = queryFilter
                    };
                    entities = (IPage<T>)_media.StreamingLocators.List(MediaAccount.ResourceGroupName, MediaAccount.Name, streamingLocatorQuery);
                    break;
                case MediaEntity.FilterAccount:
                    entities = (IPage<T>)_media.AccountFilters.List(MediaAccount.ResourceGroupName, MediaAccount.Name);
                    break;
                case MediaEntity.FilterAsset:
                    entities = (IPage<T>)_media.AssetFilters.List(MediaAccount.ResourceGroupName, MediaAccount.Name, parentEntityName);
                    break;
                case MediaEntity.LiveEvent:
                    entities = (IPage<T>)_media.LiveEvents.List(MediaAccount.ResourceGroupName, MediaAccount.Name);
                    break;
                case MediaEntity.LiveEventOutput:
                    entities = (IPage<T>)_media.LiveOutputs.List(MediaAccount.ResourceGroupName, MediaAccount.Name, parentEntityName);
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
                    case MediaEntity.FilterAccount:
                        entities = (IPage<T>)_media.AccountFilters.ListNext(currentPage.NextPageLink);
                        break;
                    case MediaEntity.FilterAsset:
                        entities = (IPage<T>)_media.AssetFilters.ListNext(currentPage.NextPageLink);
                        break;
                    case MediaEntity.LiveEvent:
                        entities = (IPage<T>)_media.LiveEvents.ListNext(currentPage.NextPageLink);
                        break;
                    case MediaEntity.LiveEventOutput:
                        entities = (IPage<T>)_media.LiveOutputs.ListNext(currentPage.NextPageLink);
                        break;
                }
            }
            return entities;
        }

        public T GetEntity<T>(MediaEntity entityType, string entityName, string parentEntityName = null) where T : Resource
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
                case MediaEntity.FilterAccount:
                    entity = _media.AccountFilters.Get(MediaAccount.ResourceGroupName, MediaAccount.Name, entityName) as T;
                    break;
                case MediaEntity.FilterAsset:
                    entity = _media.AssetFilters.Get(MediaAccount.ResourceGroupName, MediaAccount.Name, parentEntityName, entityName) as T;
                    break;
                case MediaEntity.LiveEvent:
                    entity = _media.LiveEvents.Get(MediaAccount.ResourceGroupName, MediaAccount.Name, entityName) as T;
                    break;
                case MediaEntity.LiveEventOutput:
                    entity = _media.LiveOutputs.Get(MediaAccount.ResourceGroupName, MediaAccount.Name, parentEntityName, entityName) as T;
                    break;
            }
            return entity;
        }

        public void DeleteEntity(MediaEntity entityType, string entityName, string parentEntityName = null)
        {
            switch (entityType)
            {
                case MediaEntity.Asset:
                    _media.Assets.Delete(MediaAccount.ResourceGroupName, MediaAccount.Name, entityName);
                    break;
                case MediaEntity.Transform:
                    _media.Transforms.Delete(MediaAccount.ResourceGroupName, MediaAccount.Name, entityName);
                    break;
                case MediaEntity.TransformJob:
                    _media.Jobs.Delete(MediaAccount.ResourceGroupName, MediaAccount.Name, parentEntityName, entityName);
                    using (DatabaseClient databaseClient = new DatabaseClient())
                    {
                        string collectionId = Constant.Database.Collection.MediaJobAccount;
                        databaseClient.DeleteDocument(collectionId, entityName);
                    }
                    break;
                case MediaEntity.ContentKeyPolicy:
                    _media.ContentKeyPolicies.Delete(MediaAccount.ResourceGroupName, MediaAccount.Name, entityName);
                    break;
                case MediaEntity.StreamingPolicy:
                    _media.StreamingPolicies.Delete(MediaAccount.ResourceGroupName, MediaAccount.Name, entityName);
                    break;
                case MediaEntity.StreamingEndpoint:
                    _media.StreamingEndpoints.Delete(MediaAccount.ResourceGroupName, MediaAccount.Name, entityName);
                    break;
                case MediaEntity.StreamingLocator:
                    _media.StreamingLocators.Delete(MediaAccount.ResourceGroupName, MediaAccount.Name, entityName);
                    break;
                case MediaEntity.FilterAccount:
                    _media.AccountFilters.Delete(MediaAccount.ResourceGroupName, MediaAccount.Name, entityName);
                    break;
                case MediaEntity.FilterAsset:
                    _media.AssetFilters.Delete(MediaAccount.ResourceGroupName, MediaAccount.Name, parentEntityName, entityName);
                    break;
                case MediaEntity.LiveEvent:
                    LiveEvent liveEvent = GetEntity<LiveEvent>(MediaEntity.LiveEvent, entityName);
                    if (liveEvent != null)
                    {
                        LiveOutput[] liveOutputs = GetAllEntities<LiveOutput>(MediaEntity.LiveEventOutput, null, entityName);
                        foreach (LiveOutput liveOutput in liveOutputs)
                        {
                            _media.LiveOutputs.Delete(MediaAccount.ResourceGroupName, MediaAccount.Name, entityName, liveOutput.Name);
                        }
                        _media.LiveEvents.Delete(MediaAccount.ResourceGroupName, MediaAccount.Name, entityName);
                    }
                    break;
                case MediaEntity.LiveEventOutput:
                    _media.LiveOutputs.Delete(MediaAccount.ResourceGroupName, MediaAccount.Name, parentEntityName, entityName);
                    break;
            }
        }
    }
}
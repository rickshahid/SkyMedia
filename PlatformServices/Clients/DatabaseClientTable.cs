using System;
using System.Collections.Generic;

using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace AzureSkyMedia.PlatformServices
{
    internal class TableClient
    {
        private CloudTableClient _storage;

        public TableClient()
        {
            CloudStorageAccount storageAccount = Storage.GetSystemAccount();
            _storage = storageAccount.CreateCloudTableClient();
        }

        public TableClient(string authToken, string accountName)
        {
            CloudStorageAccount storageAccount = Storage.GetUserAccount(authToken, accountName);
            _storage = storageAccount.CreateCloudTableClient();
        }

        private DynamicTableEntity GetDynamicEntity(StorageEntity entity)
        {
            if (!entity.CreatedOn.HasValue)
            {
                entity.CreatedOn = DateTime.UtcNow;
            }

            OperationContext context = new OperationContext();
            IDictionary<string, EntityProperty> properties = TableEntity.Flatten(entity, context);
            properties.Remove("PartitionKey");
            properties.Remove("RowKey");

            DynamicTableEntity dynamicEntity = new DynamicTableEntity(entity.PartitionKey, entity.RowKey)
            {
                ETag = "*",
                Properties = properties
            };
            return dynamicEntity;
        }

        private T GetEntity<T>(DynamicTableEntity dynamicEntity, OperationContext context) where T : StorageEntity
        {
            T entity = TableEntity.ConvertBack<T>(dynamicEntity.Properties, context);
            entity.PartitionKey = dynamicEntity.PartitionKey;
            entity.RowKey = dynamicEntity.RowKey;
            return entity;
        }

        public T[] GetEntities<T>(string tableName, string partitionKey) where T : StorageEntity
        {
            List<T> entities = new List<T>();
            CloudTable table = _storage.GetTableReference(tableName);
            if (table.Exists())
            {
                TableQuery query = new TableQuery();
                if (!string.IsNullOrEmpty(partitionKey))
                {
                    string filter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionKey);
                    query = query.Where(filter);
                }
                OperationContext context = new OperationContext();
                IEnumerable<DynamicTableEntity> dynamicEntities = table.ExecuteQuery(query);
                foreach (DynamicTableEntity dynamicEntity in dynamicEntities)
                {
                    T entity = GetEntity<T>(dynamicEntity, context);
                    entities.Add(entity);
                }
            }
            return entities.ToArray();
        }

        public T[] GetEntities<T>(string tableName) where T : StorageEntity
        {
            return GetEntities<T>(tableName, null);
        }

        public T GetEntity<T>(string tableName, string partitionKey, string rowKey) where T : StorageEntity
        {
            T entity = null;
            CloudTable table = _storage.GetTableReference(tableName);
            if (table.Exists())
            {
                OperationContext context = new OperationContext();
                TableOperation operation = TableOperation.Retrieve(partitionKey, rowKey);
                DynamicTableEntity dynamicEntity = table.Execute(operation).Result as DynamicTableEntity;
                if (dynamicEntity != null)
                {
                    entity = GetEntity<T>(dynamicEntity, context);
                }
            }
            return entity;
        }

        public void InsertEntity(string tableName, StorageEntity entity)
        {
            CloudTable table = _storage.GetTableReference(tableName);
            table.CreateIfNotExists();
            DynamicTableEntity dynamicEntity = GetDynamicEntity(entity);
            TableOperation operation = TableOperation.Insert(dynamicEntity);
            table.Execute(operation);
        }

        public void UpdateEntity(string tableName, StorageEntity entity)
        {
            CloudTable table = _storage.GetTableReference(tableName);
            table.CreateIfNotExists();
            DynamicTableEntity dynamicEntity = GetDynamicEntity(entity);
            TableOperation operation = TableOperation.Replace(dynamicEntity);
            table.Execute(operation);
        }

        public void UpsertEntity(string tableName, StorageEntity entity)
        {
            CloudTable table = _storage.GetTableReference(tableName);
            table.CreateIfNotExists();
            DynamicTableEntity dynamicEntity = GetDynamicEntity(entity);
            TableOperation operation = TableOperation.InsertOrReplace(dynamicEntity);
            table.Execute(operation);
        }

        public void DeleteEntity(string tableName, StorageEntity entity)
        {
            CloudTable table = _storage.GetTableReference(tableName);
            table.CreateIfNotExists();
            entity.ETag = "*";
            TableOperation operation = TableOperation.Delete(entity);
            table.Execute(operation);
        }

        public void DeleteEntities(string tableName, IList<StorageEntity> entities)
        {
            if (entities.Count> 0)
            {
                CloudTable table = _storage.GetTableReference(tableName);
                table.CreateIfNotExists();
                TableBatchOperation operations = new TableBatchOperation();
                foreach (StorageEntity entity in entities)
                {
                    entity.ETag = "*";
                    operations.Delete(entity);
                }
                table.ExecuteBatch(operations);
            }
        }

        public void DeleteTable(string tableName)
        {
            CloudTable table = _storage.GetTableReference(tableName);
            table.DeleteIfExists();
        }
    }
}
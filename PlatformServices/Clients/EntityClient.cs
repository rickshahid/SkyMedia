using System;
using System.Collections.Generic;

using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace AzureSkyMedia.PlatformServices
{
    public class EntityClient
    {
        private CloudTableClient _storage;

        public EntityClient()
        {
            CloudStorageAccount storageAccount = Storage.GetSystemAccount();
            BindContext(storageAccount);
        }

        public EntityClient(string authToken, string accountName)
        {
            CloudStorageAccount storageAccount = Storage.GetUserAccount(authToken, accountName);
            BindContext(storageAccount);
        }

        private void BindContext(CloudStorageAccount storageAccount)
        {
            _storage = storageAccount.CreateCloudTableClient();
        }

        private void SetProperties(StorageEntity entity)
        {
            entity.ETag = Constants.Storage.TableProperties.OpenConcurrency;
            if (!entity.CreatedOn.HasValue)
            {
                entity.CreatedOn = DateTime.UtcNow;
            }
        }

        private void SetProperties(IList<StorageEntity> entities)
        {
            foreach (StorageEntity entity in entities)
            {
                SetProperties(entity);
            }
        }

        public IEnumerable<T> GetEntities<T>(string tableName, string propertyName, string filterOperation, string propertyValue) where T : StorageEntity, new()
        {
            CloudTable table = _storage.GetTableReference(tableName);
            string filter = TableQuery.GenerateFilterCondition(propertyName, filterOperation, propertyValue);
            TableQuery<T> query = new TableQuery<T>().Where(filter);
            return table.ExecuteQuery(query);
        }

        public IEnumerable<T> GetEntities<T>(string tableName, string partitionKey) where T : StorageEntity, new()
        {
            string propertyName = Constants.Storage.TableProperties.PartitionKey;
            return GetEntities<T>(tableName, propertyName, QueryComparisons.Equal, partitionKey);
        }

        public T GetEntity<T>(string tableName, string partitionKey, string rowKey) where T : StorageEntity
        {
            CloudTable table = _storage.GetTableReference(tableName);
            TableOperation operation = TableOperation.Retrieve<T>(partitionKey, rowKey);
            TableResult result = table.Execute(operation);
            return (T)result.Result;
        }

        public TableResult InsertEntity(string tableName, StorageEntity entity)
        {
            SetProperties(entity);
            CloudTable table = _storage.GetTableReference(tableName);
            table.CreateIfNotExists();
            TableOperation operation = TableOperation.Insert(entity);
            return table.Execute(operation);
        }

        public IList<TableResult> InsertEntities(string tableName, IList<StorageEntity> entities)
        {
            SetProperties(entities);
            CloudTable table = _storage.GetTableReference(tableName);
            table.CreateIfNotExists();
            TableBatchOperation operation = new TableBatchOperation();
            foreach (StorageEntity entity in entities)
            {
                operation.Insert(entity);
            }
            return table.ExecuteBatch(operation);
        }

        public TableResult UpdateEntity(string tableName, StorageEntity entity)
        {
            SetProperties(entity);
            CloudTable table = _storage.GetTableReference(tableName);
            TableOperation operation = TableOperation.Replace(entity);
            return table.Execute(operation);
        }

        public IList<TableResult> UpdateEntities(string tableName, IList<StorageEntity> entities)
        {
            SetProperties(entities);
            CloudTable table = _storage.GetTableReference(tableName);
            TableBatchOperation operation = new TableBatchOperation();
            foreach (StorageEntity entity in entities)
            {
                operation.Replace(entity);
            }
            return table.ExecuteBatch(operation);
        }

        public TableResult UpsertEntity(string tableName, StorageEntity entity)
        {
            SetProperties(entity);
            CloudTable table = _storage.GetTableReference(tableName);
            table.CreateIfNotExists();
            TableOperation operation = TableOperation.InsertOrReplace(entity);
            return table.Execute(operation);
        }

        public IList<TableResult> UpsertEntities(string tableName, IList<StorageEntity> entities)
        {
            SetProperties(entities);
            CloudTable table = _storage.GetTableReference(tableName);
            table.CreateIfNotExists();
            TableBatchOperation operation = new TableBatchOperation();
            foreach (StorageEntity entity in entities)
            {
                operation.InsertOrReplace(entity);
            }
            return table.ExecuteBatch(operation);
        }

        public TableResult MergeEntity(string tableName, StorageEntity entity)
        {
            SetProperties(entity);
            CloudTable table = _storage.GetTableReference(tableName);
            table.CreateIfNotExists();
            TableOperation operation = TableOperation.Merge(entity);
            return table.Execute(operation);
        }

        public IList<TableResult> MergeEntities(string tableName, IList<StorageEntity> entities)
        {
            SetProperties(entities);
            CloudTable table = _storage.GetTableReference(tableName);
            table.CreateIfNotExists();
            TableBatchOperation operation = new TableBatchOperation();
            foreach (StorageEntity entity in entities)
            {
                operation.Merge(entity);
            }
            return table.ExecuteBatch(operation);
        }

        public TableResult DeleteEntity(string tableName, StorageEntity entity)
        {
            CloudTable table = _storage.GetTableReference(tableName);
            TableOperation operation = TableOperation.Delete(entity);
            return table.Execute(operation);
        }

        public IList<TableResult> DeleteEntities(string tableName, IList<StorageEntity> entities)
        {
            CloudTable table = _storage.GetTableReference(tableName);
            TableBatchOperation operation = new TableBatchOperation();
            foreach (StorageEntity entity in entities)
            {
                operation.Delete(entity);
            }
            return table.ExecuteBatch(operation);
        }

        public void DeleteTable(string tableName)
        {
            CloudTable table = _storage.GetTableReference(tableName);
            table.DeleteIfExists();
        }
    }
}

using System;
using System.Linq;
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

        private void SetProperties(StorageEntity entity)
        {
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

        public T[] GetEntities<T>(string tableName, string propertyName, string filterOperation, string propertyValue) where T : StorageEntity, new()
        {
            T[] entities = new T[] { };
            CloudTable table = _storage.GetTableReference(tableName);
            if (table.Exists())
            {
                TableQuery<T> query = new TableQuery<T>();
                if (!string.IsNullOrEmpty(propertyName))
                {
                    string filter = TableQuery.GenerateFilterCondition(propertyName, filterOperation, propertyValue);
                    query = query.Where(filter);
                }
                entities = table.ExecuteQuery(query).ToArray();
            }
            return entities;
        }

        public T[] GetEntities<T>(string tableName) where T : StorageEntity, new()
        {
            return GetEntities<T>(tableName, null, QueryComparisons.Equal, null);
        }

        public T GetEntity<T>(string tableName, string partitionKey, string rowKey) where T : StorageEntity
        {
            T entity = null;
            CloudTable table = _storage.GetTableReference(tableName);
            if (table.Exists())
            {
                TableOperation operation = TableOperation.Retrieve<T>(partitionKey, rowKey);
                entity = (T)table.Execute(operation).Result;
            }
            return entity;
        }

        public void InsertEntity(string tableName, StorageEntity entity)
        {
            CloudTable table = _storage.GetTableReference(tableName);
            table.CreateIfNotExists();
            SetProperties(entity);
            TableOperation operation = TableOperation.Insert(entity);
            table.Execute(operation);
        }

        public void InsertEntities(string tableName, IList<StorageEntity> entities)
        {
            CloudTable table = _storage.GetTableReference(tableName);
            table.CreateIfNotExists();
            SetProperties(entities);
            TableBatchOperation operations = new TableBatchOperation();
            foreach (StorageEntity entity in entities)
            {
                operations.Insert(entity);
            }
            table.ExecuteBatch(operations);
        }

        public void UpdateEntity(string tableName, StorageEntity entity)
        {
            CloudTable table = _storage.GetTableReference(tableName);
            table.CreateIfNotExists();
            SetProperties(entity);
            TableOperation operation = TableOperation.Replace(entity);
            table.Execute(operation);
        }

        public void UpdateEntities(string tableName, IList<StorageEntity> entities)
        {
            CloudTable table = _storage.GetTableReference(tableName);
            table.CreateIfNotExists();
            SetProperties(entities);
            TableBatchOperation operations = new TableBatchOperation();
            foreach (StorageEntity entity in entities)
            {
                operations.Replace(entity);
            }
            table.ExecuteBatch(operations);
        }

        public void UpsertEntity(string tableName, StorageEntity entity)
        {
            CloudTable table = _storage.GetTableReference(tableName);
            table.CreateIfNotExists();
            SetProperties(entity);
            TableOperation operation = TableOperation.InsertOrReplace(entity);
            table.Execute(operation);
        }

        public void UpsertEntities(string tableName, IList<StorageEntity> entities)
        {
            CloudTable table = _storage.GetTableReference(tableName);
            table.CreateIfNotExists();
            SetProperties(entities);
            TableBatchOperation operations = new TableBatchOperation();
            foreach (StorageEntity entity in entities)
            {
                operations.InsertOrReplace(entity);
            }
            table.ExecuteBatch(operations);
        }

        public void MergeEntity(string tableName, StorageEntity entity)
        {
            CloudTable table = _storage.GetTableReference(tableName);
            table.CreateIfNotExists();
            SetProperties(entity);
            TableOperation operation = TableOperation.Merge(entity);
            table.Execute(operation);
        }

        public void MergeEntities(string tableName, IList<StorageEntity> entities)
        {
            CloudTable table = _storage.GetTableReference(tableName);
            table.CreateIfNotExists();
            SetProperties(entities);
            TableBatchOperation operations = new TableBatchOperation();
            foreach (StorageEntity entity in entities)
            {
                operations.Merge(entity);
            }
            table.ExecuteBatch(operations);
        }

        public void DeleteEntity(string tableName, StorageEntity entity)
        {
            CloudTable table = _storage.GetTableReference(tableName);
            table.CreateIfNotExists();
            TableOperation operation = TableOperation.Delete(entity);
            table.Execute(operation);
        }

        public void DeleteEntities(string tableName, IList<StorageEntity> entities)
        {
            CloudTable table = _storage.GetTableReference(tableName);
            table.CreateIfNotExists();
            TableBatchOperation operations = new TableBatchOperation();
            foreach (StorageEntity entity in entities)
            {
                operations.Delete(entity);
            }
            table.ExecuteBatch(operations);
        }

        public void DeleteTable(string tableName)
        {
            CloudTable table = _storage.GetTableReference(tableName);
            table.DeleteIfExists();
        }
    }
}

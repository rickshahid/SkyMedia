using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;

using Newtonsoft.Json.Linq;

namespace AzureSkyMedia.PlatformServices
{
    public class DatabaseClient : IDisposable
    {
        private DocumentClient _database;
        private string _databaseId;

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && _database != null)
            {
                _database.Dispose();
                _database = null;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public DatabaseClient(bool readWrite)
        {
            string settingKey = readWrite ? Constant.AppSettingKey.AzureNoSqlReadWrite : Constant.AppSettingKey.AzureNoSqlReadOnly;
            string[] accountCredentials = AppSetting.GetValue(settingKey, true);
            string accountEndpoint = accountCredentials[0];
            string accountKey = accountCredentials[1];

            _database = new DocumentClient(new Uri(accountEndpoint), accountKey);

            settingKey = Constant.AppSettingKey.NoSqlDatabaseId;
            _databaseId = AppSetting.GetValue(settingKey);
        }

        private JObject GetResult(string response)
        {
            JObject result = null;
            if (!string.IsNullOrEmpty(response))
            {
                result = JObject.Parse(response);
                string[] documentProperties = Constant.Database.DocumentProperties.Split(Constant.TextDelimiter.Application);
                foreach (string documentProperty in documentProperties)
                {
                    result.Remove(documentProperty);
                }
            }
            return result;
        }

        public JObject GetDocument(string documentId)
        {
            string[] documentInfo = documentId.Split(Constant.TextDelimiter.Identifier);
            string collectionId = documentInfo[0];
            documentId = documentInfo[1];
            IQueryable<Document> query = _database.CreateDocumentQuery("dbs/" + _databaseId + "/colls/" + collectionId)
                .Where(d => d.Id == documentId);
            IEnumerable<Document> documents = query.AsEnumerable<Document>();
            Document document = documents.FirstOrDefault();
            return (document == null) ? null : GetResult(document.ToString());
        }

        public string CreateDocument(string assetAccount, string assetId, string collectionId, string jsonData)
        {
            JObject jsonDoc = JObject.Parse(jsonData);
            jsonDoc["assetAccount"] = assetAccount;
            jsonDoc["assetId"] = assetId;
            Uri collectionUri = UriFactory.CreateDocumentCollectionUri(_databaseId, collectionId);
            Task<ResourceResponse<Document>> createTask = _database.CreateDocumentAsync(collectionUri, jsonDoc);
            createTask.Wait();
            ResourceResponse<Document> responseDocument = createTask.Result;
            return responseDocument.Resource.Id;
        }

        public void DeleteDocument(string collectionId, string documentId)
        {
            string docId = string.Concat(collectionId, Constant.TextDelimiter.Identifier, documentId);
            JObject jsonDoc = GetDocument(docId);
            if (jsonDoc != null)
            {
                Uri documentUri = UriFactory.CreateDocumentUri(_databaseId, collectionId, documentId);
                Task<ResourceResponse<Document>> deleteTask = _database.DeleteDocumentAsync(documentUri);
                deleteTask.Wait();
            }
        }

        public JObject ExecuteProcedure(string collectionId, string procedureId, params dynamic[] procedureParameters)
        {
            Uri collectionUri = UriFactory.CreateDocumentCollectionUri(_databaseId, collectionId);
            Uri procedureUri = UriFactory.CreateStoredProcedureUri(_databaseId, collectionId, procedureId);
            Task<StoredProcedureResponse<JValue>> procedureTask = _database.ExecuteStoredProcedureAsync<JValue>(procedureUri, procedureParameters);
            procedureTask.Wait();
            JValue procesureResponse = procedureTask.Result.Response;
            return (procesureResponse == null) ? null : GetResult(procesureResponse.ToString());
        }
    }
}

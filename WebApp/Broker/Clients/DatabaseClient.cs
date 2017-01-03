using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;

using Newtonsoft.Json.Linq;

namespace SkyMedia.ServiceBroker
{
    internal class DatabaseClient
    {
        private DocumentClient _database;
        private string _databaseId;

        public DatabaseClient() : this(false) { }

        public DatabaseClient(bool readWrite)
        {
            string settingKey = readWrite ? Constants.ConnectionStrings.AzureNoSqlReadWrite : Constants.ConnectionStrings.AzureNoSqlReadOnly;
            string[] accountCredentials = AppSetting.GetValue(settingKey, true);
            string accountEndpoint = accountCredentials[0];
            string accountKey = accountCredentials[1];

            _database = new DocumentClient(new Uri(accountEndpoint), accountKey);

            settingKey = Constants.AppSettings.NoSqlDatabaseId;
            _databaseId = AppSetting.GetValue(settingKey);
        }

        private JObject GetResult(string document)
        {
            JObject jsonDoc = null;
            if (document != null)
            {
                jsonDoc = JObject.Parse(document.ToString());
                string settingKey = Constants.AppSettings.NoSqlDocumentProperties;
                string[] properties = AppSetting.GetValue(settingKey).Split(Constants.MultiItemSeparator);
                foreach (string property in properties)
                {
                    jsonDoc.Remove(property);
                }
            }
            return jsonDoc;
        }

        public JObject GetDocument(string documentId)
        {
            string[] documentInfo = documentId.Split(Constants.MultiItemSeparator);
            string collectionId = documentInfo[0];
            documentId = documentInfo[1];
            IQueryable<Document> query = _database.CreateDocumentQuery("dbs/" + _databaseId + "/colls/" + collectionId)
                .Where(d => d.Id == documentId);
            IEnumerable<Document> documents = query.AsEnumerable<Document>();
            Document document = documents.FirstOrDefault();
            return (document == null) ? null : GetResult(document.ToString());
        }

        public string CreateDocument(string collectionId, string jsonData)
        {
            JObject jsonDoc = JObject.Parse(jsonData);
            Uri collectionUri = UriFactory.CreateDocumentCollectionUri(_databaseId, collectionId);
            Task<ResourceResponse<Document>> createTask = _database.CreateDocumentAsync(collectionUri, jsonDoc);
            createTask.Wait();
            ResourceResponse<Document> responseDocument = createTask.Result;
            return responseDocument.Resource.Id;
        }

        public void DeleteDocument(string collectionId, string documentId)
        {
            string docId = string.Concat(collectionId, Constants.MultiItemSeparator, documentId);
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

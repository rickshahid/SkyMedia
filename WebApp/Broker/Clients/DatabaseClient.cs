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

        public JObject GetDocument(string documentId)
        {
            string[] documentInfo = documentId.Split(Constants.MultiItemSeparator);
            string collectionId = documentInfo[0];
            documentId = documentInfo[1];

            IQueryable<Document> query = _database.CreateDocumentQuery("dbs/" + _databaseId + "/colls/" + collectionId)
                .Where(d => d.Id == documentId);
            IEnumerable<Document> documents = query.AsEnumerable<Document>();
            Document document = documents.FirstOrDefault();

            JObject jsonDoc = null;
            if (document != null)
            {
                jsonDoc = JObject.Parse(document.ToString());
                string settingKey = Constants.AppSettings.NoSqlDocumentProperties;
                string[] documentProperties = AppSetting.GetValue(settingKey).Split(Constants.MultiItemSeparator);
                foreach (string documentProperty in documentProperties)
                {
                    jsonDoc.Remove(documentProperty);
                }
            }
            return (jsonDoc == null) ? new JObject() : jsonDoc;
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
    }
}

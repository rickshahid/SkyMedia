using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;

using Newtonsoft.Json.Linq;

namespace AzureSkyMedia.PlatformServices
{
    internal class DatabaseClient : IDisposable
    {
        private DocumentClient _cosmos;
        private string _databaseId;

        public DatabaseClient(bool readWrite)
        {
            string settingKey = readWrite ? Constant.AppSettingKey.AzureDatabaseReadWrite : Constant.AppSettingKey.AzureDatabaseReadOnly;
            string[] accountCredentials = AppSetting.GetValue(settingKey, true);
            string accountEndpoint = accountCredentials[0];
            string accountKey = accountCredentials[1];
            _databaseId = accountCredentials[2];

            settingKey = Constant.AppSettingKey.DatabaseRegions;
            string[] regionNames = AppSetting.GetValue(settingKey, true);
            ConnectionPolicy connectionPolicy = new ConnectionPolicy()
            {
                UseMultipleWriteLocations = true
            };
            foreach (string regionName in regionNames)
            {
                connectionPolicy.PreferredLocations.Add(regionName);
            }

            _cosmos = new DocumentClient(new Uri(accountEndpoint), accountKey, connectionPolicy);
        }

        private JObject GetDocument(string jsonData)
        {
            JObject document = JObject.Parse(jsonData);
            JProperty[] properties = document.Properties().ToArray();
            foreach (JProperty property in properties)
            {
                if (property.Name.StartsWith(Constant.Database.Document.SystemPropertyPrefix))
                {
                    document.Remove(property.Name);
                }
            }
            return document;
        }

        private IQueryable<Document> GetDocumentQuery(string collectionId)
        {
            Uri collectionUri = UriFactory.CreateDocumentCollectionUri(_databaseId, collectionId);
            return _cosmos.CreateDocumentQuery(collectionUri);
        }

        public T[] GetDocuments<T>(string collectionId)
        {
            List<T> documents = new List<T>();
            JObject[] docs = GetDocuments(collectionId);
            foreach (JObject doc in docs)
            {
                T document = doc.ToObject<T>();
                documents.Add(document);
            }
            return documents.ToArray();
        }

        public JObject[] GetDocuments(string collectionId)
        {
            List<JObject> documents = new List<JObject>();
            IQueryable<Document> query = GetDocumentQuery(collectionId);
            IEnumerable<Document> docs = query.AsEnumerable<Document>();
            foreach (Document doc in docs)
            {
                JObject document = GetDocument(doc.ToString());
                documents.Add(document);
            }
            return documents.ToArray();
        }

        public T GetDocument<T>(string collectionId, string documentId)
        {
            JObject document = GetDocument(collectionId, documentId);
            return document == null ? default(T): document.ToObject<T>();
        }

        public JObject GetDocument(string collectionId, string documentId)
        {
            JObject document = null;
            IQueryable<Document> query = GetDocumentQuery(collectionId);
            query = query.Where(d => d.Id == documentId);
            IEnumerable<Document> docs = query.AsEnumerable<Document>();
            Document doc = docs.SingleOrDefault();
            if (doc != null)
            {
                document = GetDocument(doc.ToString());
            }
            return document;
        }

        public JObject ExecuteProcedure(string collectionId, string procedureId, params dynamic[] procedureParameters)
        {
            Uri procedureUri = UriFactory.CreateStoredProcedureUri(_databaseId, collectionId, procedureId);
            Task<StoredProcedureResponse<JValue>> procedureTask = _cosmos.ExecuteStoredProcedureAsync<JValue>(procedureUri, procedureParameters);
            StoredProcedureResponse<JValue> procedureResult = procedureTask.Result;
            JValue procedureValue = procedureResult.Response;
            return GetDocument(procedureValue.ToString());
        }

        public string UpsertDocument(string collectionId, object entity)
        {
            JObject document = JObject.FromObject(entity);
            return UpsertDocument(collectionId, document);
        }

        public string UpsertDocument(string collectionId, JObject document)
        {
            Uri collectionUri = UriFactory.CreateDocumentCollectionUri(_databaseId, collectionId);
            Task<ResourceResponse<Document>> upsertTask = _cosmos.UpsertDocumentAsync(collectionUri, document);
            ResourceResponse<Document> upsertResponse = upsertTask.Result;
            return upsertResponse.Resource.Id;
        }

        public void DeleteDocument(string collectionId, string documentId)
        {
            JObject document = GetDocument(collectionId, documentId);
            if (document != null)
            {
                Uri documentUri = UriFactory.CreateDocumentUri(_databaseId, collectionId, documentId);
                _cosmos.DeleteDocumentAsync(documentUri);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && _cosmos != null)
            {
                _cosmos.Dispose();
                _cosmos = null;
            }
        }
    }
}
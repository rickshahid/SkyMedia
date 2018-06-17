using System;
using System.IO;
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
        private string _authToken;

        public DatabaseClient() : this(string.Empty)
        {
        }

        public DatabaseClient(string authToken)
        {
            string settingKey = Constant.AppSettingKey.AzureDatabase;
            string[] accountCredentials = AppSetting.GetValue(settingKey, true);
            string accountEndpoint = accountCredentials[0];
            string accountKey = accountCredentials[1];
            _databaseId = accountCredentials[2];

            settingKey = Constant.AppSettingKey.DatabaseRegionsRead;
            string[] dataRegionsRead = AppSetting.GetValue(settingKey, true);
            ConnectionPolicy connectionPolicy = new ConnectionPolicy();
            foreach (string dataRegionRead in dataRegionsRead)
            {
                connectionPolicy.PreferredLocations.Add(dataRegionRead);
            }

            _authToken = authToken;
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
            string collectionLink = string.Concat("dbs/", _databaseId, "/colls/", collectionId);
            FeedOptions feedOptions = new FeedOptions()
            {
                SessionToken = _authToken
            };
            return _cosmos.CreateDocumentQuery(collectionLink, feedOptions);
        }

        private Uri CreateCollection(Uri databaseUri, string collectionId)
        {
            string settingKey = Constant.AppSettingKey.DatabaseCollectionThroughputUnits;
            string throughputUnits = AppSetting.GetValue(settingKey);
            RequestOptions collectionOptions = new RequestOptions() { OfferThroughput = int.Parse(throughputUnits) };
            DocumentCollection documentCollection = new DocumentCollection() { Id = collectionId };
            ResourceResponse<DocumentCollection> collection = _cosmos.CreateDocumentCollectionIfNotExistsAsync(databaseUri, documentCollection, collectionOptions).Result;
            return UriFactory.CreateDocumentCollectionUri(_databaseId, collectionId);
        }

        public void Initialize(string modelsDirectory)
        {
            Database database = new Database { Id = _databaseId };
            _cosmos.CreateDatabaseIfNotExistsAsync(database).Wait();
            Uri databaseUri = UriFactory.CreateDatabaseUri(_databaseId);

            string collectionId = Constant.Database.Collection.InputWorkflow;
            Uri collectionUri = CreateCollection(databaseUri, collectionId);

            collectionId = Constant.Database.Collection.OutputPublish;
            collectionUri = CreateCollection(databaseUri, collectionId);

            collectionId = Constant.Database.Collection.OutputInsight;
            collectionUri = CreateCollection(databaseUri, collectionId);

            string collectionDirectory = string.Concat(modelsDirectory, collectionId);

            string scriptFile = string.Concat(collectionDirectory, @"\isTimecodeFragment.js");
            UserDefinedFunction function = new UserDefinedFunction()
            {
                Id = "isTimecodeFragment",
                Body = File.ReadAllText(scriptFile)
            };
            _cosmos.CreateUserDefinedFunctionAsync(collectionUri, function);

            scriptFile = string.Concat(collectionDirectory, @"\getTimecodeFragment.js");
            StoredProcedure procedure = new StoredProcedure()
            {
                Id = "getTimecodeFragment",
                Body = File.ReadAllText(scriptFile)
            };
            _cosmos.CreateStoredProcedureAsync(collectionUri, procedure);
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
            Document doc = docs.FirstOrDefault();
            if (doc != null)
            {
                document = GetDocument(doc.ToString());
            }
            return document;
        }

        public JObject GetDocument(string collectionId, string procedureId, params dynamic[] procedureParameters)
        {
            Uri collectionUri = UriFactory.CreateDocumentCollectionUri(_databaseId, collectionId);
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
            ResourceResponse<Document> upsertResult = upsertTask.Result;
            return upsertResult.Resource.Id;
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
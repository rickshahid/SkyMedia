using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using DocClient = Microsoft.Azure.Documents.Client.DocumentClient;

namespace AzureSkyMedia.PlatformServices
{
    internal class DocumentClient : IDisposable
    {
        private DocClient _database;
        private string _databaseId;

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && _database != null)
            {
                _database.Dispose();
                _database = null;
            }
        }

        public DocumentClient()
        {
            string settingKey = Constant.AppSettingKey.AzureDatabaseDocument;
            string[] accountCredentials = AppSetting.GetValue(settingKey, true);
            string accountEndpoint = accountCredentials[0];
            string accountKey = accountCredentials[1];
            _databaseId = accountCredentials[2];
            _database = new DocClient(new Uri(accountEndpoint), accountKey);
        }

        private IQueryable<Document> GetDocumentQuery(string collectionId)
        {
            string collectionLink = string.Concat("dbs/", _databaseId, "/colls/", collectionId);
            FeedOptions feedOptions = new FeedOptions()
            {
                EnableCrossPartitionQuery = true
            };
            return _database.CreateDocumentQuery(collectionLink, feedOptions);
        }

        private JObject GetResponseResult(string responseData)
        {
            JObject result = null;
            if (!string.IsNullOrEmpty(responseData))
            {
                result = JObject.Parse(responseData);
                JProperty[] properties = result.Properties().ToArray();
                foreach (JProperty property in properties)
                {
                    if (property.Name.StartsWith(Constant.Database.Document.SystemPropertyPrefix))
                    {
                        result.Remove(property.Name);
                    }
                }
            }
            return result;
        }

        public void Initialize(string modelsDirectory)
        {
            Database databaseId = new Database { Id = _databaseId };
            ResourceResponse<Database> database = _database.CreateDatabaseIfNotExistsAsync(databaseId).Result;
            Uri databaseUri = UriFactory.CreateDatabaseUri(_databaseId);

            string settingKey = Constant.AppSettingKey.DatabaseThroughputUnits;
            string throughputUnits = AppSetting.GetValue(settingKey);

            RequestOptions collectionOptions = new RequestOptions();
            collectionOptions.OfferThroughput = int.Parse(throughputUnits);

            DocumentCollection documentCollection = new DocumentCollection();
            documentCollection.Id = Constant.Database.Collection.ContentInsight;

            ResourceResponse<DocumentCollection> collection = _database.CreateDocumentCollectionIfNotExistsAsync(databaseUri, documentCollection, collectionOptions).Result;
            Uri collectionUri = UriFactory.CreateDocumentCollectionUri(_databaseId, documentCollection.Id);

            string collectionDirectory = string.Concat(modelsDirectory, @"\", documentCollection.Id);

            string jsFile = string.Concat(collectionDirectory, @"\Functions\isTimecodeFragment.js");
            UserDefinedFunction function = new UserDefinedFunction();
            function.Id = "isTimecodeFragment";
            function.Body = File.ReadAllText(jsFile);
            _database.CreateUserDefinedFunctionAsync(collectionUri, function);

            jsFile = string.Concat(collectionDirectory, @"\Procedures\getTimecodeFragment.js");
            StoredProcedure procedure = new StoredProcedure();
            procedure.Id = "getTimecodeFragment";
            procedure.Body = File.ReadAllText(jsFile);
            _database.CreateStoredProcedureAsync(collectionUri, procedure);

            documentCollection = new DocumentCollection();
            documentCollection.Id = Constant.Database.Collection.ProcessorConfig;

            collection = _database.CreateDocumentCollectionIfNotExistsAsync(databaseUri, documentCollection, collectionOptions).Result;
            collectionUri = UriFactory.CreateDocumentCollectionUri(_databaseId, documentCollection.Id);

            collectionDirectory = string.Concat(modelsDirectory, @"\", documentCollection.Id);

            jsFile = string.Concat(collectionDirectory, @"\Procedures\getProcessorConfig.js");
            procedure = new StoredProcedure();
            procedure.Id = "getProcessorConfig";
            procedure.Body = File.ReadAllText(jsFile);
            _database.CreateStoredProcedureAsync(collectionUri, procedure);

            string presetsDirectory = string.Concat(modelsDirectory, @"\ProcessorPresets");
            string[] mediaProcessors = Directory.GetDirectories(presetsDirectory);
            foreach (string mediaProcessor in mediaProcessors)
            {
                string[] processorPresets = Directory.GetFiles(mediaProcessor);
                foreach (string processorPreset in processorPresets)
                {
                    if (processorPreset.EndsWith(Constant.Media.FileExtension.Json, StringComparison.OrdinalIgnoreCase))
                    {
                        StreamReader streamReader = new StreamReader(processorPreset);
                        JsonTextReader presetReader = new JsonTextReader(streamReader);
                        JObject presetConfig = JObject.Load(presetReader);
                        UpsertDocument(documentCollection.Id, presetConfig);
                    }
                }
            }
        }

        public JObject[] GetDocuments(string collectionId)
        {
            List<JObject> documents = new List<JObject>();
            IQueryable<Document> query = GetDocumentQuery(collectionId);
            IEnumerable<Document> docs = query.AsEnumerable<Document>();
            foreach (Document doc in docs)
            {
                JObject result = GetResponseResult(doc.ToString());
                documents.Add(result);
            }
            return documents.ToArray();
        }

        public JObject GetDocument(string documentId)
        {
            JObject result = null;
            string[] documentInfo = documentId.Split(Constant.TextDelimiter.Identifier);
            string collectionId = documentInfo[0];
            documentId = documentInfo[1];
            IQueryable<Document> query = GetDocumentQuery(collectionId);
            query = query.Where(d => d.Id == documentId);
            IEnumerable<Document> documents = query.AsEnumerable<Document>();
            Document document = documents.FirstOrDefault();
            if (document != null)
            {
                result = GetResponseResult(document.ToString());
            }
            return result;
        }

        public static JObject SetContext(JObject jsonDoc, string accountId, string accountDomain, string accountEndpoint, string clientId, string clientKey, string assetId)
        {
            jsonDoc["accountId"] = accountId;
            jsonDoc["accountDomain"] = accountDomain;
            jsonDoc["accountEndpoint"] = accountEndpoint;
            jsonDoc["clientId"] = clientId;
            jsonDoc["clientKey"] = clientKey;
            jsonDoc["assetId"] = assetId;
            return jsonDoc;
        }

        public string UpsertDocument(string collectionId, JObject jsonDoc)
        {
            Uri collectionUri = UriFactory.CreateDocumentCollectionUri(_databaseId, collectionId);
            Task<ResourceResponse<Document>> createTask = _database.UpsertDocumentAsync(collectionUri, jsonDoc);
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
                string accountId = jsonDoc["accountId"].ToString();
                Uri documentUri = UriFactory.CreateDocumentUri(_databaseId, collectionId, documentId);
                Task<ResourceResponse<Document>> deleteTask = _database.DeleteDocumentAsync(documentUri);
                deleteTask.Wait();
            }
        }

        public JObject ExecuteProcedure(string collectionId, string procedureId, params dynamic[] procedureParameters)
        {
            JObject result = null;
            Uri collectionUri = UriFactory.CreateDocumentCollectionUri(_databaseId, collectionId);
            Uri procedureUri = UriFactory.CreateStoredProcedureUri(_databaseId, collectionId, procedureId);
            Task<StoredProcedureResponse<JValue>> procedureTask = _database.ExecuteStoredProcedureAsync<JValue>(procedureUri, procedureParameters);
            procedureTask.Wait();
            JValue procesureResponse = procedureTask.Result.Response;
            if (procesureResponse != null)
            {
                result = GetResponseResult(procesureResponse.ToString());
            }
            return result;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.WindowsAzure.MediaServices.Client;

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

        private JObject ParseDocument(string documentData)
        {
            JObject document = null;
            if (!string.IsNullOrEmpty(documentData))
            {
                document = JObject.Parse(documentData);
                JProperty[] properties = document.Properties().ToArray();
                foreach (JProperty property in properties)
                {
                    if (property.Name.StartsWith(Constant.Database.Document.SystemPropertyPrefix))
                    {
                        document.Remove(property.Name);
                    }
                }
            }
            return document;
        }

        public void Initialize(string modelsDirectory)
        {
            Database databaseId = new Database { Id = _databaseId };
            ResourceResponse<Database> database = _database.CreateDatabaseIfNotExistsAsync(databaseId).Result;
            Uri databaseUri = UriFactory.CreateDatabaseUri(_databaseId);

            string settingKey = Constant.AppSettingKey.DatabaseThroughputUnits;
            string throughputUnits = AppSetting.GetValue(settingKey);

            RequestOptions collectionOptions = new RequestOptions()
            {
                OfferThroughput = int.Parse(throughputUnits)
            };

            DocumentCollection documentCollection = new DocumentCollection()
            {
                Id = Constant.Database.Collection.ContentInsight
            };

            ResourceResponse<DocumentCollection> collection = _database.CreateDocumentCollectionIfNotExistsAsync(databaseUri, documentCollection, collectionOptions).Result;
            Uri collectionUri = UriFactory.CreateDocumentCollectionUri(_databaseId, documentCollection.Id);

            string collectionDirectory = string.Concat(modelsDirectory, @"\", documentCollection.Id);

            string jsFile = string.Concat(collectionDirectory, @"\Functions\isTimecodeFragment.js");
            UserDefinedFunction function = new UserDefinedFunction()
            {
                Id = "isTimecodeFragment",
                Body = File.ReadAllText(jsFile)
            };
            _database.CreateUserDefinedFunctionAsync(collectionUri, function);

            jsFile = string.Concat(collectionDirectory, @"\Procedures\getTimecodeFragment.js");
            StoredProcedure procedure = new StoredProcedure()
            {
                Id = "getTimecodeFragment",
                Body = File.ReadAllText(jsFile)
            };
            _database.CreateStoredProcedureAsync(collectionUri, procedure);

            documentCollection = new DocumentCollection()
            {
                Id = Constant.Database.Collection.ProcessorConfig
            };

            collection = _database.CreateDocumentCollectionIfNotExistsAsync(databaseUri, documentCollection, collectionOptions).Result;
            collectionUri = UriFactory.CreateDocumentCollectionUri(_databaseId, documentCollection.Id);

            collectionDirectory = string.Concat(modelsDirectory, @"\", documentCollection.Id);

            jsFile = string.Concat(collectionDirectory, @"\Procedures\getProcessorConfig.js");
            procedure = new StoredProcedure()
            {
                Id = "getProcessorConfig",
                Body = File.ReadAllText(jsFile)
            };
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

        public static JObject SetContext(JObject document, string accountId, string accountDomain, string accountEndpoint,
                                         string clientId, string clientKey, string assetId)
        {
            document["accountId"] = accountId;
            document["accountDomain"] = accountDomain;
            document["accountEndpoint"] = accountEndpoint;
            document["clientId"] = clientId;
            document["clientKey"] = clientKey;
            document["assetId"] = assetId;
            return document;
        }

        public static string GetDocumentId(IAsset asset, out bool videoIndexer)
        {
            videoIndexer = false;
            string documentId = string.Empty;
            if (!string.IsNullOrEmpty(asset.AlternateId) && asset.AlternateId.Contains(Constant.TextDelimiter.Identifier.ToString()))
            {
                string[] alternateId = asset.AlternateId.Split(Constant.TextDelimiter.Identifier);
                videoIndexer = string.Equals(alternateId[0], Processor.GetProcessorName(MediaProcessor.VideoIndexer), StringComparison.OrdinalIgnoreCase);
                documentId = alternateId[1];
            }
            return documentId;
        }

        public JObject[] GetDocuments(string collectionId)
        {
            List<JObject> documents = new List<JObject>();
            IQueryable<Document> query = GetDocumentQuery(collectionId);
            IEnumerable<Document> docs = query.AsEnumerable<Document>();
            foreach (Document doc in docs)
            {
                JObject document = ParseDocument(doc.ToString());
                documents.Add(document);
            }
            return documents.ToArray();
        }

        public JObject GetDocument(string documentId)
        {
            JObject document = null;
            string[] id = documentId.Split(Constant.TextDelimiter.Identifier);
            string collectionId = id[0];
            documentId = id[1];
            IQueryable<Document> query = GetDocumentQuery(collectionId);
            query = query.Where(d => d.Id == documentId);
            IEnumerable<Document> documents = query.AsEnumerable<Document>();
            Document doc = documents.FirstOrDefault();
            if (doc != null)
            {
                document = ParseDocument(doc.ToString());
            }
            return document;
        }

        public string UpsertDocument(string collectionId, JObject document)
        {
            Uri collectionUri = UriFactory.CreateDocumentCollectionUri(_databaseId, collectionId);
            Task<ResourceResponse<Document>> upsertTask = _database.UpsertDocumentAsync(collectionUri, document);
            upsertTask.Wait();
            ResourceResponse<Document> resourceResponse = upsertTask.Result;
            return resourceResponse.Resource.Id;
        }

        public void DeleteDocument(string collectionId, string documentId)
        {
            string docId = string.Concat(collectionId, Constant.TextDelimiter.Identifier, documentId);
            JObject document = GetDocument(docId);
            if (document != null)
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
            StoredProcedureResponse<JValue> procedureResponse = procedureTask.Result;
            JValue procedureValue = procedureResponse.Response;
            return ParseDocument(procedureValue.ToString());
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
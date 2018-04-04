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

namespace AzureSkyMedia.PlatformServices
{
    internal class DatabaseClient : IDisposable
    {
        private DocumentClient _cosmos;
        private string _databaseId;

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && _cosmos != null)
            {
                _cosmos.Dispose();
                _cosmos = null;
            }
        }

        public DatabaseClient()
        {
            string settingKey = Constant.AppSettingKey.AzureDatabase;
            string[] accountCredentials = AppSetting.GetValue(settingKey, true);
            string accountEndpoint = accountCredentials[0];
            string accountKey = accountCredentials[1];

            settingKey = Constant.AppSettingKey.DatabaseRegionsRead;
            string[] dataRegionsRead = AppSetting.GetValue(settingKey, true);
            ConnectionPolicy connectionPolicy = new ConnectionPolicy();
            foreach (string dataRegionRead in dataRegionsRead)
            {
                connectionPolicy.PreferredLocations.Add(dataRegionRead);
            }

            _databaseId = accountCredentials[2];
            _cosmos = new DocumentClient(new Uri(accountEndpoint), accountKey, connectionPolicy);
        }

        private IQueryable<Document> GetDocumentQuery(string collectionId)
        {
            string collectionLink = string.Concat("dbs/", _databaseId, "/colls/", collectionId);
            FeedOptions feedOptions = new FeedOptions()
            {
                EnableCrossPartitionQuery = true
            };
            return _cosmos.CreateDocumentQuery(collectionLink, feedOptions);
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

        private Uri CreateCollection(Uri databaseUri, string collectionId)
        {
            string settingKey = Constant.AppSettingKey.DatabaseCollectionThroughputUnits;
            string throughputUnits = AppSetting.GetValue(settingKey);

            RequestOptions collectionOptions = new RequestOptions()
            {
                OfferThroughput = int.Parse(throughputUnits)
            };

            DocumentCollection documentCollection = new DocumentCollection()
            {
                Id = collectionId
            };

            ResourceResponse<DocumentCollection> collection = _cosmos.CreateDocumentCollectionIfNotExistsAsync(databaseUri, documentCollection, collectionOptions).Result;
            return UriFactory.CreateDocumentCollectionUri(_databaseId, collectionId);
        }

        public void Initialize(string modelsDirectory)
        {
            Database databaseId = new Database { Id = _databaseId };
            ResourceResponse<Database> database = _cosmos.CreateDatabaseIfNotExistsAsync(databaseId).Result;
            Uri databaseUri = UriFactory.CreateDatabaseUri(_databaseId);

            string collectionId = Constant.Database.Collection.MediaPublish;
            Uri collectionUri = CreateCollection(databaseUri, collectionId);

            collectionId = Constant.Database.Collection.MediaInsight;
            collectionUri = CreateCollection(databaseUri, collectionId);

            string collectionDirectory = string.Concat(modelsDirectory, @"\", collectionId);

            string jsFile = string.Concat(collectionDirectory, @"\Functions\isTimecodeFragment.js");
            UserDefinedFunction function = new UserDefinedFunction()
            {
                Id = "isTimecodeFragment",
                Body = File.ReadAllText(jsFile)
            };
            _cosmos.CreateUserDefinedFunctionAsync(collectionUri, function);

            jsFile = string.Concat(collectionDirectory, @"\Procedures\getTimecodeFragment.js");
            StoredProcedure procedure = new StoredProcedure()
            {
                Id = "getTimecodeFragment",
                Body = File.ReadAllText(jsFile)
            };
            _cosmos.CreateStoredProcedureAsync(collectionUri, procedure);

            collectionId = Constant.Database.Collection.ProcessorConfig;
            collectionUri = CreateCollection(databaseUri, collectionId);

            collectionDirectory = string.Concat(modelsDirectory, @"\", collectionId);

            jsFile = string.Concat(collectionDirectory, @"\Procedures\getProcessorConfig.js");
            procedure = new StoredProcedure()
            {
                Id = "getProcessorConfig",
                Body = File.ReadAllText(jsFile)
            };
            _cosmos.CreateStoredProcedureAsync(collectionUri, procedure);

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
                        UpsertDocument(collectionId, presetConfig);
                    }
                }
            }
        }

        public static JObject SetContext(JObject document, MediaAccount mediaAccount, string assetId)
        {
            document["accountId"] = mediaAccount.Id;
            document["accountDomain"] = mediaAccount.DomainName;
            document["accountEndpoint"] = mediaAccount.EndpointUrl;
            document["clientId"] = mediaAccount.ClientId;
            document["clientKey"] = mediaAccount.ClientKey;
            document["assetId"] = assetId;
            return document;
        }

        public static string GetDocumentId(IAsset asset, out bool videoIndexer)
        {
            videoIndexer = false;
            string documentId = string.Empty;
            string alternateId = asset.AlternateId;
            if (string.IsNullOrEmpty(alternateId) && asset.ParentAssets.Count == 1)
            {
                alternateId = asset.ParentAssets[0].AlternateId;
            }
            if (!string.IsNullOrEmpty(alternateId) && alternateId.Contains(Constant.TextDelimiter.Identifier.ToString()))
            {
                string[] alternateIdInfo = alternateId.Split(Constant.TextDelimiter.Identifier);
                videoIndexer = string.Equals(alternateIdInfo[0], MediaProcessor.VideoIndexer.ToString(), StringComparison.OrdinalIgnoreCase);
                documentId = alternateIdInfo[1];
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
            IEnumerable<Document> documents = query.AsEnumerable<Document>();
            Document doc = documents.FirstOrDefault();
            if (doc != null)
            {
                document = ParseDocument(doc.ToString());
            }
            return document;
        }

        public JObject GetDocument(string collectionId, string procedureId, params dynamic[] procedureParameters)
        {
            Uri collectionUri = UriFactory.CreateDocumentCollectionUri(_databaseId, collectionId);
            Uri procedureUri = UriFactory.CreateStoredProcedureUri(_databaseId, collectionId, procedureId);
            Task<StoredProcedureResponse<JValue>> procedureTask = _cosmos.ExecuteStoredProcedureAsync<JValue>(procedureUri, procedureParameters);
            procedureTask.Wait();
            StoredProcedureResponse<JValue> procedureResponse = procedureTask.Result;
            JValue procedureValue = procedureResponse.Response;
            return ParseDocument(procedureValue.ToString());
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
            upsertTask.Wait();
            ResourceResponse<Document> resourceResponse = upsertTask.Result;
            return resourceResponse.Resource.Id;
        }

        public void DeleteDocument(string collectionId, string documentId)
        {
            JObject document = GetDocument(collectionId, documentId);
            if (document != null)
            {
                Uri documentUri = UriFactory.CreateDocumentUri(_databaseId, collectionId, documentId);
                Task<ResourceResponse<Document>> deleteTask = _cosmos.DeleteDocumentAsync(documentUri);
                deleteTask.Wait();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;

using Newtonsoft.Json.Linq;

namespace AzureSkyMedia.PlatformServices
{
    internal class CosmosClient : IDisposable
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

        public CosmosClient(bool readWrite)
        {
            string settingKey = readWrite ? Constant.AppSettingKey.AzureCosmosReadWrite : Constant.AppSettingKey.AzureCosmosReadOnly;
            string[] accountCredentials = AppSetting.GetValue(settingKey, true);
            string accountEndpoint = accountCredentials[0];
            string accountKey = accountCredentials[1];
            _databaseId = accountCredentials[2];
            _cosmos = new DocumentClient(new Uri(accountEndpoint), accountKey);
        }

        private RequestOptions GetRequestOptions(string accountId)
        {
            RequestOptions requestOptions = new RequestOptions()
            {
                PartitionKey = new PartitionKey(accountId)
            };
            return requestOptions;
        }

        private IQueryable<Document> GetDocumentQuery(string collectionId)
        {
            FeedOptions feedOptions = new FeedOptions()
            {
                EnableCrossPartitionQuery = true
            };
            return _cosmos.CreateDocumentQuery("dbs/" + _databaseId + "/colls/" + collectionId, feedOptions);
        }

        private JObject GetResponseResult(string responseData, bool systemProperties)
        {
            JObject result = null;
            if (!string.IsNullOrEmpty(responseData))
            {
                result = JObject.Parse(responseData);
                if (!systemProperties)
                {
                    JProperty[] properties = result.Properties().ToArray();
                    for (int i = properties.Length - 1; i >= 0; i--)
                    {
                        JProperty property = properties[i];
                        if (property.Name.StartsWith("_"))
                        {
                            result.Remove(property.Name);
                        }
                    }
                    result.Remove("id");
                }
            }
            return result;
        }

        public JObject[] GetDocuments(string collectionId)
        {
            List<JObject> documents = new List<JObject>();
            IQueryable<Document> query = GetDocumentQuery(collectionId);
            IEnumerable<Document> docs = query.AsEnumerable<Document>();
            foreach (Document doc in docs)
            {
                JObject result = GetResponseResult(doc.ToString(), true);
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
                result = GetResponseResult(document.ToString(), false);
            }
            return result;
        }

        public string UpsertDocument(string collectionId, JObject jsonDoc, string accountId, string accountDomain, string accountUrl, string clientId, string clientKey, string assetId)
        {
            jsonDoc["accountId"] = accountId;
            jsonDoc["accountDomain"] = accountDomain;
            jsonDoc["accountUrl"] = accountUrl;
            jsonDoc["clientId"] = clientId;
            jsonDoc["clientKey"] = clientKey;
            jsonDoc["assetId"] = assetId;
            Uri collectionUri = UriFactory.CreateDocumentCollectionUri(_databaseId, collectionId);
            RequestOptions requestOptions = collectionId == Constant.Database.Collection.ContentInsight ? GetRequestOptions(accountId) : null;
            Task<ResourceResponse<Document>> createTask = _cosmos.UpsertDocumentAsync(collectionUri, jsonDoc, requestOptions);
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
                RequestOptions requestOptions = collectionId == Constant.Database.Collection.ContentInsight ? GetRequestOptions(accountId) : null;
                Task<ResourceResponse<Document>> deleteTask = _cosmos.DeleteDocumentAsync(documentUri, requestOptions);
                deleteTask.Wait();
            }
        }

        public JObject ExecuteProcedure(string collectionId, string procedureId, params dynamic[] procedureParameters)
        {
            JObject result = null;
            Uri collectionUri = UriFactory.CreateDocumentCollectionUri(_databaseId, collectionId);
            Uri procedureUri = UriFactory.CreateStoredProcedureUri(_databaseId, collectionId, procedureId);
            Task<StoredProcedureResponse<JValue>> procedureTask = _cosmos.ExecuteStoredProcedureAsync<JValue>(procedureUri, procedureParameters);
            procedureTask.Wait();
            JValue procesureResponse = procedureTask.Result.Response;
            if (procesureResponse != null)
            {
                result = GetResponseResult(procesureResponse.ToString(), false);
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
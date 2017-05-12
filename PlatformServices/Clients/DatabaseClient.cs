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
        private string _databaseId;
        private DocumentClient _database;

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
            _databaseId = accountCredentials[2];
            _database = new DocumentClient(new Uri(accountEndpoint), accountKey);
        }

        private JObject GetResult(string responseData, bool systemProperties)
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
            IQueryable<Document> query = _database.CreateDocumentQuery("dbs/" + _databaseId + "/colls/" + collectionId);
            IEnumerable<Document> docs = query.AsEnumerable<Document>();
            foreach (Document doc in docs)
            {
                JObject result = GetResult(doc.ToString(), true);
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
            IQueryable<Document> query = _database.CreateDocumentQuery("dbs/" + _databaseId + "/colls/" + collectionId)
                .Where(d => d.Id == documentId);
            IEnumerable<Document> documents = query.AsEnumerable<Document>();
            Document document = documents.FirstOrDefault();
            if (document != null)
            {
                result = GetResult(document.ToString(), false);
            }
            return result;
        }

        public string CreateDocument(string collectionId, string docData, IDictionary<string, string> docAttributes)
        {
            JObject jsonDoc = JObject.Parse(docData);
            foreach (KeyValuePair<string, string> docAttribute in docAttributes)
            {
                jsonDoc[docAttribute.Key] = docAttribute.Value;
            }
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
            JObject result = null;
            Uri collectionUri = UriFactory.CreateDocumentCollectionUri(_databaseId, collectionId);
            Uri procedureUri = UriFactory.CreateStoredProcedureUri(_databaseId, collectionId, procedureId);
            Task<StoredProcedureResponse<JValue>> procedureTask = _database.ExecuteStoredProcedureAsync<JValue>(procedureUri, procedureParameters);
            procedureTask.Wait();
            JValue procesureResponse = procedureTask.Result.Response;
            if (procesureResponse != null)
            {
                result = GetResult(procesureResponse.ToString(), false);
            }
            return result;
        }
    }
}

using Microsoft.WindowsAzure.MediaServices.Client;

using Newtonsoft.Json.Linq;

namespace AzureSkyMedia.PlatformServices
{
    internal partial class MediaClient
    {
        public static void PurgePublishContent(TableClient tableClient)
        {
            string tableName = Constant.Storage.TableName.ContentPublish;
            MediaContentPublish[] contentPublishes = tableClient.GetEntities<MediaContentPublish>(tableName);
            foreach (MediaContentPublish contentPublish in contentPublishes)
            {
                string accountDomain = contentPublish.MediaAccountDomainName;
                string accountEndpoint = contentPublish.MediaAccountEndpointUrl;
                string clientId = contentPublish.MediaAccountClientId;
                string clientKey = contentPublish.MediaAccountClientKey;
                string jobId = contentPublish.RowKey;

                MediaClient mediaClient = new MediaClient(accountDomain, accountEndpoint, clientId, clientKey);
                IJob job = mediaClient.GetEntityById(MediaEntity.Job, jobId) as IJob;
                if (job == null)
                {
                    tableClient.DeleteEntity(tableName, contentPublish);

                    tableName = Constant.Storage.TableName.ContentProtection;
                    ContentProtection contentProtection = tableClient.GetEntity<ContentProtection>(tableName, contentPublish.PartitionKey, contentPublish.RowKey);
                    if (contentProtection != null)
                    {
                        tableClient.DeleteEntity(tableName, contentProtection);
                    }
                }
            }
        }

        public static void PurgePublishInsight(TableClient tableClient)
        {
            DocumentClient documentClient = new DocumentClient();
            string collectionId = Constant.Database.Collection.ContentInsight;
            JObject[] documents = documentClient.GetDocuments(collectionId);
            foreach (JObject document in documents)
            {
                if (document["accountId"] != null)
                {
                    string accountId = document["accountId"].ToString();
                    string accountDomain = document["accountDomain"].ToString();
                    string accountEndpoint = document["accountEndpoint"].ToString();
                    string clientId = document["clientId"].ToString();
                    string clientKey = document["clientKey"].ToString();
                    string assetId = document["assetId"].ToString();

                    MediaClient mediaClient = new MediaClient(accountDomain, accountEndpoint, clientId, clientKey);
                    IAsset asset = mediaClient.GetEntityById(MediaEntity.Asset, assetId) as IAsset;
                    if (asset == null)
                    {
                        string documentId = document["id"].ToString();
                        documentClient.DeleteDocument(collectionId, documentId);

                        string tableName = Constant.Storage.TableName.InsightPublish;
                        MediaInsightPublish insightPublish = tableClient.GetEntity<MediaInsightPublish>(tableName, accountId, documentId);
                        if (insightPublish != null)
                        {
                            tableClient.DeleteEntity(tableName, insightPublish);
                        }
                    }
                }
            }
        }
    }
}
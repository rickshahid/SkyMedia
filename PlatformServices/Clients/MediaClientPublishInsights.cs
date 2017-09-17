using Newtonsoft.Json.Linq;

using Microsoft.WindowsAzure.MediaServices.Client;

namespace AzureSkyMedia.PlatformServices
{
    internal partial class MediaClient
    {
        private static JToken GetExternalId(JObject index)
        {
            JToken externalId = null;
            if (index != null && index["breakdowns"] != null)
            {
                externalId = index["breakdowns"][0]["externalId"];
            }
            return externalId;
        }

        public static MediaPublish PublishInsights(string queueName)
        {
            MediaPublish mediaPublish = null;
            MessageClient messageClient = new MessageClient();
            MediaInsightsPublish insightsPublish = messageClient.GetMessage<MediaInsightsPublish>(queueName, out string messageId, out string popReceipt);
            if (insightsPublish != null)
            {
                string accountId = insightsPublish.PartitionKey;
                string indexerKey = insightsPublish.IndexerAccountKey;
                string indexId = insightsPublish.RowKey;

                IndexerClient indexerClient = new IndexerClient(null, accountId, indexerKey);
                JObject index = indexerClient.GetIndex(indexId, null, false);

                JToken externalId = GetExternalId(index);
                if (externalId != null)
                {
                    string accountDomain = insightsPublish.MediaAccountDomainName;
                    string accountUrl = insightsPublish.MediaAccountEndpointUrl;
                    string clientId = insightsPublish.MediaAccountClientId;
                    string clientKey = insightsPublish.MediaAccountClientKey;
                    string assetId = externalId.ToString();

                    CosmosClient cosmosClient = new CosmosClient(true);
                    string collectionId = Constant.Database.Collection.ContentInsight;
                    string documentId = cosmosClient.UpsertDocument(collectionId, index, accountId, accountDomain, accountUrl, clientId, clientKey, assetId);

                    mediaPublish = new MediaPublish
                    {
                        AssetId = assetId,
                        IndexId = indexId,
                        DocumentId = documentId,
                        UserId = insightsPublish.UserId,
                        MobileNumber = insightsPublish.MobileNumber,
                        StatusMessage = string.Empty
                    };
                }
                messageClient.DeleteMessage(queueName, messageId, popReceipt);
            }
            return mediaPublish;
        }

        public static void PurgePublishInsights(EntityClient entityClient)
        {
            CosmosClient cosmosClient = new CosmosClient(true);
            string collectionId = Constant.Database.Collection.ContentInsight;
            JObject[] documents = cosmosClient.GetDocuments(collectionId);
            foreach (JObject document in documents)
            {
                if (document["accountId"] != null)
                {
                    string accountId = document["accountId"].ToString();
                    string accountDomain = document["accountDomain"].ToString();
                    string accountUrl = document["accountUrl"].ToString();
                    string clientId = document["clientId"].ToString();
                    string clientKey = document["clientKey"].ToString();
                    string assetId = document["assetId"].ToString();

                    MediaClient mediaClient = new MediaClient(accountDomain, accountUrl, clientId, clientKey);
                    IAsset asset = mediaClient.GetEntityById(MediaEntity.Asset, assetId) as IAsset;
                    if (asset == null)
                    {
                        string documentId = document["id"].ToString();
                        cosmosClient.DeleteDocument(collectionId, documentId);

                        string tableName = Constant.Storage.TableName.InsightPublish;
                        MediaInsightsPublish insightsPublish = entityClient.GetEntity<MediaInsightsPublish>(tableName, accountId, documentId);
                        if (insightsPublish != null)
                        {
                            entityClient.DeleteEntity(tableName, insightsPublish);
                        }
                    }
                }
            }
        }
    }
}
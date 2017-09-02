using Newtonsoft.Json.Linq;

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
            MessageClient messageClient = new MessageClient();
            MediaInsightsPublish insightsPublish = messageClient.GetMessage<MediaInsightsPublish>(queueName, out string messageId, out string popReceipt);

            MediaPublish mediaPublish = null;
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
                    string accountKey = insightsPublish.MediaAccountKey;
                    string assetId = externalId.ToString();

                    CosmosClient cosmosClient = new CosmosClient(true);
                    string collectionId = Constant.Database.Collection.ContentInsight;
                    string documentId = cosmosClient.UpsertDocument(collectionId, index, accountId, accountKey, assetId);

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
    }
}
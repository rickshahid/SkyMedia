using Microsoft.WindowsAzure.MediaServices.Client;

using Newtonsoft.Json.Linq;

namespace AzureSkyMedia.PlatformServices
{
    internal partial class MediaClient
    {
        public static void PurgePublishContent(TableClient tableClient)
        {
            string tableName = Constant.Storage.Table.ContentPublish;
            MediaPublish[] contentPublishes = tableClient.GetEntities<MediaPublish>(tableName);
            foreach (MediaPublish contentPublish in contentPublishes)
            {
                string jobId = contentPublish.RowKey;
                MediaClient mediaClient = new MediaClient(contentPublish.MediaAccount);
                IJob job = mediaClient.GetEntityById(MediaEntity.Job, jobId) as IJob;
                if (job == null)
                {
                    tableClient.DeleteEntity(tableName, contentPublish);
                    DeleteContentProtections(tableClient, contentPublish.RowKey);
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
                    MediaAccount mediaAccount = new MediaAccount()
                    {
                        Id = document["accountId"].ToString(),
                        DomainName = document["accountDomain"].ToString(),
                        EndpointUrl = document["accountEndpoint"].ToString(),
                        ClientId = document["clientId"].ToString(),
                        ClientKey = document["clientKey"].ToString()
                    };
                    string assetId = document["assetId"].ToString();

                    MediaClient mediaClient = new MediaClient(mediaAccount);
                    IAsset asset = mediaClient.GetEntityById(MediaEntity.Asset, assetId) as IAsset;
                    if (asset == null)
                    {
                        string documentId = document["id"].ToString();
                        documentClient.DeleteDocument(collectionId, documentId);

                        string tableName = Constant.Storage.Table.InsightPublish;
                        MediaPublish insightPublish = tableClient.GetEntity<MediaPublish>(tableName, mediaAccount.Id, documentId);
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
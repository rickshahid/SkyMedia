using System;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.MediaServices.Client;

using Newtonsoft.Json.Linq;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.FunctionApp
{
    public static class PurgePublish
    {
        [FunctionName("PurgePublish")]
        public static void Run([TimerTrigger("0 0 12 * * *")]TimerInfo timer, TraceWriter log)
        {
            TimeSpan expirationDays = new TimeSpan(30, 0, 0, 0);

            EntityClient entityClient = new EntityClient();
            string tableName = Constant.Storage.TableName.ContentProtection;
            entityClient.PurgeEntities<ContentProtection>(tableName, expirationDays);
            tableName = Constant.Storage.TableName.ContentPublish;
            entityClient.PurgeEntities<MediaContentPublish>(tableName, expirationDays);
            tableName = Constant.Storage.TableName.InsightPublish;
            entityClient.PurgeEntities<MediaInsightsPublish>(tableName, expirationDays);

            CosmosClient cosmosClient = new CosmosClient(true);
            string collectionId = Constant.Database.Collection.ContentInsight;
            JObject[] documents = cosmosClient.GetDocuments(collectionId);
            foreach (JObject document in documents)
            {
                if (document["accountId"] != null)
                {
                    string accountId = document["accountId"].ToString();
                    string accountKey = document["accountKey"].ToString();
                    string assetId = document["assetId"].ToString();
                    MediaClient mediaClient = new MediaClient(accountId, accountKey);
                    IAsset asset = mediaClient.GetEntityById(MediaEntity.Asset, assetId) as IAsset;
                    if (asset == null)
                    {
                        string documentId = document["id"].ToString();
                        cosmosClient.DeleteDocument(collectionId, documentId);
                    }
                }
            }
        }
    }
}
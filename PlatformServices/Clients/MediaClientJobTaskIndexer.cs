using Newtonsoft.Json.Linq;

using Microsoft.WindowsAzure.MediaServices.Client;

namespace AzureSkyMedia.PlatformServices
{
    public partial class MediaClient
    {
        public static string GetIndexId(string alternateId)
        {
            string indexId = string.Empty;
            string[] idInfo = alternateId.Split(Constant.TextDelimiter.Identifier);
            if (idInfo.Length > 2)
            {
                indexId = idInfo[2];
            }
            return indexId;
        }

        public static void UploadVideo(string authToken, string indexerKey, IndexerClient indexerClient, IAsset asset, string locatorUrl, MediaJobTask jobTask)
        {
            string settingKey = Constant.AppSettingKey.MediaNotificationIndexerCallbackUrl;
            string callbackUrl = AppSetting.GetValue(settingKey);

            MediaPrivacy videoPrivacy = jobTask.ProcessorConfigBoolean[MediaProcessorConfig.PublicVideo.ToString()] ? MediaPrivacy.Public : MediaPrivacy.Private;
            string transcriptLanguage = jobTask.ProcessorConfigString[MediaProcessorConfig.TranscriptLanguage.ToString()];
            string searchPartition = jobTask.ProcessorConfigString[MediaProcessorConfig.SearchPartition.ToString()];
            string indexId = indexerClient.UploadVideo(asset.Name, videoPrivacy, transcriptLanguage, searchPartition, asset.Id, locatorUrl, callbackUrl);

            MediaIndexPublish indexPublish = new MediaIndexPublish();
            indexPublish.PartitionKey = Constant.Storage.TableProperty.PartitionKey;
            indexPublish.RowKey = indexId;
            indexPublish.IndexerAccountKey = indexerKey;
            indexPublish.MediaAccountName = AuthToken.GetClaimValue(authToken, Constant.UserAttribute.MediaAccountName);
            indexPublish.MediaAccountKey = AuthToken.GetClaimValue(authToken, Constant.UserAttribute.MediaAccountKey);

            EntityClient entityClient = new EntityClient();
            string tableName = Constant.Storage.TableName.IndexPublish;
            entityClient.InsertEntity(tableName, indexPublish);
        }

        public static string PublishIndex(string indexId)
        {
            EntityClient entityClient = new EntityClient();
            string tableName = Constant.Storage.TableName.IndexPublish;
            string partitionKey = Constant.Storage.TableProperty.PartitionKey;
            MediaIndexPublish indexPublish = entityClient.GetEntity<MediaIndexPublish>(tableName, partitionKey, indexId);

            string assetId = string.Empty;
            if (indexPublish != null)
            {
                IndexerClient indexerClient = new IndexerClient(indexPublish.IndexerAccountKey);
                JObject index = indexerClient.GetIndex(indexId, null, false);
                if (index != null)
                {
                    index.Remove("id");
                    index["indexId"] = indexId;
                    CosmosClient cosmosClient = new CosmosClient(true);
                    string collectionId = Constant.Database.Collection.Metadata;
                    string documentId = cosmosClient.CreateDocument(collectionId, index);
                    string alternateId = string.Concat(MediaProcessor.VideoIndexer.ToString(), Constant.TextDelimiter.Identifier, documentId);

                    JToken externalId = index["breakdowns"][0]["externalId"];
                    if (externalId != null)
                    {
                        assetId = externalId.ToString();
                        MediaClient mediaClient = new MediaClient(indexPublish.MediaAccountName, indexPublish.MediaAccountKey);
                        IAsset asset = mediaClient.GetEntityById(MediaEntity.Asset, assetId) as IAsset;
                        if (asset != null)
                        {
                            asset.AlternateId = string.Concat(alternateId, Constant.TextDelimiter.Identifier, indexId);
                            asset.Update();
                        }
                    }
                }
                entityClient.DeleteEntity(tableName, indexPublish);
            }
            return assetId;
        }
    }
}

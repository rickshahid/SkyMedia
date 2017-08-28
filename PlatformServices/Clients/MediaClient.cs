using Microsoft.WindowsAzure.MediaServices.Client;

namespace AzureSkyMedia.PlatformServices
{
    internal partial class MediaClient
    {
        private CloudMediaContext _media;

        public MediaClient(string authToken)
        {
            string attributeName = Constant.UserAttribute.MediaAccountName;
            string accountName = AuthToken.GetClaimValue(authToken, attributeName);

            attributeName = Constant.UserAttribute.MediaAccountKey;
            string accountKey = AuthToken.GetClaimValue(authToken, attributeName);

            BindContext(accountName, accountKey);
        }

        public MediaClient(string accountName, string accountKey)
        {
            BindContext(accountName, accountKey);
        }

        private void BindContext(string accountName, string accountKey)
        {
            MediaServicesCredentials credentials = new MediaServicesCredentials(accountName, accountKey);
            _media = new CloudMediaContext(credentials);
            IStorageAccount storageAccount = this.DefaultStorageAccount;
        }

        public IStorageAccount DefaultStorageAccount
        {
            get { return _media.DefaultStorageAccount; }
        }

        public static void UploadVideo(string authToken, IndexerClient indexerClient, IAsset asset, string locatorUrl, MediaJobTask jobTask)
        {
            bool publicVideo = jobTask.ProcessorConfigBoolean[MediaProcessorConfig.PublicVideo.ToString()];
            string transcriptLanguage = jobTask.ProcessorConfigString[MediaProcessorConfig.TranscriptLanguage.ToString()];
            string searchPartition = jobTask.ProcessorConfigString[MediaProcessorConfig.SearchPartition.ToString()];
            string indexId = indexerClient.UploadVideo(asset.Name, publicVideo, transcriptLanguage, searchPartition, asset.Id, locatorUrl);

            MediaInsightsPublish insightsPublish = new MediaInsightsPublish
            {
                PartitionKey = AuthToken.GetClaimValue(authToken, Constant.UserAttribute.MediaAccountName),
                RowKey = indexId,
                MediaAccountKey = AuthToken.GetClaimValue(authToken, Constant.UserAttribute.MediaAccountKey),
                IndexerAccountKey = AuthToken.GetClaimValue(authToken, Constant.UserAttribute.VideoIndexerKey),
            };

            EntityClient entityClient = new EntityClient();
            string tableName = Constant.Storage.TableName.InsightPublish;
            entityClient.InsertEntity(tableName, insightsPublish);
        }
    }
}
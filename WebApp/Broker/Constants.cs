using Microsoft.WindowsAzure.MediaServices.Client.DynamicEncryption;

namespace SkyMedia.ServiceBroker
{
    public struct Constants
    {
        public const char MultiItemSeparator = ',';
        public const char MultiItemsSeparator = ';';

        public const string FormatNumber = "N2";
        public const string FormatTime = "hh':'mm':'ss";

        public struct AppSettings
        {
            public const string AppTitle = "App.Title";
            public const string AppDomain = "App.Domain";
            public const string AppRegion = "App.Region";

            public const string AppLocalData = "App.LocalData";
            public const string AppSourceCode = "App.SourceCode";

            public const string AppApiTitle = "App.Api.Title";
            public const string AppApiDescription = "App.Api.Description";
            public const string AppApiVersion = "App.Api.Version";

            public const string AppInsightsInstrumentationKey = "App.Insights.InstrumentationKey";

            public const string DirectoryDiscoveryUrl = "Directory.DiscoveryUrl";
            public const string DirectoryIssuerUrl = "Directory.IssuerUrl";

            public const string DirectoryClientId = "Directory.ClientId";
            public const string DirectoryClientSecret = "Directory.ClientSecret";

            public const string DirectoryPolicyIdSignUpIn = "Directory.PolicyId.SignUpIn";
            public const string DirectoryPolicyIdProfileEdit = "Directory.PolicyId.ProfileEdit";

            public const string StorageServerTimeoutSeconds = "Storage.ServerTimeoutSeconds";
            public const string StorageMaxExecutionTimeSeconds = "Storage.MaxExecutionTimeSeconds";
            public const string StorageMaxSingleBlobUploadBytes = "Storage.MaxSingleBlobUploadBytes";
            public const string StorageParallelOperationThreadCount = "Storage.ParallelOperationThreadCount";

            public const string StorageBlockChunkSize = "Storage.BlockChunkSize";
            public const string StorageMaxFileSize = "Storage.MaxFileSize";
            public const string StorageMaxRetryCount = "Storage.MaxRetryCount";

            public const string StorageCdnUrl = "Storage.CdnUrl";

            public const string NoSqlDatabaseId = "NoSql.DatabaseId";
            public const string NoSqlDocumentProperties = "NoSql.DocumentProperties";

            public const string MediaConcurrentTransferCount = "Media.ConcurrentTransferCount";
            public const string MediaParallelTransferThreadCount = "Media.ParallelTransferThreadCount";

            public const string MediaStream1Name = "Media.Stream1.Name";
            public const string MediaStream1SourceUrl = "Media.Stream1.SourceUrl";
            public const string MediaStream1TextTracks = "Media.Stream1.TextTracks";
            public const string MediaStream1ProtectionTypes = "Media.Stream1.ProtectionTypes";

            public const string MediaStream2Name = "Media.Stream2.Name";
            public const string MediaStream2SourceUrl = "Media.Stream2.SourceUrl";
            public const string MediaStream2TextTracks = "Media.Stream2.TextTracks";
            public const string MediaStream2ProtectionTypes = "Media.Stream2.ProtectionTypes";

            public const string MediaStream3Name = "Media.Stream3.Name";
            public const string MediaStream3SourceUrl = "Media.Stream3.SourceUrl";
            public const string MediaStream3TextTracks = "Media.Stream3.TextTracks";
            public const string MediaStream3ProtectionTypes = "Media.Stream3.ProtectionTypes";

            public const string MediaStream4Name = "Media.Stream4.Name";
            public const string MediaStream4SourceUrl = "Media.Stream4.SourceUrl";
            public const string MediaStream4TextTracks = "Media.Stream4.TextTracks";
            public const string MediaStream4ProtectionTypes = "Media.Stream4.ProtectionTypes";

            public const string MediaStream5Name = "Media.Stream5.Name";
            public const string MediaStream5SourceUrl = "Media.Stream5.SourceUrl";
            public const string MediaStream5TextTracks = "Media.Stream5.TextTracks";
            public const string MediaStream5ProtectionTypes = "Media.Stream5.ProtectionTypes";

            public const string MediaProcessorEncoderStandardId = "Media.Processor.EncoderStandardId";
            public const string MediaProcessorEncoderStandardDocumentId = "Media.Processor.EncoderStandardDocumentId";
            public const string MediaProcessorEncoderPremiumId = "Media.Processor.EncoderPremiumId";

            public const string MediaProcessorIndexerV1Id = "Media.Processor.IndexerV1Id";
            public const string MediaProcessorIndexerV1DocumentId = "Media.Processor.IndexerV1DocumentId";
            public const string MediaProcessorIndexerV2Id = "Media.Processor.IndexerV2Id";
            public const string MediaProcessorIndexerV2DocumentId = "Media.Processor.IndexerV2DocumentId";

            public const string MediaProcessorFaceDetectionId = "Media.Processor.FaceDetectionId";
            public const string MediaProcessorFaceDetectionDocumentId = "Media.Processor.FaceDetectionDocumentId";
            public const string MediaProcessorFaceRedactionId = "Media.Processor.FaceRedactionId";
            public const string MediaProcessorFaceRedactionDocumentId = "Media.Processor.FaceRedactionDocumentId";

            public const string MediaProcessorMotionDetectionId = "Media.Processor.MotionDetectionId";
            public const string MediaProcessorMotionDetectionDocumentId = "Media.Processor.MotionDetectionDocumentId";
            public const string MediaProcessorMotionHyperlapseId = "Media.Processor.MotionHyperlapseId";
            public const string MediaProcessorMotionHyperlapseDocumentId = "Media.Processor.MotionHyperlapseDocumentId";

            public const string MediaProcessorVideoSummarizationId = "Media.Processor.VideoSummarizationId";
            public const string MediaProcessorVideoSummarizationDocumentId = "Media.Processor.VideoSummarizationDocumentId";
            public const string MediaProcessorCharacterRecognitionId = "Media.Processor.CharacterRecognitionId";
            public const string MediaProcessorCharacterRecognitionDocumentId = "Media.Processor.CharacterRecognitionDocumentId";

            public const string MediaChannelProgramArchiveMinutes = "Media.Channel.ProgramArchiveMinutes";
            public const string MediaChannelAdvertisementSeconds = "Media.Channel.AdvertisementSeconds";

            public const string MediaLocatorReadDurationDays = "Media.Locator.ReadDurationDays";
            public const string MediaLocatorWriteDurationHours = "Media.Locator.WriteDurationHours";
            public const string MediaLocatorAutoExtend = "Media.Locator.AutoExtend";

            public const string MediaPlayerVersion = "Media.Player.Version";

            public const string MediaLiveAccount = "Media.Live.Account";
            public const string MediaLiveChannelName = "Media.Live.ChannelName";
            public const string MediaLiveStartDateTime = "Media.Live.StartDateTime";

            public const string SigniantTransferApi = "Signiant.TransferApi";

            public const string AsperaTransferApi = "Aspera.TransferApi";
            public const string AsperaTransferInfo = "Aspera.TransferInfo";
            public const string AsperaUploadSetup = "Aspera.UploadSetup";
            public const string AsperaDownloadSetup = "Aspera.DownloadSetup";

            public const string TwilioMessageApi = "Twilio.MessageApi";
            public const string TwilioMessageFrom = "Twilio.MessageFrom";
        }

        public struct ConnectionStrings
        {
            public const string AccountNamePrefix = "AccountName=";
            public const string AccountKeyPrefix = "AccountKey=";

            public const string AzureStorage = "Storage";
            public const string AzureMedia = "Media";

            public const string AzureNoSqlReadOnly = "NoSql.ReadOnly";
            public const string AzureNoSqlReadWrite = "NoSql.ReadWrite";

            public const string AzureSearchReadOnly = "Search.ReadOnly";
            public const string AzureSearchReadWrite = "Search.ReadWrite";

            public const string Twilio = "Twilio";
        }

        public struct HttpHeaders
        {
            public const string ApiVersion = "x-ms-version";
            public const string AuthHeader = "Authorization";
            public const string AuthPrefix = "Bearer=";
        }

        public struct HttpForm
        {
            public const string IdToken = "id_token";
        }

        public struct HttpCookies
        {
            public const string UserAuthToken = "SkyMedia.UserAuthToken";
        }

        public struct ContentType
        {
            public const string Json = "application/json";
            public const string Url = "application/x-www-form-urlencoded";
        }

        public struct UserAttribute
        {
            public const string UserId = "emails";
            public const string MobileNumber = "extension_MobileNumber";
            public const string MediaAccountName = "extension_MediaAccountName";
            public const string MediaAccountKey = "extension_MediaAccountKey";
            public const string StorageAccountName = "extension_StorageAccountName";
            public const string StorageAccountKey = "extension_StorageAccountKey";
            public const string SigniantServiceGateway = "extension_SigniantServiceGateway";
            public const string SigniantAccountKey = "extension_SigniantAccountKey";
            public const string AsperaServiceGateway = "extension_AsperaServiceGateway";
            public const string AsperaAccountId = "extension_AsperaAccountId";
            public const string AsperaAccountKey = "extension_AsperaAccountKey";
        }

        public struct Storage
        {
            public const string SigniantContainer = " \"account-name\": \"{0}\", \"access-key\": \"{1}\", \"container\": \"{2}\" ";
            public const string AsperaContainer = "azu://{0}:{1}@blob.core.windows.net";
            public const string AsperaWorker = "-worker";

            public struct Account
            {
                public const string Connection = "DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1}";
                public const string DefaultSuffix = " - Default Account";
            }

            public struct Analytics
            {
                public const string NotAvailable = "N/A";
            }

            public struct Blob
            {
                public const int MaxCopyMinutes = 120;
            }

            public struct ContainerNames
            {
                public const string Upload = "upload";
            }

            public struct TableNames
            {
                public const string AssetProtection = "AssetProtection";
                public const string AssetPublish = "AssetPublish";
                public const string FileUpload = "FileUpload";
            }

            public struct TableProperties
            {
                public const string PartitionKey = "PartitionKey";
                public const string OpenConcurrency = "*";
            }

            public struct QueueName
            {
                public const string JobStatus = "job-status";
            }
        }

        public struct Media
        {
            public const string TrackSubtitles = "subtitles";
            public const string AddressesAll = "All Addresses";

            public struct ProcessorConfig
            {
                public const string IndexerV1XPath = "/configuration/features/feature/settings/add";
            }

            public struct ContentProtection
            {
                public const int EncryptionKeyByteCount = 16;

                public const string ContentKeyNameAES = "AES Key";
                public const string ContentKeyNameASK = "ASK Key";
                public const string ContentKeyNameDRMPlayReady = "DRM (PlayReady) Key";
                public const string ContentKeyNameDRMWidevine = "DRM (Widevine) Key";
                public const string ContentKeyNameDRMPlayReadyWidevine = "DRM (PlayReady & Widevine) Key";
                public const string ContentKeyNameDRMFairPlay = "DRM (FairPlay) Key";

                public const string AuthPolicyName = " Auth Policy";
                public const string AuthPolicyOpenRestrictionName = " Open Restriction";
                public const string AuthPolicyTokenRestrictionName = " Web Token Restriction";
                public const string AuthPolicyAddressRestrictionName = " IP Address Restriction";
                public const string AuthPolicyOptionNameAES = " Option";
                public const string AuthPolicyOptionNameDRMPlayReady = " Option PlayReady";
                public const string AuthPolicyOptionNameDRMWidevine = " Option Widevine";
                public const string AuthPolicyOptionNameDRMFairPlay = " Option FairPlay";

                public const string AuthAddressRangeXML = "<Allowed addressType=\"IPv4\"><AddressRange start=\"{0}\" end=\"{1}\" /></Allowed>";
            }

            public struct DeliveryProtocol
            {
                public const AssetDeliveryProtocol AES = AssetDeliveryProtocol.Dash | AssetDeliveryProtocol.HLS |
                    AssetDeliveryProtocol.SmoothStreaming;

                public const AssetDeliveryProtocol DRMPlayReady = AssetDeliveryProtocol.Dash | AssetDeliveryProtocol.HLS |
                    AssetDeliveryProtocol.SmoothStreaming;

                public const AssetDeliveryProtocol DRMWidevine = AssetDeliveryProtocol.Dash;

                public const AssetDeliveryProtocol DRMFairPlay = AssetDeliveryProtocol.HLS;
            }

            public struct DeliveryPolicy
            {
                public const string DecryptionStorage = "Dynamic Storage Decryption";
                public const string EncryptionAES = "Dynamic AES Encryption";
                public const string EncryptionDRMPlayReady = "Dynamic DRM (PlayReady) Encryption";
                public const string EncryptionDRMWidevine = "Dynamic DRM (Widevine) Encryption";
                public const string EncryptionDRMPlayReadyWidevine = "Dynamic DRM (PlayReady & Widevine) Encryption";
                public const string EncryptionDRMFairPlay = "Dynamic DRM (FairPlay) Encryption";
            }

            public struct AccessPolicy
            {
                public const string ReadPolicyName = "Default Read Access Policy";
                public const string WritePolicyName = "Default Write Access Policy";
            }

            public struct Job
            {
                public const string PremiumWorkflowSuffix = ".workflow";
                public const string NotificationEndpointName = "Job Notification Queue";
            }

            public struct Streaming
            {
                public const string DefaultEndpointName = "default";
            }

            public struct TreeIcon
            {
                public const string MediaAsset = "/MediaAsset.png";
                public const string MediaFile = "/MediaFile.png";
            }

            public struct AssetManifest
            {
                public const string FileExtension = ".ism";
                public const string LocatorSuffix = "/manifest";
            }

            public struct AssetMetadata
            {
                public const string WebVttExtension = ".vtt";
                public const string DocumentCollection = "Metadata";
            }
        }
    }
}

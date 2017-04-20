using Microsoft.WindowsAzure.MediaServices.Client.DynamicEncryption;

namespace AzureSkyMedia.PlatformServices
{
    public struct Constant
    {
        public const string NotAvailable = "N/A";

        public struct TextDelimiter
        {
            public const char Connection = ';';
            public const char Identifier = '_';
            public const char Application = ',';
        }

        public struct TextFormatter
        {
            public const string Numeric = "N2";
            public const string ClockTime = "hh':'mm':'ss";

            public const string SpacePattern = @"\B[A-Z]";
            public const string SpaceReplacement = " $0";
        }
        
        public struct AppSettingKey
        {
            public const string AppRegion = "App.Region";

            public const string AppApiTitle = "App.Api.Title";
            public const string AppApiVersion = "App.Api.Version";
            public const string AppApiDescription = "App.Api.Description";
            public const string AppApiEndpointUrl = "App.Api.EndpointUrl";

            public const string AppInsightsInstrumentationKey = "App.Insights.InstrumentationKey";

            public const string DirectoryDiscoveryUrl = "Directory.DiscoveryUrl";
            public const string DirectoryIssuerUrl = "Directory.IssuerUrl";

            public const string DirectoryClientId = "Directory.ClientId";
            public const string DirectoryClientSecret = "Directory.ClientSecret";

            public const string DirectoryPolicyIdSignUpIn = "Directory.PolicyId.SignUpIn";
            public const string DirectoryPolicyIdProfileEdit = "Directory.PolicyId.ProfileEdit";
            public const string DirectoryPolicyIdPasswordReset = "Directory.PolicyId.PasswordReset";

            public const string StorageCdnUrl = "Storage.CdnUrl";

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

            public const string MediaProcessorThumbnailGenerationDocumentId = "Media.Processor.ThumbnailGenerationDocumentId";
            public const string MediaProcessorIndexerDocumentId = "Media.Processor.IndexerDocumentId";

            public const string MediaProcessorFaceDetectionDocumentId = "Media.Processor.FaceDetectionDocumentId";
            public const string MediaProcessorFaceRedactionDocumentId = "Media.Processor.FaceRedactionDocumentId";

            public const string MediaProcessorMotionDetectionDocumentId = "Media.Processor.MotionDetectionDocumentId";
            public const string MediaProcessorMotionHyperlapseDocumentId = "Media.Processor.MotionHyperlapseDocumentId";
            public const string MediaProcessorMotionStabilizationDocumentId = "Media.Processor.MotionStabilizationDocumentId";

            public const string MediaProcessorVideoAnnotationDocumentId = "Media.Processor.VideoSummarizationDocumentId";
            public const string MediaProcessorVideoSummarizationDocumentId = "Media.Processor.VideoSummarizationDocumentId";

            public const string MediaProcessorCharacterRecognitionDocumentId = "Media.Processor.CharacterRecognitionDocumentId";
            public const string MediaProcessorContentModerationDocumentId = "Media.Processor.ContentModerationDocumentId";

            public const string MediaJobNotificationStorageQueueName = "Media.Job.Notification.StorageQueueName";
            public const string MediaJobNotificationWebHookUrl = "Media.Job.Notification.WebHookUrl";

            public const string MediaChannelProgramArchiveMinutes = "Media.Channel.ProgramArchiveMinutes";
            public const string MediaChannelAdvertisementSeconds = "Media.Channel.AdvertisementSeconds";

            public const string MediaLocatorReadDurationDays = "Media.Locator.ReadDurationDays";
            public const string MediaLocatorMaxStreamCount = "Media.Locator.MaxStreamCount";

            public const string MediaPlayerVersion = "Media.Player.Version";

            public const string SigniantTransferApi = "Signiant.TransferApi";

            public const string AsperaTransferApi = "Aspera.TransferApi";
            public const string AsperaTransferInfo = "Aspera.TransferInfo";
            public const string AsperaUploadSetup = "Aspera.UploadSetup";
            public const string AsperaDownloadSetup = "Aspera.DownloadSetup";

            public const string AccountNamePrefix = "AccountName=";
            public const string AccountKeyPrefix = "AccountKey=";
            public const string DatabaseIdPrefix = "DatabaseId=";

            public const string AzureStorage = "Storage";
            public const string AzureNoSqlReadOnly = "NoSql.ReadOnly";
            public const string AzureNoSqlReadWrite = "NoSql.ReadWrite";

            public const string AzureMedia = "Media";
            public const string AzureCache = "Cache";
            public const string AzureSearch = "Search";
        }

        public struct HttpHeader
        {
            public const string AuthPrefix = "Bearer=";
            public const string AuthHeader = "Authorization";
            public const string ApiVersion = "x-ms-version";
        }

        public struct HttpForm
        {
            public const string IdToken = "id_token";
        }

        public struct HttpCookie
        {
            public const string UserAuthToken = " AzureSkyMedia.UserAuthToken";
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
            public struct Partner
            {
                public const string SigniantContainer = " \"account-name\": \"{0}\", \"access-key\": \"{1}\", \"container\": \"{2}\" ";
                public const string AsperaContainer = "azu://{0}:{1}@blob.core.windows.net";
                public const string AsperaWorker = "-worker";
            }

            public struct Account
            {
                public const string Connection = "DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1}";
            }

            public struct Blob
            {
                public const int WriteDurationHours = 1;

                public struct Container
                {
                    public const string Upload = "upload";
                }
            }

            public struct Queue
            {
                public const string PoisonSuffix = "-poison";
            }

            public struct TableName
            {
                public const string FileUpload = "FileUpload";
                public const string JobPublish = "JobPublish";
                public const string ContentProtection = "ContentProtection";
                public const string LiveEvent = "LiveEvent";
            }

            public struct TableProperty
            {
                public const string PartitionKey = "PartitionKey";
                public const string OpenConcurrency = "*";
            }
        }

        public struct Database
        {
            public const string DocumentProperties = "id,name,_rid,_self,_etag,_ts,_attachments";

            public struct Collection
            {
                public const string Encoding = "Encoding";
                public const string Metadata = "Metadata";
            }

            public struct Procedure
            {
                public const string EncoderConfig = "getEncoderConfig";
                public const string MetadataFragment = "getTimecodeFragment";
            }
        }

        public struct Media
        {
            public const int RenderedClipMode = 2;

            public struct ProcessorId
            {
                public const string EncoderStandard = "nb:mpid:UUID:ff4df607-d419-42f0-bc17-a481b1331e56";
                public const string EncoderPremium = "nb:mpid:UUID:77fea72a-107c-439e-b0bb-f88153b93461";
                public const string EncoderUltra = "nb:mpid:UUID:816a4fda-76dc-463b-866b-9aa2f65deeac";
                public const string Indexer = "nb:mpid:UUID:1927f26d-0aa5-4ca1-95a3-1a3f95b0f706";
                public const string FaceDetection = "nb:mpid:UUID:6a9b8239-81ea-4762-8125-66b4f45737a2";
                public const string FaceRedaction = "nb:mpid:UUID:3806d7a6-4985-4437-b098-50e3733310e8";
                public const string VideoAnnotation = "nb:mpid:UUID:4b8b1e57-3bf3-4a07-b21a-12c3cdcc0894";
                public const string VideoSummarization = "nb:mpid:UUID:d4d94427-b8e7-44b5-addb-5f3a26124385";
                public const string CharacterRecognition = "nb:mpid:UUID:074c3899-d9fb-448f-9ae1-4ebcbe633056";
                public const string ContentModeration = "nb:mpid:UUID:bb312589-3bd4-4f2e-af26-2df8a984b395";
                public const string MotionDetection = "nb:mpid:UUID:464c4ede-daad-4edd-9c3c-3b5f667eef08";
                public const string MotionHyperlapse = "nb:mpid:UUID:db657ff0-fc6e-407c-a03a-80fdca3b81cd";
                public const string MotionStabilization = "nb:mpid:UUID:73845a7d-8505-421d-9af9-d4bdc7838bdf";
            }

            public struct ProcessorConfig
            {
                public const string EncoderStandardDefaultPreset = "H264 Multiple Bitrate 720p Audio 5.1";
                public const string EncoderStandardThumbnailsPreset = "Thumbnails";
                public const string EncoderStandardThumbnailsFormat = "PngImage";
                public const string EncoderPremiumWorkflowExtension = ".workflow";
            }

            public struct ContentProtection
            {
                public const int EncryptionKeyByteCount = 16;

                public const string ContentKeyNameAes = "AES Key";
                public const string ContentKeyNameDrmPlayReady = "DRM (PlayReady) Key";
                public const string ContentKeyNameDrmWidevine = "DRM (Widevine) Key";
                public const string ContentKeyNameDrmPlayReadyWidevine = "DRM (PlayReady & Widevine) Key";

                public const string AuthPolicyName = " Auth Policy";
                public const string AuthPolicyOpenRestrictionName = " Open Restriction";
                public const string AuthPolicyTokenRestrictionName = " Web Token Restriction";
                public const string AuthPolicyAddressRestrictionName = " IP Address Restriction";
                public const string AuthPolicyOptionNameAes = " Option";
                public const string AuthPolicyOptionNameDrmPlayReady = " Option PlayReady";
                public const string AuthPolicyOptionNameDrmWidevine = " Option Widevine";

                public const string AuthAddressRangeXml = "<Allowed addressType=\"IPv4\"><AddressRange start=\"{0}\" end=\"{1}\" /></Allowed>";
            }

            public struct DeliveryProtocol
            {
                public const AssetDeliveryProtocol Aes = AssetDeliveryProtocol.Dash | AssetDeliveryProtocol.HLS |
                    AssetDeliveryProtocol.SmoothStreaming;

                public const AssetDeliveryProtocol DrmPlayReady = AssetDeliveryProtocol.Dash | AssetDeliveryProtocol.HLS |
                    AssetDeliveryProtocol.SmoothStreaming;

                public const AssetDeliveryProtocol DrmWidevine = AssetDeliveryProtocol.Dash;
            }

            public struct DeliveryPolicy
            {
                public const string DecryptionStorage = "Dynamic Storage Decryption";
                public const string EncryptionAes = "Dynamic AES Encryption";
                public const string EncryptionDrmPlayReady = "Dynamic DRM (PlayReady) Encryption";
                public const string EncryptionDrmWidevine = "Dynamic DRM (Widevine) Encryption";
                public const string EncryptionDrmPlayReadyWidevine = "Dynamic DRM (PlayReady & Widevine) Encryption";
            }

            public struct AccessPolicy
            {
                public const string ReadPolicyName = "Default Read Access Policy";
                public const string WritePolicyName = "Default Write Access Policy";
            }

            public struct JobNotification
            {
                public const string EndpointName = "Job Notification Web Hook";
            }

            public struct TreeIcon
            {
                public const string MediaAsset = "/MediaAsset.png";
                public const string MediaFile = "/MediaFile.png";
            }

            public struct FileExtension
            {
                public const string Manifest = ".ism";
                public const string WebVtt = ".vtt";
                public const string Json = ".json";
            }

            public struct Stream
            {
                public const string LocatorIdPrefix = "nb:lid:UUID:";
                public const string LocatorManifestSuffix = "/manifest";
                public const string AssetFilteredSuffix = " (Filtered)";
                public const string TextTrackSubtitles = "subtitles";
                public const string AddressRangeAll = "All IP Addresses";
            }
        }

        public struct Cache
        {
            public struct ItemKey
            {
                public const string MediaProcessors = "AzureMediaProcessors";
            }
        }

        public struct Message
        {
            public const string StreamingEndpointNotRunning = "Your media account does not have a streaming endpoint running!";
        }
    }
}

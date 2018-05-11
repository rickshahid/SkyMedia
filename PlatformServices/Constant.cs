using Microsoft.WindowsAzure.MediaServices.Client.DynamicEncryption;

namespace AzureSkyMedia.PlatformServices
{
    public struct Constant
    {
        public const string AppSettings = "appsettings.json";

        public struct DirectoryService
        {
            public const string B2B = "B2B";
            public const string B2C = "B2C";
        }

        public struct TextDelimiter
        {
            public const char Connection = ';';
            public const char Identifier = '_';
            public const char Application = ',';
            public const char File = '-';
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
            public const string AppRegionName = "App.RegionName";

            public const string AppApiTitle = "App.API.Title";
            public const string AppApiVersion = "App.API.Version";
            public const string AppApiEndpointUrl = "App.API.EndpointUrl";
            public const string AppApiDescription = "App.API.Description";

            public const string AppInsightsInstrumentationKey = "APPINSIGHTS_INSTRUMENTATIONKEY";

            public const string AzureResourceManagementValidateUrl = "Azure.ResourceManagement.ValidateUrl";
            public const string AzureResourceManagementAudienceUrl = "Azure.ResourceManagement.AudienceUrl";

            public const string DirectoryIssuerUrl = "Directory.IssuerUrl";
            public const string DirectoryDiscoveryUrl = "Directory.DiscoveryUrl";

            public const string DirectoryDefaultId = "Directory.DefaultId";
            public const string DirectoryAuthorityUrl = "Directory.{0}.AuthorityUrl";

            public const string DirectoryTenantId = "Directory.{0}.TenantId";
            public const string DirectoryTenantDomain = "Directory.{0}.TenantDomain";

            public const string DirectoryClientId = "Directory.{0}.ClientId";
            public const string DirectoryClientSecret = "Directory.{0}.ClientSecret";

            public const string DirectoryPolicyIdSignUpIn = "Directory.B2C.PolicyId.SignUpIn";
            public const string DirectoryPolicyIdProfileEdit = "Directory.B2C.PolicyId.ProfileEdit";
            public const string DirectoryPolicyIdPasswordReset = "Directory.B2C.PolicyId.PasswordReset";

            public const string StorageContentDeliveryEndpointUrl = "Storage.CDN.EndpointUrl";

            public const string DatabaseRegionsRead = "Database.Regions.Read";
            public const string DatabaseCollectionThroughputUnits = "Database.Collection.ThroughputUnits";

            public const string MediaStream1Name = "Media.Stream1.Name";
            public const string MediaStream1SourceUrl = "Media.Stream1.SourceUrl";
            public const string MediaStream1TextTracks = "Media.Stream1.TextTracks";

            public const string MediaStream2Name = "Media.Stream2.Name";
            public const string MediaStream2SourceUrl = "Media.Stream2.SourceUrl";
            public const string MediaStream2TextTracks = "Media.Stream2.TextTracks";

            public const string MediaStream3Name = "Media.Stream3.Name";
            public const string MediaStream3SourceUrl = "Media.Stream3.SourceUrl";
            public const string MediaStream3TextTracks = "Media.Stream3.TextTracks";

            public const string MediaStream4Name = "Media.Stream4.Name";
            public const string MediaStream4SourceUrl = "Media.Stream4.SourceUrl";
            public const string MediaStream4TextTracks = "Media.Stream4.TextTracks";

            public const string MediaChannelProgramArchiveMinutes = "Media.Channel.ProgramArchiveMinutes";
            public const string MediaChannelAdDurationSeconds = "Media.Channel.AdDurationSeconds";

            public const string MediaLocatorWriteDurationMinutes = "Media.Locator.WriteDurationMinutes";
            public const string MediaLocatorReadDurationDays = "Media.Locator.ReadDurationDays";
            public const string MediaLocatorTunerPageSize = "Media.Locator.TunerPageSize";
            public const string MediaLocatorAutoRenewal = "Media.Locator.AutoRenewal";

            public const string MediaPublishUrl = "Media.Publish.Url";
            public const string MediaPublishQueue = "Media.Publish.Queue";

            public const string MediaPlayerVersion = "Media.Player.Version";
            public const string MediaPlayerSkin = "Media.Player.Skin";

            public const string MediaClipperVersion = "Media.Clipper.Version";

            public const string MediaIndexerServiceUrl = "Media.Indexer.ServiceUrl";

            public const string AccountEndpointPrefix = "AccountEndpoint=";
            public const string AccountNamePrefix = "AccountName=";
            public const string AccountKeyPrefix = "AccountKey=";
            public const string DatabaseIdPrefix = "DatabaseId=";

            public const string AzureStorage = "Storage";
            public const string AzureDatabase = "Database";
            public const string AzureCache = "Cache";
        }

        public struct HttpHeader
        {
            public const string ApiManagementKey = "Ocp-Apim-Subscription-Key";
        }

        public struct HttpForm
        {
            public const string IdToken = "id_token";
        }

        public struct HttpCookie
        {
            public const string UserAuthToken = "UserAuthToken";
        }

        public struct HttpQueryString
        {
            public const string ActiveDirectory = "aad";
        }

        public struct UserAttribute
        {
            public const string EMail = "email";
            public const string EMails = "emails";
            public const string MobileNumber = "extension_MobileNumber";

            public const string MediaAccountName = "extension_MediaAccountName";
            public const string MediaAccountSubscriptionId = "extension_MediaAccountSubscriptionId";
            public const string MediaAccountResourceGroupName = "extension_MediaAccountResourceGroupName";
            public const string MediaAccountDirectoryTenantId = "extension_MediaAccountDirectoryTenantId";
            public const string MediaAccountClientApplicationId = "extension_MediaAccountClientApplicationId";
            public const string MediaAccountClientApplicationKey = "extension_MediaAccountClientApplicationKey";

            public const string StorageAccount1Name = "extension_StorageAccount1Name";
            public const string StorageAccount1Key = "extension_StorageAccount1Key";
            public const string StorageAccount2Name = "extension_StorageAccount2Name";
            public const string StorageAccount2Key = "extension_StorageAccount2Key";
            public const string StorageAccount3Name = "extension_StorageAccount3Name";
            public const string StorageAccount3Key = "extension_StorageAccount3Key";
        }

        public struct Storage
        {
            public const string NotAvailable = "N/A";
            public const string CapacityData = "data";

            public struct Account
            {
                public const string Connection = "DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1}";
            }

            public struct Blob
            {
                public struct Container
                {
                    public const string FileUpload = "upload";
                    public const string MediaProcess = "ams";
                    public const string ContentDelivery = "cdn";
                }
            }

            public struct Queue
            {
                public const string PoisonSuffix = "-poison";
            }
        }

        public struct Database
        {

            public struct Collection
            {
                public const string InputWorkflow = "InputWorkflow";
                public const string OutputInsight = "OutputInsight";
                public const string OutputPublish = "OutputPublish";
                public const string ProcessorPreset = "ProcessorPreset";
            }

            public struct Procedure
            {
                public const string ProcessorPreset = "getProcessorPreset";
                public const string TimecodeFragment = "getTimecodeFragment";
            }

            public struct Document
            {
                public const string SystemPropertyPrefix = "_";
                public const string DefaultIdSuffix = "_Default";
            }
        }

        public struct Media
        {
            public const string Models = @"\Models\";
            public const string Presets = @"ProcessorPreset\";

            public struct IdPrefix
            {
                public const string Asset = "nb:cid:";
                public const string Processor = "nb:mpid:";
            }

            public struct ProcessorId
            {
                public const string EncoderStandard = "nb:mpid:UUID:ff4df607-d419-42f0-bc17-a481b1331e56";
                public const string EncoderPremium = "nb:mpid:UUID:77fea72a-107c-439e-b0bb-f88153b93461";
                public const string VideoIndexer = "VideoIndexer";

                public const string AudioAnalyzer = "nb:mpid:UUID:1927f26d-0aa5-4ca1-95a3-1a3f95b0f706";
                public const string VideoAnalyzer = "VideoAnalyzer";
                //public const string VideoAnnotation = "nb:mpid:UUID:4b8b1e57-3bf3-4a07-b21a-12c3cdcc0894";
                public const string VideoSummarization = "nb:mpid:UUID:d4d94427-b8e7-44b5-addb-5f3a26124385";
                public const string FaceDetection = "nb:mpid:UUID:6a9b8239-81ea-4762-8125-66b4f45737a2";
                public const string FaceRedaction = "nb:mpid:UUID:3806d7a6-4985-4437-b098-50e3733310e8";
                public const string MotionDetection = "nb:mpid:UUID:464c4ede-daad-4edd-9c3c-3b5f667eef08";
                //public const string ContentModeration = "nb:mpid:UUID:bb312589-3bd4-4f2e-af26-2df8a984b395";
                //public const string CharacterRecognition = "nb:mpid:UUID:074c3899-d9fb-448f-9ae1-4ebcbe633056";
            }

            public struct ProcessorPreset
            {
                public const int ThumbnailJpgQuality = 90;

                public const string StreamingLadderPresetName = "H.264 Ladder Adaptive Streaming (Uninterleaved)";
                public const string StreamingLadderPresetValue = "Adaptive Streaming";

                public const string DownloadLadderPresetName = "H.264 Ladder Streaming & Download (Interleaved)";
                public const string DownloadLadderPresetValue = "Content Adaptive Multiple Bitrate MP4";
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

                public const AssetDeliveryProtocol DrmPlayReady = AssetDeliveryProtocol.Dash | AssetDeliveryProtocol.SmoothStreaming;

                public const AssetDeliveryProtocol DrmWidevine = AssetDeliveryProtocol.Dash;

                public const AssetDeliveryProtocol DrmFairPlay = AssetDeliveryProtocol.HLS;
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
                public const string ReadOnlyPolicyName = "Default Read Only Policy";
                public const string ReadWritePolicyName = "Default Read Write Policy";
            }

            public struct Job
            {
                public const string MultipleInputAssets = "Multiple Input Assets";
                public const string NotificationEndpointName = "Job Notification Web Hook";
            }

            public struct TreeIcon
            {
                public const string MediaAsset = "/MediaAsset.png";
                public const string MediaFile = "/MediaFile.png";
            }

            public struct FileExtension
            {
                public const string MP4 = ".mp4";
                public const string JobLog = ".log";
                public const string JobManifest = ".txt";
                public const string StreamManifest = ".ism";
                public const string PremiumWorkflow = ".workflow";

                public const string Json = ".json";
                public const string WebVtt = ".vtt";
                public const string Annotations = "_annotations.json";
            }

            public struct Live
            {
                public const string ChannelEncodingPreset = "Default720p";

                public const string ProgramSuffixClear = " Clear";
                public const string ProgramSuffixAes = " AES";
                public const string ProgramSuffixDrm = " DRM";

                public const string AllowAnyAddress = "Allow Any Address";
                public const string AllowAuthorizedAddress = "Authorized Address Only";
            }

            public struct Stream
            {
                public const string LocatorManifestSuffix = "/manifest";

                public struct TextTrack
                {
                    public const string Captions = "captions";
                    //public const string Subtitles = "subtitles";
                    //public const string Thumbnails = "thumbnails";
                    //public const string ThumbnailsData = "#xywh=";
                }
            }
        }

        public struct Message
        {
            public const string UserPasswordForgotten = "AADB2C90118";
            public const string StreamingEndpointNotStarted = "Your media account streaming endpoint has not been started.";
        }
    }
}
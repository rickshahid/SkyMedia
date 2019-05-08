using System.Text.RegularExpressions;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("AzureSkyMedia.WebApp")]
[assembly: InternalsVisibleTo("AzureSkyMedia.FunctionApp")]

namespace AzureSkyMedia.PlatformServices
{
    public struct Constant
    {
        public const string NotAvailable = "N/A";
        public const string AppSettingsFile = "appsettings.json";

        public struct TextDelimiter
        {
            public const char Connection = ';';
            public const char Application = ',';

            public const string TransformPresetName = ", ";
            public const string JobOutputLabel = "Preset";
            public const string AssetName = " - ";
        }

        public struct TextFormatter
        {
            public const string Numeric = "N2";
            public const string NumericLong = "N0";

            public static readonly string[] SpacingPatterns = new string[] { "([a-z])([A-Z])", "(H264)([A-Z])", "([a-z])(720p|1080p)" };
            public static readonly string[] SpacingInserts = new string[] { "$1 $2", "$1 $2", "$1 $2" };

            public static string FormatValue(string value)
            {
                for (int i = 0; i < SpacingPatterns.Length; i++)
                {
                    value = Regex.Replace(value, SpacingPatterns[i], SpacingInserts[i]);
                }
                return value;
            }
        }

        public struct AppSettingKey
        {
            public const string AppName = "App.Name";
            public const string AppRegionName = "App.RegionName";

            public const string AppApiVersion = "App.Api.Version";
            public const string AppApiEndpointUrl = "App.Api.EndpointUrl";
            public const string AppApiDescription = "App.Api.Description";

            public const string DirectoryTenantId = "Directory.TenantId";
            public const string DirectoryAuthorityUrl = "Directory.AuthorityUrl";
            public const string DirectoryDiscoveryPath = "Directory.DiscoveryPath";

            public const string DirectoryClientId = "Directory.ClientId";
            public const string DirectoryClientSecret = "Directory.ClientSecret";

            public const string DirectoryPolicyIdSignUpIn = "Directory.PolicyId.SignUpIn";
            public const string DirectoryPolicyIdProfileEdit = "Directory.PolicyId.ProfileEdit";
            public const string DirectoryPolicyIdPasswordReset = "Directory.PolicyId.PasswordReset";

            public const string DatabaseRegions = "Database.Regions";

            public const string StorageCdnEndpointUrl = "Storage.CdnEndpointUrl";
            public const string StorageSharedAccessMinutes = "Storage.SharedAccessMinutes";

            public const string MediaStreamTunerPageSize = "Media.Stream.TunerPageSize";

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

            public const string MediaStream5Name = "Media.Stream5.Name";
            public const string MediaStream5SourceUrl = "Media.Stream5.SourceUrl";
            public const string MediaStream5TextTracks = "Media.Stream5.TextTracks";

            public const string MediaStream6Name = "Media.Stream6.Name";
            public const string MediaStream6SourceUrl = "Media.Stream6.SourceUrl";
            public const string MediaStream6TextTracks = "Media.Stream6.TextTracks";

            public const string MediaPlayerVersion = "Media.Player.Version";
            public const string MediaPlayerSkin = "Media.Player.Skin";

            public const string EventGridJobOutputProgressUrl = "EventGrid.JobOutputProgressUrl";
            public const string EventGridJobStateFinalUrl = "EventGrid.JobStateFinalUrl";
            public const string EventGridLiveEventUrl = "EventGrid.LiveEventUrl";

            public const string AccountEndpointPrefix = "AccountEndpoint=";
            public const string AccountNamePrefix = "AccountName=";
            public const string AccountKeyPrefix = "AccountKey=";
            public const string DatabaseIdPrefix = "DatabaseId=";

            public const string AzureStorage = "Storage";

            public const string AzureDatabaseReadOnly = "Database.ReadOnly";
            public const string AzureDatabaseReadWrite = "Database.ReadWrite";

            public const string AzureVideoIndexerApiUrl = "VideoIndexer.ApiUrl";

            public const string TwilioAccountId = "Twilio.Account.Id";
            public const string TwilioAccountToken = "Twilio.Account.Token";
            public const string TwilioMessageFrom = "Twilio.Message.From";
        }

        public struct AuthIntegration
        {
            public const string TokenKey = "id_token";
            public const string AuthScheme = "Bearer";
            public const string AuthHeader = "Authorization";
            public const string ApiManagementKey = "ocp-apim-subscription-key";
        }

        public struct UserAttribute
        {
            public const string EMail = "email";
            public const string EMails = "emails";
            public const string MobilePhoneNumber = "extension_MobilePhoneNumber";

            public const string MediaAccountName = "extension_MediaAccountName";
            public const string MediaAccountSubscriptionId = "extension_MediaAccountSubscriptionId";
            public const string MediaAccountResourceGroupName = "extension_MediaAccountResourceGroupName";
            public const string MediaAccountDirectoryTenantId = "extension_MediaAccountDirectoryTenantId";
            public const string MediaAccountServicePrincipalId = "extension_MediaAccountServicePrincipalId";
            public const string MediaAccountServicePrincipalKey = "extension_MediaAccountServicePrincipalKey";
            public const string MediaAccountVideoIndexerId = "extension_MediaAccountVideoIndexerId";
            public const string MediaAccountVideoIndexerKey = "extension_MediaAccountVideoIndexerKey";
            public const string MediaAccountVideoIndexerRegion = "extension_MediaAccountVideoIndexerRegion";

            public const string StorageAccount1Name = "extension_StorageAccount1Name";
            public const string StorageAccount1Key = "extension_StorageAccount1Key";
            public const string StorageAccount2Name = "extension_StorageAccount2Name";
            public const string StorageAccount2Key = "extension_StorageAccount2Key";
            public const string StorageAccount3Name = "extension_StorageAccount3Name";
            public const string StorageAccount3Key = "extension_StorageAccount3Key";

            public const string SearchAccountName = "extension_SearchAccountName";
            public const string SearchAccountKeyReadOnly = "extension_SearchAccountKeyReadOnly";
            public const string SearchAccountKeyReadWrite = "extension_SearchAccountKeyReadWrite";
        }

        public struct Storage
        {
            public struct Blob
            {
                public const string WorkflowContainersPath = "/blobServices/default/containers/";
                public const string WorkflowContainerName = "azure-media-services";
                public const string WorkflowContainerFiles = "*";
                public const string WorkflowManifestPath = WorkflowContainerName + "/" + WorkflowManifestFile;
                public const string WorkflowManifestFile = "WorkflowManifest.json";
            }
        }

        public struct Database
        {
            public struct Collection
            {
                public const string MediaAssets = "Assets";
                public const string MediaInsight = "Insights";
            }

            //public struct Script
            //{
            //    public const string IsTimecodeFragment = "isTimecodeFragment.js";
            //    public const string GetTimecodeFragment = "getTimecodeFragment.js";
            //}

            public struct Document
            {
                public const string SystemPropertyPrefix = "_";
            }
        }

        public struct Media
        {
            public const string PredefinedPrefix = "Predefined_";
            public const string AccountResourceId = "/subscriptions/{0}/resourceGroups/{1}/providers/microsoft.media/mediaServices/{2}";

            public struct Channel9
            {
                public const int DefaultIngestVideoCount = 8;
                //public const string NamespacePrefix = "media";
                //public const string NamespaceUrl = "http://search.yahoo.com/mrss/";
                //public const string XPathQuery = "media:group/media:content[@type='video/mp4']";
                //public const string UrlHttp = "http://video.ch9.ms/";
                //public const string UrlHttps = "https://sec.ch9.ms/";
                //public const string Http = "http://";
                //public const string Https = "https://";
            }

            public struct ContentType
            {
                public const string IngestContent = "video/mp4";
                public const string IngestManifest = "application/json";
            }

            public struct ContentKeyPolicy
            {
                public const string Aes = "Envelope Encryption (AES)";
                public const string Drm = "Multiple DRM Encryption";
                public const string DrmCenc = "Multiple DRM Encryption (CENC)";
            }

            public struct Asset
            {
                public const string SingleBitrate = "SBR";
                public const string ContainerPrefix = "asset-";
            }

            public struct Job
            {
                public struct EventType
                {
                    public const string Errored = "Microsoft.Media.JobErrored";
                    public const string Finished = "Microsoft.Media.JobFinished";
                    public const string Canceled = "Microsoft.Media.JobCanceled";
                }

                public struct OutputAssetNameSuffix
                {
                    public const string StandardEncoder = "MES";
                    public const string VideoAnalyzer = "VAI";
                    public const string AudioAnalyzer = "AAI";
                    public const string FaceDetector = "FAI";
                }

                public struct CorrelationData
                {
                    public const string UserAccount = "UserAccount";
                    public const string MediaAccount = "MediaAccount";
                    public const string OutputPublish = "OutputPublish";
                }
            }

            public struct Stream
            {
                public const string DefaultScheme = "https";
                public const string DefaultFormat = "(format=mpd-time-cmaf)";
                public const string DefaultEndpointName = "default";
                public const string ManifestExtension = ".ism";
                public const string ManifestSuffix = "/manifest";
            }

            public struct Track
            {
                public struct AudioTranscript
                {
                    public const string SubtitlesType = "subtitles";
                    public const string SubtitlesLabel = "On";
                    public const string CaptionsType = "captions";
                    public const string CaptionsLabel = "Captions On";
                    public const string FileName = "transcript.vtt";
                }
            }

            public struct Thumbnail
            {
                public const string FileNamePrefix = "Thumbnail";
                public const string FileNameSuffix = "{Index}{Extension}";
            }

            public struct LiveEvent
            {
                public const int OutputArchiveWindowMinutes = 60;
                public const string OutputNameSuffix = "-Output";
            }
        }

        public struct EventGrid
        {
            public const string JobOutputProgressSubscriptionName = "Media-Job-Output-Progress";
            public static string[] JobOutputProgressSubscriptionEvents = new string[] {
                "Microsoft.Media.JobOutputProgress"
            };

            public const string JobStateFinalSubscriptionName = "Media-Job-State-Final";
            public static string[] JobStateFinalSubscriptionEvents = new string[] {
                Media.Job.EventType.Errored,
                Media.Job.EventType.Finished,
                Media.Job.EventType.Canceled
            };

            public const string LiveEventSubscriptionName = "Media-Live-Event";
            public static string[] LiveEventSubscriptionEvents = new string[] {
                "Microsoft.Media.LiveEventConnectionRejected",
                "Microsoft.Media.LiveEventEncoderConnected",
                "Microsoft.Media.LiveEventEncoderDisconnected",
                "Microsoft.Media.LiveEventIncomingDataChunkDropped",
                "Microsoft.Media.LiveEventIncomingStreamReceived",
                "Microsoft.Media.LiveEventIncomingStreamsOutOfSync",
                "Microsoft.Media.LiveEventIncomingVideoStreamsOutOfSync",
                "Microsoft.Media.LiveEventTrackDiscontinuityDetected"
            };
        }

        public struct TimerSchedule
        {
            public const string Daily = "0 0 0 * * *";
            public const string Weekly = "0 0 0 * * 1";
            public const string Monthly = "0 0 0 1 * *";
        }

        public struct Message
        {
            public const string NewLine = "\n\n";
            public const string UserPasswordForgotten = "AADB2C90118";
            public const string WorkflowInputNotComplete = "Workflow Input Not Complete: Missing '{0}'";
            public const string StorageAccountReadPermission = " (Your media account service principal does not have storage account reader access)";
            public const string JobPublishNotification = "Transform: {0}" + NewLine + "Job: {1}" + NewLine + "Event: {2}";
            public const string JobUnpublishNotification = "The '{0}' job output assets have been unpublished.";
            public const string AssetUnpublished = "The '{0}' asset has been unpublished.";
            public const string JobCreated = "New Job Created: {0}";
            public const string InsightCreated = "New Insight Created: {0}";
        }
    }
}
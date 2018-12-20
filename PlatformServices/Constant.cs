using System.Text.RegularExpressions;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("AzureSkyMedia.WebApp")]
[assembly: InternalsVisibleTo("AzureSkyMedia.FunctionApp")]

namespace AzureSkyMedia.PlatformServices
{
    public struct Constant
    {
        public const string AppSettingsFile = "appsettings.json";
        public const string ModelsDirectory = "Models";
        public const string NotAvailable = "N/A";

        public struct TextDelimiter
        {
            public const char Connection = ';';
            public const char Application = ',';
            public const char Manifest = '.';
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

            public const string AzureResourceManagementServiceUrl = "Arm.ServiceUrl";
            public const string AzureResourceManagementTokenScope = "Arm.TokenScope";

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

            public const string MediaEventGridLiveUrl = "Media.EventGrid.LiveUrl";
            public const string MediaEventGridPublishUrl = "Media.EventGrid.PublishUrl";

            public const string MediaIndexerApiUrl = "Media.Indexer.ApiUrl";

            public const string MediaClipperVersion = "Media.Clipper.Version";

            public const string MediaPlayerVersion = "Media.Player.Version";
            public const string MediaPlayerSkin = "Media.Player.Skin";

            public const string AccountEndpointPrefix = "AccountEndpoint=";
            public const string AccountNamePrefix = "AccountName=";
            public const string AccountKeyPrefix = "AccountKey=";
            public const string DatabaseIdPrefix = "DatabaseId=";

            public const string AzureStorage = "Storage";
            public const string AzureDatabase = "Database";

            public const string TwilioAccountId = "Twilio.Account.Id";
            public const string TwilioAccountToken = "Twilio.Account.Token";
            public const string TwilioMessageFrom = "Twilio.Message.From";
        }

        public struct AuthIntegration
        {
            public const string AuthScheme = "Bearer";
            public const string UserToken = "id_token";
            public const string RedirectUri = "urn:ietf:wg:oauth:2.0:oob";
            public const string SubscriptionKey = "Ocp-Apim-Subscription-Key";
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
            public const string MediaAccountVideoIndexerRegion = "extension_MediaAccountVideoIndexerRegion";
            public const string MediaAccountVideoIndexerKey = "extension_MediaAccountVideoIndexerKey";

            public const string StorageAccount1Name = "extension_StorageAccount1Name";
            public const string StorageAccount1Key = "extension_StorageAccount1Key";
            public const string StorageAccount2Name = "extension_StorageAccount2Name";
            public const string StorageAccount2Key = "extension_StorageAccount2Key";
            public const string StorageAccount3Name = "extension_StorageAccount3Name";
            public const string StorageAccount3Key = "extension_StorageAccount3Key";
        }

        public struct Storage
        {
            public struct BlobContainer
            {
                public const string MediaServices = "ams";
            }
        }

        public struct Database
        {
            public struct Collection
            {
                public const string MediaJobAccount = "Media Job Account";
                public const string MediaContentInsight = "Media Content Insight";
                public const string MediaIngestManifest = "Media Ingest Manifest";
            }

            public struct Script
            {
                public const string IsTimecodeFragment = "isTimecodeFragment.js";
                public const string GetTimecodeFragment = "getTimecodeFragment.js";
            }

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
                public const string NamespacePrefix = "media";
                public const string NamespaceUrl = "http://search.yahoo.com/mrss/";
                public const string XPathQuery = "media:group/media:content[@type='video/mp4']";
                public const string UrlHttp = "http://video.ch9.ms/";
                public const string UrlHttps = "https://sec.ch9.ms/";
                public const string Http = "http://";
                public const string Https = "https://";
            }

            public struct IngestManifest
            {
                public const string TriggerPrefix = "MediaIngestManifest";
                public const string FileExtension = ".json";
                public const string FileExtensionLog = ".log";
            }

            public struct ContentType
            {
                public const string IngestContent = "video/mp4";
                public const string IngestManifest = "application/json";
            }

            public struct ContentKey
            {
                public const string PolicyAES = "Content Key Policy AES";
                public const string PolicyCENC = "Content Key Policy DRM (CENC)";
                public const string PolicyDRM = "Content Key Policy DRM";
            }

            public struct Asset
            {
                public const string NameDelimiter = " - ";
                public const string SingleBitrate = "SBR";
                public const string ContainerPrefix = "asset-";
            }

            public struct Transform
            {
                public struct Preset
                {
                    public const string NameDelimiter = ", ";
                }
            }

            public struct Job
            {
                public const string OutputPublish = "OutputPublish";

                public struct OutputAssetNameSuffix
                {
                    public const string MultipleBitrate = "MBR";
                    public const string SingleBitrate = "SBR";

                    public const string VideoAnalyzer = "VAI";
                    public const string AudioAnalyzer = "AAI";
                }
            }

            public struct EventGrid
            {
                public const string PublishSubscriptionName = "AMS-Publish";
                public static readonly string[] PublishEventTypes = new string[] {
                    "Microsoft.Media.JobStateChange"
                };

                public const string LiveSubscriptionName = "AMS-Live";
                public static readonly string[] LiveEventTypes = new string[] {
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

            public struct Stream
            {
                public const string DefaultEndpoint = "default";
                public const string ManifestExtension = ".ism";
                public const string InsightExtension = ".json";
            }

            public struct Track
            {
                public const string CaptionsType = "captions";
                public const string CaptionsLabel = "Captions On";
                public const string TranscriptFile = "transcript.vtt";
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

        public struct Message
        {
            public const string UserPasswordForgotten = "AADB2C90118";
            public const string AssetUnpublished = "The '{0}' asset has been unpublished.";
            public const string JobOutputUnpublished = "The '{0}' job output assets have been unpublished.";
            public const string StreamingEndpointNotStarted = "Your media account ({0}) does not have a streaming endpoint started.";
            public const string StorageAccountReadPermission = " (Your AMS Service Principal does not have storage account read permission)";

        }
    }
}
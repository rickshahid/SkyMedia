using System.Text.RegularExpressions;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("AzureSkyMedia.WebApp")]
[assembly: InternalsVisibleTo("AzureSkyMedia.FunctionApp")]

namespace AzureSkyMedia.PlatformServices
{
    public struct Constant
    {
        public const string WebModels = "Models";
        public const string AppSettings = "appsettings.json";

        public struct TextDelimiter
        {
            public const char Connection = ';';
            public const char Application = ',';
        }

        public struct TextFormatter
        {
            public const string NumericLong = "N0";
            //public const string Numeric = "N2";
            //public const string ClockTime = "hh':'mm':'ss";

            public static readonly string[] SpacingPatterns = new string[] { "([a-z])([A-Z])", "(H264)([A-Z])", "([a-z])(720p|1080p)" };
            public static readonly string[] SpacingInserts = new string[] { "$1 $2", "$1 $2", "$1 $2" };

            public static string GetValue(string value)
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
            public const string AppTitle = "App.Title";
            public const string AppRegionName = "App.RegionName";

            public const string AppApiVersion = "App.Api.Version";
            public const string AppApiEndpointUrl = "App.Api.EndpointUrl";
            public const string AppApiDescription = "App.Api.Description";

            public const string AzureResourceManagementEndpointUrl = "Arm.EndpointUrl";
            public const string AzureResourceManagementAudienceUrl = "Arm.AudienceUrl";

            public const string DirectoryTenantId = "Directory.TenantId";
            public const string DirectoryIssuerUrl = "Directory.IssuerUrl";
            public const string DirectoryDiscoveryPath = "Directory.DiscoveryPath";

            public const string DirectoryClientId = "Directory.ClientId";
            public const string DirectoryClientSecret = "Directory.ClientSecret";

            public const string DirectoryPolicyIdSignUpIn = "Directory.PolicyId.SignUpIn";
            public const string DirectoryPolicyIdProfileEdit = "Directory.PolicyId.ProfileEdit";
            public const string DirectoryPolicyIdPasswordReset = "Directory.PolicyId.PasswordReset";

            public const string DatabaseRegionsRead = "Database.Regions.Read";
            public const string DatabaseCollectionThroughputUnits = "Database.Collection.ThroughputUnits";

            public const string StorageCdnEndpointUrl = "Storage.CdnEndpointUrl";
            public const string StorageSharedAccessMinutes = "Storage.SharedAccessMinutes";

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

            public const string MediaPublishUrl = "Media.Publish.Url";
            public const string MediaPublishQueue = "Media.Publish.Queue";

            public const string MediaPlayerVersion = "Media.Player.Version";
            public const string MediaPlayerSkin = "Media.Player.Skin";

            public const string MediaClipperVersion = "Media.Clipper.Version";

            public const string MediaIndexerAuthUrl = "Media.Indexer.AuthUrl";

            public const string AccountEndpointPrefix = "AccountEndpoint=";
            public const string AccountNamePrefix = "AccountName=";
            public const string AccountKeyPrefix = "AccountKey=";
            public const string DatabaseIdPrefix = "DatabaseId=";

            public const string AzureStorage = "Storage";
            public const string AzureDatabase = "Database";
            public const string AzureCache = "Cache";

            public const string TwilioAccountId = "Twilio.Account.Id";
            public const string TwilioAccountToken = "Twilio.Account.Token";
            public const string TwilioMessageFrom = "Twilio.Message.From";
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

        public struct UserAttribute
        {
            public const string EMail = "email";
            public const string EMails = "emails";
            public const string MobileNumber = "extension_MobileNumber";

            public const string MediaAccountName = "extension_MediaAccountName";
            public const string MediaAccountSubscriptionId = "extension_MediaAccountSubscriptionId";
            public const string MediaAccountResourceGroupName = "extension_MediaAccountResourceGroupName";
            public const string MediaAccountDirectoryTenantId = "extension_MediaAccountDirectoryTenantId";
            public const string MediaAccountServicePrincipalId = "extension_MediaAccountServicePrincipalId";
            public const string MediaAccountServicePrincipalKey = "extension_MediaAccountServicePrincipalKey";
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
            public const string NotAvailable = "N/A";
            public const string CapacityData = "data";

            public struct Blob
            {
                public struct Container
                {
                    public const string MediaProcess = "ams";
                    public const string FileUpload = "upload";
                    //public const string ContentDelivery = "cdn";
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
                public const string OutputPublish = "OutputPublish";
                public const string OutputInsight = "OutputInsight";
            }

            public struct Procedure
            {
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
            public const string PredefinedPrefix = "Predefined_";
            public const string AccountResourceId = "/subscriptions/{0}/resourceGroups/{1}/providers/microsoft.media/mediaServices/{2}";

            public struct ContentKey
            {
                public const string PolicyAES = "Content Key Policy AES";
                public const string PolicyDRM = "Content Key Policy DRM";
            }

            public struct ContentStreaming
            {
                public const string PolicyAES = "Streaming Policy AES";
                public const string PolicyDRM = "Streaming Policy DRM";
            }

            public struct Asset
            {
                public const string ModelDirectory = "Asset";
                public const string SingleBitrate = "SBR";

                public const string NameDefault = " Asset ";
                public const string NameDelimiter = " - ";
            }

            public struct Transform
            {
                public const string PresetNameDelimiter = ", ";
                public const string PresetNameAnalyzerVideo = "VideoAnalyzer";
                public const string PresetNameAnalyzerAudio = "AudioAnalyzer";
            }

            public struct Job
            {
                public const string OutputAssetSuffixEncoderStandard = " (MBR)";
                public const string OutputAssetSuffixAnalyzerVideo = " (VAI)";
                public const string OutputAssetSuffixAnalyzerAudio = " (AAI)";
            }

            public struct Analyzer
            {
                public const string TranscriptFile = "transcript.vtt";
            }

            //public struct FileExtension
            //{
            //    public const string MP4 = ".mp4";
            //    public const string JobLog = ".log";
            //    public const string JobManifest = ".txt";
            //    public const string StreamManifest = ".ism";
            //    public const string PremiumWorkflow = ".workflow";

            //    public const string Json = ".json";
            //    public const string WebVtt = ".vtt";
            //    public const string Annotations = "_annotations.json";
            //}

            //public struct Live
            //{
            //    public const string ProgramSuffixClear = " Clear";
            //    public const string ProgramSuffixAes = " AES";
            //    public const string ProgramSuffixDrm = " DRM";

            //    public const string AllowAnyAddress = "Allow Any Address";
            //    public const string AllowAuthorizedAddress = "Authorized Address Only";
            //}

            public struct Publish
            {
                public const string EventTriggerName = "AMS-Job-State";
                public static readonly string[] EventTriggerTypes = new string[] { "Microsoft.Media.JobStateChange" };
            }

            public struct Stream
            {
                public const string DefaultEndpoint = "default";
                public const int TunerPageSize = 10;
            }

            public struct Track
            {
                public const string Captions = "captions";
                public const string CaptionsLabel = "Captions On";

                //    public const string Subtitles = "subtitles";
                //    public const string Thumbnails = "thumbnails";
                //    public const string ThumbnailsData = "#xywh=";
            }
        }

        public struct Message
        {
            public const string UserPasswordForgotten = "AADB2C90118";
            public const string MobileNumberNotAvailable = "No message sent. Mobile number not available.";
            public const string StreamingEndpointNotStarted = "Your media account streaming endpoint has not been started.";
            public const string StorageAccountReadPermission = " (Your AMS Service Principal does not have storage account read permission)";
            public const string VideoIndexerKeyMissing = "Your account currently does not have a Video Indexer key associated with it,<br>which is required for the functionality in the {0} module.<br><br>Use the Account Profile Edit page to associate a Video Indexer key with your account.";
        }
    }
}
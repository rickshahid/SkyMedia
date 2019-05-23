using System;
using System.Collections.Generic;

using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Management.Media.Models;

using Newtonsoft.Json.Linq;

using Twilio.Types;
using Twilio.Rest.Api.V2010.Account;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.FunctionApp
{
    public static class MediaLinkSync
    {
        [FunctionName("MediaLinkSync")]
        [return: TwilioSms(AccountSidSetting = "Twilio.AccountId", AuthTokenSetting = "Twilio.AccountToken", From = "%Twilio.PhoneNumber.From%")]
        public static CreateMessageOptions Run([TimerTrigger(Constant.TimerSchedule.Daily)] TimerInfo timer, ILogger logger)
        {
            CreateMessageOptions twilioMessage = null;
            try
            {
                logger.LogInformation("Media Link Sync @ {0}", DateTime.UtcNow);
                using (DatabaseClient databaseClient = new DatabaseClient(true))
                {
                    SyncMediaAssets(databaseClient, logger);
                }
            }
            catch (Exception ex)
            {
                string settingKey = Constant.AppSettingKey.TwilioPhoneNumberTo;
                string phoneNumberTo = AppSetting.GetValue(settingKey);
                PhoneNumber twilioPhoneNumber = new PhoneNumber(phoneNumberTo);
                twilioMessage = new CreateMessageOptions(twilioPhoneNumber)
                {
                    Body = ex.Message
                };
            }
            return twilioMessage;
        }

        private static void SyncMediaAssets(DatabaseClient databaseClient, ILogger logger)
        {
            string collectionId = Constant.Database.Collection.MediaAssets;
            MediaAssetLink[] assetLinks = databaseClient.GetDocuments<MediaAssetLink>(collectionId);
            foreach (MediaAssetLink assetLink in assetLinks)
            {
                using (MediaClient mediaClient = new MediaClient(assetLink.MediaAccount, assetLink.UserAccount))
                {
                    Asset parentAsset = mediaClient.GetEntity<Asset>(MediaEntity.Asset, assetLink.AssetName);
                    if (parentAsset == null)
                    {
                        foreach (KeyValuePair<MediaTransformPreset, string> jobOutput in assetLink.JobOutputs)
                        {
                            if (jobOutput.Key == MediaTransformPreset.VideoIndexer || jobOutput.Key == MediaTransformPreset.AudioIndexer)
                            {
                                string insightId = jobOutput.Value;
                                mediaClient.IndexerDeleteVideo(insightId);
                            }
                            else
                            {
                                string assetName = jobOutput.Value;
                                mediaClient.DeleteEntity(MediaEntity.Asset, assetName);
                            }
                        }
                        string documentId = assetLink.AssetName;
                        databaseClient.DeleteDocument(collectionId, documentId);
                        logger.LogInformation("Asset Link Deleted: {0}", documentId);
                    }
                    else
                    {
                        List<MediaTransformPreset> missingJobOutputs = new List<MediaTransformPreset>();
                        foreach (KeyValuePair<MediaTransformPreset, string> jobOutput in assetLink.JobOutputs)
                        {
                            if (jobOutput.Key == MediaTransformPreset.VideoIndexer || jobOutput.Key == MediaTransformPreset.AudioIndexer)
                            {
                                string insightId = jobOutput.Value;
                                bool insightExists = mediaClient.IndexerInsightExists(insightId, out JObject insight);
                                if (!insightExists)
                                {
                                    missingJobOutputs.Add(jobOutput.Key);
                                    logger.LogInformation("Missing Indexer Insight: {0}", insightId);
                                }
                            }
                            else
                            {
                                string assetName = jobOutput.Value;
                                Asset childAsset = mediaClient.GetEntity<Asset>(MediaEntity.Asset, assetName);
                                if (childAsset == null)
                                {
                                    missingJobOutputs.Add(jobOutput.Key);
                                    logger.LogInformation("Missing Output Asset: {0}", assetName);
                                }
                            }
                        }
                        if (missingJobOutputs.Count > 0)
                        {
                            foreach (MediaTransformPreset missingJobOutput in missingJobOutputs)
                            {
                                assetLink.JobOutputs.Remove(missingJobOutput);
                            }
                            databaseClient.UpsertDocument(collectionId, assetLink);
                            logger.LogInformation("Asset Link Upserted: {0}", assetLink);
                        }
                    }
                }
            }
        }
    }
}
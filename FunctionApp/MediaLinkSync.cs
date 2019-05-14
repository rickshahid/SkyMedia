using System;
using System.Collections.Generic;

using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Management.Media.Models;

using Newtonsoft.Json.Linq;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.FunctionApp
{
    public static class MediaLinkSync
    {
        [FunctionName("MediaLinkSync")]
        public static void Run([TimerTrigger(Constant.TimerSchedule.Daily)] TimerInfo timer, ILogger logger)
        {
            logger.LogInformation("Media Link Sync @ {0}", DateTime.UtcNow);
            using (DatabaseClient databaseClient = new DatabaseClient(true))
            {
                SyncMediaAssets(databaseClient, logger);
                SyncMediaInsights(databaseClient, logger);
            }
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
                        string documentId = assetLink.AssetName;
                        databaseClient.DeleteDocument(collectionId, documentId);
                        logger.LogInformation("Asset Link Deleted: {0}", documentId);
                    }
                    else
                    {
                        List<MediaTransformPreset> missingJobOutputs = new List<MediaTransformPreset>();
                        foreach (KeyValuePair<MediaTransformPreset, string> jobOutput in assetLink.JobOutputs)
                        {
                            Asset childAsset = mediaClient.GetEntity<Asset>(MediaEntity.Asset, jobOutput.Value);
                            if (childAsset == null)
                            {
                                missingJobOutputs.Add(jobOutput.Key);
                                logger.LogInformation("Missing Job Output: {0}", jobOutput.Value);
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

        private static void SyncMediaInsights(DatabaseClient databaseClient, ILogger logger)
        {
            string collectionId = Constant.Database.Collection.MediaInsight;
            MediaInsightLink[] insightLinks = databaseClient.GetDocuments<MediaInsightLink>(collectionId);
            foreach (MediaInsightLink insightLink in insightLinks)
            {
                using (MediaClient mediaClient = new MediaClient(insightLink.MediaAccount, insightLink.UserAccount))
                {
                    bool insightExists = mediaClient.IndexerInsightExists(insightLink.InsightId, out JObject insight);
                    if (!insightExists)
                    {
                        string documentId = insightLink.InsightId;
                        databaseClient.DeleteDocument(collectionId, documentId);
                        logger.LogInformation("Insight Link Deleted: {0}", documentId);
                    }
                }
            }
        }
    }
}
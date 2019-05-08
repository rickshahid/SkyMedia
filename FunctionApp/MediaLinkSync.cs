using System;
using System.Collections.Generic;

using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Management.Media.Models;

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
                string collectionId = Constant.Database.Collection.MediaAssets;
                MediaAssetLink[] assetLinks = databaseClient.GetDocuments<MediaAssetLink>(collectionId);
                foreach (MediaAssetLink assetLink in assetLinks)
                {
                    using (MediaClient mediaClient = new MediaClient(assetLink.MediaAccount, null))
                    {
                        Asset parentAsset = mediaClient.GetEntity<Asset>(MediaEntity.Asset, assetLink.AssetName);
                        if (parentAsset == null)
                        {
                            string documentId = assetLink.AssetName;
                            databaseClient.DeleteDocument(collectionId, documentId);
                            logger.LogInformation("Document Deleted: {0}", documentId);
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
                                logger.LogInformation("Document Upserted: {0}", assetLink);
                            }
                        }
                    }
                }
            }
        }
    }
}
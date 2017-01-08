using System;
using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.WindowsAzure.MediaServices.Client;

namespace SkyMedia.ServiceBroker
{
    internal static class Selections
    {
        public static SelectListItem[] GetStorageAccounts(string authToken)
        {
            MediaClient mediaClient = new MediaClient(authToken);
            List<SelectListItem> storageAccounts = new List<SelectListItem>();
            IStorageAccount[] accounts = mediaClient.GetEntities(EntityType.StorageAccount) as IStorageAccount[];
            foreach (IStorageAccount account in accounts)
            {
                SelectListItem storageAccount = new SelectListItem();
                storageAccount.Text = string.Concat("Account: ", account.Name);
                storageAccount.Value = account.Name;
                storageAccount.Selected = account.IsDefault;
                string storageUsed = Storage.GetCapacityUsed(authToken, account.Name);
                if (storageUsed != null)
                {
                    storageAccount.Text = string.Concat(storageAccount.Text, ", Storage Used: ", storageUsed, ")");
                    storageAccounts.Add(storageAccount);
                }
            }
            return storageAccounts.ToArray();
        }

        public static SelectListItem[] GetMediaProcessors()
        {
            List<SelectListItem> mediaProcessors = new List<SelectListItem>();

            SelectListItem mediaProcessor = new SelectListItem();
            mediaProcessor.Text = string.Empty;
            mediaProcessor.Value = MediaProcessor.None.ToString();
            mediaProcessors.Add(mediaProcessor);

            mediaProcessor = new SelectListItem();
            mediaProcessor.Text = "Encoder Standard";
            mediaProcessor.Value = MediaProcessor.EncoderStandard.ToString();
            mediaProcessors.Add(mediaProcessor);

            mediaProcessor = new SelectListItem();
            mediaProcessor.Text = "Encoder Premium";
            mediaProcessor.Value = MediaProcessor.EncoderPremium.ToString();
            mediaProcessors.Add(mediaProcessor);

            mediaProcessor = new SelectListItem();
            mediaProcessor.Text = "Indexer v1";
            mediaProcessor.Value = MediaProcessor.IndexerV1.ToString();
            mediaProcessors.Add(mediaProcessor);

            mediaProcessor = new SelectListItem();
            mediaProcessor.Text = "Indexer v2";
            mediaProcessor.Value = MediaProcessor.IndexerV2.ToString();
            mediaProcessors.Add(mediaProcessor);

            mediaProcessor = new SelectListItem();
            mediaProcessor.Text = "Face Detection";
            mediaProcessor.Value = MediaProcessor.FaceDetection.ToString();
            mediaProcessors.Add(mediaProcessor);

            mediaProcessor = new SelectListItem();
            mediaProcessor.Text = "Face Redaction";
            mediaProcessor.Value = MediaProcessor.FaceRedaction.ToString();
            mediaProcessors.Add(mediaProcessor);

            mediaProcessor = new SelectListItem();
            mediaProcessor.Text = "Motion Detection";
            mediaProcessor.Value = MediaProcessor.MotionDetection.ToString();
            mediaProcessors.Add(mediaProcessor);

            mediaProcessor = new SelectListItem();
            mediaProcessor.Text = "Motion Hyperlapse";
            mediaProcessor.Value = MediaProcessor.MotionHyperlapse.ToString();
            mediaProcessors.Add(mediaProcessor);

            mediaProcessor = new SelectListItem();
            mediaProcessor.Text = "Video Summarization";
            mediaProcessor.Value = MediaProcessor.VideoSummarization.ToString();
            mediaProcessors.Add(mediaProcessor);

            mediaProcessor = new SelectListItem();
            mediaProcessor.Text = "Character Recognition";
            mediaProcessor.Value = MediaProcessor.CharacterRecognition.ToString();
            mediaProcessors.Add(mediaProcessor);

            return mediaProcessors.ToArray();
        }

        public static string GetProcessorName(MediaProcessor mediaProcessor)
        {
            string processorName = string.Empty;
            SelectListItem[] processors = GetMediaProcessors();
            foreach (SelectListItem processor in processors)
            {
                if (string.Equals(processor.Value, mediaProcessor.ToString(), StringComparison.InvariantCultureIgnoreCase))
                {
                    processorName = processor.Text;
                }
            }
            return processorName;
        }
    }
}

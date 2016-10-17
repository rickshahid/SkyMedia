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
            IStorageAccount[] storageAccounts = mediaClient.GetEntities(EntityType.StorageAccount) as IStorageAccount[];
            List<SelectListItem> accounts = new List<SelectListItem>();
            foreach (IStorageAccount storageAccount in storageAccounts)
            {
                SelectListItem account = new SelectListItem();
                account.Text = string.Concat("Account Name: ", storageAccount.Name);
                account.Value = storageAccount.Name;
                account.Selected = storageAccount.IsDefault;
                string storageUsed = Storage.GetCapacityUsed(authToken, storageAccount.Name);
                if (storageUsed != null)
                {
                    account.Text = string.Concat(account.Text, ", Storage Used: ", storageUsed, ")");
                    if (storageAccount.IsDefault)
                    {
                        account.Text = string.Concat(account.Text, Constants.Storage.Account.DefaultSuffix);
                    }
                    accounts.Add(account);
                }
            }
            return accounts.ToArray();
        }

        public static SelectListItem[] GetMediaProcessors(bool analyticsView)
        {
            List<SelectListItem> processors = new List<SelectListItem>();

            SelectListItem processor = new SelectListItem();
            processor.Text = string.Empty;
            processor.Value = MediaProcessor.None.ToString();
            processors.Add(processor);

            if (!analyticsView)
            {
                processor = new SelectListItem();
                processor.Text = "Encoder Standard";
                processor.Value = MediaProcessor.EncoderStandard.ToString();
                processors.Add(processor);

                processor = new SelectListItem();
                processor.Text = "Encoder Premium";
                processor.Value = MediaProcessor.EncoderPremium.ToString();
                processors.Add(processor);
            }

            //processor = new SelectListItem();
            //processor.Text = "Indexer v1";
            //processor.Value = MediaProcessor.IndexerV1.ToString();
            //processors.Add(processor);

            processor = new SelectListItem();
            processor.Text = "Indexer v2";
            processor.Value = MediaProcessor.IndexerV2.ToString();
            processors.Add(processor);

            //if (!analyticsView)
            //{
            //    processor = new SelectListItem();
            //    processor.Text = "Video Summarization";
            //    processor.Value = MediaProcessor.VideoSummarization.ToString();
            //    processors.Add(processor);
            //}

            //processor = new SelectListItem();
            //processor.Text = "Character Recognition";
            //processor.Value = MediaProcessor.CharacterRecognition.ToString();
            //processors.Add(processor);

            //processor = new SelectListItem();
            //processor.Text = "Object Detection";
            //processor.Value = MediaProcessor.ObjectDetection.ToString();
            //processors.Add(processor);

            //processor = new SelectListItem();
            //processor.Text = "Face & Emotion Detection";
            //processor.Value = MediaProcessor.FaceDetection.ToString();
            //processors.Add(processor);

            //processor = new SelectListItem();
            //processor.Text = "Motion Detection (Surveillance)";
            //processor.Value = MediaProcessor.MotionDetection.ToString();
            //processors.Add(processor);

            //if (!analyticsView)
            //{
            //    processor = new SelectListItem();
            //    processor.Text = "Motion Acceleration (Hyperlapse)";
            //    processor.Value = MediaProcessor.MotionHyperlapse.ToString();
            //    processors.Add(processor);

            //    processor = new SelectListItem();
            //    processor.Text = "Motion Stabilization";
            //    processor.Value = MediaProcessor.MotionStabilization.ToString();
            //    processors.Add(processor);
            //}

            return processors.ToArray();
        }

        public static SelectListItem[] GetImageFormats()
        {
            List<SelectListItem> formats = new List<SelectListItem>();

            SelectListItem format = new SelectListItem();
            format.Text = "PNG";
            format.Value = "Png";
            formats.Add(format);

            format = new SelectListItem();
            format.Text = "JPG";
            format.Value = "Jpg";
            formats.Add(format);

            format = new SelectListItem();
            format.Text = "BMP";
            format.Value = "Bmp";
            formats.Add(format);

            return formats.ToArray();
        }

        public static SelectListItem[] GetSpokenLanguages()
        {
            List<SelectListItem> languages = new List<SelectListItem>();

            SelectListItem language = new SelectListItem();
            language.Text = "English";
            language.Value = "EnUs";
            language.Selected = true;
            languages.Add(language);

            language = new SelectListItem();
            language.Text = "Spanish";
            language.Value = "EsEs";
            languages.Add(language);

            language = new SelectListItem();
            language.Text = "Arabic (Egyptian)";
            language.Value = "ArEg";
            languages.Add(language);

            language = new SelectListItem();
            language.Text = "Chinese";
            language.Value = "ZhCn";
            languages.Add(language);

            language = new SelectListItem();
            language.Text = "French";
            language.Value = "FrFr";
            languages.Add(language);

            language = new SelectListItem();
            language.Text = "German";
            language.Value = "DeDe";
            languages.Add(language);

            language = new SelectListItem();
            language.Text = "Italian";
            language.Value = "ItIt";
            languages.Add(language);

            language = new SelectListItem();
            language.Text = "Portuguese";
            language.Value = "PtBr";
            languages.Add(language);

            return languages.ToArray();
        }

        public static string GetLanguageLabel(string languageCode)
        {
            string languageLabel = string.Empty;
            SelectListItem[] languages = GetSpokenLanguages();
            foreach (SelectListItem language in languages)
            {
                if (language.Value.StartsWith(languageCode, StringComparison.InvariantCultureIgnoreCase))
                {
                    languageLabel = language.Text;
                }
            }
            return languageLabel;
        }
    }
}

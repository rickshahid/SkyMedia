using System;
using System.IO;
using System.Threading.Tasks;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.EventGrid.Models;

using Newtonsoft.Json;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.FunctionApp
{
    public static partial class MediaWorkflow
    {
        [FunctionName("MediaWorkflow-Blob")]
        public static async Task Trigger([QueueTrigger(Constant.Storage.Blob.WorkflowContainerName)] EventGridEvent queueMessage,
                                         [Blob(Constant.Storage.Blob.WorkflowManifestPath, FileAccess.Read)] Stream manifestInput,
                                         [OrchestrationClient] DurableOrchestrationClient client)
        {
            if (InputComplete(queueMessage, manifestInput, out MediaWorkflowManifest workflowManifest))
            {
                MediaClient mediaClient = new MediaClient(workflowManifest);
                EventGridClient.SetMediaSubscription(mediaClient.MediaAccount);

                ValueTuple<EventGridEvent, MediaWorkflowManifest> workflowInput = (queueMessage, workflowManifest);
                await client.StartNewAsync("MediaWorkflow", workflowInput);
            }
        }

        private static bool InputComplete(EventGridEvent queueMessage, Stream manifestInput, out MediaWorkflowManifest workflowManifest)
        {
            workflowManifest = null;
            bool inputComplete = false;
            if (manifestInput != null)
            {
                string workflowManifestJson;
                using (StreamReader manifestReader = new StreamReader(manifestInput))
                {
                    workflowManifestJson = manifestReader.ReadToEnd();
                }
                workflowManifest = JsonConvert.DeserializeObject<MediaWorkflowManifest>(workflowManifestJson);
                if (string.IsNullOrEmpty(workflowManifest.InputFileName))
                {
                    inputComplete = !queueMessage.Subject.EndsWith(Constant.Storage.Blob.WorkflowManifestFile, _stringComparison);
                }
                else if (workflowManifest.InputFileName.Equals(Constant.Storage.Blob.WorkflowContainerFiles))
                {
                    inputComplete = true;
                }
                else
                {
                    CloudBlockBlob blob = _blobClient.GetBlockBlob(_containerName, null, workflowManifest.InputFileName);
                    inputComplete = blob.Exists();
                }
            }
            return inputComplete;
        }
    }
}
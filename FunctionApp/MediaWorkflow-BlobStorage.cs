using System;
using System.IO;
using System.Threading.Tasks;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.FunctionApp
{
    public static partial class MediaWorkflow
    {
        [FunctionName("MediaWorkflow-BlobStorage")]
        public static async Task Trigger([QueueTrigger(Constant.Storage.Blob.WorkflowContainerName)] EventGridEvent queueMessage,
                                            [Blob(Constant.Storage.Blob.WorkflowManifestPath, FileAccess.Read)] Stream manifestInput,
                                            [OrchestrationClient] DurableOrchestrationClient client, ILogger logger)
        {
            _logger = logger;
            _logger.LogInformation(JsonConvert.SerializeObject(queueMessage, Formatting.Indented));
            if (InputComplete(queueMessage, manifestInput, out MediaWorkflowManifest workflowManifest))
            {
                ValueTuple<EventGridEvent, MediaWorkflowManifest> workflowInput = (queueMessage, workflowManifest);
                string orchestrationId = await client.StartNewAsync("MediaWorkflow", workflowInput);
                _logger.LogInformation(Constant.Message.OrchestrationStarted, orchestrationId);
            }
        }

        private static bool InputComplete(EventGridEvent queueMessage, Stream manifestInput, out MediaWorkflowManifest workflowManifest)
        {
            workflowManifest = null;
            bool inputComplete = false;
            if (manifestInput == null)
            {
                _logger.LogInformation(string.Format(Constant.Message.WorkflowInputNotComplete, Constant.Storage.Blob.WorkflowManifestFile));
            }
            else
            {
                string workflowManifestJson;
                using (StreamReader manifestReader = new StreamReader(manifestInput))
                {
                    workflowManifestJson = manifestReader.ReadToEnd();
                }
                _logger.LogInformation(workflowManifestJson);
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
                    if (!inputComplete)
                    {
                        _logger.LogInformation(string.Format(Constant.Message.WorkflowInputNotComplete, workflowManifest.InputFileName));
                    }
                }
            }
            return inputComplete;
        }
    }
}
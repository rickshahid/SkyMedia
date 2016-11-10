using System.Net;

using Microsoft.AspNetCore.Mvc;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.MediaServices.Client;

using SkyMedia.ServiceBroker;
using SkyMedia.WebApp.Models;

namespace SkyMedia.WebApp.Controllers
{
    public class apiController : Controller
    {
        [HttpGet]
        [Route("/publish")]
        public PublishNotification publish()
        {
            CloudQueueMessage queueMessage;
            MessageClient messageClient = new MessageClient();
            string queueName = Constants.Storage.QueueName.JobStatus;
            PublishNotification publishNotification = new PublishNotification();
            MediaJobEvent jobEvent = messageClient.GetMessage<MediaJobEvent>(queueName, out queueMessage);
            if (queueMessage != null)
            {
                string partitionKey = jobEvent.Properties.AccountName;
                string rowKey = jobEvent.Properties.JobId;
                if (!string.IsNullOrEmpty(partitionKey) && !string.IsNullOrEmpty(rowKey))
                {
                    EntityClient entityClient = new EntityClient();
                    string tableName = Constants.Storage.TableNames.AssetPublish;
                    ContentPublish contentPublish = entityClient.GetEntity<ContentPublish>(tableName, partitionKey, rowKey);
                    if (contentPublish != null)
                    {
                        tableName = Constants.Storage.TableNames.AssetProtection;
                        ContentProtection contentProtection = entityClient.GetEntity<ContentProtection>(tableName, partitionKey, rowKey);
                        MediaClient mediaClient = new MediaClient(contentPublish);
                        IJob job = mediaClient.GetEntityById(EntityType.Job, jobEvent.Properties.JobId) as IJob;
                        if (job != null)
                        {
                            publishNotification.MessageText = MediaClient.GetNotificationMessage(partitionKey, job);
                            publishNotification.MobileNumber = contentPublish.MobileNumber;
                            if (jobEvent.Properties.NewState == JobState.Finished)
                            {
                                MediaClient.PublishJob(mediaClient, job, contentPublish, contentProtection);
                            }
                        }
                        if (contentProtection != null)
                        {
                            tableName = Constants.Storage.TableNames.AssetProtection;
                            entityClient.DeleteEntity(tableName, contentProtection);
                        }
                        tableName = Constants.Storage.TableNames.AssetPublish;
                        entityClient.DeleteEntity(tableName, contentPublish);
                    }
                }
                messageClient.DeleteMessage(queueName, queueMessage);
            }
            return publishNotification;
        }

        [HttpPost]
        [Route("/notify")]
        public HttpWebResponse notify([FromBody] PublishNotification publishNotification)
        {
            HttpWebResponse webResponse = null;
            if (!string.IsNullOrEmpty(publishNotification.MessageText) && !string.IsNullOrEmpty(publishNotification.MobileNumber))
            {
                webResponse = MessageClient.SendText(publishNotification.MessageText, publishNotification.MobileNumber);
            }
            return webResponse;
        }
    }
}

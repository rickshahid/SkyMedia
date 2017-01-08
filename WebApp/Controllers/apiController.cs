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
        [Route("/job/event")]
        public MediaJobEvent GetJobEvent()
        {
            CloudQueueMessage queueMessage;
            MessageClient messageClient = new MessageClient();
            string queueName = Constants.Storage.QueueName.JobStatus;
            MediaJobEvent jobEvent = messageClient.GetMessage<MediaJobEvent>(queueName, out queueMessage);
            if (queueMessage == null)
            {
                MediaJobEventProperties jobEventProperties = new MediaJobEventProperties();
                jobEventProperties.JobId = string.Empty;
                jobEvent = new MediaJobEvent();
                jobEvent.Properties = jobEventProperties;
            }
            else
            {
                string accountName = jobEvent.Properties.AccountName;
                string jobId = jobEvent.Properties.JobId;
                if (string.IsNullOrEmpty(accountName) || string.IsNullOrEmpty(jobId))
                {
                    messageClient.DeleteMessage(queueName, queueMessage);
                }
            }
            return jobEvent;
        }

        [HttpPost]
        [Route("/job/publish")]
        public PublishNotification PublishJob([FromBody] MediaJobPublish jobPublish)
        {
            PublishNotification notification = new PublishNotification();
            notification.MessageText = string.Empty;
            notification.MobileNumber = string.Empty;
            EntityClient entityClient = new EntityClient();
            string tableName = Constants.Storage.TableNames.AssetPublish;
            ContentPublish contentPublish = entityClient.GetEntity<ContentPublish>(tableName, jobPublish.AccountName, jobPublish.JobId);
            if (contentPublish != null)
            {
                tableName = Constants.Storage.TableNames.AssetProtection;
                ContentProtection contentProtection = entityClient.GetEntity<ContentProtection>(tableName, jobPublish.AccountName, jobPublish.JobId);
                MediaClient mediaClient = new MediaClient(contentPublish);
                IJob job = mediaClient.GetEntityById(EntityType.Job, jobPublish.JobId) as IJob;
                if (job != null)
                {
                    mediaClient.SetReservedUnits(job);
                    notification.MessageText = MediaClient.GetNotificationMessage(jobPublish.AccountName, job);
                    notification.MobileNumber = contentPublish.MobileNumber;
                    if (jobPublish.NewState == JobState.Finished)
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
            return notification;
        }
    }
}

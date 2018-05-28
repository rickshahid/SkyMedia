using System;
using System.Text;

namespace AzureSkyMedia.PlatformServices
{
    internal partial class MediaClient
    {
        //private INotificationEndPoint GetNotificationEndpoint()
        //{
        //    string settingKey = Constant.AppSettingKey.MediaPublishUrl;
        //    string endpointAddress = AppSetting.GetValue(settingKey);
        //    string endpointName = Constant.Media.Job.NotificationEndpointName;
        //    INotificationEndPoint notificationEndpoint = GetEntityByName(MediaEntity.NotificationEndpoint, endpointName) as INotificationEndPoint;
        //    if (notificationEndpoint != null && !string.Equals(notificationEndpoint.EndPointAddress, endpointAddress, StringComparison.OrdinalIgnoreCase))
        //    {
        //        notificationEndpoint.Delete();
        //        notificationEndpoint = null;
        //    }
        //    if (notificationEndpoint == null)
        //    {
        //        NotificationEndPointType endpointType = NotificationEndPointType.WebHook;
        //        notificationEndpoint = _media2.NotificationEndPoints.Create(endpointName, endpointType, endpointAddress);
        //    }
        //    return notificationEndpoint;
        //}

        //private static string GetNotificationMessage(MediaAccount mediaAccount, IJob job)
        //{
        //    StringBuilder message = new StringBuilder();
        //    message.Append("Azure Media Services Job ");
        //    message.Append(job.State.ToString());
        //    message.Append("\n\n");
        //    message.Append("Media Account Name: ");
        //    message.Append(mediaAccount.Name);
        //    message.Append("\n\n");
        //    message.Append("Job Id: ");
        //    message.Append(job.Id);
        //    message.Append("\n\n");
        //    message.Append("Job Name: ");
        //    message.Append(job.Name);
        //    message.Append("\n\n");
        //    message.Append("Job Duration: ");
        //    message.Append(job.RunningDuration.ToString(Constant.TextFormatter.ClockTime));
        //    message.Append("\n\n");
        //    foreach (ITask jobTask in job.Tasks)
        //    {
        //        message.Append("Job Task Name: ");
        //        message.Append(jobTask.Name);
        //        message.Append("\n\n");
        //        message.Append("Job Task Duration: ");
        //        message.Append(jobTask.RunningDuration.ToString(Constant.TextFormatter.ClockTime));
        //        message.Append("\n\n");
        //        message.Append("Job Task Performance: ");
        //        message.Append(jobTask.PerfMessage.Trim());
        //        message.Append("\n\n");
        //        if (jobTask.ErrorDetails != null)
        //        {
        //            foreach (ErrorDetail taskError in jobTask.ErrorDetails)
        //            {
        //                message.Append("Job Task Error Code: ");
        //                message.Append(taskError.Code);
        //                message.Append("\n\n");
        //                message.Append("Job Task Error Message: ");
        //                message.Append(taskError.Message);
        //                message.Append("\n\n");
        //            }
        //        }
        //    }
        //    return message.ToString();
        //}
    }
}
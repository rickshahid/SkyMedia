#r "Newtonsoft.Json"
#r "AzureSkyMedia.PlatformServices.dll"

using System;
using System.Net;
using System.Text;

using Newtonsoft.Json;

using Microsoft.WindowsAzure.MediaServices.Client;

using AzureSkyMedia.PlatformServices;

public static async Task<object> Run(HttpRequestMessage req, TraceWriter log)
{
    byte[] requestBody = await req.Content.ReadAsByteArrayAsync();
    string requestMessage = Encoding.UTF8.GetString(requestBody);
    MediaJobNotification jobNotification = JsonConvert.DeserializeObject<MediaJobNotification>(requestMessage);
    log.Info(string.Concat("Job Notification: ", JsonConvert.SerializeObject(jobNotification)));

    object responseData = jobNotification;
    if (jobNotification.EventType == MediaJobNotificationEvent.JobStateChange &&
        (jobNotification.Properties.NewState == JobState.Error ||
         jobNotification.Properties.NewState == JobState.Canceled ||
         jobNotification.Properties.NewState == JobState.Finished))
    {
        responseData = MediaClient.PublishJob(jobNotification);
        log.Info(string.Concat("Job Publication: ", responseData));
    }

    return req.CreateResponse(HttpStatusCode.OK, responseData);
}

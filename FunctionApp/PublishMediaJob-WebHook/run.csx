#r "AzureSkyMedia.PlatformServices.dll"

using System;
using System.Net;
using System.Text;

using AzureSkyMedia.PlatformServices;

public static async Task<object> Run(HttpRequestMessage req, TraceWriter log)
{
    byte[] requestMessage = await req.Content.ReadAsByteArrayAsync();
    string jobNotification = Encoding.UTF8.GetString(requestMessage);
    log.Info($"Job Notification: {jobNotification}");

    string jobPublication = MediaClient.PublishJob(jobNotification, true);
    log.Info($"Job Publication: {jobPublication}");

    return req.CreateResponse(HttpStatusCode.OK, jobPublication);
}

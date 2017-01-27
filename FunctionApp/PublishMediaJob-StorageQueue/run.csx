#r "AzureSkyMedia.PlatformServices.dll"

using System;

using AzureSkyMedia.PlatformServices;

public static void Run(string jobNotification, TraceWriter log)
{
    log.Info($"Job Notification: {jobNotification}");

    string jobPublication = MediaClient.PublishJob(jobNotification, false);

    log.Info($"Job Publication: {jobPublication}");
}

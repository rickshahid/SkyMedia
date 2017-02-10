#r "Newtonsoft.Json"
#r "..\bin\AzureSkyMedia.PlatformServices.dll"

using System;

using Newtonsoft.Json;

using AzureSkyMedia.PlatformServices;

public static void Run(string queueMessage, TraceWriter log)
{
    log.Info($"Queue Message: {queueMessage}");
    if (queueMessage.StartsWith("{"))
    {
        MediaJobNotification jobNotification = JsonConvert.DeserializeObject<MediaJobNotification>(queueMessage);
        if (jobNotification != null)
        {
            string jobPublication = MediaClient.PublishJob(jobNotification, false);
            log.Info($"Job Publication: {jobPublication}");
        }
    }
}

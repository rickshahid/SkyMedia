#r "Newtonsoft.Json"
#r "..\bin\AzureSkyMedia.PlatformServices.dll"

using System.Net;

using Newtonsoft.Json;

using AzureSkyMedia.PlatformServices;

public static async Task<object> Run(HttpRequestMessage req, TraceWriter log)
{
    string webMessage = await req.Content.ReadAsStringAsync();
    log.Info($"Web Message: {webMessage}");
    if (webMessage.StartsWith("{"))
    {
        MediaJobNotification jobNotification = JsonConvert.DeserializeObject<MediaJobNotification>(webMessage);
        if (jobNotification != null)
        {
            JobPublication jobPublication = MediaClient.PublishJob(jobNotification, true);
            string logMessage = !string.IsNullOrEmpty(jobPublication.ErrorMessage) ? jobPublication.ErrorMessage : jobPublication.UserMessage;
            log.Info($"Job Publication: {logMessage}");
        }
    }
    return req.CreateResponse(HttpStatusCode.OK);
}

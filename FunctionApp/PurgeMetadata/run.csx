#r "..\bin\AzureSkyMedia.PlatformServices.dll"

using AzureSkyMedia.PlatformServices;

public static void Run(TimerInfo timer, TraceWriter log)
{
    MediaClient.PurgeMetadata();
}

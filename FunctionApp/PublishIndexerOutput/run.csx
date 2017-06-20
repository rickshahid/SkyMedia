#r "..\bin\AzureSkyMedia.PlatformServices.dll"

using System.Net;

using AzureSkyMedia.PlatformServices;

public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, TraceWriter log)
{
    IEnumerable<KeyValuePair<string, string>> nameValuePairs = req.GetQueryNameValuePairs();
    string indexId = nameValuePairs.FirstOrDefault(q => string.Compare(q.Key, "id", true) == 0).Value;
    log.Info($"Index Id: {indexId}");
    if (!string.IsNullOrEmpty(indexId))
    {
        string assetId = IndexerClient.PublishIndex(indexId);
        log.Info($"Asset Id: {assetId}");
    }
    return req.CreateResponse(HttpStatusCode.OK);
}

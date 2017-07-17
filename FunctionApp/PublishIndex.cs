using System.Net;
using System.Net.Http;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.WebJobs.Extensions.Http;

using AzureSkyMedia.PlatformServices;

namespace AzureSkyMedia.FunctionApp
{
    public static class PublishIndex
    {
        [FunctionName("PublishIndex")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            IEnumerable<KeyValuePair<string, string>> nameValuePairs = req.GetQueryNameValuePairs();
            string indexId = nameValuePairs.FirstOrDefault(q => string.Compare(q.Key, "id", true) == 0).Value;
            log.Info($"Index Id: {indexId}");
            if (!string.IsNullOrEmpty(indexId))
            {
                string assetId = MediaClient.PublishIndex(indexId);
                log.Info($"Asset Id: {assetId}");
            }
            return req.CreateResponse(HttpStatusCode.OK);
        }
    }
}

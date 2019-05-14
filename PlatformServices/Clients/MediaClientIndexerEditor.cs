using System.Net.Http;

using Newtonsoft.Json.Linq;

namespace AzureSkyMedia.PlatformServices
{
    internal partial class MediaClient
    {
        public JObject IndexerGetProjects(int? pageSize, int? skipCount)
        {
            JObject insights;
            string requestUrl = GetRequestUrl("/projects", null, null);
            if (pageSize.HasValue)
            {
                requestUrl = string.Concat(requestUrl, "&pageSize=", pageSize.Value);
            }
            if (skipCount.HasValue)
            {
                requestUrl = string.Concat(requestUrl, "&skip=", skipCount.Value);
            }
            using (WebClient webClient = new WebClient(MediaAccount.VideoIndexerKey))
            {
                HttpRequestMessage webRequest = webClient.GetRequest(HttpMethod.Get, requestUrl);
                insights = webClient.GetResponse<JObject>(webRequest);
            }
            return insights;
        }

        public JArray IndexerGetProjects()
        {
            //bool lastPage;
            //JObject insights = null;
            JArray allInsights = new JArray();
            //do
            //{
            //    int skipCount = 0;
            //    if (insights != null)
            //    {
            //        skipCount = (int)insights["nextPage"]["skip"];
            //    }
            //    insights = IndexerGetProjects(null, skipCount);
            //    lastPage = (bool)insights["nextPage"]["done"];
            //    foreach (JToken insight in insights["results"])
            //    {
            //        allInsights.Add(insight);
            //    }
            //} while (!lastPage);
            return allInsights;
        }

        public void IndexerDeleteProject(string projectId)
        {
            string requestUrl = GetRequestUrl("/projects/", projectId, null);
            using (WebClient webClient = new WebClient(MediaAccount.VideoIndexerKey))
            {
                HttpRequestMessage webRequest = webClient.GetRequest(HttpMethod.Delete, requestUrl);
                HttpResponseMessage webResponse = webClient.GetResponse<HttpResponseMessage>(webRequest);
            }
        }
    }
}
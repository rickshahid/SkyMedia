using System.Net.Http;

using Newtonsoft.Json.Linq;

namespace AzureSkyMedia.PlatformServices
{
    internal partial class MediaClient
    {
        private string GetParentPath(MediaInsightModel modelType)
        {
            string parentPath = null;
            switch (modelType)
            {
                case MediaInsightModel.People:
                    parentPath = "/customization/personModels";
                    break;
                case MediaInsightModel.Language:
                    parentPath = "/customization/language";
                    break;
                case MediaInsightModel.Brand:
                    parentPath = "/customization/brands";
                    break;
            }
            return parentPath;
        }

        public JArray IndexerGetModels(MediaInsightModel modelType)
        {
            JArray models;
            string parentPath = GetParentPath(modelType);
            string requestUrl = GetRequestUrl(parentPath, null, null);
            using (WebClient webClient = new WebClient(MediaAccount.VideoIndexerKey))
            {
                HttpRequestMessage webRequest = webClient.GetRequest(HttpMethod.Get, requestUrl);
                models = webClient.GetResponse<JArray>(webRequest);
            }
            return models;
        }

        public void IndexerDeleteModel(MediaInsightModel modelType, string modelId)
        {
            string parentPath = GetParentPath(modelType);
            string requestUrl = GetRequestUrl(parentPath, modelId, null);
            using (WebClient webClient = new WebClient(MediaAccount.VideoIndexerKey))
            {
                HttpRequestMessage webRequest = webClient.GetRequest(HttpMethod.Delete, requestUrl);
                HttpResponseMessage webResponse = webClient.GetResponse<HttpResponseMessage>(webRequest);
            }
        }
    }
}
using System.Net.Http;
using System.IdentityModel.Tokens.Jwt;

using Microsoft.Azure.CognitiveServices.ContentModerator;

namespace AzureSkyMedia.PlatformServices
{
    internal partial class CognitiveClient
    {
        private static JwtSecurityToken GetAuthToken(bool neuralVoice)
        {
            string subscriptionKey = "f602dc999fb14ac792bae999deeddd06";
            WebClient webClient = new WebClient(subscriptionKey);
            string requestUrl = "https://westus.api.cognitive.microsoft.com/sts/v1.0/issueToken";
            if (neuralVoice)
            {
                subscriptionKey = "abc343b8b2fa4b94bee41768bb8a6e09";
                webClient = new WebClient(subscriptionKey);
                requestUrl = "https://eastus.api.cognitive.microsoft.com/sts/v1.0/issueToken";
            }
            HttpRequestMessage webRequest = webClient.GetRequest(HttpMethod.Post, requestUrl);
            return webClient.GetResponse<JwtSecurityToken>(webRequest);
        }
    }
}
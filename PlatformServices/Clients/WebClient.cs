using System;
using System.Net.Http;
using System.IdentityModel.Tokens.Jwt;

using Newtonsoft.Json.Linq;

namespace AzureSkyMedia.PlatformServices
{
    internal class WebClient : IDisposable
    {
        private HttpClient _httpClient;

        public WebClient()
        {
            _httpClient = new HttpClient();
        }

        public WebClient(string subscriptionKey) : this()
        {
            _httpClient.DefaultRequestHeaders.Add(Constant.AuthIntegration.ApiManagementKey, subscriptionKey);
        }

        public WebClient(JwtSecurityToken authToken) : this()
        {
            string authHeader = string.Concat(Constant.AuthIntegration.AuthScheme, " ", authToken.EncodedPayload);
            _httpClient.DefaultRequestHeaders.Add(Constant.AuthIntegration.AuthHeader, authHeader);
        }

        private T MapResponse<T>(string responseContent)
        {
            T responseData = default(T);
            if (typeof(T) == typeof(string))
            {
                responseContent = responseContent.Trim('\"');
                responseData = (T)Convert.ChangeType(responseContent, typeof(T));
            }
            else if (typeof(T) == typeof(JwtSecurityToken))
            {
                JwtSecurityToken authToken = new JwtSecurityToken(responseContent);
                responseData = (T)Convert.ChangeType(authToken, typeof(T));
            }
            else if (typeof(T) == typeof(JObject))
            {
                JObject jsonData = JObject.Parse(responseContent);
                responseData = (T)Convert.ChangeType(jsonData, typeof(T));
            }
            else if (typeof(T) == typeof(JArray))
            {
                JArray jsonData = JArray.Parse(responseContent);
                responseData = (T)Convert.ChangeType(jsonData, typeof(T));
            }
            return responseData;
        }

        public HttpRequestMessage GetRequest(HttpMethod requestMethod, string requestUrl)
        {
            return new HttpRequestMessage(requestMethod, requestUrl);
        }

        public T GetResponse<T>(HttpRequestMessage webRequest)
        {
            T responseData = default(T);
            using (HttpResponseMessage webResponse = _httpClient.SendAsync(webRequest).Result)
            {
                webResponse.EnsureSuccessStatusCode();
                if (typeof(T) == typeof(byte[]))
                {
                    byte[] responseContent = webResponse.Content.ReadAsByteArrayAsync().Result;
                    responseData = (T)Convert.ChangeType(responseContent, typeof(T));
                }
                else
                {
                    string responseContent = webResponse.Content.ReadAsStringAsync().Result;
                    responseData = MapResponse<T>(responseContent);
                }
            }
            return responseData;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && _httpClient != null)
            {
                _httpClient.Dispose();
                _httpClient = null;
            }
        }
    }
}
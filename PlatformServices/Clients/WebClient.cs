using System;
using System.Text;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AzureSkyMedia.PlatformServices
{
    internal class WebClient
    {
        private string _authKey;
        private string _authToken;
        private NetworkCredential _authCredentials;

        public WebClient(string authKey)
        {
            _authKey = authKey;
        }

        public WebClient(JwtSecurityToken authToken)
        {
            _authToken = authToken.EncodedPayload;
        }

        public WebClient(string accountId, string accountKey)
        {
            _authCredentials = new NetworkCredential(accountId, accountKey);
        }

        private T GetResponseData<T>(HttpResponseMessage response)
        {
            T responseData;
            Task<string> responseContent = response.Content.ReadAsStringAsync();
            string responseText = responseContent.Result;
            if (typeof(T) == typeof(string))
            {
                responseText = responseText.Replace("\"", "");
                responseData = (T)Convert.ChangeType(responseText, typeof(T));
            }
            else if (typeof(T) == typeof(JObject))
            {
                responseData = (T)Convert.ChangeType(JObject.Parse(responseText), typeof(T));
            }
            else if (typeof(T) == typeof(JArray))
            {
                responseData = (T)Convert.ChangeType(JArray.Parse(responseText), typeof(T));
            }
            else
            {
                responseData = JsonConvert.DeserializeObject<T>(responseText);
            }
            return responseData;
        }

        public HttpRequestMessage GetRequest(HttpMethod requestMethod, string requestUrl, object requestData)
        {
            HttpRequestMessage request = new HttpRequestMessage(requestMethod, requestUrl);
            if (requestData != null)
            {
                string requestJson = JsonConvert.SerializeObject(requestData);
                byte[] requestBytes = Encoding.UTF8.GetBytes(requestJson);
                request.Content = new ByteArrayContent(requestBytes);
            }
            return request;
        }

        public HttpRequestMessage GetRequest(HttpMethod requestMethod, string requestUrl)
        {
            return GetRequest(requestMethod, requestUrl, null);
        }

        public T GetResponse<T>(HttpRequestMessage request, out HttpStatusCode statusCode)
        {
            HttpClient client;
            T responseData = default(T);
            if (_authCredentials != null)
            {
                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.Credentials = _authCredentials;
                client = new HttpClient(clientHandler);
            }
            else
            {
                client = new HttpClient();
            }
            if (!string.IsNullOrEmpty(_authKey))
            {
                client.DefaultRequestHeaders.Add(Constant.HttpHeader.ApiKey, _authKey);
            }
            if (!string.IsNullOrEmpty(_authToken))
            {
                string authHeader = string.Concat(Constant.HttpHeader.AuthPrefix, _authToken);
                client.DefaultRequestHeaders.Add(Constant.HttpHeader.AuthHeader, authHeader);
            }
            using (client)
            {
                Task<HttpResponseMessage> task = client.SendAsync(request);
                using (HttpResponseMessage response = task.Result)
                {
                    statusCode = response.StatusCode;
                    if (response.IsSuccessStatusCode)
                    {
                        responseData = GetResponseData<T>(response);
                    }
                }
            }
            return responseData;
        }

        public T GetResponse<T>(HttpRequestMessage request)
        {
            HttpStatusCode statusCode;
            return GetResponse<T>(request, out statusCode);
        }
    }
}
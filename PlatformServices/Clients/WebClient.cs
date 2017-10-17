using System;
using System.Net;
using System.Net.Http;
using System.IdentityModel.Tokens.Jwt;

using RequestEncoding = System.Text.UTF8Encoding;

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

        private T GetResponseData<T>(HttpContent responseContent)
        {
            T responseData;
            string responseText = responseContent.ReadAsStringAsync().Result;
            if (typeof(T) == typeof(string))
            {
                responseText = responseText.Trim('\"');
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
            HttpRequestMessage requestMessage = new HttpRequestMessage(requestMethod, requestUrl);
            if (requestData != null)
            {
                string requestJson = JsonConvert.SerializeObject(requestData);
                byte[] requestBytes = RequestEncoding.UTF8.GetBytes(requestJson);
                requestMessage.Content = new ByteArrayContent(requestBytes);
            }
            return requestMessage;
        }

        public HttpRequestMessage GetRequest(HttpMethod requestMethod, string requestUrl)
        {
            return GetRequest(requestMethod, requestUrl, null);
        }

        public T GetResponse<T>(HttpRequestMessage requestMessage, out HttpStatusCode statusCode)
        {
            HttpClient httpClient;
            T responseData = default(T);
            if (_authCredentials != null)
            {
                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.Credentials = _authCredentials;
                httpClient = new HttpClient(clientHandler);
            }
            else
            {
                httpClient = new HttpClient();
            }
            if (!string.IsNullOrEmpty(_authKey))
            {
                httpClient.DefaultRequestHeaders.Add(Constant.HttpHeader.ApiKey, _authKey);
            }
            else if (!string.IsNullOrEmpty(_authToken))
            {
                string authHeader = string.Concat(Constant.HttpHeader.AuthPrefix, _authToken);
                httpClient.DefaultRequestHeaders.Add(Constant.HttpHeader.AuthHeader, authHeader);
            }
            using (HttpResponseMessage responseMessage = httpClient.SendAsync(requestMessage).Result)
            {
                statusCode = responseMessage.StatusCode;
                if (responseMessage.IsSuccessStatusCode)
                {
                    HttpContent responseContent = responseMessage.Content;
                    responseData = GetResponseData<T>(responseContent);
                }
            }
            httpClient.Dispose();
            return responseData;
        }

        public T GetResponse<T>(HttpRequestMessage requestMessage)
        {
            return GetResponse<T>(requestMessage, out HttpStatusCode statusCode);
        }
    }
}

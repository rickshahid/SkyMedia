using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AzureSkyMedia.PlatformServices
{
    internal class WebClient
    {
        private string _apimKey;

        public WebClient(string apimKey)
        {
            _apimKey = apimKey;
        }

        public static string GetData(string requestUrl)
        {
            WebClient webClient = new WebClient(null);
            HttpRequestMessage requestMessage = webClient.GetRequest(HttpMethod.Get, requestUrl);
            return webClient.GetResponse<string>(requestMessage, out HttpStatusCode statusCode);
        }

        public static Stream GetStream(string requestUrl)
        {
            WebClient webClient = new WebClient(null);
            HttpRequestMessage requestMessage = webClient.GetRequest(HttpMethod.Get, requestUrl);
            return webClient.GetResponse(requestMessage, out HttpStatusCode statusCode);
        }

        private HttpClient GetClient()
        {
            HttpClient httpClient = new HttpClient();
            if (!string.IsNullOrEmpty(_apimKey))
            {
                httpClient.DefaultRequestHeaders.Add(Constant.HttpHeader.ApiManagementKey, _apimKey);
            }
            return httpClient;
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
            if (requestUrl.StartsWith("//"))
            {
                requestUrl = string.Concat("https:", requestUrl);
            }
            HttpRequestMessage requestMessage = new HttpRequestMessage(requestMethod, requestUrl);
            if (requestData != null)
            {
                string requestJson = JsonConvert.SerializeObject(requestData);
                byte[] requestBytes = UTF8Encoding.UTF8.GetBytes(requestJson);
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
            T responseData = default(T);
            HttpClient httpClient = GetClient();
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

        private Stream GetResponse(HttpRequestMessage requestMessage, out HttpStatusCode statusCode)
        {
            Stream responseData = null;
            HttpClient httpClient = GetClient();
            HttpResponseMessage responseMessage = httpClient.SendAsync(requestMessage).Result;
            statusCode = responseMessage.StatusCode;
            if (responseMessage.IsSuccessStatusCode)
            {
                HttpContent responseContent = responseMessage.Content;
                responseData = responseContent.ReadAsStreamAsync().Result;
            }
            return responseData;
        }
    }
}
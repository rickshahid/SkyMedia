using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Net.Http;

using Microsoft.Rest;
using Microsoft.Azure.Management.Media.Models;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AzureSkyMedia.PlatformServices
{
    internal class WebClient : IDisposable
    {
        private static HttpClient _httpClient;

        public WebClient(string subscriptionKey)
        {
            if (!string.IsNullOrEmpty(subscriptionKey))
            {
                HttpClient.DefaultRequestHeaders.Add(Constant.AuthIntegration.SubscriptionKey, subscriptionKey);
            }
        }

        private HttpClient HttpClient
        {
            get
            {
                if (_httpClient == null)
                {
                    _httpClient = new HttpClient();
                }
                return _httpClient;
            }
        }

        public static string GetData(string requestUrl)
        {
            HttpClient httpClient = new HttpClient();
            return httpClient.GetStringAsync(requestUrl).Result;
        }

        public static Stream GetStream(string requestUrl)
        {
            Stream responseData = null;
            HttpClient httpClient = new HttpClient();
            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, requestUrl);
            HttpResponseMessage responseMessage = httpClient.SendAsync(requestMessage).Result;
            if (responseMessage.IsSuccessStatusCode)
            {
                HttpContent responseContent = responseMessage.Content;
                responseData = responseContent.ReadAsStreamAsync().Result;
            }
            return responseData;
        }

        public static void SendAsync(string requestUrl)
        {
            HttpClient httpClient = new HttpClient();
            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, requestUrl);
            httpClient.SendAsync(requestMessage);
        }

        private T GetResponseData<T>(string responseContent)
        {
            T responseData;
            if (typeof(T) == typeof(string))
            {
                responseContent = responseContent.Trim('\"');
                responseData = (T)Convert.ChangeType(responseContent, typeof(T));
            }
            else if (typeof(T) == typeof(XmlDocument))
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(responseContent);
                responseData = (T)Convert.ChangeType(xmlDocument, typeof(T));
            }
            else if (typeof(T) == typeof(JObject))
            {
                responseData = (T)Convert.ChangeType(JObject.Parse(responseContent), typeof(T));
            }
            else if (typeof(T) == typeof(JArray))
            {
                responseData = (T)Convert.ChangeType(JArray.Parse(responseContent), typeof(T));
            }
            else
            {
                responseData = JsonConvert.DeserializeObject<T>(responseContent);
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
                byte[] requestBytes = Encoding.UTF8.GetBytes(requestJson);
                requestMessage.Content = new ByteArrayContent(requestBytes);
            }
            return requestMessage;
        }

        public HttpRequestMessage GetRequest(HttpMethod requestMethod, string requestUrl)
        {
            return GetRequest(requestMethod, requestUrl, null);
        }

        public T GetResponse<T>(HttpRequestMessage webRequest)
        {
            T responseData = default(T);
            using (HttpResponseMessage webResponse = HttpClient.SendAsync(webRequest).Result)
            {
                string responseContent = webResponse.Content.ReadAsStringAsync().Result;
                if (!webResponse.IsSuccessStatusCode)
                {
                    throw new ApiErrorException()
                    {
                        Response = new HttpResponseMessageWrapper(webResponse, responseContent)
                    };
                }
                else
                {
                    responseData = GetResponseData<T>(responseContent);
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
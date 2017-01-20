using System;
using System.IO;
using System.Net;
using System.Text;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AzureSkyMedia.ServiceBroker
{
    internal class WebClient
    {
        private string _authToken;
        private string _apiVersion;
        private NetworkCredential _credentials;

        public WebClient(string apiVersion)
        {
            _apiVersion = apiVersion;
        }

        public WebClient(string authToken, string apiVersion) : this(apiVersion)
        {
            _authToken = authToken;
        }

        public WebClient(string userName, string userPassword, string apiVersion) : this(apiVersion)
        {
            _credentials = new NetworkCredential(userName, userPassword);
        }

        public HttpWebRequest GetRequest(string apiEndpoint, string requestType, string requestBody)
        {
            HttpWebRequest request = WebRequest.CreateHttp(apiEndpoint);
            request.Method = requestType.ToUpper();
            request.Accept = Constants.ContentType.Json;
            request.ContentType = Constants.ContentType.Json;
            if (!string.IsNullOrEmpty(_apiVersion))
            {
                request.Headers.Add(Constants.HttpHeaders.ApiVersion, _apiVersion);
            }
            if (!string.IsNullOrEmpty(_authToken))
            {
                string authHeader = string.Concat(Constants.HttpHeaders.AuthPrefix, _authToken);
                request.Headers.Add(Constants.HttpHeaders.AuthHeader, authHeader);
            }
            if (_credentials != null)
            {
                request.Credentials = _credentials;
            }
            if (!string.IsNullOrEmpty(requestBody))
            {
                using (Stream requestStream = request.GetRequestStream())
                {
                    byte[] requestBytes = Encoding.UTF8.GetBytes(requestBody);
                    requestStream.Write(requestBytes, 0, requestBytes.Length);
                }
            }
            return request;
        }

        public HttpWebRequest GetRequest<T>(string apiEndpoint, string requestType, T requestBody)
        {
            string requestJson = JsonConvert.SerializeObject(requestBody);
            return GetRequest(apiEndpoint, requestType, requestJson);
        }

        public T GetResponse<T>(HttpWebRequest request, out HttpStatusCode statusCode)
        {
            string responseBody;
            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                statusCode = response.StatusCode;
                Stream responseStream = response.GetResponseStream();
                StreamReader responseReader = new StreamReader(responseStream);
                responseBody = responseReader.ReadToEnd();
            }
            if (typeof(T) == typeof(string))
            {
                return (T)Convert.ChangeType(responseBody, typeof(T));
            }
            else if (typeof(T) == typeof(JObject))
            {
                return (T)Convert.ChangeType(JObject.Parse(responseBody), typeof(T));
            }
            else if (typeof(T) == typeof(JArray))
            {
                return (T)Convert.ChangeType(JArray.Parse(responseBody), typeof(T));
            }
            return JsonConvert.DeserializeObject<T>(responseBody);
        }

        public T GetResponse<T>(HttpWebRequest request)
        {
            HttpStatusCode statusCode;
            return GetResponse<T>(request, out statusCode);
        }
    }
}

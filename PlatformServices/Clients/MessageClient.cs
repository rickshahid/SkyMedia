using System;
using System.IO;
using System.Net;
using System.Text;
using System.Collections.Generic;

using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;

using Newtonsoft.Json;

namespace AzureSkyMedia.PlatformServices
{
    public class MessageClient
    {
        private CloudQueueClient _storage;

        public MessageClient(string authToken)
        {
            CloudStorageAccount storageAccount = Storage.GetUserAccount(authToken, null);
            BindContext(storageAccount);
        }

        public MessageClient(string authToken, string accountName)
        {
            CloudStorageAccount storageAccount = Storage.GetUserAccount(authToken, accountName);
            BindContext(storageAccount);
        }

        private void BindContext(CloudStorageAccount storageAccount)
        {
            _storage = storageAccount.CreateCloudQueueClient();

            string settingKey = Constants.AppSettings.StorageServerTimeoutSeconds;
            string serverTimeoutSeconds = AppSetting.GetValue(settingKey);

            settingKey = Constants.AppSettings.StorageMaxExecutionTimeSeconds;
            string maxExecutionTimeSeconds = AppSetting.GetValue(settingKey);

            int settingValueInt;
            if (int.TryParse(serverTimeoutSeconds, out settingValueInt))
            {
                _storage.DefaultRequestOptions.ServerTimeout = new TimeSpan(0, 0, settingValueInt);
            }

            if (int.TryParse(maxExecutionTimeSeconds, out settingValueInt))
            {
                _storage.DefaultRequestOptions.MaximumExecutionTime = new TimeSpan(0, 0, settingValueInt);
            }
        }

        private static string EncodeParameters(IDictionary<string, string> parameters)
        {
            List<string> encodedParameters = new List<string>();
            foreach (KeyValuePair<string, string> parameter in parameters)
            {
                string parameterName = WebUtility.UrlEncode(parameter.Key);
                string parameterValue = WebUtility.UrlEncode(parameter.Value);
                string encodedParameter = string.Concat(parameterName, "=", parameterValue);
                encodedParameters.Add(encodedParameter);
            }
            return string.Join("&", encodedParameters);
        }

        public static HttpWebResponse SendText(string messageText, string mobileNumber)
        {
            string settingKey = Constants.ConnectionStrings.Twilio;
            string[] accountCredentials = AppSetting.GetValue(settingKey, true);
            string accountName = accountCredentials[0];
            string accountKey = accountCredentials[1];

            settingKey = Constants.AppSettings.TwilioMessageApi;
            string messageApi = string.Format(AppSetting.GetValue(settingKey), accountName);
            HttpWebRequest httpRequest = WebRequest.CreateHttp(messageApi);
            httpRequest.Method = "POST";
            httpRequest.ContentType = Constants.ContentType.Url;

            string authToken = string.Concat(accountName, ":", accountKey);
            byte[] authBytes = Encoding.UTF8.GetBytes(authToken);
            authToken = string.Concat("Basic ", Convert.ToBase64String(authBytes));
            httpRequest.Headers.Add("Authorization", authToken);

            Dictionary<string, string> parameters = new Dictionary<string, string>();

            string parameterName = "From";
            settingKey = Constants.AppSettings.TwilioMessageFrom;
            string parameterValue = AppSetting.GetValue(settingKey);
            parameters.Add(parameterName, parameterValue);

            parameterName = "To";
            parameterValue = mobileNumber;
            parameters.Add(parameterName, parameterValue);

            parameterName = "Body";
            parameterValue = messageText;
            parameters.Add(parameterName, parameterValue);

            string requestParams = EncodeParameters(parameters);
            httpRequest.ContentLength = Encoding.UTF8.GetByteCount(requestParams);
            using (StreamWriter requestWriter = new StreamWriter(httpRequest.GetRequestStream()))
            {
                requestWriter.Write(requestParams);
            }

            return (HttpWebResponse)httpRequest.GetResponse();
        }

        public CloudQueue GetQueue(string queueName, bool enableCreate)
        {
            CloudQueue queue = _storage.GetQueueReference(queueName);
            if (enableCreate)
            {
                queue.CreateIfNotExists();
            }
            return queue;
        }

        public T GetMessage<T>(string queueName, TimeSpan? visibilityTimeout, out CloudQueueMessage queueMessage) where T: class
        {
            T message = null;
            queueMessage = null;
            CloudQueue queue = GetQueue(queueName, false);
            if (queue.Exists())
            {
                queueMessage = queue.GetMessage(visibilityTimeout);
                if (queueMessage != null)
                {
                    message = queueMessage.Deserialize<T>();
                }
            }
            return message;
        }

        public T GetMessage<T>(string queueName, out CloudQueueMessage queueMessage) where T : class
        {
            return GetMessage<T>(queueName, null, out queueMessage);
        }

        public T GetMessage<T>(string queueName) where T : class
        {
            CloudQueueMessage queueMessage;
            return GetMessage<T>(queueName, null, out queueMessage);
        }

        public void AddMessage<T>(string queueName, T message, TimeSpan? timeToLive, TimeSpan? initialVisibilityDelay) where T : class
        {
            CloudQueue queue = GetQueue(queueName, true);
            CloudQueueMessage queueMessage = message.Serialize();
            queue.AddMessage(queueMessage, timeToLive, initialVisibilityDelay);
        }

        public void AddMessage<T>(string queueName, T message) where T : class
        {
            AddMessage<T>(queueName, message, null, null);
        }

        public void DeleteMessage(string queueName, CloudQueueMessage queueMessage)
        {
            CloudQueue queue = GetQueue(queueName, false);
            if (queue.Exists())
            {
                queue.DeleteMessage(queueMessage);
            }
        }

        public void DeleteMessage(string queueName, string messageId, string popReceipt)
        {
            CloudQueue queue = GetQueue(queueName, false);
            if (queue.Exists())
            {
                queue.DeleteMessage(messageId, popReceipt);
            }
        }
    }

    internal static class CloudQueueMessageExtension
    {
        public static CloudQueueMessage Serialize<T>(this T message)
        {
            string messageContent = JsonConvert.SerializeObject(message);
            return new CloudQueueMessage(messageContent);
        }

        public static T Deserialize<T>(this CloudQueueMessage queueMessage)
        {
            string messageContent = queueMessage.AsString;
            return JsonConvert.DeserializeObject<T>(messageContent);
        }
    }
}

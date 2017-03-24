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

        public MessageClient()
        {
            CloudStorageAccount storageAccount = Storage.GetSystemAccount();
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
            string settingKey = Constant.AppSettingKey.Twilio;
            string[] accountCredentials = AppSetting.GetValue(settingKey, true);
            string accountName = accountCredentials[0];
            string accountKey = accountCredentials[1];

            settingKey = Constant.AppSettingKey.TwilioMessageApi;
            string messageApi = string.Format(AppSetting.GetValue(settingKey), accountName);
            HttpWebRequest httpRequest = WebRequest.CreateHttp(messageApi);
            httpRequest.Method = "POST";
            httpRequest.ContentType = Constant.ContentType.Url;

            string authToken = string.Concat(accountName, ":", accountKey);
            byte[] authBytes = Encoding.UTF8.GetBytes(authToken);
            authToken = string.Concat("Basic ", Convert.ToBase64String(authBytes));
            httpRequest.Headers.Add("Authorization", authToken);

            Dictionary<string, string> parameters = new Dictionary<string, string>();

            string parameterName = "From";
            settingKey = Constant.AppSettingKey.TwilioMessageFrom;
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

        public string GetMessage(string queueName, TimeSpan? visibilityTimeout, out CloudQueueMessage queueMessage, out string messageId, out string popReceipt)
        {
            queueMessage = null;
            messageId = null;
            popReceipt = null;
            string message = string.Empty;
            CloudQueue queue = GetQueue(queueName, false);
            if (queue.Exists())
            {
                queueMessage = queue.GetMessage(visibilityTimeout);
                if (queueMessage != null)
                {
                    message = queueMessage.AsString;
                    messageId = queueMessage.Id;
                    popReceipt = queueMessage.PopReceipt;
                }
            }
            return message;
        }

        public string GetMessage(string queueName, out string messageId, out string popReceipt)
        {
            CloudQueueMessage queueMessage;
            return GetMessage(queueName, null, out queueMessage, out messageId, out popReceipt);
        }

        public string GetMessage(string queueName, out CloudQueueMessage queueMessage)
        {
            string messageId;
            string popReceipt;
            return GetMessage(queueName, null, out queueMessage, out messageId, out popReceipt);
        }

        public string GetMessage(string queueName)
        {
            CloudQueueMessage queueMessage;
            string messageId;
            string popReceipt;
            return GetMessage(queueName, null, out queueMessage, out messageId, out popReceipt);
        }

        public void AddMessage(string queueName, object message, TimeSpan? timeToLive, TimeSpan? initialVisibilityDelay)
        {
            CloudQueue queue = GetQueue(queueName, true);
            string messageText = JsonConvert.SerializeObject(message);
            CloudQueueMessage queueMessage = new CloudQueueMessage(messageText);
            queue.AddMessage(queueMessage, timeToLive, initialVisibilityDelay);
        }

        public void AddMessage(string queueName, object message)
        {
            AddMessage(queueName, message, null, null);
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
}

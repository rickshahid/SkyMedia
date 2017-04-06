using System;

using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
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

        public MessageClient(string[] accountCredentials)
        {
            string accountName = accountCredentials[0];
            string accountKey = accountCredentials[1];
            StorageCredentials storageCredentials = new StorageCredentials(accountName, accountKey);
            CloudStorageAccount storageAccount = new CloudStorageAccount(storageCredentials, true);
            BindContext(storageAccount);
        }

        private void BindContext(CloudStorageAccount storageAccount)
        {
            _storage = storageAccount.CreateCloudQueueClient();
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

        public string AddMessage(string queueName, object message, TimeSpan? timeToLive, TimeSpan? initialVisibilityDelay)
        {
            CloudQueue queue = GetQueue(queueName, true);
            string messageText = JsonConvert.SerializeObject(message);
            CloudQueueMessage queueMessage = new CloudQueueMessage(messageText);
            queue.AddMessage(queueMessage, timeToLive, initialVisibilityDelay);
            return messageText;
        }

        public string AddMessage(string queueName, object message)
        {
            return AddMessage(queueName, message, null, null);
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

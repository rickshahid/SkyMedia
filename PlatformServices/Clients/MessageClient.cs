using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;

using Newtonsoft.Json;

namespace AzureSkyMedia.PlatformServices
{
    internal class MessageClient
    {
        private CloudQueueClient _storage;

        public MessageClient()
        {
            CloudStorageAccount storageAccount = Storage.GetSystemAccount();
            _storage = storageAccount.CreateCloudQueueClient();
        }

        private CloudQueue GetQueue(string queueName)
        {
            CloudQueue queue = _storage.GetQueueReference(queueName);
            queue.CreateIfNotExists();
            return queue;
        }

        private CloudQueueMessage GetQueueMessage(string queueName)
        {
            CloudQueue queue = GetQueue(queueName);
            return queue.GetMessage();
        }

        public string GetMessage(string queueName, out string messageId, out string popReceipt)
        {
            string message = string.Empty;
            messageId = string.Empty;
            popReceipt = string.Empty;
            CloudQueueMessage queueMessage = GetQueueMessage(queueName);
            if (queueMessage != null)
            {
                message = queueMessage.AsString;
                messageId = queueMessage.Id;
                popReceipt = queueMessage.PopReceipt;
            }
            return message;
        }

        public string GetMessage(string queueName, out CloudQueueMessage queueMessage)
        {
            string message = string.Empty;
            queueMessage = GetQueueMessage(queueName);
            if (queueMessage != null)
            {
                message = queueMessage.AsString;
            }
            return message;
        }

        public string GetMessage(string queueName)
        {
            CloudQueueMessage queueMessage;
            return GetMessage(queueName, out queueMessage);
        }

        public string AddMessage(string queueName, object message)
        {
            CloudQueue queue = GetQueue(queueName);
            string messageContent = JsonConvert.SerializeObject(message);
            CloudQueueMessage queueMessage = new CloudQueueMessage(messageContent);
            queue.AddMessage(queueMessage);
            return queueMessage.AsString;
        }

        public void DeleteMessage(string queueName, CloudQueueMessage queueMessage)
        {
            CloudQueue queue = GetQueue(queueName);
            queue.DeleteMessage(queueMessage);
        }

        public void DeleteMessage(string queueName, string messageId, string popReceipt)
        {
            CloudQueue queue = GetQueue(queueName);
            queue.DeleteMessage(messageId, popReceipt);
        }
    }
}

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

        public T GetMessage<T>(string queueName, out string messageId, out string popReceipt)
        {
            T message = default(T);
            messageId = string.Empty;
            popReceipt = string.Empty;
            CloudQueueMessage queueMessage = GetQueueMessage(queueName);
            if (queueMessage != null)
            {
                messageId = queueMessage.Id;
                popReceipt = queueMessage.PopReceipt;
                message = JsonConvert.DeserializeObject<T>(queueMessage.AsString);
            }
            return message;
        }

        public T GetMessage<T>(string queueName)
        {
            string messageId;
            string popReceipt;
            return GetMessage<T>(queueName, out messageId, out popReceipt);
        }

        public string AddMessage(string queueName, object message)
        {
            CloudQueue queue = GetQueue(queueName);
            string messageContent = JsonConvert.SerializeObject(message);
            CloudQueueMessage queueMessage = new CloudQueueMessage(messageContent);
            queue.AddMessage(queueMessage);
            return queueMessage.AsString;
        }

        public void DeleteMessage(string queueName, string messageId, string popReceipt)
        {
            CloudQueue queue = GetQueue(queueName);
            queue.DeleteMessage(messageId, popReceipt);
        }
    }
}
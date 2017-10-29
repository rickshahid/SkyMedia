using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;

using Newtonsoft.Json;

namespace AzureSkyMedia.PlatformServices
{
    internal class QueueClient
    {
        private CloudQueueClient _storage;

        public QueueClient()
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

        public T GetMessage<T>(string queueName, out string messageId, out string popReceipt)
        {
            T message = default(T);
            messageId = string.Empty;
            popReceipt = string.Empty;
            CloudQueue queue = GetQueue(queueName);
            CloudQueueMessage queueMessage = queue.GetMessage();
            if (queueMessage != null)
            {
                messageId = queueMessage.Id;
                popReceipt = queueMessage.PopReceipt;
                message = JsonConvert.DeserializeObject<T>(queueMessage.AsString);
            }
            return message;
        }

        public string AddMessage(string queueName, object messageData)
        {
            CloudQueue queue = GetQueue(queueName);
            string messageJson = JsonConvert.SerializeObject(messageData);
            CloudQueueMessage queueMessage = new CloudQueueMessage(messageJson);
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

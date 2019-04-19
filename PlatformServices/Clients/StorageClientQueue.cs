using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Queue;

using Newtonsoft.Json;

namespace AzureSkyMedia.PlatformServices
{
    internal partial class StorageQueueClient
    {
        private CloudQueueClient _queueClient;

        public StorageQueueClient()
        {
            CloudStorageAccount storageAccount = Account.GetStorageAccount();
            _queueClient = storageAccount.CreateCloudQueueClient();
        }

        private CloudQueue GetQueue(string queueName)
        {
            CloudQueue queue = _queueClient.GetQueueReference(queueName);
            queue.CreateIfNotExistsAsync().Wait();
            return queue;
        }

        public T GetMessage<T>(string queueName, out string messageId, out string popReceipt)
        {
            T message = default(T);
            messageId = string.Empty;
            popReceipt = string.Empty;
            CloudQueue queue = GetQueue(queueName);
            CloudQueueMessage queueMessage = queue.GetMessageAsync().Result;
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
            queue.AddMessageAsync(queueMessage).Wait();
            return queueMessage.AsString;
        }

        public void DeleteMessage(string queueName, string messageId, string popReceipt)
        {
            CloudQueue queue = GetQueue(queueName);
            queue.DeleteMessageAsync(messageId, popReceipt).Wait();
        }
    }
}
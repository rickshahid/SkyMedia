using System.Threading.Tasks;

using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Queue;

namespace AzureSkyMedia.PlatformServices
{
    internal class StorageQueueClient
    {
        private CloudQueueClient _queueClient;

        public StorageQueueClient()
        {
            CloudStorageAccount storageAccount = Account.GetStorageAccount();
            _queueClient = storageAccount.CreateCloudQueueClient();
        }

        private async Task<CloudQueue> GetQueue(string queueName)
        {
            CloudQueue queue = _queueClient.GetQueueReference(queueName);
            await queue.CreateIfNotExistsAsync();
            return queue;
        }

        public async Task<CloudQueueMessage> GetMessage(string queueName)
        {
            CloudQueue queue = await GetQueue(queueName);
            return await queue.GetMessageAsync();
        }

        public async Task<CloudQueueMessage> AddMessage(string queueName, string messageContent)
        {
            CloudQueue queue = await GetQueue(queueName);
            CloudQueueMessage queueMessage = new CloudQueueMessage(messageContent);
            await queue.AddMessageAsync(queueMessage);
            return queueMessage;
        }

        public async Task DeleteMessage(string queueName, CloudQueueMessage queueMessage)
        {
            CloudQueue queue = await GetQueue(queueName);
            await queue.DeleteMessageAsync(queueMessage);
        }
    }
}
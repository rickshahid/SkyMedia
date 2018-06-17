using System.Threading.Tasks;

using Microsoft.Rest.Azure;
using Microsoft.Azure.Management.Media.Models;

namespace AzureSkyMedia.PlatformServices
{
    internal partial class MediaClient
    {
        public Transform CreateTransform(string name, string description, TransformOutput[] outputs)
        {
            Task<AzureOperationResponse<Transform>> createTask = _media.Transforms.CreateOrUpdateWithHttpMessagesAsync(MediaAccount.ResourceGroupName, MediaAccount.Name, name, outputs, description);
            AzureOperationResponse<Transform> createResponse = createTask.Result;
            return createResponse.Body;
        }
    }
}
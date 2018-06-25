using System.Threading.Tasks;

using Microsoft.Rest.Azure;
using Microsoft.Azure.Management.Media.Models;

namespace AzureSkyMedia.PlatformServices
{
    internal partial class MediaClient
    {
        public Transform CreateTransform(string transformName, string transformDescription, TransformOutput[] transformOutputs)
        {
            Task<AzureOperationResponse<Transform>> task = _media.Transforms.CreateOrUpdateWithHttpMessagesAsync(MediaAccount.ResourceGroupName, MediaAccount.Name, transformName, transformOutputs, transformDescription);
            AzureOperationResponse<Transform> response = task.Result;
            return response.Body;
        }
    }
}
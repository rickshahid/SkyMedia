using Microsoft.Azure.Management.Media;
using Microsoft.Azure.Management.Media.Models;

namespace AzureSkyMedia.PlatformServices
{
    internal partial class MediaClient
    {
        public Transform CreateTransform(string transformName, string transformDescription, TransformOutput[] transformOutputs)
        {
            return _media.Transforms.CreateOrUpdate(MediaAccount.ResourceGroupName, MediaAccount.Name, transformName, transformOutputs, transformDescription);
        }
    }
}
using Microsoft.Azure.Management.Media;
using Microsoft.Azure.Management.Media.Models;

namespace AzureSkyMedia.PlatformServices
{
    internal partial class MediaClient
    {
        public AccountFilter CreateFilter(string filterName)
        {
            AccountFilter accountFilter = new AccountFilter();
            return _media.AccountFilters.CreateOrUpdate(MediaAccount.ResourceGroupName, MediaAccount.Name, filterName, accountFilter);
        }

        public AssetFilter CreateFilter(string assetName, string filterName)
        {
            AssetFilter assetFilter = new AssetFilter();
            return _media.AssetFilters.CreateOrUpdate(MediaAccount.ResourceGroupName, MediaAccount.Name, assetName, filterName, assetFilter);
        }
    }
}
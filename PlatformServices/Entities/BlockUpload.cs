using System;

using Microsoft.WindowsAzure.Storage.Table;

namespace AzureSkyMedia.PlatformServices
{
    public class StorageEntity : TableEntity
    {
        public DateTime? CreatedOn { get; set; }
    }

    internal class BlockUpload : StorageEntity
    {
        private string _blocks;
        private string[] _blockIds;

        public string Blocks
        {
            get { return _blocks; }

            set
            {
                _blocks = value;
                _blockIds = (_blocks == null) ? null : _blocks.Split(Constants.MultiItemSeparator);
            }
        }

        public string[] BlockIds
        {
            get { return _blockIds; }

            set
            {
                _blockIds = value;
                _blocks = (_blockIds == null) ? null : string.Join(Constants.MultiItemSeparator.ToString(), _blockIds);
            }
        }
    }
}

namespace AzureSkyMedia.PlatformServices
{
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
                _blockIds = null;
                if (_blocks != null)
                {
                    _blockIds = _blocks.Split(Constant.TextDelimiter.Application);
                }
            }
        }

        public string[] BlockIds
        {
            get { return _blockIds; }

            set
            {
                _blocks = null;
                _blockIds = value;
                if (_blockIds != null)
                {
                    _blocks = string.Join(Constant.TextDelimiter.Application.ToString(), _blockIds);
                }
            }
        }
    }
}

namespace AzureSkyMedia.PlatformServices
{
    public struct MediaThumbnails
    {
        public MediaThumbnailFormat Format { get; set; }

        public string Width { get; set; }

        public string Height { get; set; }

        public int? Quality { get; set; }

        public string Start { get; set; }

        public string Step { get; set; }

        public string Range { get; set; }

        public int? Columns { get; set; }
    }
}
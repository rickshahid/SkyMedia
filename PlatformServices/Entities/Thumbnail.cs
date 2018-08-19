namespace AzureSkyMedia.PlatformServices
{
    public class ThumbnailGeneration
    {
        public MediaImageFormat Format { get; set; }

        public bool Best { get; set; }

        public int? Columns { get; set; }

        public string Width { get; set; }

        public string Height { get; set; }

        public string Start { get; set; }

        public string Step { get; set; }

        public string Range { get; set; }
    }
}
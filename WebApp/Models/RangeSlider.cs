namespace AzureSkyMedia.WebApp.Models
{
    public class RangeSlider
    {
        public string SliderId { get; set; }

        public string SliderClass { get; set; }

        public string LabelId { get; set; }

        public string LabelClass { get; set; }

        public int SkipCount { get; set; }

        public int ValueCount { get; set; }

        public bool LastPage { get; set; }
    }
}
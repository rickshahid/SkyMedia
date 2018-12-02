namespace AzureSkyMedia.WebApp.Models
{
    public class RangeSlider
    {
        public string RangeId { get; set; }

        public string RangeClass { get; set; }

        public string ValueId { get; set; }

        public string ValueClass { get; set; }

        public int SkipCount { get; set; }

        public int ValueCount { get; set; }

        public bool LastPage { get; set; }
    }
}
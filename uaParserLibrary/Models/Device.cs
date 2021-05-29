namespace uaParserLibrary.Models
{
    public sealed class Device
    {
        public string Vendor { get; set; }
        public string Type { get; set; }
        public string Model { get; set; }

        public Device Empty
        {
            get
            {
                Vendor = "Other";
                Type = "Other";
                Model = "Other";
                return this;
            }
        }

        public override string ToString() => $"{{Vendor: \"{Vendor}\", Type: \"{Type}\", Model: \"{Model}\"}}";
    }
}
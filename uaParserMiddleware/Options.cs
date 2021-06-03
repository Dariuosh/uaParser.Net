namespace uaParserMiddleware
{
    public class Options  
    {
      

        public bool BrowserParser { get; set; } = true;
        public bool CPUParser { get; set; } = true;
        public bool EngineParser { get; set; } = true;
        public bool OSParser { get; set; } = true;
        public bool DeviceParser { get; set; } = false;
    }
}
namespace R2022.Utils.Buttons
{
    public struct DllButtonData
    {
        public string DllPath { get; set; }
        public string StartClass { get; set; }
        public string ButtonName { get; set; }
        public string ButtonImagePath { get; set; }
        public string Description { get; set; }
        
        public DllButtonData(string buttonName, string dllPath, string startClass, string buttonImage = null, string description = null)
        {
            DllPath = dllPath;
            StartClass = startClass;
            ButtonName = buttonName;
            ButtonImagePath = buttonImage;
            Description = description;
        }
    }
}
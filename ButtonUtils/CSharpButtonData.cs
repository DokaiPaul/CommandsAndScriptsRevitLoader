namespace R2022.ButtonUtils
{
    public struct CSharpButtonData
    {
        public string DllPath { get; set; }
        public string ButtonName { get; set; }
        public string CommandName { get; set; }
        public string ButtonImagePath { get; set; }
        public string Description { get; set; }

        public CSharpButtonData(string buttonName, string dllPath, string commandName, string buttonImage = null, string description = null)
        {
            ButtonName = buttonName;
            DllPath = dllPath;
            CommandName = commandName;
            ButtonImagePath = buttonImage;
            Description = description;
        }
    }
}
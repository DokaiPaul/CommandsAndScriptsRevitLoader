using R2022.ENUM;

namespace R2022.ButtonUtils
{
    public struct CSharpCustomButtonData : ICustomButtonData
    {
        public string FilePath { get; set; }
        public string ButtonName { get; set; }
        public string CommandName { get; set; }
        public string ButtonImagePath { get; set; }
        public string Description { get; set; }
        public ToolTypes ToolType { get; set; }

        public CSharpCustomButtonData(string buttonName, string filePath, string commandName, string buttonImage = null, string description = null)
        {
            ButtonName = buttonName;
            FilePath = filePath;
            CommandName = commandName;
            ButtonImagePath = buttonImage;
            Description = description;
            
            ToolType = ToolTypes.CSharp;
        }
    }
}
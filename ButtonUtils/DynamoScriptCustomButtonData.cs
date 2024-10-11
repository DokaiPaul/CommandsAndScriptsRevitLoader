using R2022.ENUM;

namespace R2022.ButtonUtils
{
    public struct DynamoScriptCustomButtonData : ICustomButtonData
    {
        public string FilePath { get; set; }
        public string ButtonName { get; set; }
        public string ButtonImagePath { get; set; }
        public string Description { get; set; }
        public string CommandName { get; set; }
        public ToolTypes ToolType { get; set; }

        public DynamoScriptCustomButtonData(string buttonName, string filePath, string buttonImage = null, string description = null)
        {
            ButtonName = buttonName;
            FilePath = filePath;
            ButtonImagePath = buttonImage;
            Description = description;
            
            CommandName = "R2022.RunDynamoScript";
            ToolType = ToolTypes.Dynamo;
        }
    }
}
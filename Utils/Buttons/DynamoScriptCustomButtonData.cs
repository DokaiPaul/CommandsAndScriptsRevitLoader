using System;
using R2022.Types.ENUM;

namespace R2022.Utils.Buttons
{
    public struct DynamoScriptCustomButtonData : ICustomButtonData
    {
        public Guid Id { get; set; }
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
            Id = Guid.NewGuid();
        }
    }
}
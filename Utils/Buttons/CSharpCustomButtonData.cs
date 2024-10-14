using System;
using R2022.Types.ENUM;

namespace R2022.Utils.Buttons
{
    public struct CSharpCustomButtonData : ICustomButtonData
    {
        public Guid Id { get; set; }
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
            Id = Guid.NewGuid();
        }
    }
}
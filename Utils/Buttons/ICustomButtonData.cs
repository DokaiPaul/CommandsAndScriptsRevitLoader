using System;
using R2022.Types.ENUM;

namespace R2022.Utils.Buttons
{
    public interface ICustomButtonData
    {
        Guid Id { get; set; }
        string FilePath { get; set; }
        string ButtonName { get; set; }
        string ButtonImagePath { get; set; }
        string Description { get; set; }
        string CommandName { get; set; }
        ToolTypes ToolType { get; set; }
    }
}
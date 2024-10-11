using R2022.ENUM;

namespace R2022.ButtonUtils
{
    public interface ICustomButtonData
    {
        string FilePath { get; set; }
        string ButtonName { get; set; }
        string ButtonImagePath { get; set; }
        string Description { get; set; }
        string CommandName { get; set; }
        ToolTypes ToolType { get; set; }
    }
}
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using R2022.UserConfig.Views.DisplaySettings;

namespace R2022.UserConfig.Commands
{
    // [-] - Implement the class for managing display settings (visibility) of the current tab
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class OpenDisplayManagement: IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            new UpdateDisplaySettingsView().Show();
            
            return Result.Succeeded;
        }
    }
}
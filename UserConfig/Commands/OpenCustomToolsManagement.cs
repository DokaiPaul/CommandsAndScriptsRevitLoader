using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using R2022.UserConfig.Views.CustomToolsManagement;

namespace R2022.UserConfig.Commands
{
    // [-] - Implement the class for managing custom items in plugin
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class OpenCustomToolsManagement : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            new MainCustomToolsView().Show();
            
            return Result.Succeeded;
        }
    }
}
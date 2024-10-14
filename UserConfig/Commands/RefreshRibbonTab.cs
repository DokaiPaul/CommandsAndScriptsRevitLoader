using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using R2022.Utils;

namespace R2022.UserConfig.Commands
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class RefreshRibbonTab : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication application = commandData.Application;
            var ribbonPanelManager = new RibbonPanelManager("Planit", application);
            
            // Trace.WriteLine("Before refreshing ribbon tab");
            
            ribbonPanelManager.UpdateCustomScriptsPanel();
            
            return Result.Succeeded;
        }
    }
}
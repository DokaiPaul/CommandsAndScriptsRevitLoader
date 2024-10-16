using System.Windows.Threading;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using R2022.UserConfig.Views.CustomToolsManagement;
using R2022.Utils;

namespace R2022.UserConfig.Commands
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class OpenCustomToolsManagement : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiApplication = commandData.Application;
            var ribbonPanelManager = new RibbonPanelManager("Planit", uiApplication);

            var window = new MainCustomToolsView();
            window.ShowDialog();

            ribbonPanelManager.UpdateCustomScriptsPanel();
            
            return Result.Succeeded;
        }
    }
}
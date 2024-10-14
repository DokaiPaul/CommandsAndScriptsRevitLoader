using System;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using R2022.Utils;

namespace R2022
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class StartApplication : IExternalApplication
    {
        private const string TabName = "Planit";
        private UIControlledApplication _application;
        private RibbonPanelManager _ribbonPanelManager;

        #region Main methods used by Revit
        public Result OnStartup(UIControlledApplication application)
        {
            _application = application;
            _ribbonPanelManager = new RibbonPanelManager(TabName, _application);
            
            ClearDynamoDllsFolder(); // clear Dynamo/DLLs folder before loading new ones. (Created every time when Revit is launched)

            _ribbonPanelManager.GenerateRibbonPanels();

            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }
        #endregion
        

        #region Local utilities
        
        private void ClearDynamoDllsFolder()
        {
            string userName = Environment.UserName;

            string staticScriptFolderPath = @"C:\Users\" + userName +
                                   @"\AppData\Roaming\Autodesk\Revit\Addins\2022\PlanitPlugin\Dynamo\DLLs\";
            FolderManager.DeleteAllFilesInFolder(staticScriptFolderPath); // clear generated dlls for base dynamo scripts
            
            string customScriptFolderPath = @"C:\Users\" + userName +
                                            @"\AppData\Roaming\Autodesk\Revit\Addins\2022\PlanitPlugin\CustomTools\Dynamo\DLLs\";
            FolderManager.DeleteAllFilesInFolder(customScriptFolderPath); // clear generated dlls for custom added dynamo scripts
        }
        #endregion
    }
}
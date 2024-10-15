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
            
            ClearDynamoBackup(); // clear all dynamo backup files

            _ribbonPanelManager.GenerateRibbonPanels();

            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }
        #endregion
        

        #region Local utilities
        
        private void ClearDynamoBackup()
        {
            string userName = Environment.UserName;
        
            string staticScriptFolderPath = @"C:\Users\" + userName +
                                   @"\AppData\Roaming\Autodesk\Revit\Addins\2022\PlanitPlugin\Dynamo\backup\";
            FolderManager.DeleteAllFilesInFolder(staticScriptFolderPath);
            
            string customScriptFolderPath = @"C:\Users\" + userName +
                                            @"\AppData\Roaming\Autodesk\Revit\Addins\2022\PlanitPlugin\CustomTools\Dynamo\backup\";
            FolderManager.DeleteAllFilesInFolder(customScriptFolderPath);
        }
        #endregion
    }
}
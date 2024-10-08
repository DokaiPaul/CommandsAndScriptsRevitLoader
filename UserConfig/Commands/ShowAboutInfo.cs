using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using R2022.Types;

namespace R2022.UserConfig.Commands
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class ShowAboutInfo : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var dialogBox = new TaskDialog("About");
            
            var configManager = new ConfigManager();
            About aboutInfo = configManager.GetAboutInfo();
            
            dialogBox.TitleAutoPrefix = false;
            dialogBox.MainContent = $"Main information about Planit plugin.\nHere are collected main set of tools used for boosting your productivity. You can use them or even add custom tools powered by Dynamo or C# via settings in the plugin. \n\nVersion: {aboutInfo.version}\nDevelopers: {string.Join(", ", aboutInfo.developers)}";
            dialogBox.Show();
            
            return Result.Succeeded;
        }
    }
}
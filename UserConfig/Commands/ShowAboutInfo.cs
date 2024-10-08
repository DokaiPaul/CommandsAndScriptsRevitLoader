using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace R2022.UserConfig.Commands
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class ShowAboutInfo : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // return dialogBox.Show("About", "This is a test message");
            var dialogBox = new TaskDialog("About");
            
            dialogBox.TitleAutoPrefix = false;
            // [-] - Implement returning the version of the plugin and other important information. Maybe add roles as developers and show other important information that were hidden for regular users.
            dialogBox.MainContent = "This is a test message";
            dialogBox.Show();
            
            return Result.Succeeded;
        }
    }
}
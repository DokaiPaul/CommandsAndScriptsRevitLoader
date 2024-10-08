using System;
using System.Linq;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using R2022.ButtonUtils;
using R2022.Tabs.Custom;
using R2022.Tabs.PlanitProcessItems;
using R2022.Tabs.RedmineItems;
using R2022.Tabs.SettingsItems;
using R2022.UserConfig;
using R2022.Utils;

namespace R2022
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class StartApplication : IExternalApplication
    {
        private const string TabName = "Planit";
        private UIControlledApplication _application;
        private ConfigManager _configManager;

        #region Main methods used by Revit
        public Result OnStartup(UIControlledApplication application)
        {
            _application = application;
            _configManager = new ConfigManager();
            
            ClearDynamoDllsFolder(); // clear Dynamo/DLLs folder before loading new ones. (Created every time when Revit is launched)

            GenerateRibbonPanels();

            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }
        #endregion

        #region Global loaders
        private void GenerateRibbonPanels()
        {
            _application.CreateRibbonTab(TabName); // create a new ribbon tab

            
            // create a ribbon panels
            GenerateSettingsPanel();
            GeneratePlanitProcessPanel();
            GenerateRedminePanel();
            GenerateCustomScriptsPanel();
        }
        
        private void GeneratePlanitProcessPanel()
        {
            RibbonPanel panel = _application.CreateRibbonPanel(TabName, "Planit Process");

            string[] buttonsToDisable = _configManager.GetButtonNamesToExclude();
            
            var containerButtons = new BasePullButtonItem[]
            {
                new C10Interior(panel, "C10 Interior", buttonsToDisable),
                new B10Structure(panel, "B10 Structure", buttonsToDisable),
                new D20Plumbing(panel, "D20 Plumbing", buttonsToDisable),
                new General(panel, "General", buttonsToDisable)
            };
            
            LoadPrimaryButtons(containerButtons, buttonsToDisable);
        }

        #endregion

        #region Individual panel loaders
        private void GenerateSettingsPanel()
        {
            RibbonPanel panel = _application.CreateRibbonPanel(TabName, "Settings");

            string[] disabledButtons = Array.Empty<string>();
            var containerButtons = new BasePullButtonItem[]
            {
                new UserSetting(panel, "Settings", disabledButtons)
            };

            LoadPrimaryButtons(containerButtons, disabledButtons);
        }

        private void GenerateRedminePanel()
        {
            RibbonPanel panel = _application.CreateRibbonPanel(TabName, "Redmine");

            string[] disabledButtons = Array.Empty<string>();
            var containerButtons = new BasePullButtonItem[]
            {
                new RedmineTask(panel, "Tasks", disabledButtons)
            };

            LoadPrimaryButtons(containerButtons, disabledButtons);
        }
        
        private void GenerateCustomScriptsPanel()
        {
            RibbonPanel panel = _application.CreateRibbonPanel(TabName, "Custom Scripts");

            string[] disabledButtons = Array.Empty<string>();
            var containerButtons = new BasePullButtonItem[]
            {
                new DynamoTools(panel, "Dynamo Scripts", disabledButtons),
                new CSharpTools(panel, "C# Tools", disabledButtons),
            };

            LoadPrimaryButtons(containerButtons, disabledButtons);
        }
        #endregion

        #region Local utilities
        private void LoadPrimaryButtons(BasePullButtonItem[] buttons, string[] disabledButtons)
        {
            foreach (BasePullButtonItem button in buttons)
            {
                if (disabledButtons.Contains(button.GetButtonName()))
                {
                    continue;
                }
                button.PopulateButtonWithItems();
            }
        }
        
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
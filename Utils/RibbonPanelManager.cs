using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autodesk.Revit.UI;
using R2022.Tabs.Custom;
using R2022.Tabs.PlanitProcessItems;
using R2022.Tabs.RedmineItems;
using R2022.Tabs.SettingsItems;
using R2022.UserConfig;
using R2022.Utils.Buttons;
using AW = Autodesk.Windows;

namespace R2022.Utils
{
    public class RibbonPanelManager
    {
        private readonly string _tabName;
        private readonly string _compiledDynamoToolsFolder;
        private readonly UIApplication _uiApplication = null;
        private readonly UIControlledApplication _uiControlledApplication = null;
        private readonly ConfigManager _configManager;

        public RibbonPanelManager(string tabName, UIApplication uiApplication)
        {
            _tabName = tabName;
            _uiApplication = uiApplication;
            _configManager = new ConfigManager();

            string userName = Environment.UserName;
            _compiledDynamoToolsFolder = @"C:\Users\" + userName +
                                         @"\AppData\Roaming\Autodesk\Revit\Addins\2022\PlanitPlugin\CustomTools\Dynamo\DLLs";
        }

        public RibbonPanelManager(string tabName, UIControlledApplication uiControlledApplication)
        {
            _tabName = tabName;
            _uiControlledApplication = uiControlledApplication;
            _configManager = new ConfigManager();
        }

        #region Global loaders

        public void GenerateRibbonPanels()
        {
            if (_uiApplication != null)
                _uiApplication.CreateRibbonTab(_tabName); // create a new ribbon tab
            else
            {
                _uiControlledApplication?.CreateRibbonTab(_tabName); // create a new ribbon tab
            }

            // create a ribbon panels
            GenerateSettingsPanel();
            GeneratePlanitProcessPanel();
            GenerateRedminePanel();
            GenerateCustomScriptsPanel();
        }

        private void GeneratePlanitProcessPanel()
        {
            RibbonPanel panel = CreateRibbonPanel("Planit Process");

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

        #region Individual button updaters

        public void UpdateCustomScriptsPanel()
        {
            const string panelName = "Custom Scripts";

            RemovePanelByName(panelName);
            RibbonPanel refreshedPanel = CreateRibbonPanel(panelName);

            string[] disabledButtons = Array.Empty<string>();
            var containerButtons = new BasePullButtonItem[]
            {
                new DynamoTools(refreshedPanel, "Dynamo Scripts", disabledButtons),
                new CSharpTools(refreshedPanel, "C# Tools", disabledButtons)
            };

            ClearDynamoDllFolder(); // action required to remove the old DLLs and they are replaced by new ones when Dynamo buttons were loaded.
            LoadPrimaryButtons(containerButtons, disabledButtons);
        }

        #endregion

        #region Individual panel loaders

        private void GenerateSettingsPanel()
        {
            RibbonPanel panel = CreateRibbonPanel("Settings");

            string[] disabledButtons = Array.Empty<string>();
            var containerButtons = new BasePullButtonItem[]
            {
                new UserSetting(panel, "Settings", disabledButtons)
            };

            LoadPrimaryButtons(containerButtons, disabledButtons);
        }

        private void GenerateRedminePanel()
        {
            RibbonPanel panel = CreateRibbonPanel("Redmine");

            string[] disabledButtons = Array.Empty<string>();
            var containerButtons = new BasePullButtonItem[]
            {
                new RedmineTask(panel, "Tasks", disabledButtons)
            };

            LoadPrimaryButtons(containerButtons, disabledButtons);
        }

        private void GenerateCustomScriptsPanel()
        {
            RibbonPanel panel = CreateRibbonPanel("Custom Scripts");

            string[] disabledButtons = Array.Empty<string>();
            var containerButtons = new BasePullButtonItem[]
            {
                new DynamoTools(panel, "Dynamo Scripts", disabledButtons),
                new CSharpTools(panel, "C# Tools", disabledButtons)
            };

            LoadPrimaryButtons(containerButtons, disabledButtons);
        }

        #endregion


        #region Local utilities

        private RibbonPanel CreateRibbonPanel(string panelName)
        {
            if (_uiApplication != null)
            {
                return _uiApplication.CreateRibbonPanel(_tabName, panelName);
            }
            else if (_uiControlledApplication != null)
            {
                return _uiControlledApplication.CreateRibbonPanel(_tabName, panelName);
            }

            throw new InvalidOperationException("Both UIApplication and UIControlledApplication are null.");
        }

        private RibbonPanel GetRibbonPanel(string panelName)
        {
            if (_uiApplication != null)
            {
                return _uiApplication.GetRibbonPanels(_tabName).FirstOrDefault(x => x.Name == panelName);
            }
            else if (_uiControlledApplication != null)
            {
                return _uiControlledApplication.GetRibbonPanels(_tabName).FirstOrDefault(x => x.Name == panelName);
            }

            throw new InvalidOperationException("Both UIApplication and UIControlledApplication are null.");
        }

        private void RemovePanelByName(string panelName)
        {
            AW.RibbonControl ribbon = AW.ComponentManager.Ribbon;
            foreach (AW.RibbonTab tab in ribbon.Tabs)
            {
                if (tab.Name != _tabName) continue;
                AW.RibbonPanel panel = tab.Panels.FirstOrDefault(x => x.Source.Title == panelName);

                tab.Panels.Remove(panel);
                Type uiApplicationType = typeof(UIApplication);
                PropertyInfo ribbonItemsProperty = uiApplicationType.GetProperty("RibbonItemDictionary",
                    BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly);
                if (ribbonItemsProperty == null) continue;

                var ribbonItems =
                    (Dictionary<string, Dictionary<string, RibbonPanel>>)ribbonItemsProperty
                        .GetValue(typeof(UIApplication));
                if (ribbonItems.TryGetValue(tab.Id, out var tabItem))
                    tabItem.Remove(panelName);
            }
        }

        private void ClearDynamoDllFolder()
        {
            var di = new System.IO.DirectoryInfo(_compiledDynamoToolsFolder);

            foreach (System.IO.FileInfo file in di.GetFiles())
            {
                try
                {
                    file.Delete();
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Tried to delete file: {file.FullName}, but skipped it during the error below.");
                    Console.WriteLine(e);
                }
            }
        }

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

        #endregion
    }
}
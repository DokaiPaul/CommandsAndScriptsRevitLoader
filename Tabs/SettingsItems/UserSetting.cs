using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Media.Imaging;
using Autodesk.Revit.UI;
using R2022.ButtonUtils;

namespace R2022.Tabs.SettingsItems
{
    public class UserSetting : BasePullButtonItem
    {
        public UserSetting(RibbonPanel ribbonPanel, string buttonName, string[] disabledButtons) : base(ribbonPanel, buttonName, disabledButtons)
        {
            SetAppearance();
        }

        protected sealed override void SetAppearance()
        {
            var largeImage = new BitmapImage(new Uri(ImageRootPath + @"\PullButtonImages\Settings.png"));
            base.Button.LargeImage = largeImage;
        }

        public override void PopulateButtonWithItems()
        {
            AddDllButtons();
        }
        
        protected override void AddDllButtons()
        {
            string assemblyLocation = Assembly.GetExecutingAssembly().Location;
            const string rootNamespace = "R2022.UserConfig.Commands";
            
            var dllButtonsData = new List<DllButtonData>
            {
                new DllButtonData(
                    "Custom scripts management",
                    assemblyLocation,
                    $"{rootNamespace}.OpenCustomItemsManagement",
                    null,
                    "Here you can modify display settings of the current tab."),
                
                new DllButtonData(
                    "Display management",
                    assemblyLocation,
                    $"{rootNamespace}.OpenDisplayManagement",
                    null,
                    "Here you can add your own custom dynamo scripts and tools powered by C#."),
                
                new DllButtonData(
                    "About",
                    assemblyLocation,
                    $"{rootNamespace}.ShowAboutInfo",
                    null,
                    "Information about the plugin."),
            };
            
            foreach (DllButtonData buttonData in dllButtonsData)
            {
                base.AddPushButton(
                    buttonData.DllPath,
                    buttonData.ButtonName,
                    buttonData.StartClass,
                    buttonData.ButtonImagePath,
                    buttonData.Description);
            }
        }
        
        protected override void AddDynamoButtons()
        {
        }
    }
}
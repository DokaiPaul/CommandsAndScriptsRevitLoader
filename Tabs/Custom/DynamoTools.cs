using System;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using Autodesk.Revit.UI;
using R2022.UserConfig;
using R2022.Utils;
using R2022.Utils.Buttons;

namespace R2022.Tabs.Custom
{
    public class DynamoTools : BasePullButtonItem
    {
        private readonly DynamoScriptCustomButtonData[] _customDynamoTools;
        
        public DynamoTools(RibbonPanel ribbonPanel, string buttonName, string[] disabledButtons) : base(ribbonPanel, buttonName, disabledButtons)
        {
            var presetManager = new ConfigManager();
            _customDynamoTools = presetManager.GetCustomDynamoTools();
            
            SetAppearance();
        }

        protected sealed override void SetAppearance()
        {
            var largeImage = new BitmapImage(new Uri(ImageRootPath + @"\PullButtonImages\Dynamo Logo.png"));
            base.Button.LargeImage = largeImage;
        }

        public override void PopulateButtonWithItems()
        {
            AddDynamoButtons();
        }
        
        protected override void AddDllButtons()
        {
        }
        
        protected override void AddDynamoButtons()
        {
            string userName = Environment.UserName;
            string dllFolder = @"C:\Users\" + userName +
                               @"\AppData\Roaming\Autodesk\Revit\Addins\2022\PlanitPlugin\CustomTools\Dynamo\DLLs\";
            
            var dynamoButtonsData = new List<DynamoScriptCustomButtonData>();

            foreach (DynamoScriptCustomButtonData item in _customDynamoTools)
            {
                string buttonName = item.ButtonName;
                string scriptPath = item.FilePath;
                string imagePath = item.ButtonImagePath;
                string description = item.Description;

                dynamoButtonsData.Add(new DynamoScriptCustomButtonData(buttonName, scriptPath, imagePath, description));
            }
            
            FolderManager.TryDeleteAllFilesInFolder(dllFolder); // to remove all previous tools removed by user
            base.GenerateDynamoPushButtons(dynamoButtonsData, dllFolder);
        }
    }
}
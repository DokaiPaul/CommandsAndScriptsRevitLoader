using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media.Imaging;
using Autodesk.Revit.UI;
using R2022.ButtonUtils;
using R2022.UserConfig;
using R2022.Utils;

namespace R2022.Tabs.Custom
{
    public class CSharpTools : BasePullButtonItem
    {
        private readonly CSharpButtonData[] _customCsharpTools;

        public CSharpTools(RibbonPanel ribbonPanel, string buttonName, string[] disabledButtons) : base(ribbonPanel,
            buttonName, disabledButtons)
        {
            var presetManager = new ConfigManager();
            _customCsharpTools = presetManager.GetCustomCsharpTools();

            SetAppearance();
        }

        protected sealed override void SetAppearance()
        {
            var largeImage = new BitmapImage(new Uri(ImageRootPath + @"\PullButtonImages\CSharp Logo.png"));
            base.Button.LargeImage = largeImage;
        }

        public override void PopulateButtonWithItems()
        {
            this.AddDllButtons();
        }

        protected override void AddDllButtons()
        {
            foreach (CSharpButtonData item in _customCsharpTools)
            {
                string name = item.ButtonName;
                string description = item.Description;
                string filePath = item.DllPath;
                string imagePath = item.ButtonImagePath;
                string toolName = Path.GetFileNameWithoutExtension(filePath);
                string commandName = item.CommandName;


                base.AddPushButton(
                    filePath,
                    name,
                    $"{toolName}.{commandName}",
                    imagePath,
                    description);
            }
        }

        protected override void AddDynamoButtons()
        {
            return;
        }
    }
}
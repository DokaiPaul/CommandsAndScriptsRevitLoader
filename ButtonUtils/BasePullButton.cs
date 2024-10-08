using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Imaging;
using Autodesk.Revit.UI;
using R2022.Utils;
using R2022.Utils.Dynamo;

namespace R2022.ButtonUtils
{
    public abstract class BasePullButtonItem
    {
        private readonly string _buttonName;
        private readonly PulldownButtonData _buttonData;
        private readonly RibbonPanel _ribbonPanel;
        protected readonly PulldownButton Button;
        protected string ImageRootPath;
        protected string AddinFolderPath;
        protected string DynamoFolderPath;

        protected BasePullButtonItem(RibbonPanel ribbonPanel, string buttonName, string[] disabledButtons)
        {
            _buttonName = buttonName;
            _ribbonPanel = ribbonPanel;
            _buttonData = new PulldownButtonData(buttonName, buttonName);
            if (disabledButtons.Contains(buttonName) == false)
            {
                Button = (PulldownButton)_ribbonPanel.AddItem(_buttonData);
            }

            BuildUserPath();
        }

        public PulldownButtonData GetPulldownButtonData()
        {
            return _buttonData;
        }

        public string GetButtonName()
        {
            return _buttonName;
        }

        protected abstract void SetAppearance();
        protected abstract void AddDynamoButtons();
        protected abstract void AddDllButtons();
        public abstract void PopulateButtonWithItems();

        protected void AddPushButton(string path, string buttonText, string startClass, string imagePath,
            string description = "")
        {
            var buttonData = new PushButtonData(startClass, buttonText, path, startClass)
            {
                ToolTip = description
            };
            if (imagePath != null) buttonData.LargeImage = new BitmapImage(new Uri(imagePath));

            Button.AddPushButton(buttonData);
        }

        protected void GenerateDynamoPushButtons(List<DynamoScriptButtonData> buttons, string dllFolder)
        {
            foreach (DynamoScriptButtonData buttonData in buttons)
            {
                // extract name of dynamo script
                string[] parts = buttonData.ScriptPath.Split('\\');
                string scriptName = parts[parts.Length - 1].Split('.')[0];

                // generate class handler of the command
                string classPath = dllFolder + scriptName + ".dll";

                Type dynamicClass =
                    DynamoClassGenerator.GenerateDynamoCommandClass(scriptName, buttonData.ScriptPath, classPath);

                // add button to the pulldown button
                AddPushButton(
                    classPath,
                    buttonData.ButtonName,
                    dynamicClass.FullName,
                    buttonData.ButtonImagePath,
                    buttonData.Description);
            }
        }

        private void BuildUserPath()
        {
            string userName = Environment.UserName;
            string basePath = @"C:\Users\" + userName + @"\AppData\Roaming\Autodesk\Revit\Addins\2022\PlanitPlugin\";

            ImageRootPath = basePath + @"Resources\";
            AddinFolderPath = basePath + @"DLLs\";
            DynamoFolderPath = basePath + @"Dynamo\";
        }
    }
}
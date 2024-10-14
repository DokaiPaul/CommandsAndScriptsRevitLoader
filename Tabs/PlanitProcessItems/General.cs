using System;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using Autodesk.Revit.UI;
using R2022.Utils.Buttons;

namespace R2022.Tabs.PlanitProcessItems
{
    // create public class General derived from BasePullButton and implement the class
    public class General : BasePullButtonItem
    {
        public General(RibbonPanel ribbonPanel, string buttonName, string[] disabledButtons) : base(ribbonPanel, buttonName, disabledButtons)
        {
            SetAppearance();
        }

        protected sealed override void SetAppearance()
        {
            var largeImage = new BitmapImage(new Uri(ImageRootPath + @"Default.png"));
            base.Button.LargeImage = largeImage;
        }

        public override void PopulateButtonWithItems()
        {
            this.AddDllButtons();
            this.AddDynamoButtons();
        }

        protected override void AddDllButtons()
        {
            var dllButtonsData = new List<DllButtonData>
            {
                new DllButtonData(
                    "Change Values in Parameters",
                    AddinFolderPath + @"ChangeValuesInParameters.dll",
                    "ChangeMaterialByParameters_2022.ChangeValuesInParameters",
                    null,
                    "Change values in parameters of selected elements.\n\nImplemented by: DOP")
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
            string userName = Environment.UserName;
            string dllFolder = @"C:\Users\" + userName +
                               @"\AppData\Roaming\Autodesk\Revit\Addins\2022\PlanitPlugin\Dynamo\DLLs\";

            var dynamoButtonsData = new List<DynamoScriptCustomButtonData>
            {
                new DynamoScriptCustomButtonData(
                    "Wall by Railing",
                    DynamoFolderPath + "WallByRailing.dyn",
                    null,
                    "This script creates walls by selected railings.\n\nImplemented by: BAP"),
            };

            base.GenerateDynamoPushButtons(dynamoButtonsData, dllFolder);
        }
    }
}
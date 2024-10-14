using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Imaging;
using Autodesk.Revit.UI;
using R2022.Utils.Buttons;

namespace R2022.Tabs.PlanitProcessItems
{
    public class B10Structure : BasePullButtonItem
    {
        public B10Structure(RibbonPanel ribbonPanel, string buttonName, string[] disabledButtons) : base(ribbonPanel,
            buttonName, disabledButtons)
        {
            if (disabledButtons.Contains(buttonName) == false)
            {
                SetAppearance();
            }
        }

        protected sealed override void SetAppearance()
        {
            var largeImage = new BitmapImage(new Uri(ImageRootPath + @"\PullButtonImages\B10 Structure.png"));
            base.Button.LargeImage = largeImage;
        }

        public override void PopulateButtonWithItems()
        {
            this.AddDynamoButtons();
        }

        protected override void AddDllButtons()
        {
        }

        protected override void AddDynamoButtons()
        {
            string userName = Environment.UserName;
            string dllFolder = @"C:\Users\" + userName +
                               @"\AppData\Roaming\Autodesk\Revit\Addins\2022\PlanitPlugin\Dynamo\DLLs\";

            var dynamoButtonsData = new List<DynamoScriptCustomButtonData>
            {
                new DynamoScriptCustomButtonData(
                    "APL Overlay",
                    DynamoFolderPath + "APL_Overlay.dyn",
                    ImageRootPath + @"\PullButtonImages\B10 Structure.png"),

                new DynamoScriptCustomButtonData("Tags Updater",
                    DynamoFolderPath + "TagsUpdater_D1.34.dyn",
                    ImageRootPath + @"\PullButtonImages\B10 Structure.png"),
            };

            base.GenerateDynamoPushButtons(dynamoButtonsData, dllFolder);
        }
    }
}
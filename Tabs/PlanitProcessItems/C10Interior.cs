using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Imaging;
using Autodesk.Revit.UI;
using R2022.ButtonUtils;

namespace R2022.Tabs.PlanitProcessItems
{
    public class C10Interior : BasePullButtonItem
    {
        public C10Interior(RibbonPanel ribbonPanel, string buttonName, string[] disabledButtons) : base(ribbonPanel, buttonName, disabledButtons)
        {
            if (disabledButtons.Contains(buttonName))
            {
                return;
            }
            SetAppearance();
        }
        
        protected sealed override void SetAppearance()
        {
            var largeImage = new BitmapImage(new Uri(ImageRootPath + @"\PullButtonImages\C10 Interior.png"));
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
                               @"\AppData\Roaming\Autodesk\Revit\Addins\2022\PlanitPlugin\Dynamo\DLLs\";

            var dynamoButtonsData = new List<DynamoScriptCustomButtonData>
            {
                new DynamoScriptCustomButtonData("BeamSystems for Ceilings",
                    DynamoFolderPath + "BeamSystemsForCeilings.dyn",
                    ImageRootPath + "Default_16.png"),

            };

            base.GenerateDynamoPushButtons(dynamoButtonsData, dllFolder);
        }
    }
}
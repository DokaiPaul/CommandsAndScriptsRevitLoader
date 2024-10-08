using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Autodesk.Revit.UI;
using R2022.ButtonUtils;

namespace R2022.Tabs.PlanitProcessItems
{
    public class D20Plumbing : BasePullButtonItem
    {
        public D20Plumbing(RibbonPanel ribbonPanel, string buttonName, string[] disabledButtons) : base(ribbonPanel, buttonName, disabledButtons)
        {
            SetAppearance();
        }

        protected sealed override void SetAppearance()
        {
            var largeImage = new BitmapImage(new Uri(ImageRootPath + @"\PullButtonImages\D20 Plumbing.png"));
            base.Button.LargeImage = largeImage;
        }

        public override void PopulateButtonWithItems()
        {
            this.AddDllButtons();
        }
        
        protected override void AddDllButtons()
        {
            var dllButtonsData = new List<DllButtonData>
            {
                new DllButtonData(
                    "Numerate Fabrication Parts",
                    AddinFolderPath + @"AutoNumerationFabricationParts.dll",
                    "AutoNumerationFabricationParts_R2022.StartCommand",
                    ImageRootPath + "Default_16.png")
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
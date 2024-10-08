using System;
using System.Windows.Media.Imaging;
using Autodesk.Revit.UI;
using R2022.ButtonUtils;

namespace R2022.Tabs.RedmineItems
{
    public class RedmineTask : BasePullButtonItem
    {
        public RedmineTask(RibbonPanel ribbonPanel, string buttonName, string[] disabledButtons) : base(ribbonPanel, buttonName, disabledButtons)
        {
            SetAppearance();
        }

        protected sealed override void SetAppearance()
        {
            var largeImage = new BitmapImage(new Uri(ImageRootPath + @"\PullButtonImages\Redmine Logo.ico"));
            base.Button.LargeImage = largeImage;
        }

        public override void PopulateButtonWithItems()
        {
        }
        
        protected override void AddDllButtons()
        {
        }
        
        protected override void AddDynamoButtons()
        {
        }
    }
}
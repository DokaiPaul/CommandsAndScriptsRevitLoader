using System.Windows.Media;

namespace R2022.Constants.WindowColors.Buttons
{
    public static class BackgroundHover
    {
        public static readonly Brush AddItem = Brushes.DarkSeaGreen;
        public static readonly Brush EditItem = Brushes.Orange;
        public static readonly Brush DeleteItem = Brushes.PaleVioletRed;
        
        public static readonly Brush Cancel = Brushes.PaleVioletRed;
    }
       
    public static class BackgroundClicked
    {
        public static readonly Brush AddItem = Brushes.SeaGreen;
        public static readonly Brush EditItem = Brushes.DarkGoldenrod;
        public static readonly Brush DeleteItem = Brushes.IndianRed;
        
        public static readonly Brush Cancel = Brushes.IndianRed;
    }
    
    public static class ForegroundClicked
    {
        public static readonly Brush AddItem = Brushes.White;
        public static readonly Brush EditItem = Brushes.White;
        public static readonly Brush DeleteItem = Brushes.White;
        
        public static readonly Brush Cancel = Brushes.White;
    }
    
}
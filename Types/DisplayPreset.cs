namespace R2022.Types
{
    public struct DisplayPreset
    {
        public Disabled disabled;
        public string theme;
    }

    public struct Disabled
    {
        public string[] baseButtons;
        public string[] panels;
    }

    
}
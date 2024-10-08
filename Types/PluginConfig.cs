namespace R2022.Types
{
    public struct PluginConfig
    {
        public DisplayPreset displayPreset;
        public CustomScriptItems customScriptItems;
        public About about;
    }
    
    public struct About
    {
        public string version;
        public string[] developers;
    }
}
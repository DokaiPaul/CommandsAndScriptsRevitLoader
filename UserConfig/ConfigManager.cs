using System.IO;
using Newtonsoft.Json;
using R2022.ButtonUtils;
using R2022.Types;

namespace R2022.UserConfig
{
    public class ConfigManager
    {
        private const string PresetLocation = @"C:\Users\_DOP_\AppData\Roaming\Autodesk\Revit\Addins\2022\PlanitPlugin\mainConfig.json";

        private string[] _buttonNamesToExclude;
        private DynamoScriptButtonData[] _customDynamoTools;
        private CSharpButtonData[] _customCsharpTools;
        private About _aboutInfo;

        public ConfigManager()
        {
            ParsePresetFile();
        }
        
        private void ParsePresetFile()
        {
            // read config file
            string content = File.ReadAllText(PresetLocation);
            
            // parse the JSON string
            if (string.IsNullOrEmpty(content)) return;
            var config = JsonConvert.DeserializeObject<PluginConfig>(content);

            _buttonNamesToExclude = config.displayPreset.disabled.baseButtons;
            
            ExtractCustomTools(config);
            ExtractAboutInfo(config);
        }
        
        private void ExtractCustomTools(PluginConfig config)
        {
            _customDynamoTools = config.customScriptItems.dynamo;
            _customCsharpTools = config.customScriptItems.csharp;
        }
        
        private void ExtractAboutInfo(PluginConfig config)
        {
            _aboutInfo = config.about;
        }
        
        public string[] GetButtonNamesToExclude()
        {
            return _buttonNamesToExclude;
        }
        
        public DynamoScriptButtonData[] GetCustomDynamoTools()
        {
            return _customDynamoTools;
        }
        
        public CSharpButtonData[] GetCustomCsharpTools()
        {
            return _customCsharpTools;
        }

        public About GetAboutInfo()
        {
            return _aboutInfo;
        }
    }
}
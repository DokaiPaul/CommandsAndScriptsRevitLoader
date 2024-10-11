using System;
using System.Collections;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using R2022.ButtonUtils;
using R2022.ENUM;
using R2022.Types;
using static R2022.ENUM.ToolTypes;

namespace R2022.UserConfig
{
    public class ConfigManager
    {
        private const string PresetLocation =
            @"C:\Users\_DOP_\AppData\Roaming\Autodesk\Revit\Addins\2022\PlanitPlugin\mainConfig.json";

        private PluginConfig _pluginConfig;

        // private string[] _buttonNamesToExclude;
        private DynamoScriptCustomButtonData[] _customDynamoTools;
        private CSharpCustomButtonData[] _customCsharpTools;
        private string _theme;
        private About _aboutInfo;

        public ConfigManager()
        {
            ParsePresetFile();
        }

        public string[] GetButtonNamesToExclude()
        {
            // return _buttonNamesToExclude;
            return Array.Empty<string>();
        }

        public DynamoScriptCustomButtonData[] GetCustomDynamoTools()
        {
            return _customDynamoTools;
        }

        public CSharpCustomButtonData[] GetCustomCsharpTools()
        {
            return _customCsharpTools;
        }

        public About GetAboutInfo()
        {
            return _aboutInfo;
        }

        public string GetTheme()
        {
            return _theme;
        }

        public void SetTheme(string theme)
        {
            _theme = theme;

            // update the config file
            _pluginConfig.displayPreset.theme = theme;
            UpdateConfigFile(_pluginConfig);
        }

        public void AddNewTool(ICustomButtonData toolData)
        {
            string fileName = Path.GetFileName(toolData.FilePath);

            bool nameUsed = IsFileNameUsed(fileName, toolData.ToolType);
            if (nameUsed)
                throw new Exception("File with such name already exists. Change the selected file name.");

            string targetFileFolder;
            string targetIconFolder;
            string currentUser = Environment.UserName;
            if (toolData.ToolType == ToolTypes.Dynamo)
            {
                targetFileFolder = @"C:\Users\" + currentUser +
                                   @"\AppData\Roaming\Autodesk\Revit\Addins\2022\PlanitPlugin\CustomTools\Dynamo";
                targetIconFolder = @"C:\Users\" + currentUser +
                                   @"\AppData\Roaming\Autodesk\Revit\Addins\2022\PlanitPlugin\CustomTools\Dynamo\Images";

                string iconPath = !String.IsNullOrEmpty(toolData.ButtonImagePath)
                    ? $"{targetIconFolder}\\{Path.GetFileName(toolData.ButtonImagePath)}"
                    : null;

                var newTool = new DynamoScriptCustomButtonData(
                    toolData.ButtonName,
                    $"{targetFileFolder}\\{fileName}",
                    iconPath,
                    toolData.Description
                );

                _customDynamoTools = _customDynamoTools.Append(newTool).ToArray();
                _pluginConfig.customScriptItems.dynamo = _customDynamoTools;
            }
            else
            {
                targetFileFolder = @"C:\Users\" + currentUser +
                                   @"\AppData\Roaming\Autodesk\Revit\Addins\2022\PlanitPlugin\CustomTools\CSharp";
                targetIconFolder = @"C:\Users\" + currentUser +
                                   @"\AppData\Roaming\Autodesk\Revit\Addins\2022\PlanitPlugin\CustomTools\CSharp\Images";

                string iconPath = !String.IsNullOrEmpty(toolData.ButtonImagePath)
                    ? $"{targetIconFolder}\\{Path.GetFileName(toolData.ButtonImagePath)}"
                    : null;

                var newTool = new CSharpCustomButtonData(
                    toolData.ButtonName,
                    $"{targetFileFolder}\\{fileName}",
                    "Start",
                    iconPath,
                    toolData.Description
                );

                _customCsharpTools = _customCsharpTools.Append(newTool).ToArray();
                _pluginConfig.customScriptItems.csharp = _customCsharpTools;
            }

            // Strict order of the file copying is important: first copy the icon, then the file and update the config file
            
            // TODO: wrap the file copying in a try-catch block. If error happens, show a message box with the error message
            if (!String.IsNullOrEmpty(toolData.ButtonImagePath))
                File.Copy(toolData.ButtonImagePath,
                    Path.Combine(targetIconFolder, Path.GetFileName(toolData.ButtonImagePath)));
            
            // TODO: wrap the file copying in a try-catch block. If error happens, show a message box with the error message
            if (toolData.FilePath != null)
                File.Copy(toolData.FilePath, Path.Combine(targetFileFolder, fileName));

            UpdateConfigFile(_pluginConfig);
        }
        
        public void RemoveTool(string filePath, string iconPath, ToolTypes toolType)
        {
            if (toolType == ToolTypes.Dynamo)
            {
                _customDynamoTools = _customDynamoTools
                    .Where(tool => tool.FilePath != filePath)
                    .ToArray();
                _pluginConfig.customScriptItems.dynamo = _customDynamoTools;
            }
            else
            {
                _customCsharpTools = _customCsharpTools
                    .Where(tool => tool.FilePath != filePath)
                    .ToArray();
                _pluginConfig.customScriptItems.csharp = _customCsharpTools;
            }


            // TODO: think it over if we're able to delete the icon file because it might be used by Revit.
            if (!String.IsNullOrEmpty(iconPath))
                File.Delete(iconPath);
            
            // TODO: wrap the file deletion in a try-catch block. If error happens, show a message box with the error message
            File.Delete(filePath);

            UpdateConfigFile(_pluginConfig);
        }

        private void ParsePresetFile()
        {
            // read config file
            string content = File.ReadAllText(PresetLocation);

            // parse the JSON string
            if (string.IsNullOrEmpty(content)) return;
            var config = JsonConvert.DeserializeObject<PluginConfig>(content);

            _pluginConfig = config;

            ExtractCustomTools();
            ExtractAboutInfo();
            ExtractTheme();
        }

        private void UpdateConfigFile(PluginConfig file)
        {
            string json = JsonConvert.SerializeObject(file, Formatting.Indented);
            File.WriteAllText(PresetLocation, json);
        }

        private void ExtractCustomTools()
        {
            _customDynamoTools = _pluginConfig.customScriptItems.dynamo;
            _customCsharpTools = _pluginConfig.customScriptItems.csharp;
        }

        private void ExtractAboutInfo()
        {
            _aboutInfo = _pluginConfig.about;
        }

        private void ExtractTheme()
        {
            _theme = _pluginConfig.displayPreset.theme;
        }

        private bool IsFileNameUsed(string fileName, ToolTypes toolType)
        {
            var tools = toolType == ToolTypes.Dynamo
                ? _customDynamoTools.Cast<ICustomButtonData>().ToArray()
                : _customCsharpTools.Cast<ICustomButtonData>().ToArray();

            return tools
                .Select(tool => Path.GetFileName(tool.FilePath))
                .Any(toolName => toolName == fileName);
        }
    }
}
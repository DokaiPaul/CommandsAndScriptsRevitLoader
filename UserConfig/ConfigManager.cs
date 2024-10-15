using System;
using System.IO;
using System.Linq;
using Autodesk.Revit.DB;
using Newtonsoft.Json;
using R2022.Types;
using R2022.Types.ENUM;
using R2022.Utils.Buttons;

namespace R2022.UserConfig
{
    public class ConfigManager
    {
        private readonly string _presetLocation;

        private PluginConfig _pluginConfig;

        // private string[] _buttonNamesToExclude;
        private DynamoScriptCustomButtonData[] _customDynamoTools;
        private CSharpCustomButtonData[] _customCsharpTools;
        private string _theme;
        private About _aboutInfo;

        public ConfigManager()
        {
            string userName = Environment.UserName;
            _presetLocation = @"C:\Users\" + userName + @"\AppData\Roaming\Autodesk\Revit\Addins\2022\PlanitPlugin\mainConfig.json";
            
            ParsePresetFile();
        }

        #region Accessors

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

        #endregion

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

        public void RemoveTool(Guid toolId, ToolTypes toolType)
        {
            ICustomButtonData toolToRemove;
            if (toolType == ToolTypes.Dynamo)
            {
                toolToRemove = _customDynamoTools
                    .FirstOrDefault(tool => tool.Id == toolId);

                _customDynamoTools = _customDynamoTools
                    .Where(tool => tool.Id != toolId)
                    .ToArray();
                _pluginConfig.customScriptItems.dynamo = _customDynamoTools;
            }
            else
            {
                toolToRemove = _customCsharpTools
                    .FirstOrDefault(tool => tool.Id == toolId);

                _customCsharpTools = _customCsharpTools
                    .Where(tool => tool.Id != toolId)
                    .ToArray();
                _pluginConfig.customScriptItems.csharp = _customCsharpTools;
            }


            // TODO: Think how to clean icon when it is not used by Revit
            try
            {
                if (!String.IsNullOrEmpty(toolToRemove.ButtonImagePath))
                    File.Delete(toolToRemove.ButtonImagePath);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error during deleting the icon file.");
                Console.WriteLine(e);
            }

            SafeDeleteFile(toolToRemove.FilePath);
            
            // TODO: set flag and check if error was catched during execution.
            // If so, show a message box with the error message and process transaction rollback 

            UpdateConfigFile(_pluginConfig);
        }

        public void UpdateTool(ICustomButtonData updatedToolData)
        {
            ToolTypes toolType = updatedToolData.ToolType;
            var allTools = toolType == ToolTypes.Dynamo
                ? _customDynamoTools.Cast<ICustomButtonData>().ToArray()
                : _customCsharpTools.Cast<ICustomButtonData>().ToArray();

            ICustomButtonData originalToolData = allTools
                .FirstOrDefault(t => t.Id == updatedToolData.Id);
            if (originalToolData == null)
                throw new Exception("Error during searching the tool to be updated.");

            string currentUser = Environment.UserName;
            if (originalToolData.FilePath != updatedToolData.FilePath)
            {
                string fileName = Path.GetFileName(updatedToolData.FilePath);
                bool nameUsed = IsFileNameUsed(fileName, toolType);
                // Check if the file name is already used by another tool. But name could be the same as for tool to be updated
                if (nameUsed && fileName != Path.GetFileName(originalToolData.FilePath))
                    throw new Exception("File with such name already exists. Change the selected file name.");

                // delete the old file
                if (!String.IsNullOrEmpty(originalToolData.FilePath))
                    SafeDeleteFile(originalToolData.FilePath);

                // copy the new file
                string targetFileFolder = toolType == ToolTypes.Dynamo
                    ? @"C:\Users\" + currentUser +
                      @"\AppData\Roaming\Autodesk\Revit\Addins\2022\PlanitPlugin\CustomTools\Dynamo"
                    : @"C:\Users\" + currentUser +
                      @"\AppData\Roaming\Autodesk\Revit\Addins\2022\PlanitPlugin\CustomTools\CSharp";

                // TODO: wrap the file copying in a try-catch block. If error happens, show a message box with the error message
                if (updatedToolData.FilePath != null)
                    File.Copy(updatedToolData.FilePath, Path.Combine(targetFileFolder, fileName));

                // update the file path in the tool data to follow correct path in folder hierarchy
                updatedToolData.FilePath = $"{targetFileFolder}\\{fileName}";
            }

            if (originalToolData.ButtonImagePath != updatedToolData.ButtonImagePath)
            {
                // delete the old icon
                if (originalToolData.ButtonImagePath != null) File.Delete(originalToolData.ButtonImagePath);

                // copy the new icon
                string targetIconFolder = toolType == ToolTypes.Dynamo
                    ? @"C:\Users\" + currentUser +
                      @"\AppData\Roaming\Autodesk\Revit\Addins\2022\PlanitPlugin\CustomTools\Dynamo\Images"
                    : @"C:\Users\" + currentUser +
                      @"\AppData\Roaming\Autodesk\Revit\Addins\2022\PlanitPlugin\CustomTools\CSharp\Images";

                // TODO: wrap the file copying in a try-catch block. If error happens, show a message box with the error message
                if (!String.IsNullOrEmpty(updatedToolData.ButtonImagePath))
                    File.Copy(updatedToolData.ButtonImagePath,
                        Path.Combine(targetIconFolder, Path.GetFileName(updatedToolData.ButtonImagePath)));

                updatedToolData.ButtonImagePath =
                    $"{targetIconFolder}\\{Path.GetFileName(updatedToolData.ButtonImagePath)}";
            }

            // replace the old tool with the updated one in the array
            if (toolType == ToolTypes.Dynamo)
            {
                _customDynamoTools = _customDynamoTools
                    .Where(t => t.Id != updatedToolData.Id)
                    .Append((DynamoScriptCustomButtonData)updatedToolData)
                    .ToArray();
                _pluginConfig.customScriptItems.dynamo = _customDynamoTools;
            }
            else
            {
                _customCsharpTools = _customCsharpTools
                    .Where(t => t.Id != updatedToolData.Id)
                    .Append((CSharpCustomButtonData)updatedToolData)
                    .ToArray();
                _pluginConfig.customScriptItems.csharp = _customCsharpTools;
            }

            // TODO: set flag and check if error was catched during execution.
            // If so, show a message box with the error message and process transaction rollback 
            
            UpdateConfigFile(_pluginConfig);
        }

        #region Utilities

        private void ParsePresetFile()
        {
            // read config file
            string content = File.ReadAllText(_presetLocation);

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
            File.WriteAllText(_presetLocation, json);
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
        
        private void SafeDeleteFile(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                    File.Delete(filePath);
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine("Most likely file in use: " + ex.Message);
                throw new UnauthorizedAccessException("It seems that file You try to delete/replace is used by Revit. Try to restart Revit and repeat desired action with custom tool again before You launch the tool in the Revit.");
            }
            catch (IOException ex)
            {
                Console.WriteLine("File in use or other I/O error: " + ex.Message);
                throw new IOException(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An unexpected error occurred: " + ex.Message);
                throw new Exception(ex.Message);
            }
        }
        #endregion
    }
}
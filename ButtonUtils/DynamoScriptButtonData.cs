namespace R2022.ButtonUtils
{
    public struct DynamoScriptButtonData
    {
        public string ScriptPath { get; set; }
        public string ButtonName { get; set; }
        public string ButtonImagePath { get; set; }
        public string Description { get; set; }

        public DynamoScriptButtonData(string buttonName, string scriptPath, string buttonImage = null, string description = null)
        {
            ButtonName = buttonName;
            ScriptPath = scriptPath;
            ButtonImagePath = buttonImage;
            Description = description;
        }
    }
}
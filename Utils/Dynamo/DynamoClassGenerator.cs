using System;
using System.CodeDom.Compiler;
using System.IO;
using Microsoft.CSharp;

namespace R2022.Utils.Dynamo
{
    public static class DynamoClassGenerator
    {
        public static Type GenerateDynamoCommandClass(string className, string scriptPath, string dllPath)
        {
            // Check if the DLL file already exists
            if (File.Exists(dllPath))
            {
                try
                {
                    // Attempt to delete the existing DLL file
                    File.Delete(dllPath);
                }
                catch (Exception ex)
                {
                    // Return a message to the user if the file cannot be deleted
                    throw new InvalidOperationException($"The DLL file '{dllPath}' could not be deleted. Please restart Revit and try again.", ex);
                }
            }
            
            string classCode = $@"
            using System.Collections.Generic;
            using Autodesk.Revit.Attributes;
            using Autodesk.Revit.DB;
            using Autodesk.Revit.UI;
            using Dynamo.Applications;

            namespace R2022.DynamoUtils
            {{
                [Transaction(TransactionMode.Manual)]
                [Regeneration(RegenerationOption.Manual)]
                public class {className} : IExternalCommand
                {{
                    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
                    {{
                        var dynamoRevit = new DynamoRevit();

                        IDictionary<string, string> journalData = new Dictionary<string, string>
                        {{
                            {{JournalKeys.ShowUiKey, false.ToString()}},
                            {{JournalKeys.AutomationModeKey, false.ToString()}},
                            {{JournalKeys.DynPathKey, @""{scriptPath}""}},
                            {{JournalKeys.DynPathExecuteKey, true.ToString()}},
                            {{JournalKeys.ForceManualRunKey, true.ToString()}},
                            {{JournalKeys.ModelShutDownKey, false.ToString()}},
                            {{JournalKeys.ModelNodesInfo, false.ToString()}},
                        }};
                        
                        var dynamoRevitCommandData = new DynamoRevitCommandData
                        {{
                            Application = commandData.Application,
                            JournalData = journalData
                        }};

                        return dynamoRevit.ExecuteCommand(dynamoRevitCommandData);
                    }}
                }}
            }}";

            var parameters = new CompilerParameters
            {
                GenerateInMemory = true,
                GenerateExecutable = false,
                OutputAssembly = dllPath
            };
            
            // TODO: check for routes for current assemblies before creating installer
            // Add references to necessary assemblies
            parameters.ReferencedAssemblies.Add("RevitAPI.dll");
            parameters.ReferencedAssemblies.Add("RevitAPIUI.dll");
            parameters.ReferencedAssemblies.Add("System.dll");
            parameters.ReferencedAssemblies.Add("System.Core.dll");
            parameters.ReferencedAssemblies.Add(@"C:\Program Files\Autodesk\Revit 2022\AddIns\DynamoForRevit\DynamoApplications.dll");
            parameters.ReferencedAssemblies.Add(@"C:\Program Files\Autodesk\Revit 2022\AddIns\DynamoForRevit\Revit\DynamoRevitDS.dll");

            using (var provider = new CSharpCodeProvider())
            {
                CompilerResults results = provider.CompileAssemblyFromSource(parameters, classCode);

                if (results.Errors.HasErrors)
                {
                    throw new InvalidOperationException($"Class generation failed with the following errors: {results.Errors}");
                }

                return results.CompiledAssembly.GetType($"R2022.DynamoUtils.{className}");
            }
        }
    }
}
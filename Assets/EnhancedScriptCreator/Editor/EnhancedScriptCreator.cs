using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System;
using System.Linq;
using System.Text;

namespace EnhancedScriptCreator
{
    public class EnhancedScriptCreator : AssetPostprocessor
    {

#region Variables

        // The path to the folder with this script in it
        private static string _baseAssetPath;
        private static string baseAssetPath
        {
            get 
            {
                if (string.IsNullOrEmpty(_baseAssetPath))
                {
                    _baseAssetPath = GetBaseAssetPath();
                }

                return _baseAssetPath;
            }
            set 
            {
                _baseAssetPath = value;
            }
        }

        // The folder containing the scripts (used in conjunction with baseAssetPath)
        private static string scriptsPath = "Template Scripts";
        // The name of class that gets generated containing the menu headers
        private static string generatedFileName = "MenuHeaders";
        // True when this class has processed the assets. Used to prevent infinite asset processing
        private static bool assetsProcessed = false;

#endregion

#region C# Script Creation

        // Called by methods in the generated class. Used to actually create the new class
        public static void CreateClass(string fileName, string codeFilePath)
        {
            var obj = Selection.activeObject;
            string currentPath = AssetDatabase.GetAssetPath(obj); // Find the folder the user is currently in in the editor

            // Open a save dialog
            string savePath = EditorUtility.SaveFilePanelInProject("Save Your Class", fileName.Substring(0, fileName.LastIndexOf('.')) + ".cs", 
                "cs", "Please enter a name for your new class", currentPath);

            if (!string.IsNullOrEmpty(savePath)) // After the user has pressed save
            {
                string code = File.ReadAllText(codeFilePath); // Get all the code from the text file
                
                fileName = Path.GetFileName(savePath);
                fileName = fileName.Substring(0, fileName.LastIndexOf("."));
                
                ReplacePlaceholders(ref code, fileName);

                // Save the new file and force an asset database refresh
                File.WriteAllText(savePath, code);
                AssetDatabase.ImportAsset(savePath, ImportAssetOptions.ForceUpdate);
            }
        }

        private static void ReplacePlaceholders(ref string code, string fileName)
        {
            code = code.Replace("#SCRIPTNAME#", fileName); // Remove any placeholders
        }

#endregion

#region Menu Header Generation

        // Called by Unity when an asset has finished importing
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPath)
        {
            if (assetsProcessed) // If this function has already completed once on this asset import
            {
                assetsProcessed = false;
                return; // Do not do it again
            }

            // Check all the assets that have changed to see if they are within the scriptsPath directory
            bool directoryFound = CheckPathsForDirectory(importedAssets);
            if (!directoryFound) directoryFound = CheckPathsForDirectory(deletedAssets);
            if (!directoryFound) directoryFound = CheckPathsForDirectory(movedAssets);
            if (!directoryFound) directoryFound = CheckPathsForDirectory(movedFromAssetPath);

            // If the assets that have changed are not within the scriptsPath directory
            if (!directoryFound)
            {
                return;
            }

            if (CheckBaseAssetPathChange(movedAssets))
            {
                baseAssetPath = GetBaseAssetPath();
            }

            GenerateMenuHeaders();
        }

        // Generates the menu headers for the classes
        private static void GenerateMenuHeaders()
        {
            StringBuilder code = new StringBuilder();
            string scriptsFolderPath = Path.Combine(baseAssetPath, scriptsPath);
            string generatedFilePath = baseAssetPath;

            DirectoryInfo directory = new DirectoryInfo(scriptsFolderPath);
            FileInfo[] classes = directory.GetFiles("*.txt"); // Find all the .txt classes within the scripts folder

            AddHeader(ref code);

            for (int i = 0; i < classes.Length; i++)
            {
                AddMethod(ref code, classes[i], i); // Add the method body to the generated class
            }

            AddFooter(ref code);

            Directory.CreateDirectory(generatedFilePath); // Make sure the directory exists

            generatedFilePath = Path.Combine(generatedFilePath, generatedFileName + ".cs");

            // Create the file containing the code and then force an asset database update
            File.WriteAllText(generatedFilePath, code.ToString());
            AssetDatabase.ImportAsset("Assets" + generatedFilePath.Replace(Application.dataPath, ""), ImportAssetOptions.ForceUpdate);

            // Set flag to true so this function doesn't get called infinitely
            assetsProcessed = true;
        }

        // Add a header to the generated class including usings, namepsace and class name
        private static void AddHeader(ref StringBuilder s)
        {
            s.Append("using UnityEditor;" + Environment.NewLine);
            s.Append("using UnityEngine;" + Environment.NewLine);
            s.Append("using System.IO;" + Environment.NewLine);
            s.Append("namespace EnhancedScriptCreator" + Environment.NewLine);
            s.Append("{" + Environment.NewLine);

            s.Append("public class " + generatedFileName + Environment.NewLine);
            s.Append("{" + Environment.NewLine);
        }

        // Add a menu header method that allows this method to be called from the editor menu
        private static void AddMethod(ref StringBuilder s, FileInfo fileInfo, int index)
        {
            string name = fileInfo.Name.Substring(0, fileInfo.Name.LastIndexOf(".")).Replace(" ", "");

            s.Append("[MenuItem (\"Assets/Create/eC# Script/" + name + "\", false, 81)]" + Environment.NewLine);
            s.Append("private static void MenuItem" + index + "()" + Environment.NewLine);
            s.Append("{" + Environment.NewLine);

            AddMethodBody(ref s, fileInfo);

            s.Append("}" + Environment.NewLine);
            s.Append(Environment.NewLine);
        }

        // Add the body of the method which provides a call to the function which creates the new class
        private static void AddMethodBody(ref StringBuilder s, FileInfo fileInfo)
        {
            s.Append("EnhancedScriptCreator.CreateClass(\"" + fileInfo.Name + "\", \"" + fileInfo.FullName + "\");" + Environment.NewLine);
        }

        private static void AddFooter(ref StringBuilder s)
        {
            s.Append("}" + Environment.NewLine);
            s.Append("}" + Environment.NewLine);
        }

        // Loops through an array of strings to find a specific string
        private static bool CheckPathsForDirectory(string[] strings)
        {
            for (int i = 0; i < strings.Length; i++)
            {
                if (strings[i].Contains(scriptsPath))
                {
                    return true;
                }
            }

            return false;
        }

#endregion

        private static string GetBaseAssetPath()
        {
            var GUIDs = AssetDatabase.FindAssets("t:Script EnhancedScriptCreator");
            var path = Application.dataPath.Substring(0, Application.dataPath.Length - "/Assets".Length) + System.IO.Path.DirectorySeparatorChar +
                        AssetDatabase.GUIDToAssetPath(GUIDs[0]).Replace('/', System.IO.Path.DirectorySeparatorChar);         
            path = path.Replace("EnhancedScriptCreator.cs", "");

            return path;
        }

        private static bool CheckBaseAssetPathChange(string[] strings)
        {
            for (int i = 0; i < strings.Length; i++)
            {
                if (strings[i].Contains("EnhancedScriptCreator.cs"))
                {
                    return true;
                }
            }

            return false;
        }
    }
}

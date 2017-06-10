using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System;
using System.Linq;

namespace EnhancedScriptCreator
{
    public class EnhancedScriptCreator : AssetPostprocessor
    {
        // The path to the folder with this script in it
        private static string baseAssetPath = Application.dataPath + "/EnhancedScriptCreator/Editor";
        // The folder containing the scripts (used in conjunction with baseAssetPath)
        private static string scriptsPath = "eC#Scripts";
        // The name of class that gets generated containing the menu headers
        private static string generatedFileName = "MenuHeaders";

        // True when this class has processed the assets. Used to prevent infinite asset processing
        private static bool assetsProcessed = false;

        // Called by Unity when an asset has finished importing
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPath)
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

            GenerateMenuHeaders();
        }

        // Generates the menu headers for the classes
        static void GenerateMenuHeaders()
        {
            // TODO: CHANGE TO STRING BUILDER
            string code = AddHeader();

            string path = Path.Combine(baseAssetPath, scriptsPath);

            DirectoryInfo dir = new DirectoryInfo(path);
            FileInfo[] info = dir.GetFiles("*.txt");
            foreach (FileInfo f in info)
            {
                code += ProcessFile(f);
            }

            code += AddFooter();

            if (info.Length > 0)
            {
                string newPath = baseAssetPath;

                Directory.CreateDirectory(newPath);

                newPath = Path.Combine(newPath, generatedFileName + ".cs");
                File.WriteAllText(newPath, code);
                AssetDatabase.ImportAsset("Assets" + newPath.Replace(Application.dataPath, ""), ImportAssetOptions.ForceUpdate);

                assetsProcessed = true;
            }
        }

        static string AddHeader()
        {
            string s = "using UnityEditor;" + Environment.NewLine;
            s += "using UnityEngine;" + Environment.NewLine;
            s += "using System.IO;" + Environment.NewLine;
            s += "namespace EnhancedScriptCreator" + Environment.NewLine;
            s += "{" + Environment.NewLine;

            s += "public class GeneratedCode" + Environment.NewLine;
            s += "{" + Environment.NewLine;

            return s;
        }

        static string ProcessFile(FileInfo fileInfo)
        {
            string name = fileInfo.Name.Substring(0, fileInfo.Name.LastIndexOf(".")).Replace(" ", "");

            string s = "[MenuItem (\"Assets/Create/eC# Script/" + name + "\", false, 81)]" + Environment.NewLine;
            s += "private static void MenuItem" + name + "()" + Environment.NewLine;
            s += "{" + Environment.NewLine;
            s += AddBody(fileInfo);
            s += "}" + Environment.NewLine;
            s += Environment.NewLine;

            return s;
        }

        static string AddBody(FileInfo fileInfo)
        {
            string str = "CreateScriptWindow window = (CreateScriptWindow)EditorWindow.GetWindow(typeof(CreateScriptWindow));" + Environment.NewLine;
            str += "window.codeFilePath = " + @"""" + fileInfo.FullName + @"""" + ";" + Environment.NewLine;
            str += "window.Show();" + Environment.NewLine;

            return str;
        }

        static string AddFooter()
        {
            string s = "}" + Environment.NewLine;
            s += "}" + Environment.NewLine;

            return s;
        }

        static bool CheckPathsForDirectory(string[] strings)
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
    }

    public class CreateScriptWindow : EditorWindow
    {
        public string codeFilePath;

        string className;

        void OnGUI()
        {
            className = EditorGUILayout.TextField("Class Name", className);

            if (GUILayout.Button("Create Script"))
            {
                OnClickCreate();

                GUIUtility.ExitGUI();
            }
        }

        void OnClickCreate()
        {
            if (string.IsNullOrEmpty(className))
            {
                EditorUtility.DisplayDialog("Unable to create class", "Please enter a valid class name", "Close");
                return;
            }

            var obj = Selection.activeObject;
            string path = AssetDatabase.GetAssetPath(obj);

            string filePath = path + "/" + className + ".cs";

            string code = ReplacePlaceholders(File.ReadAllText(codeFilePath));

            File.WriteAllText(filePath, code);
            AssetDatabase.ImportAsset(filePath, ImportAssetOptions.ForceUpdate);

            Close();
        }

        string ReplacePlaceholders(string code)
        {
            return code.Replace("#SCRIPTNAME#", className);
        }
    }
}

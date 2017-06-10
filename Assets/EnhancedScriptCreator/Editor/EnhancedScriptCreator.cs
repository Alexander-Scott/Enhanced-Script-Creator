using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System;

namespace EnhancedScriptCreator
{
    public class EnhancedScriptCreator : UnityEditor.AssetModificationProcessor
    {
        private static string assetPath = Application.dataPath + "/EnhancedScriptCreator/Editor";

        private static string scriptsPath = "eC#Scripts";
        private static string generatedPath = "GeneratedCode";

        private static string generatedFileName = "MenuHeaders";


        [InitializeOnLoad]
        public class BuildInfo
        {
            static BuildInfo()
            {
                EditorApplication.update += Update;
            }

            static bool isCompiling = true;
            static void Update()
            {
                if (!EditorApplication.isCompiling && isCompiling)
                {
                    GenerateMenuMethods();
                }

                isCompiling = EditorApplication.isCompiling;
            }

            static void GenerateMenuMethods()
            {
                string code = AddHeader();

                string path = Path.Combine(assetPath, scriptsPath);

                DirectoryInfo dir = new DirectoryInfo(path);
                FileInfo[] info = dir.GetFiles("*.txt");
                foreach (FileInfo f in info)
                {
                    code += ProcessFile(f);
                }

                code += AddFooter();

                if (info.Length > 0)
                {
                    string newPath = Path.Combine(assetPath, generatedPath);

                    Directory.CreateDirectory(newPath);
 
                    newPath = Path.Combine(newPath, generatedFileName + ".cs");
                    File.WriteAllText(newPath, code);
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
                string name = fileInfo.Name.Substring(0, fileInfo.Name.LastIndexOf("."));

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
                // string str = "string name = " +  @"""" + fileInfo.FullName + @"""" + ";" + Environment.NewLine;
                // str += "var obj = Selection.activeObject;" + Environment.NewLine;
                // str += "string path = AssetDatabase.GetAssetPath(obj);" + Environment.NewLine;
                // str += "string code = File.ReadAllText(name);" + Environment.NewLine;
                // str += "string filePath = path + \"/NewFile.cs\";" + Environment.NewLine;
                // str += "File.WriteAllText(filePath, code);" + Environment.NewLine;
                // str += "AssetDatabase.ImportAsset(filePath, ImportAssetOptions.ForceUpdate);" + Environment.NewLine;

                string str = "SaveWindow window = (SaveWindow)EditorWindow.GetWindow(typeof(SaveWindow));" + Environment.NewLine;
                str += "window.codeFilePath = " +  @"""" + fileInfo.FullName + @"""" + ";" + Environment.NewLine;
                str += "window.Show();" + Environment.NewLine;

                return str;
            }

            static string AddFooter()
            {
                string s = "}" + Environment.NewLine;
                s += "}" + Environment.NewLine;

                return s;
            }
        }
    }
}

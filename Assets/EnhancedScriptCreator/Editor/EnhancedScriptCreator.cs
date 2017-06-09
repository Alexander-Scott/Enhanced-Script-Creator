using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System;

namespace EnhancedScriptCreator
{
    public class EnhancedScriptCreator : UnityEditor.AssetModificationProcessor
    {
        private static string assetPath = "EnhancedScriptCreator/Editor";

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

                string path = Path.Combine(Application.dataPath, assetPath);
                path = Path.Combine(path, scriptsPath);

                DirectoryInfo dir = new DirectoryInfo(path);
                FileInfo[] info = dir.GetFiles("*.txt");
                foreach (FileInfo f in info)
                {
                    code += ProcessFile(f);
                }

                code += AddFooter();

                if (info.Length > 0)
                {
                    string newPath = Path.Combine(Application.dataPath, assetPath);
                    newPath = Path.Combine(newPath, generatedPath);

                    Directory.CreateDirectory(newPath);

                    newPath = Path.Combine(Application.dataPath, assetPath);
                    newPath = Path.Combine(newPath, generatedPath);
                    newPath = Path.Combine(newPath, generatedFileName + ".cs");
                    File.WriteAllText(newPath, code);
                }
            }

            static string AddHeader()
            {
                string s = "using UnityEditor;" + Environment.NewLine;
                s += "using UnityEngine;" + Environment.NewLine;
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
                // var obj = Selection.activeObject;
                // string path = AssetDatabase.GetAssetPath(obj);

                // string newPath = path.Substring(0, fileInfo.Name.LastIndexOf("\\"));

                // ProjectWindowUtil.CreateAsset(Resources.LoadAssetAtPath(fileInfo.Name, typeof(TextAsset)), newPath + "\\NewFile.cs");    
                // fileInfo.FullName.Replace("/", @"/""")          

                string str = "string name = " +  @"""" + fileInfo.FullName + @"""" + ";" + Environment.NewLine;
                str += "var obj = Selection.activeObject;" + Environment.NewLine;
                str += "string path = AssetDatabase.GetAssetPath(obj);" + Environment.NewLine;
                //str += "string newPath = name.Substring(0, name.LastIndexOf(\"/\"));" + Environment.NewLine;
                str += "ProjectWindowUtil.CreateAsset(Resources.LoadAssetAtPath(name, typeof(TextAsset)), path + \"//NewFile.cs\");" + Environment.NewLine;

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

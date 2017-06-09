using UnityEditor;
using UnityEngine;
namespace EnhancedScriptCreator
{
public class GeneratedCode
{
[MenuItem ("Assets/Create/eC# Script/Test", false, 81)]
private static void MenuItemTest()
{
string name = "/Users/alexscott/Dropbox/Projects/Enhanced-Script-Creator/Assets/EnhancedScriptCreator/Editor/eC#Scripts/Test.txt";
var obj = Selection.activeObject;
string path = AssetDatabase.GetAssetPath(obj);
ProjectWindowUtil.CreateAsset(Resources.LoadAssetAtPath(name, typeof(TextAsset)), path + "//NewFile.cs");
}

}
}

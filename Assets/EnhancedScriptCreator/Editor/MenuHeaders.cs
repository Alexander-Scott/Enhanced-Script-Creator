using UnityEditor;
using UnityEngine;
using System.IO;
namespace EnhancedScriptCreator
{
public class GeneratedCode
{
[MenuItem ("Assets/Create/eC# Script/Singleton1", false, 81)]
private static void MenuItemSingleton1()
{
CreateScriptWindow window = (CreateScriptWindow)EditorWindow.GetWindow(typeof(CreateScriptWindow));
window.codeFilePath = "/Users/alexscott/Dropbox/Projects/Enhanced-Script-Creator/Assets/EnhancedScriptCreator/Editor/eC#Scripts/Singleton1.txt";
window.Show();
}

}
}

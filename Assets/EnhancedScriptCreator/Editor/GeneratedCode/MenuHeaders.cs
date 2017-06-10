using UnityEditor;
using UnityEngine;
using System.IO;
namespace EnhancedScriptCreator
{
public class GeneratedCode
{
[MenuItem ("Assets/Create/eC# Script/Test", false, 81)]
private static void MenuItemTest()
{
SaveWindow window = (SaveWindow)EditorWindow.GetWindow(typeof(SaveWindow));
window.codeFilePath = "/Users/alexscott/Dropbox/Projects/Enhanced-Script-Creator/Assets/EnhancedScriptCreator/Editor/eC#Scripts/Test.txt";
window.Show();
}

}
}

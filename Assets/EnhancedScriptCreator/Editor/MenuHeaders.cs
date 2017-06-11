using UnityEditor;
using UnityEngine;
using System.IO;
namespace EnhancedScriptCreator
{
public class MenuHeaders
{
[MenuItem ("Assets/Create/eC# Script/MonoBehaviour", false, 81)]
private static void MenuItem0()
{
EnhancedScriptCreator.CreateClass("MonoBehaviour.txt", "/Users/alexscott/Dropbox/Projects/Enhanced-Script-Creator/Assets/EnhancedScriptCreator/Editor/Template Scripts/MonoBehaviour.txt");
}

[MenuItem ("Assets/Create/eC# Script/ObjectPool", false, 81)]
private static void MenuItem1()
{
EnhancedScriptCreator.CreateClass("ObjectPool.txt", "/Users/alexscott/Dropbox/Projects/Enhanced-Script-Creator/Assets/EnhancedScriptCreator/Editor/Template Scripts/ObjectPool.txt");
}

[MenuItem ("Assets/Create/eC# Script/Singleton", false, 81)]
private static void MenuItem2()
{
EnhancedScriptCreator.CreateClass("Singleton.txt", "/Users/alexscott/Dropbox/Projects/Enhanced-Script-Creator/Assets/EnhancedScriptCreator/Editor/Template Scripts/Singleton.txt");
}

}
}

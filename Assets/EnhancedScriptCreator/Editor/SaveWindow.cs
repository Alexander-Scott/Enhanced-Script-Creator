using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;

public class SaveWindow : EditorWindow 
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Voidless
{
public class VDataManagerWindow : EditorWindow
{
	protected const string VDATAMANAGERWINDOW_PATH = "PLEASE/REPLACE/THIS VDataManagerWindow's PATH"; 	/// <summary>VDataManagerWindow's path.</summary>

	public static VDataManagerWindow vDataManagerWindow; 										/// <summary>VDataManagerWindow's static reference</summary>
	private SerializedProperty property;
	private SerializedObject obj;

	/// <summary>Creates a new VDataManagerWindow window.</summary>
	/// <returns>Created VDataManagerWindow window.</summary>
	[MenuItem(VDATAMANAGERWINDOW_PATH)]
	public static VDataManagerWindow CreateVDataManagerWindow()
	{
		vDataManagerWindow = GetWindow<VDataManagerWindow>("Voidless' Data Manager Window");
		
		return vDataManagerWindow;
	}

	/// <summary>Use OnGUI to draw all the controls of your window.</summary>
	private void OnGUI()
	{
		StringStringDictionary dictionary = new StringStringDictionary();

		dictionary.Add("Number 1", "Cool Element");
		dictionary.Add("Number 2", "Not So Cool Element");

		/*obj = Editor.SerializedObject(dictionary);

		VEditorGUILayout.DictionaryField();*/
	}
}
}
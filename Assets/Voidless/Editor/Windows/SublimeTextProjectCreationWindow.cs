﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text;

namespace Voidless
{
public class SublimeTextProjectCreationWindow : EditorWindow
{
	protected const string SUBLIMETEXTPROJECTCREATIONWINDOW_PATH = "Voidless/Sublime Text Utilities/Create Sublime Project";
	public const string EXTERNAL_SCRIPT_EDITOR_ARGS = "$(File):$(Line)";
	public const string GIT_IGNOREEXTENSION_SUBLIME_PROJECT = "*.sublime-project";
	public const string GIT_IGNOREEXTENSION_SUBLIME_WORKSPACE = "*.sublime-workspace";

	public static SublimeTextProjectCreationWindow sublimeTextProjectCreationWindow;
	private SublimeProjectSettings settings;
	private HashSet<string> subPaths;
	private HashSet<string> deleteSubPaths;

	/// <summary>Creates a new SublimeTextProjectCreationWindow window.</summary>
	/// <returns>Created SublimeTextProjectCreationWindow window.</summary>
	[MenuItem(SUBLIMETEXTPROJECTCREATIONWINDOW_PATH)]
	public static SublimeTextProjectCreationWindow CreateSublimeTextProjectCreationWindow()
	{
		sublimeTextProjectCreationWindow = GetWindow<SublimeTextProjectCreationWindow>("Create Sublime Text Project");
		
		if(sublimeTextProjectCreationWindow == null) return null;

		sublimeTextProjectCreationWindow.LoadData();

		return sublimeTextProjectCreationWindow;
	}

	/// <summary>Use OnGUI to draw all the controls of your window.</summary>
	private void OnGUI()
	{
		if(GUILayout.Button("Copy External Scripts Editor Args to Clipboard"))
		{
			VGUIUtility.CopyToClipboard(EXTERNAL_SCRIPT_EDITOR_ARGS);
		}
		GUILayout.Space(5.0f);
		GUILayout.Label("Folder Paths: ");
		GUILayout.BeginVertical();
		foreach(string subPath in subPaths)
		{
			GUILayout.BeginHorizontal();
			GUILayout.Label(subPath);
			if(GUILayout.Button("Delete")) deleteSubPaths.Add(subPath);
			GUILayout.EndHorizontal();
		}
		GUILayout.EndVertical();
		if(GUILayout.Button("Add Path"))
		{
			string newSubPath = EditorUtility.SaveFolderPanel("Select Folder Sub-Path", string.Empty, string.Empty);
			newSubPath = TrimmedPath(newSubPath);
			bool empty = string.IsNullOrEmpty(newSubPath);

			if(!empty && !subPaths.Contains(newSubPath)) subPaths.Add(newSubPath);
			else
			{
				string title = empty ? "Path Not Provided" : "Path Already Included";
				string message = empty ? "You didn't provide a valid path." : ("Path " + newSubPath + " is already included.");
				EditorUtility.DisplayDialog(title, message, "Okay");
			}
		}
		if(GUILayout.Button("Save")) SaveData();

		if(deleteSubPaths.Count > 0)
		{
			foreach(string subPath in deleteSubPaths)
			{
				subPaths.Remove(subPath);
			}

			deleteSubPaths.Clear();
		}
	}

	/// <returns>Settings' Path.</returns>
	private static string GetSettingsPath()
	{
		StringBuilder builder = new StringBuilder();

		builder.Append(VString.GetProjectPath());
		builder.Append("/SublimeProject_");
		builder.Append(VString.GetProjectName());
		builder.Append(".sublime-project");

		return builder.ToString();
	}

	/// <summary>Converts full-path into a trimmed part [the path begins from Assets/...].</summary>
	/// <param name="fullPath">Full path.</param>
	/// <returns>Trimmed path.</returns>
	private string TrimmedPath(string fullPath)
	{
		StringBuilder builder = new StringBuilder();
		string[] split = fullPath.Split('/');
		int index = 0;
		int length = 0;

		foreach(string s in split)
		{
			if(s == "Assets") break;
			else index++;
		}

		for(int i = index; i < split.Length; i++)
		{
			builder.Append(split[i]);
			if(i < split.Length - 1) builder.Append("/");
		}

		return builder.ToString();
	}

	/// <summary>Loads Data.</summary>
	private void LoadData()
	{
		subPaths = new HashSet<string>();
		deleteSubPaths = new HashSet<string>();

		string path = GetSettingsPath();

		settings = VJSONSerializer.DeserializeFromJSONFromPath<SublimeProjectSettings>(path);

		if(settings.folders == null || settings.folders.Length == 0)
		settings.folders = new SublimeProjectFolder[]
		{
			new SublimeProjectFolder("Assets/Scripts"),
			new SublimeProjectFolder("Assets/Editor")
		};

		foreach(SublimeProjectFolder folder in settings.folders)
		{
			subPaths.Add(folder.path);
		}

		string json = JsonUtility.ToJson(settings, true); // True for Pretty Print...

		VFileIO.WriteFile(json, path);
	}

	/// <summary>Saves Data.</summary>
	private void SaveData()
	{
		string path = GetSettingsPath();

		List<string> subPathsList = new List<string>(subPaths);
		List<SublimeProjectFolder> folders = new List<SublimeProjectFolder>();

		foreach(SublimeProjectFolder folder in settings.folders)
		{
			string folderPath = folder.path;
			if(!subPaths.Contains(folderPath)) subPathsList.Add(folderPath);
		}

		foreach(string subPath in subPathsList)
		{
			folders.Add(new SublimeProjectFolder(subPath));
		}

		settings.folders = folders.ToArray();

		string json = JsonUtility.ToJson(settings, true);

		VGit.AddToGitIgnore(GIT_IGNOREEXTENSION_SUBLIME_PROJECT, GIT_IGNOREEXTENSION_SUBLIME_WORKSPACE);
		VFileIO.WriteFile(json, path);

		AssetDatabase.SaveAssets();
   		AssetDatabase.Refresh();

   		EditorUtility.DisplayDialog("Sublime Project Saved", "Sublime Project successfully saved at: " + path + ".", "Okay");
	}
}
}
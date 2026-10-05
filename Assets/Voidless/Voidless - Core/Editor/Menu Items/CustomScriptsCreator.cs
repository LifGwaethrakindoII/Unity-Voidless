using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;

/// \TODO Give most of the credit to Liorta's Code Template Asset on repository: https://bitbucket.org/liortal/code-templates
namespace Voidless
{
	public class DoCreateCodeFile : EndNameEditAction
	{
		/// \TODO Check EndNameEditAction's API.
		public override void Action(int _instanceId, string _pathName, string _resourceFile)
		{
			Object obj = CustomScriptsCreator.CreateScript(_pathName, _resourceFile);
			ProjectWindowUtil.ShowCreatedAsset(obj);
		}
	}

	public class CustomScriptsCreator
	{
		private const string ICON_CSHARP_SCRIPT = "cs Script Icon";
		private const string ICON_DOCUMENT = "TextAsset Icon";
		private const int MENU_ITEM_PRIORITY = 70;

#region StringConstants:
		private const string REPLACE_SCRIPTNAME_TAG = "#SCRIPTNAME#";
		private const string REPLACE_UPPERCASE_SCRIPTNAME_TAG = "#UPPERCASE_SCRIPTNAME#";
		private const string REPLACE_NOTRIM_TAG = "#NOTRIM#";
		private const string REPLACE_TARGETNAME_TAG = "#TARGETNAME#";
		private const string REPLACE_TARGETVARIABLE_TAG = "#TARGETVARIABLE#";
		private const string REPLACE_INSPECTOR_TAG = "Inspector";
		private const string REPLACE_PROPERTYDRAW_TAG = "PropertyDrawer";
		private const string MONOBEHAVIOUR_INITIAL_NAME = "NewBehavior.cs";
		private const string INSPECTOR_INITIAL_NAME = "NewInspector.cs";
		private const string SCRIPTABLEOBJECT_INITIAL_NAME = "NewScriptableObject.cs";
		private const string C_SHARP_INITIAL_NAME = "NewClass.cs";
		private const string INTERFACE_INITIAL_NAME = "NewInterface.cs";
		private const string PROPERTYDRAWER_INITIAL_NAME = "NewPropertyDrawer.cs";
		private const string STRUCT_INITIAL_NAME = "NewStruct.cs";
		private const string EDITORWINDOW_INITIAL_NAME = "NewEditorWindow.cs";
		private const string XML_INITIAL_NAME = "NewXMLDocument.xml";
		private const string TXT_INITIAL_NAME = "NewTXTDocument.txt";
		private const string MENU_ITEM_PATH = "Assets";
		private const string MENU_ITEM_CREATE_SCRIPT = "/Create Custom Script";
		private const string MENU_ITEM_CREATE_DOCUMENT = "/Create Document";
		private const string MENU_ITEM_C_SHARP_PATH = "/C#";
		private const string MENU_ITEM_CUSTOM_MONOBEHAVIOUR = "/MonoBehaviour";
		private const string MENU_ITEM_CUSTOM_INSPECTOR = "/Inspector";
		private const string MENU_ITEM_CUSTOM_SCRIPTABLEOBJECT = "/Scriptable Object";
		private const string MENU_ITEM_CUSTOM_EDITORWINDOW = "/Editor Window";
		private const string MENU_ITEM_CUSTOM_C_SHARP = "/Class";
		private const string MENU_ITEM_CUSTOM_INTERFACE = "/Interface";
		private const string MENU_ITEM_CUSTOM_PROPERTYDRAWER = "/Property Drawer";
		private const string MENU_ITEM_CUSTOM_STRUCT = "/Struct";
		private const string MENU_ITEM_XML = "/XML";
		private const string MENU_ITEM_TXT = "/TXT Document";
		private const string TEMPLATES_FOLDER_PATH = "Assets/Voidless/Editor/Data~/Custom Script Templates";
		private const string TEMPLATE_MONOBEHAVIOUR_PATH = "/Template_MonoBehaviour.cs.txt";
		private const string TEMPLATE_INSPECTOR_PATH = "/Template_Inspector.cs.txt";
		private const string TEMPLATE_SCRIPTABLEOBJECT_PATH = "/Template_ScriptableObject.cs.txt";
		private const string TEMPLATE_EDITORWINDOW_PATH = "/Template_EditorWindow.cs.txt";
		private const string TEMPLATE_C_SHARP_PATH = "/Template_C#_Class.cs.txt";
		private const string TEMPLATE_INTERFACE_PATH = "/Template_C#_Interface.cs.txt";
		private const string TEMPLATE_PROPERTYDRAWER_PATH = "/Template_PropertyDrawer.cs.txt";
		private const string TEMPLATE_STRUCT_PATH = "/Template_C#_Struct.cs.txt";
		private const string TEMPLATE_XML_PATH = "/Template_XML_Document.xml.txt";
		private const string TEMPLATE_TXT_PATH = "/Template_TXT_Document.txt";
		private const string WINDOW_ERROR_LABEL = "Script Template Not Found!";
		private const string WINDOW_ERROR_CONTENT = "There is no Template at path: \n\n";
		private const string WINDOW_OK_ANSWER = "Ok";
#endregion

		private static Texture2D scriptIcon = (EditorGUIUtility.IconContent(ICON_CSHARP_SCRIPT).image as Texture2D);
		private static Texture2D documentIcon = (EditorGUIUtility.IconContent(ICON_DOCUMENT).image as Texture2D);

#region ScriptCreationMethods:
		/// <summary>MonoBehaviour Script's creation procedures.</summary>
		[MenuItem(MENU_ITEM_PATH + MENU_ITEM_CREATE_SCRIPT + MENU_ITEM_C_SHARP_PATH + MENU_ITEM_CUSTOM_MONOBEHAVIOUR, false, MENU_ITEM_PRIORITY)]
		public static void CreateMonoBehaviourScript()
		{
			CreateFromTemplate(MONOBEHAVIOUR_INITIAL_NAME, TEMPLATES_FOLDER_PATH + TEMPLATE_MONOBEHAVIOUR_PATH);
		}

		/// <summary>Inspector Script's creation procedures.</summary>
		[MenuItem(MENU_ITEM_PATH + MENU_ITEM_CREATE_SCRIPT + MENU_ITEM_C_SHARP_PATH + MENU_ITEM_CUSTOM_INSPECTOR, false, MENU_ITEM_PRIORITY)]
		public static void CreateInspectorScript()
		{
			CreateFromTemplate(INSPECTOR_INITIAL_NAME, TEMPLATES_FOLDER_PATH + TEMPLATE_INSPECTOR_PATH);
		}

		/// <summary>ScriptableObject Script's creation procedures.</summary>
		[MenuItem(MENU_ITEM_PATH + MENU_ITEM_CREATE_SCRIPT + MENU_ITEM_C_SHARP_PATH + MENU_ITEM_CUSTOM_SCRIPTABLEOBJECT, false, MENU_ITEM_PRIORITY)]
		public static void CreateScriptableObjectScript()
		{
			CreateFromTemplate(SCRIPTABLEOBJECT_INITIAL_NAME, TEMPLATES_FOLDER_PATH + TEMPLATE_SCRIPTABLEOBJECT_PATH);
		}

		/// <summary>PropertyDrawer's Script's creation procedures.</summary>
		[MenuItem(MENU_ITEM_PATH + MENU_ITEM_CREATE_SCRIPT + MENU_ITEM_C_SHARP_PATH + MENU_ITEM_CUSTOM_PROPERTYDRAWER, false, MENU_ITEM_PRIORITY)]
		public static void CreatePropertyDrawerScript()
		{
			CreateFromTemplate(PROPERTYDRAWER_INITIAL_NAME, TEMPLATES_FOLDER_PATH + TEMPLATE_PROPERTYDRAWER_PATH);
		}

		/// <summary>EditorWindow Script's creation procedures.</summary>
		[MenuItem(MENU_ITEM_PATH + MENU_ITEM_CREATE_SCRIPT + MENU_ITEM_C_SHARP_PATH + MENU_ITEM_CUSTOM_EDITORWINDOW, false, MENU_ITEM_PRIORITY)]
		public static void CreateEditorWindowScript()
		{
			CreateFromTemplate(EDITORWINDOW_INITIAL_NAME, TEMPLATES_FOLDER_PATH + TEMPLATE_EDITORWINDOW_PATH);
		}

		/// <summary>C# Class's Script's creation procedures.</summary>
		[MenuItem(MENU_ITEM_PATH + MENU_ITEM_CREATE_SCRIPT + MENU_ITEM_C_SHARP_PATH + MENU_ITEM_CUSTOM_C_SHARP, false, MENU_ITEM_PRIORITY)]
		public static void CreateCSharpClassScript()
		{
			CreateFromTemplate(C_SHARP_INITIAL_NAME, TEMPLATES_FOLDER_PATH + TEMPLATE_C_SHARP_PATH);
		}

		/// <summary>Struct's Script's creation procedures.</summary>
		[MenuItem(MENU_ITEM_PATH + MENU_ITEM_CREATE_SCRIPT + MENU_ITEM_C_SHARP_PATH + MENU_ITEM_CUSTOM_STRUCT, false, MENU_ITEM_PRIORITY)]
		public static void CreateStructClassScript()
		{
			CreateFromTemplate(STRUCT_INITIAL_NAME, TEMPLATES_FOLDER_PATH + TEMPLATE_STRUCT_PATH);
		}

		/// <summary>Interface's Script's creation procedures.</summary>
		[MenuItem(MENU_ITEM_PATH + MENU_ITEM_CREATE_SCRIPT + MENU_ITEM_C_SHARP_PATH + MENU_ITEM_CUSTOM_INTERFACE, false, MENU_ITEM_PRIORITY)]
		public static void CreateInterfaceScript()
		{
			CreateFromTemplate(INTERFACE_INITIAL_NAME, TEMPLATES_FOLDER_PATH + TEMPLATE_INTERFACE_PATH);
		}

		/// <summary>XML Document's creation procedures.</summary>
		[MenuItem(MENU_ITEM_PATH + MENU_ITEM_CREATE_DOCUMENT + MENU_ITEM_XML, false, MENU_ITEM_PRIORITY)]
		public static void CreateXMLDocument()
		{
			CreateFromTemplate(XML_INITIAL_NAME, TEMPLATES_FOLDER_PATH + TEMPLATE_XML_PATH);
		}

		/// <summary>TXT Document's creation procedures.</summary>
		[MenuItem(MENU_ITEM_PATH + MENU_ITEM_CREATE_DOCUMENT + MENU_ITEM_TXT, false, MENU_ITEM_PRIORITY)]
		public static void CreateTXTDocument()
		{
			CreateFromTemplate(TXT_INITIAL_NAME, TEMPLATES_FOLDER_PATH + TEMPLATE_TXT_PATH);
		}
#endregion
			
		/// <summary>Creates Script on Path.</summary>
		/// <param name="_filePath">Path to import the Script.</param>
		/// <param name="_templatePath">Template's Path.</param>
		/// <param name="_scriptType">Type of script that will be created.</param>
		/// <returns>Created Asset.</returns>
		internal static UnityEngine.Object CreateScript(string _filePath, string _templatePath)
		{
			string newFilePath = Path.GetFullPath(_filePath);
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(_filePath);
			string className = NormalizeClassName(fileNameWithoutExtension);
			string templateText = string.Empty;

			if (File.Exists(_templatePath))
			{
				UTF8Encoding encoding = new UTF8Encoding(true, false);

				/// Read the file:
				// --- OLD WAY (Commented out for learning purposes) ---
		        // StreamReader reader = new StreamReader(_templatePath, encoding);
		        // templateText = reader.ReadToEnd();
		        // reader.Close();
		        // -----------------------------------------------------

		        // --- NEW WAY: Cleaner System.IO approach ---
		        // This works perfectly even if _templatePath is inside an invisible "~" folder
		        templateText = File.ReadAllText(_templatePath, encoding);

				templateText = templateText.Replace(REPLACE_SCRIPTNAME_TAG, className);
				templateText = templateText.Replace(REPLACE_NOTRIM_TAG, string.Empty);

				switch(_templatePath)
				{
					case TEMPLATES_FOLDER_PATH + TEMPLATE_INSPECTOR_PATH: //If the path given is the same of the Inspector's Template, make its respective procedures.
					string inspectorTargetName = className.Replace(REPLACE_INSPECTOR_TAG, string.Empty);
					templateText = templateText.Replace(REPLACE_TARGETNAME_TAG, inspectorTargetName);
					templateText = templateText.Replace(REPLACE_TARGETVARIABLE_TAG, inspectorTargetName.ToCamelCase());
					break;

					case TEMPLATES_FOLDER_PATH + TEMPLATE_PROPERTYDRAWER_PATH:
					string propertyDrawerName = className.Replace(REPLACE_PROPERTYDRAW_TAG, string.Empty);
					templateText = templateText.Replace(REPLACE_TARGETNAME_TAG, propertyDrawerName);
					break;

					case TEMPLATES_FOLDER_PATH + TEMPLATE_EDITORWINDOW_PATH: //If the path given is the same of the EditorWindow's Template, make its respective procedures.
					templateText = templateText.Replace(REPLACE_TARGETVARIABLE_TAG, className.ToCamelCase());
					templateText = templateText.Replace(REPLACE_UPPERCASE_SCRIPTNAME_TAG, className.ToUpper());
					break;

					default:
					break;
				}
				
				/// Write the file:
				StreamWriter writer = new StreamWriter(newFilePath, false, encoding);
				writer.Write(templateText);
				writer.Close();
				
				AssetDatabase.ImportAsset(_filePath);	
				return AssetDatabase.LoadAssetAtPath(_filePath, typeof(Object));
			}
			else
			{
				EditorUtility.DisplayDialog
	        	(
		            WINDOW_ERROR_LABEL,
		            WINDOW_ERROR_CONTENT + _templatePath,
		            WINDOW_OK_ANSWER
		        );

				return null;
			}
		}

		/// <summary>Normalizes class's name by replacing unnecessary characters.</summary>
		/// <param name="_fileName">File's name.</param>
		/// <returns>Normalized class's name.</returns>
		private static string NormalizeClassName(string _fileName)
		{
			return _fileName.Replace(" ", string.Empty);
		}

		/// <summary>Creates a new code file from a template file.</summary>
		/// <param name="_initialName">The initial name to give the file in the UI</param>
		/// <param name="_templatePath">The full path of the template file to use</param>
		public static void CreateFromTemplate(string _initialName, string _templatePath)
		{
			Texture2D icon = _templatePath == TEMPLATES_FOLDER_PATH + TEMPLATE_XML_PATH || _templatePath == TEMPLATES_FOLDER_PATH + TEMPLATE_TXT_PATH ? documentIcon : scriptIcon;
			ProjectWindowUtil.StartNameEditingIfProjectWindowExists
			(
				0,
				ScriptableObject.CreateInstance<DoCreateCodeFile>(),
				_initialName,
				icon,
				_templatePath
			);
		}
	}
}
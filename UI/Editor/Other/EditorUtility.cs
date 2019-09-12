using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System;
using System.CodeDom.Compiler;
using System.IO;
using UnityEngine.Assertions;

namespace Devdog.General.Editors
{
    using Object = UnityEngine.Object;

    public static class EditorUtility
    {
        public static void ErrorIfEmpty(System.Object o, string msg)
        {
            if (o == null)
            {
                EditorGUILayout.HelpBox(msg, MessageType.Error);
            }
        }

        public static void ErrorIfEmpty(Object o, string msg)
        {
            if (o == null)
            {
                EditorGUILayout.HelpBox(msg, MessageType.Error);
            }
        }

        public static void ErrorIfEmpty(bool o, string msg)
        {
            if (o)
            {
                EditorGUILayout.HelpBox(msg, MessageType.Error);
            }
        }

		public static void EditableLabel(
			SerializedProperty property,
			bool useArea, 
			Action markToSave,
			string helpText = null)
		{
			EditorGUILayout.LabelField(property.displayName, property.stringValue);

			if(GUILayout.Button("Edit..."))
			{
				var dialog = ScriptableObject.CreateInstance<InputDialog>();
				dialog.Init(property.displayName, property.stringValue, useArea, helpText);

				dialog.OnAccept += (
					newName => 
					{
						property.stringValue = newName;
						
						property.serializedObject.ApplyModifiedProperties();
						//UnityEditor.EditorUtility.SetDirty(property.serializedObject.targetObject);
						markToSave(); //To save 

					});

				dialog.Show();
			}
		}
    }

	public class InputDialog : EditorWindow
	{
		public event Action<string> OnAccept;

		private string _inputValue;
		private string _labelName;
		private string _helpMessage;
		private bool _useArea;

		private void OnCancel()
		{
			Close();
		}

		public void Init(string labelName, string inputValue, bool useArea, string helpMessage = null)
		{
			titleContent = new GUIContent("Edit " + labelName);

			_inputValue = inputValue;
			_labelName = labelName;
			_useArea = useArea;
			_helpMessage = helpMessage;

			position = new Rect(Screen.width / 2, Screen.height / 2, Screen.width / 2, Screen.height / 2);
		}

		private void OnAcceptImpl()
		{
			if(OnAccept != null)
			{
				OnAccept(_inputValue);
			}
			Close();
		}

		void OnGUI()
		{
			if(_useArea)
			{
				EditorGUILayout.LabelField(_labelName, GUILayout.Width(EditorGUIUtility.labelWidth - 5));
				
				EditorGUILayout.BeginVertical();
				EditorGUILayout.HelpBox(_helpMessage, MessageType.Info);
				_inputValue = EditorGUILayout.TextArea(_inputValue, EditorStyles.richTextArea);
				EditorGUILayout.EndVertical();
			}
			else
			{
				_inputValue = EditorGUILayout.TextField(_labelName, _inputValue);
			}
			
			GUILayout.Space(30);

			GUILayout.BeginHorizontal();

			if (GUILayout.Button("OK"))
			{
				OnAcceptImpl();
			}

			if (GUILayout.Button("Cancel" ))
			{
				OnCancel();
			}

			EditorGUILayout.EndHorizontal();
		}
	}
}
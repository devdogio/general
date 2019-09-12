using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;
using System;

namespace Devdog.General.Editors
{
    public static class ScriptableObjectUtility
    {

        public static T CreateAsset<T>(string path, string fileName) where T : ScriptableObject
        {
            return (T)CreateAsset(typeof (T), path, fileName, true);
        }

        public static T CreateAsset<T>(string path, string fileName, bool saveAndRefresh) where T : ScriptableObject
        {
            return (T)CreateAsset(typeof(T), path, fileName, saveAndRefresh);
        }

        public static ScriptableObject CreateAsset(Type type, string savePath, string fileName)
        {
            return CreateAsset(type, savePath, fileName, true);
        }

        public static ScriptableObject CreateAsset(Type type, string savePath, string fileName, bool saveAndRefresh)
        {
            if (savePath == string.Empty || Directory.Exists(savePath) == false)
            {
                Debug.LogWarning("The directory you're trying to save to doesn't exist! (" + savePath + ")");
                return null;
            }

            var asset = ScriptableObject.CreateInstance(type);
            if (fileName.EndsWith(".asset") == false)
            {
                fileName += ".asset";
            }

            if (savePath.Contains(Application.dataPath))
            {
                savePath = "Assets" + savePath.Replace(Application.dataPath, "");
            }

            AssetDatabase.CreateAsset(asset, savePath + "/" + fileName);
            AssetDatabase.SetLabels(asset, new string[] { type.Name });
            if (saveAndRefresh)
            {
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            return asset;
        }

        public static T CreateAssetChooseSaveFolder<T>(string defaultFileName, bool saveAndRefresh)
            where T : ScriptableObject
        {
            var path = UnityEditor.EditorUtility.SaveFilePanelInProject("Save file", defaultFileName, "asset", "Choose a location where to save.");
            if (path.EndsWith(".asset"))
            {
                int indexOf = path.LastIndexOf(Path.DirectorySeparatorChar);
                if (indexOf == -1)
                {
                    indexOf = path.LastIndexOf('/');
                }

                path = path.Substring(0, indexOf);
            }

            if (string.IsNullOrEmpty(path) == false)
            {
                return CreateAsset<T>(path, defaultFileName, saveAndRefresh);
            }

            return null;
        }
    }
}
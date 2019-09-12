using System;
using System.Collections.Generic;
using Devdog.General.ThirdParty.UniLinq;
using UnityEngine;

namespace Devdog.General.Localization
{
    [System.Serializable]
    [CreateAssetMenu(menuName = "Devdog/Localization Database")]
    public class LocalizationDatabase : BetterScriptableObject
    {
        [SerializeField]
        private string _lang = "en-US";

        public string lang
        {
            get { return _lang; }
            set { _lang = value; }
        }

        [SerializeField]
        private Dictionary<string, string> _localizedStrings = new Dictionary<string, string>()
        {
            {LocalizationManager.NoKeyConstant, string.Empty}
        };

        [SerializeField]
        private Dictionary<string, UnityEngine.Object> _localizedObjects = new Dictionary<string, UnityEngine.Object>();



#if UNITY_EDITOR

        public Dictionary<string, string> _EditorGetAllStrings()
        {
            return _localizedStrings;
        }

        public Dictionary<string, UnityEngine.Object> _EditorGetAllObjects()
        {
            return _localizedObjects;
        }

#endif


        public bool ContainsString(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return false;
            }

            return _localizedStrings.ContainsKey(key);
        }

        public string GetString(string key, string notFound = "")
        {
            if (string.IsNullOrEmpty(key))
            {
                return notFound;
            }

            string result;
            var got = _localizedStrings.TryGetValue(key, out result);
            if (got)
            {
                return result;
            }

            return notFound;
        }

        public void SetString(string key, string msg)
        {
            if (string.IsNullOrEmpty(key))
            {
                return;
            }

            if (string.IsNullOrEmpty(msg))
            {
                DeleteString(key);
                return;
            }

            _localizedStrings[key] = msg;

#if UNITY_EDITOR
//            if(UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode || UnityEditor.EditorApplication.isPlaying == false)
//            {
                UnityEditor.EditorUtility.SetDirty(this);
//            }
#endif
        }

        public void DeleteString(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return;
            }

            _localizedStrings.Remove(key);
        }

        public bool ContainsObject(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return false;
            }

            return _localizedObjects.ContainsKey(key);
        }

        public T GetObject<T>(string key, T notFound = null) where T : UnityEngine.Object
        {
            if (string.IsNullOrEmpty(key))
            {
                return notFound;
            }

            UnityEngine.Object result;
            var got = _localizedObjects.TryGetValue(key, out result);
            if (got)
            {
                return (T)result;
            }

            return notFound;
        }

        public void SetObject<T>(string key, T obj) where T : UnityEngine.Object
        {
            if (string.IsNullOrEmpty(key))
            {
                return;
            }

            if (obj == null)
            {
                DeleteObject(key);
                return;
            }

            _localizedObjects[key] = obj;

#if UNITY_EDITOR
//            if(UnityEditor.EditorApplication.isPlaying == false)
//            {
                UnityEditor.EditorUtility.SetDirty(this);
//            }
#endif
        }

        public void DeleteObject(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return;
            }

            _localizedObjects.Remove(key);
        }
    }
}

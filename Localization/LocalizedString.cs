using System;
using System.Collections.Generic;
using UnityEngine;

namespace Devdog.General.Localization
{
    [System.Serializable]
    public class LocalizedString
    {
        /// <summary>
        /// Gets the message in the currently selected language.
        /// </summary>
        [IgnoreCustomSerialization]
        public string message
        {
            get
            {
                return LocalizationManager.GetString(_key);
            }
            set
            {
                if (LocalizationManager.currentDatabase != null)
                {
                    LocalizationManager.currentDatabase.SetString(_key, value);
                }
            }
        }

        [SerializeField]
        private string _key = LocalizationManager.NoKeyConstant;

        public LocalizedString()
        {

        }
        
        public LocalizedString(string key)
        {
            _key = key;
        }

        public override string ToString()
        {
            return message;
        }
    }
}

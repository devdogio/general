using System;
using System.Collections.Generic;
using Devdog.General.ThirdParty.UniLinq;
using System.Text;
using Devdog.General.Localization;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Devdog.General.Localization
{
    public abstract class LocalizedObjectBase<T> : ILocalizedObject<T>
        where T : UnityEngine.Object
    {
        public Object objectVal
        {
            get { return val; }
            set { val = (T) value; }
        }

        /// <summary>
        /// Gets the message in the currently selected language.
        /// </summary>
        [IgnoreCustomSerialization]
        public T val
        {
            get
            {
                return LocalizationManager.GetObject<T>(_key);
            }
            set
            {
                if (LocalizationManager.currentDatabase != null)
                {
                    LocalizationManager.currentDatabase.SetObject<T>(_key, value);
                }
            }
        }

        [SerializeField]
        private string _key;

        protected LocalizedObjectBase()
        {
            
        }

        protected LocalizedObjectBase(string key)
        {
            _key = key;
        }

        public override string ToString()
        {
            if (objectVal == null)
            {
                return "null";
            }

            return objectVal.ToString();
        }
    }
}

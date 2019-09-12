using System;
using Devdog.General.ThirdParty.UniLinq;

namespace Devdog.General.UI
{
    public static class UIWindowUtility
    {
        public static UIWindow FindByName(string name)
        {
            return FindByName<UIWindow>(name);
        }

        public static T FindByName<T>(string name) where T : UIWindow
        {
            return UnityEngine.Object.FindObjectsOfType<T>().FirstOrDefault(o => o.windowName == name);
        }

        public static UIWindow[] GetAllWindowsWithInput()
        {
            return GetAllWindowsWithInput<UIWindow>();
        }

        public static T[] GetAllWindowsWithInput<T>() where T : UIWindow
        {
            return UnityEngine.Object.FindObjectsOfType<T>().Where(o => o.GetComponent<IUIWindowInputHandler>() != null).ToArray();
        }
    }
}

#if UNITY_5_0 || UNITY_5_1 || UNITY_5_2
#define UNITY_LOAD_DEPRECATED
#endif

using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_LOAD_DEPRECATED
#else
using UnityEngine.SceneManagement;
#endif

namespace Devdog.General
{
    public static class SceneUtility
    {
        public static AsyncOperation LoadSceneAsync(string name)
        {
            AsyncOperation async = null;
#if UNITY_LOAD_DEPRECATED
            async = Application.LoadLevelAsync(name);
#else
            async = SceneManager.LoadSceneAsync(name);
#endif

            return async;
        }

        public static AsyncOperation LoadSceneAsync(int id)
        {
            AsyncOperation async = null;
#if UNITY_LOAD_DEPRECATED
            async = Application.LoadLevelAsync(id);
#else
            async = SceneManager.LoadSceneAsync(id);
#endif

            return async;
        }


        public static void LoadScene(string name)
        {
#if UNITY_LOAD_DEPRECATED
            Application.LoadLevel(name);
#else
            SceneManager.LoadScene(name);
#endif
        }


        public static void LoadScene(int id)
        {
#if UNITY_LOAD_DEPRECATED
            Application.LoadLevel(id);
#else
            SceneManager.LoadScene(id);
#endif
        }
    }
}

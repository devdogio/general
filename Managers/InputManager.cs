using System;
using System.Collections.Generic;
using Devdog.General.ThirdParty.UniLinq;
using UnityEngine;

namespace Devdog.General
{
    public partial class InputManager : ManagerBase<InputManager>
    {
        public static event Action<bool> OnUIInputLimitChanged;
        public static event Action<bool> OnPlayerInputChanged;

        protected static List<GameObject> limitUIInputTo = new List<GameObject>(8);
        protected static List<GameObject> limitPlayerInputTo = new List<GameObject>(8);

        #region UI Input

        private static void NotifyUIInputChanged(bool b)
        {
            if (OnUIInputLimitChanged != null)
                OnUIInputLimitChanged(b);
        }

        public static void LimitUIInputTo(GameObject obj)
        {
            limitUIInputTo.Add(obj);
            NotifyUIInputChanged(true);
        }

        public static void RemoveUILimitInput(GameObject obj)
        {
            var last = limitUIInputTo.LastOrDefault();
            limitUIInputTo.Remove(obj);

            if (limitUIInputTo.Count == 0)
            {
                NotifyUIInputChanged(false);
            }
            else if (IsUIInputLimitedTo(last))
            {
                NotifyUIInputChanged(true);
            }
        }

        public static void ClearUIInputLimits(GameObject obj)
        {
            limitUIInputTo.Clear();
            NotifyUIInputChanged(false);
        }

        public static GameObject GetLimitedUIInputObject()
        {
            if (limitUIInputTo.Count > 0)
            {
                return limitUIInputTo[limitUIInputTo.Count - 1];
            }

            return null;
        }

        public static bool IsUIInputLimitedTo(GameObject obj)
        {
            return GetLimitedUIInputObject() == obj;
        }

        public static bool CanReceiveUIInput(GameObject obj)
        {
            return CanReceiveInput(obj, GetLimitedUIInputObject());
        }

        #endregion

        #region Player input

        private static void NotifyPlayerInputChanged(bool b)
        {
            if (OnPlayerInputChanged != null)
                OnPlayerInputChanged(b);
        }

        public static void LimitPlayerInputTo(GameObject obj)
        {
            limitPlayerInputTo.Add(obj);
            NotifyPlayerInputChanged(true);
        }

        public static void RemovePlayerLimitInput(GameObject obj)
        {
            var last = limitPlayerInputTo.LastOrDefault();
            limitPlayerInputTo.Remove(obj);

            if (limitPlayerInputTo.Count == 0)
            {
                NotifyPlayerInputChanged(false);
            }
            else if (IsPlayerInputLimitedTo(last))
            {
                NotifyPlayerInputChanged(true);
            }
        }

        public static void ClearPlayerInputLimits(GameObject obj)
        {
            limitPlayerInputTo.Clear();
            NotifyPlayerInputChanged(false);
        }

        public static GameObject GetLimitedPlayerInputObject()
        {
            if (limitPlayerInputTo.Count > 0)
            {
                return limitPlayerInputTo[limitPlayerInputTo.Count - 1];
            }

            return null;
        }

        public static bool IsPlayerInputLimitedTo(GameObject obj)
        {
            return GetLimitedPlayerInputObject() == obj;
        }

        public static bool CanReceivePlayerInput(GameObject obj)
        {
            return CanReceiveInput(obj, GetLimitedPlayerInputObject());
        }

        #endregion

        private static bool CanReceiveInput(GameObject obj, GameObject limitObject)
        {
            if (obj == null)
                return false;

            if (obj.activeInHierarchy == false)
                return false;

            // Input is not limited to single object.
            if (limitObject == null)
                return true;

            // Input is limited to object, object or all children are allowed to deliver input.
            if (obj == limitObject || obj.transform.IsChildOf(limitObject.transform))
                return true;

            // Obj was not a child or match, don't allow input.
            return false;
        }
    }
}

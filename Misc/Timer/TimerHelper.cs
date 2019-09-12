using System;
using System.Collections;
using System.Collections.Generic;
using Devdog.General.ThirdParty.UniLinq;
using System.Text;
using UnityEngine;

namespace Devdog.General
{
    public class TimerHelper : MonoBehaviour, ITimerHelper
    {
        [NonSerialized]
        private Dictionary<int, Coroutine> _lookups = new Dictionary<int, Coroutine>();

        [NonSerialized]
        private int _IDCounter = 0;

        public virtual void StopAllTimers()
        {
            StopAllCoroutines();
            _lookups.Clear();
        }

        public virtual void StopTimer(int id)
        {
            if (_lookups.ContainsKey(id) == false)
            {
//                QuestLogger.LogWarning("No timer with that ID exists or is already stopped.");
                return;
            }

            StopCoroutine(_lookups[id]);
            _lookups.Remove(id);

//            QuestLogger.LogVerbose("Auto removed timer with ID: " + id + " (timer completed)");
        }


        public int StartTimer(float time, Action callbackWhenTimeIsOver)
        {
            return StartTimer(time, null, callbackWhenTimeIsOver);
        }

        public virtual int StartTimer(float time, Action callbackContinous, Action callbackWhenTimeIsOver)
        {
            _IDCounter++;
            _lookups[_IDCounter] = StartCoroutine(_StartTimer(_IDCounter, time, callbackContinous, callbackWhenTimeIsOver));

            return _IDCounter;
        }

        protected virtual IEnumerator _StartTimer(int timerID, float time, Action callbackContinous, Action callbackWhenTimeIsOver)
        {
            float timer = 0f;
            while (timer < time)
            {
                timer += Time.deltaTime;
                if (callbackContinous != null)
                {
                    callbackContinous();
                }

                yield return null;
            }

            _lookups.Remove(timerID);
            if (callbackWhenTimeIsOver != null)
            {
                callbackWhenTimeIsOver();
            }
        }

        public void Dispose()
        {
            StopAllTimers();
            Destroy(gameObject);
        }
    }
}

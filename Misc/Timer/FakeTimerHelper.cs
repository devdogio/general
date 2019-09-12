using System;
using System.Collections;
using UnityEngine;

namespace Devdog.General
{
    public class FakeTimerHelper : ITimerHelper
    {
        public FakeTimerHelper()
        {
//            EditorApplication.update += Update;
        }

        public virtual void StopAllTimers()
        {

        }

        public virtual void StopTimer(int id)
        {
            
        }

        public virtual int StartTimer(float time, Action callback)
        {
            return StartTimer(time, null, callback);
        }

        public virtual int StartTimer(float time, Action callbackContinous, Action callback)
        {
            if (callbackContinous != null)
            {
                callbackContinous();
            }

            if (time <= 0f)
            {
                if (callback != null)
                {
                    callback();
                }
            }

            return 0;
        }

        public void Dispose()
        {
//            EditorApplication.update -= Update;
        }
    }
}
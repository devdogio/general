using System;
using System.Collections.Generic;
using Devdog.General.ThirdParty.UniLinq;
using System.Text;

namespace Devdog.General
{
    public interface ITimerHelper : IDisposable
    {
//        void StopAllTimers();
        void StopTimer(int id);
        int StartTimer(float time, Action callbackContinous, Action callbackWhenTimeIsOver);
        int StartTimer(float time, Action callbackWhenTimeIsOver);
    }
}

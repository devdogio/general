using System;
using System.Collections.Generic;
using Devdog.General.ThirdParty.UniLinq;
using System.Text;
using Devdog.General.UI;
using UnityEngine;

namespace Devdog.General
{
    public static class TriggerUtility
    {
        public static TriggerBase mouseOnTrigger { get; set; }
        public static bool isMouseOnTrigger
        {
            get { return mouseOnTrigger != null; }
        }
    }
}

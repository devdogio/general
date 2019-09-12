using System;
using System.Collections.Generic;
using Devdog.General.ThirdParty.UniLinq;
using System.Text;
using UnityEngine;

namespace Devdog.General
{
    [AttributeUsage(AttributeTargets.Class)]
    public class HideInCreationWindowAttribute : Attribute
    {
        public HideInCreationWindowAttribute()
        {
        }
    }
}

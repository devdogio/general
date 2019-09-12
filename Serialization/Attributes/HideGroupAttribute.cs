using System;
using System.Collections.Generic;
using Devdog.General.ThirdParty.UniLinq;
using System.Text;
using UnityEngine;

namespace Devdog.General
{
    [AttributeUsage(AttributeTargets.Field)]
    public class HideGroupAttribute : Attribute
    {
        public bool includeArrayChildren { get; protected set; }

        public HideGroupAttribute()
            : this(true)
        {
        }

        public HideGroupAttribute(bool includeArrayChildren)
        {
            this.includeArrayChildren = includeArrayChildren;
        }
    }
}

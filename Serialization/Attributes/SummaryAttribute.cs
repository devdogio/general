using System;
using System.Collections.Generic;
using Devdog.General.ThirdParty.UniLinq;
using System.Text;
using UnityEngine;

namespace Devdog.General
{
    /// <summary>
    /// When used this field will show in inside the node, as well as the properties sidebar.
    /// </summary>
    [AttributeUsage(AttributeTargets.All)]
    public class SummaryAttribute : Attribute
    {
        public string summary { get; private set; }

        public SummaryAttribute(string summary)
        {
            this.summary = summary;
        }
    }
}

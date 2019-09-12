using System;
using System.Collections.Generic;
using Devdog.General.ThirdParty.UniLinq;
using System.Text;
using UnityEngine;

namespace Devdog.General
{
    [AttributeUsage(AttributeTargets.All)]
    public class CategoryAttribute : Attribute
    {
        public string category { get; private set; }

        public CategoryAttribute(string category)
        {
            this.category = category;
        }

        public override string ToString()
        {
            return category;
        }
    }
}

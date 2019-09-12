using System;
using System.Collections.Generic;
using Devdog.General.ThirdParty.UniLinq;
using System.Text;

namespace Devdog.General.Localization
{
    public interface ILocalizedObject<T> : ILocalizedObject 
        where T: UnityEngine.Object
    {

        T val { get; set; }

    }

    public interface ILocalizedObject
    {
        UnityEngine.Object objectVal { get; set; }
    }
}

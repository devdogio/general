using System;
using UnityEditor;
using UnityEngine;

namespace Devdog.General.Localization.Editors
{
    [CustomPropertyDrawer(typeof(LocalizedTexture2D), true)]
    public class LocalizedTextureEditor : LocalizedObjectEditorBase<Texture2D, LocalizedTexture2D>
    {
        
    }
}

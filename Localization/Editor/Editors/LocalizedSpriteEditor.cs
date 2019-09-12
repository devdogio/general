using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Devdog.General.Localization.Editors
{
    [CustomPropertyDrawer(typeof(LocalizedSprite), true)]
    public class LocalizedSpriteEditor : LocalizedObjectEditorBase<Sprite, LocalizedSprite>
    {
        
    }
}

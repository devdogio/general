using System;
using UnityEngine;
using UnityEditor;

namespace Devdog.General.Editors
{
    public class ColorBlock : IDisposable
    {
        private readonly Color _before;
        private readonly bool _active;
        public ColorBlock(Color color)
            : this(color, true)
        {
            
        }

        public ColorBlock(Color color, bool active)
        {
            _before = GUI.color;
            _active = active;

            if (_active)
            {
                GUI.color = color;
            }
        }


        public void Dispose()
        {
            if (_active)
            {
                GUI.color = _before;
            }
        }
    }
}
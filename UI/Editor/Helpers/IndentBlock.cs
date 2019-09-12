using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Devdog.General.Editors
{
    public class IndentBlock : IDisposable
    {
        public Rect rect;
        private const int IndentationPixels = 15;

        public IndentBlock(Rect rect)
        {
            this.rect = rect;
            this.rect.x += IndentationPixels;
            this.rect.width -= IndentationPixels;
        }

        public void Dispose()
        {
            this.rect.x -= IndentationPixels;
            this.rect.width += IndentationPixels;
        }
    }
}
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Devdog.General.Editors
{
    public class GUIMatrixBlock : IDisposable
    {
        private readonly Matrix4x4 _oldMatrix;

        public GUIMatrixBlock(Matrix4x4 matrix)
        {
            _oldMatrix = GUI.matrix;
            GUI.matrix = matrix;
        }

        public void Dispose()
        {
            GUI.matrix = _oldMatrix;
        }
    }
}
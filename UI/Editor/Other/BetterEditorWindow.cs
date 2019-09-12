using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Devdog.General.Editors
{
    public class BetterEditorWindow : UnityEditor.EditorWindow
    {
        protected bool continuousRepaint = true;

        protected virtual void Update()
        {
            if (continuousRepaint)
            {
                Repaint();
            }
        }

        public virtual void OnGUI()
        {
            HandleEvents();
        }

        private void HandleEvents()
        {
            switch (Event.current.type)
            {
                case EventType.MouseDown:
                    OnMouseDown(Event.current.button);
                    break;
                case EventType.MouseUp:
                    OnMouseUp(Event.current.button);
                    break;
                case EventType.MouseDrag:
                    OnMouseDrag(Event.current.delta);
                    break;
                case EventType.KeyDown:
                    OnKeyDown(Event.current.keyCode);
                    break;
                case EventType.KeyUp:
                    OnKeyUp(Event.current.keyCode);
                    break;
                case EventType.ScrollWheel:
                    OnScroll(Event.current.delta);
                    break;
                case EventType.MouseMove:
                    OnMouseMove(Event.current.delta);
                    break;
                case EventType.Repaint:
                case EventType.Layout:
                case EventType.DragUpdated:
                case EventType.DragPerform:
                case EventType.DragExited:
                case EventType.Used:
                case EventType.ExecuteCommand:
                case EventType.ValidateCommand:
                case EventType.Ignore:
                    break;
                case EventType.ContextClick:
                    OnContextClick();
                    break;
                default:
                    break;
                // throw new ArgumentOutOfRangeException(Event.current.type.ToString());
            }
        }

        protected virtual void OnMouseMove(Vector2 delta)
        {
            
        }

        protected virtual void OnMouseDown(int button)
        {

        }

        protected virtual void OnMouseUp(int button)
        {

        }

        protected virtual void OnMouseDrag(Vector2 delta)
        {
//            Debug.Log("Mouse drag " + delta);
        }

        protected virtual void OnKeyDown(KeyCode keyCode)
        {

        }

        protected virtual void OnKeyUp(KeyCode keyCode)
        {

        }

        protected virtual void OnScroll(Vector2 delta)
        {

        }

        protected virtual void OnContextClick()
        {

        }
    }
}

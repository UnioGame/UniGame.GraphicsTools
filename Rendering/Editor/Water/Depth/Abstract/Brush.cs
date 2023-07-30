namespace UniGame.Rendering.Editor.Water.Depth.Abstract
{
    using Sirenix.Utilities.Editor;
    using UnityEditor;
    using UnityEngine;

    public abstract class Brush
    {
        public float Size     { get; private set; } = 1.0f;
        public float Strength { get; private set; } = 1.0f;
        
        public virtual void DrawHandle(Vector3 position)
        {
            var handleSize = HandleUtility.GetHandleSize(position);
            var normal     = Quaternion.identity * Vector3.forward;
            
            Handles.DrawWireDisc(position, normal, Size * handleSize);
            Handles.DrawWireDisc(position, normal, Size * Strength * handleSize);
        }

        public virtual void DrawInspector()
        {
            SirenixEditorGUI.BeginBox(new GUIContent("Brush Properties"));
            {
                DrawSizeInspector();
                DrawStrengthInspector();
            }
            SirenixEditorGUI.EndBox();
        }
        
        public abstract GUIContent GetContent();

        public abstract void Paint(Rect sourceRect, Rect paintRect, PaintMode paintMode, Color paintColor, Color[] sourceBuffer, ref Color[] brushBuffer, ref Color[] textureBuffer);

        protected void DrawSizeInspector()
        {
            Size = EditorGUILayout.Slider(new GUIContent("Size"), Size, 0.05f, 1.0f);
        }

        protected void DrawStrengthInspector()
        {
            Strength = EditorGUILayout.Slider(new GUIContent("Strength"), Strength, 0.0f, 1.0f);
        }
    }
}
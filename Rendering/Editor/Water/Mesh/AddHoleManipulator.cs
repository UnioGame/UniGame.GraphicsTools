namespace Taktika.Rendering.Editor.Water.Mesh
{
    using System.Collections.Generic;
    using Abstract;
    using Runtime.Water;
    using Sirenix.Utilities.Editor;
    using UnityEditor;
    using UnityEngine;

    public class AddHoleManipulator : MeshManipulator
    {
        private readonly Water _water;
        
        private float _currentHoleSize = 0.1f;

        public override bool IsEnabled { get; protected set; }
        public override bool CanChangeEnabledStatus => true;

        public AddHoleManipulator(Water water)
        {
            _water = water;
        }

        public override void DrawInspector()
        {
            SirenixEditorGUI.BeginBox(new GUIContent("Hole Editor"));
            {
                IsEnabled = GUILayout.Toggle(IsEnabled, new GUIContent("Enable Hole Drawing"), EditorStyles.miniButton);
                if (IsEnabled)
                {
                    _currentHoleSize = EditorGUILayout.Slider(new GUIContent("Hole size"), _currentHoleSize, 0.05f, 1.0f);
                }
            }
            SirenixEditorGUI.EndBox();
        }

        public override int Handle(WaterMesh waterMesh, int selectedVertex)
        {
            var mousePosition  = Event.current.mousePosition;
            var worldPosition  = (Vector2)HandleUtility.GUIPointToWorldRay(mousePosition).origin;
            var objectPosition = _water.WorldToObject(worldPosition);

            var handleSize = HandleUtility.GetHandleSize(worldPosition);
            if (Handles.Button(worldPosition, Quaternion.identity, _currentHoleSize * handleSize, _currentHoleSize * handleSize, Handles.CircleHandleCap))
            {
                if(!waterMesh.Mesh.IsPointInsideMesh(objectPosition))
                    return selectedVertex;
                
                var holeCorners  = new List<Vector3>();
                var cornerOffset = _currentHoleSize * handleSize;
                holeCorners.Add(new Vector3(objectPosition.x - cornerOffset, objectPosition.y + cornerOffset));
                holeCorners.Add(new Vector3(objectPosition.x + cornerOffset, objectPosition.y + cornerOffset));
                holeCorners.Add(new Vector3(objectPosition.x + cornerOffset, objectPosition.y - cornerOffset));
                holeCorners.Add(new Vector3(objectPosition.x - cornerOffset, objectPosition.y - cornerOffset));
                
                waterMesh.AddHole(holeCorners);
            }

            return selectedVertex;
        }
    }
}
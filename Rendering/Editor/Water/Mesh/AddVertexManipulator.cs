namespace Taktika.Rendering.Editor.Water.Mesh
{
    using Abstract;
    using Runtime.Water;
    using UnityEditor;
    using UnityEngine;

    public class AddVertexManipulator : MeshManipulator
    {
        private const float DistanceThreshold = 2.5f;
        private const float EdgeThreshold     = 0.1f;
        
        private readonly Water _water;
        
        public AddVertexManipulator(Water water)
        {
            _water = water;
        }
        
        public override void DrawInspector()
        {
            EditorGUILayout.HelpBox("How to add vertex: press and hold CTRL and click on some edge of water mesh.", MessageType.Info);
        }

        public override int Handle(WaterMesh waterMesh, int selectedVertex)
        {
            if (!Event.current.control)
                return selectedVertex;
            
            var mousePosition = Event.current.mousePosition;
            var worldPosition = (Vector2)HandleUtility.GUIPointToWorldRay(mousePosition).origin;
            
            if(!waterMesh.FindClosestPoint(_water.WorldToObject(worldPosition), DistanceThreshold, EdgeThreshold, out var point, out var pointIndex, out var isHole))
                return selectedVertex;

            var worldPoint = _water.ObjectToWorld(point);
            var handleSize = HandleUtility.GetHandleSize(worldPoint);
            if (Handles.Button(worldPoint, Quaternion.identity, 0.1f * handleSize, 0.11f * handleSize, Handles.DotHandleCap))
            {
                Undo.RecordObject(_water, "Add vertex");
                
                waterMesh.InsertVertex(pointIndex, point, isHole);
                selectedVertex = pointIndex;
            }

            return selectedVertex;
        }
    }
}
namespace Taktika.Rendering.Editor.Water.Mesh
{
    using Abstract;
    using Runtime.Water;
    using UnityEditor;
    using UnityEngine;

    public class MoveVertexManipulator : MeshManipulator
    {
        private readonly Water _water;

        public MoveVertexManipulator(Water water)
        {
            _water = water;
        }
        
        public override int Handle(WaterMesh waterMesh, int selectedVertex)
        {
            if(selectedVertex < 0 || selectedVertex >= waterMesh.VertexCount || Event.current.control)
                return selectedVertex;

            var vertexPosition = waterMesh.Vertices[selectedVertex];
            var vertex         = _water.ObjectToWorld(vertexPosition);
                
            EditorGUI.BeginChangeCheck();
            vertex = Handles.DoPositionHandle(vertex, Quaternion.identity);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(_water, "Move vertex");
                waterMesh.SetVertex(selectedVertex, _water.WorldToObject(vertex));
                
                _water.ResizeDepthMask();
            }
            
            return selectedVertex;
        }
    }
}
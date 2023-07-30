namespace Taktika.Rendering.Editor.Water.Mesh
{
    using Abstract;
    using Runtime.Water;
    using UnityEditor;
    using UnityEngine;

    public class RemoveVertexManipulator : MeshManipulator
    {
        private const int VertexCountThreshold = 3;
        
        private readonly Water _water;
        
        public RemoveVertexManipulator(Water water)
        {
            _water = water;
        }

        public override void DrawInspector()
        {
            EditorGUILayout.HelpBox("How to remove vertex: select a vertex and press DELETE.", MessageType.Info);
        }

        public override int Handle(WaterMesh waterMesh, int selectedVertex)
        {
            if(selectedVertex < 0 || selectedVertex >= waterMesh.VertexCount || waterMesh.VertexCount == VertexCountThreshold)
                return selectedVertex;
            
            if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Delete)
            {
                Event.current.Use();
                Undo.RecordObject(_water, "Remove vertex");
                
                waterMesh.RemoveVertex(selectedVertex);
                if (selectedVertex >= waterMesh.VertexCount)
                {
                    selectedVertex = 0;
                }
                
                _water.ResizeDepthMask();
            }

            return selectedVertex;
        }
    }
}
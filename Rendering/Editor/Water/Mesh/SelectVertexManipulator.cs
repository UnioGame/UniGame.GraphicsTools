namespace Taktika.Rendering.Editor.Water.Mesh
{
    using Abstract;
    using Runtime.Water;
    using UnityEditor;
    using UnityEngine;

    public class SelectVertexManipulator : MeshManipulator
    {
        private readonly Water _water;

        public SelectVertexManipulator(Water water)
        {
            _water = water;
        }

        public override int Handle(WaterMesh waterMesh, int selectedVertex)
        {
            for (var i = 0; i < waterMesh.VertexCount; i++)
            {
                var vertex     = _water.ObjectToWorld(waterMesh.Vertices[i]);
                var handleSize = HandleUtility.GetHandleSize(vertex);
                
                if (Handles.Button(vertex, Quaternion.identity, 0.05f * handleSize, 0.06f * handleSize, Handles.DotHandleCap))
                {
                    selectedVertex = i;
                }
            }

            return selectedVertex;
        }
    }
}
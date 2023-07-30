namespace Taktika.Rendering.Runtime.Water
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Triangulator;
    using UnityEngine;

    [Serializable]
    public class WaterMesh
    {
        private const string DefaultMeshName = "Water";

        [SerializeField]
        private Mesh _mesh;

        [SerializeField]
        private List<Vector3> _vertices = new List<Vector3>();
        [SerializeField]
        private List<Hole> _holes = new List<Hole>();

        public Mesh Mesh
        {
            get => _mesh;
            set => _mesh = value;
        }

        public List<Vector3> Vertices
        {
            get => _vertices;
            set
            {
                _vertices = value;
                RecalculateMesh();
            }
        }

        public List<Hole> Holes => _holes;

        public int VertexCount => _vertices.Count;

        public bool IsNull  => _mesh == null;

        public WaterMesh()
        {
        }

        public WaterMesh(Mesh mesh)
        {
            _mesh     = mesh;
            _vertices = mesh.vertices.ToList();
        }

        public static WaterMesh GetDefault()
        {
            return WaterMeshUtility.CreateDefaultMesh();
        }

        public bool SetVertex(int index, Vector3 vertex)
        {
            if (index < 0 || index >= _vertices.Count)
                return false;

            _vertices[index] = vertex;
            RecalculateMesh();

            return true;
        }

        public void InsertVertex(int index, Vector3 vertex, bool isHole)
        {
            _vertices.Insert(index, vertex);

            if(isHole)
            {
                InsertToHole(index-1);
            }
            else
            {
                ShiftHoleVertices(1);
            }

            RecalculateMesh();
        }

        public void RemoveVertex(int index)
        {
            _vertices.RemoveAt(index);

            if (!RemoveFromHole(index))
            {
                ShiftHoleVertices(-1);
            }

            RecalculateMesh();
        }

        public void AddHole(IEnumerable<Vector3> holePoints)
        {
            var holePointIndex = _vertices.Count;
            var hole = new Hole();
            
            foreach (var holePoint in holePoints)
            {
                _vertices.Add(holePoint);

                hole.Indexes.Add(holePointIndex);

                holePointIndex++;
            }
            
            _holes.Add(hole);
            
            RecalculateMesh();
        }
        
        public bool IsHoleVertex(int vertexIndex)
        {
            foreach (var hole in _holes)
            {
                if (hole.Indexes.Contains(vertexIndex))
                    return true;
            }

            return false;
        }
        
        public Hole FindHole(int vertexIndex)
        {
            foreach (var hole in _holes)
            {
                if (hole.Indexes.Contains(vertexIndex))
                    return hole;
            }

            return null;
        }

        private bool RemoveFromHole(int vertexIndex)
        {
            var hole = FindHole(vertexIndex);
            if(hole == null)
                return false;

            var indexOfVertex = hole.Indexes.IndexOf(vertexIndex);
            var holeIndex     = _holes.IndexOf(hole);
            var indexOffset   = 1;
            
            hole.Indexes.RemoveAt(indexOfVertex);
            
            if (hole.Indexes.Count < 3)
            {
                indexOffset += hole.Indexes.Count;

                ShiftHoleVertices(hole, vertexIndex, -1);
                
                var holeIndexOffset = 0;
                foreach (var holeVertexIndex in hole.Indexes)
                {
                    _vertices.RemoveAt(holeVertexIndex - holeIndexOffset);

                    holeIndexOffset++;
                }
                _holes.Remove(hole);
            }
            else
            {
                for (var i = indexOfVertex; i < hole.Indexes.Count; i++)
                {
                    hole.Indexes[i] -= indexOffset;
                }

                holeIndex++;
            }

            for (var i = holeIndex; i < _holes.Count; i++)
            {
                var nextHole = _holes[i];
                for (var v = 0; v < nextHole.Indexes.Count; v++)
                {
                    nextHole.Indexes[v] -= indexOffset;
                }
            }

            return true;
        }

        private void ShiftHoleVertices(Hole hole, int vertexIndex, int shiftValue)
        {
            for (var i = 0; i < hole.Indexes.Count; i++)
            {
                if (hole.Indexes[i] > vertexIndex)
                {
                    hole.Indexes[i] += shiftValue;
                }
            }
        }

        private void ShiftHoleVertices(int shiftValue)
        {
            foreach (var hole in _holes)
            {
                for (var i = 0; i < hole.Indexes.Count; i++)
                {
                    hole.Indexes[i] += shiftValue;
                }
            }
        }

        private void InsertToHole(int vertexIndex)
        {
            var hole = FindHole(vertexIndex);
            if(hole == null)
                return;

            var indexOfVertex = hole.Indexes.IndexOf(vertexIndex);
            hole.Indexes.Insert(indexOfVertex, vertexIndex);
            for (var i = indexOfVertex + 1; i < hole.Indexes.Count; i++)
            {
                hole.Indexes[i]++;
            }

            var holeIndex = _holes.IndexOf(hole);
            for (var i = holeIndex + 1; i < _holes.Count; i++)
            {
                var nextHole = _holes[i];
                for (var v = 0; v < nextHole.Indexes.Count; v++)
                {
                    nextHole.Indexes[v]++;
                }
            }
        }

        private void RecalculateMesh()
        {
            InitializeMesh();

            var verticesWithoutHoles = _vertices.Select(x => new Vector2(x.x, x.y)).ToList();
            var holes                = new Vector2[_holes.Count][];

            var offset = 0;
            for (var i = 0; i < _holes.Count; i++)
            {
                var hole = _holes[i];
                var holeVertices = new List<Vector2>();
                foreach (var holeVertexIndex in hole.Indexes)
                {
                    var vertex = _vertices[holeVertexIndex];
                    holeVertices.Add(new Vector2(vertex.x, vertex.y));
                    
                    verticesWithoutHoles.RemoveAt(holeVertexIndex - offset);
                    offset++;
                }

                holes[i] = holeVertices.ToArray();
            }

            var shape   = new Shape(verticesWithoutHoles.ToArray(), holes);
            var indices = EarClippingTriangulator.Triangulate(shape);
            
            _mesh.Clear();

            _mesh.SetVertices(_vertices);
            _mesh.SetTriangles(indices.ToArray(), 0);
            
            _mesh.RecalculateBounds();
            _mesh.RecalculateNormals();
            _mesh.RecalculateTangents();
            _mesh.RecalculateUVs(0);
            _mesh.RecalculateUVs(1);
            _mesh.RecalculateUVs(2);
            _mesh.RecalculateUVs(3);
        }

        private void InitializeMesh()
        {
            if(_mesh == null)
                _mesh = new Mesh {name = DefaultMeshName};
        }
    }
}
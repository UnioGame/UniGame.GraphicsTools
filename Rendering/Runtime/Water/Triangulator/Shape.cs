namespace Taktika.Rendering.Runtime.Water.Triangulator
{
    using UnityEngine;

    public class Shape
    {
        private readonly Vector2[] _points;

        private readonly int _vertexCount;

        private readonly int[] _pointPerHoleCount;
        private readonly int   _holeCount;

        private readonly int[] _holeStartIndices;

        public Vector2[] Points => _points;

        public int PointCount  => _points.Length;
        public int VertexCount => _vertexCount;

        public int[] PointPerHoleCount => _pointPerHoleCount;

        public int HoleCount => _holeCount;

        public Shape(Vector2[] vertices) : this(vertices, new Vector2[0][])
        {
        }
        
        public Shape(Vector2[] vertices, Vector2[][] holes)
        {
            _vertexCount = vertices.Length;
            _holeCount = holes.GetLength(0);

            _pointPerHoleCount = new int[_holeCount];
            _holeStartIndices = new int[_holeCount];
            
            var numHolePointsSum = 0;
            for (var i = 0; i < holes.GetLength(0); i++)
            {
                _pointPerHoleCount[i] = holes[i].Length;

                _holeStartIndices[i] = _vertexCount + numHolePointsSum;
                numHolePointsSum += _pointPerHoleCount[i];
            }

            var numPoints = _vertexCount + numHolePointsSum;
            _points = new Vector2[numPoints];


            // add hull points, ensuring they wind in counterclockwise order
            var reverseHullPointsOrder = !PointsAreCounterClockwise(vertices);
            for (var i = 0; i < _vertexCount; i++)
            {
                _points[i] = vertices[reverseHullPointsOrder ? _vertexCount - 1 - i : i];
            }

            // add hole points, ensuring they wind in clockwise order
            for (var i = 0; i < _holeCount; i++)
            {
                var reverseHolePointsOrder = PointsAreCounterClockwise(holes[i]);
                for (var j = 0; j < holes[i].Length; j++)
                {
                    _points[IndexOfPointInHole(j, i)] = holes[i][reverseHolePointsOrder ? holes[i].Length - j - 1 : j];
                }
            }
        }

        public int IndexOfPointInHole(int index, int holeIndex)
        {
            return _holeStartIndices[holeIndex] + index;
        }

        public Vector2 GetHolePoint(int index, int holeIndex)
        {
            return _points[_holeStartIndices[holeIndex] + index];
        }

        private static bool PointsAreCounterClockwise(Vector2[] testPoints)
        {
            float signedArea = 0;
            for (var i = 0; i < testPoints.Length; i++)
            {
                var nextIndex = (i + 1) % testPoints.Length;
                signedArea += (testPoints[nextIndex].x - testPoints[i].x) * (testPoints[nextIndex].y + testPoints[i].y);
            }

            return signedArea < 0;
        }
    }
}
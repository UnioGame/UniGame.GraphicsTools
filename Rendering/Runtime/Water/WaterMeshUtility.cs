namespace Taktika.Rendering.Runtime.Water
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public static class WaterMeshUtility
    {
        private const float DefaultWidth  = 1.0f;
        private const float DefaultHeight = 1.0f;
        
        public static WaterMesh CreateDefaultMesh()
        {
            var mesh = new WaterMesh();
            
            var vertices = new[]
            {
                new Vector3(0.0f, 0.0f, 0.0f), 
                new Vector3(DefaultWidth, 0.0f, 0.0f),
                new Vector3(DefaultHeight, DefaultHeight, 0.0f),
                new Vector3(0.0f, DefaultWidth, 0.0f) 
            };

            mesh.Vertices = vertices.ToList();

            return mesh;
        }

        public static List<Vector3> GetHullVertices(this WaterMesh mesh)
        {
            var verticesWithoutHoles = mesh.Vertices.ToList();

            var offset = 0;
            foreach (var hole in mesh.Holes)
            {
                foreach (var holeVertexIndex in hole.Indexes)
                {
                    verticesWithoutHoles.RemoveAt(holeVertexIndex - offset);
                    offset++;
                }
            }

            return verticesWithoutHoles;
        }

        public static List<List<Vector3>> GetHoleVertices(this WaterMesh mesh)
        {
            var holes = new List<List<Vector3>>();

            foreach (var hole in mesh.Holes)
            {
                var holeVertexList = new List<Vector3>();
                foreach (var holeVertexIndex in hole.Indexes)
                {
                    holeVertexList.Add(mesh.Vertices[holeVertexIndex]);
                }
                
                holes.Add(holeVertexList);
            }

            return holes;
        }
        
        public static float ReMap(float value, float min, float max)
        {
            var normal = Mathf.InverseLerp(min, max, value);
            return Mathf.Lerp(0, 1, normal);
        }

        public static void RecalculateUVs(this Mesh mesh, int channel)
        {
            var bounds   = mesh.bounds;
            var vertices = mesh.vertices;
            var uvs = new Vector2[vertices.Length];

            for (var i = 0; i < uvs.Length; i++)
            {
                var x = WaterMeshUtility.ReMap(vertices[i].x,  bounds.min.x,  bounds.max.x);
                var y = WaterMeshUtility.ReMap(vertices[i].y,  bounds.min.y,  bounds.max.y);
                
                uvs[i] = new Vector2(x, y);
            }
            
            mesh.SetUVs(channel, uvs);
        }

        public static bool IsPointInsideMesh(this Mesh mesh, Vector3 objectPoint)
        {
            for (var i = 0; i < mesh.triangles.Length; i += 3)
            {
                var triangleA = mesh.triangles[i];
                var triangleB = mesh.triangles[i + 1];
                var triangleC = mesh.triangles[i + 2];

                var pointA = mesh.vertices[triangleA];
                var pointB = mesh.vertices[triangleB];
                var pointC = mesh.vertices[triangleC];

                if (IsPointInsideTriangle(objectPoint, pointA, pointB, pointC))
                {
                    return true;
                }
            }

            return false;
        }
        
        public static bool IsPointInsideMesh(this Water water, Vector3 objectPoint)
        {
            if (water.WaterMesh.IsNull)
                return false;
            
            var mesh = water.WaterMesh.Mesh;
            for (var i = 0; i < mesh.triangles.Length; i += 3)
            {
                var triangleA = mesh.triangles[i];
                var triangleB = mesh.triangles[i + 1];
                var triangleC = mesh.triangles[i + 2];

                var pointA = mesh.vertices[triangleA];
                var pointB = mesh.vertices[triangleB];
                var pointC = mesh.vertices[triangleC];

                if (IsPointInsideTriangle(objectPoint, pointA, pointB, pointC))
                {
                    return true;
                }
            }

            return false;
        }

        public static Vector3 ObjectToWorld(this Water water, Vector3 position)
        {
            return water.transform.TransformPoint(position);
        }

        public static Vector3 WorldToObject(this Water water, Vector3 position)
        {
            return water.transform.InverseTransformPoint(position);
        }

        public static bool FindClosestPoint(this WaterMesh mesh, Vector3 objectPoint, float distanceThreshold, float edgeThreshold, out Vector3 point, out int pointIndex, out bool isHole)
        {
            objectPoint = new Vector3(objectPoint.x, objectPoint.y, 0.0f);

            var minDistance = float.MaxValue;
            var seekPoint   = Vector3.zero;
            var seekIndex   = -1;
            var seekIsHole  = false;

            for (var i = 0; i < mesh.VertexCount; i++)
            {
                var index     = i;
                var nextIndex = index + 1;
                if (nextIndex >= mesh.VertexCount)
                    nextIndex = 0;
                
                var pointAHole = mesh.FindHole(index);
                var pointBHole = mesh.FindHole(nextIndex);

                var pointOnEdgeOfHole = false;

                if (pointAHole == null && pointBHole != null)
                {
                    nextIndex = 0;
                }

                if (pointAHole != null && pointBHole == null || pointAHole != null && pointAHole != pointBHole)
                {
                    pointOnEdgeOfHole = true;
                    
                    var hole = mesh.FindHole(index);
                    if (hole != null)
                    {
                        nextIndex = hole.Indexes[0];
                    }
                }

                if (pointAHole != null && pointBHole != null && pointAHole == pointBHole)
                {
                    pointOnEdgeOfHole = true;
                }

                var pointA = mesh.Vertices[index];
                var pointB = mesh.Vertices[nextIndex];

                var abVector = pointB - pointA;
                var apVector = objectPoint - pointA;

                var dotProduct = Vector3.Dot(apVector, abVector) / Vector3.Dot(abVector, abVector);
                if(dotProduct > edgeThreshold && dotProduct < 1.0f - edgeThreshold)
                {
                    var pointOnEdge = pointA + dotProduct * abVector;
                    var distance      = (pointOnEdge - objectPoint).magnitude;
                    if (distance <= distanceThreshold && distance < minDistance)
                    {
                        minDistance = distance;
                        
                        seekPoint  = pointOnEdge;
                        seekIndex  = index + 1;
                        seekIsHole = pointOnEdgeOfHole;
                    }
                }
            }

            point      = seekPoint;
            pointIndex = seekIndex;
            isHole     = seekIsHole;
            
            return seekIndex != -1;
        }
        
        public static ResizeDirection GetResizeDirection(Bounds previousBounds, Bounds bounds)
        {
            const float epsilon = 0.0001f;
            
            var deltaMinX = Mathf.Abs(bounds.min.x - previousBounds.min.x);
            var deltaMaxX = Mathf.Abs(bounds.max.x - previousBounds.max.x);
            var deltaMinY = Mathf.Abs(bounds.min.y - previousBounds.min.y);
            var deltaMaxY = Mathf.Abs(bounds.max.y - previousBounds.max.y);

            var direction = ResizeDirection.None;

            if (deltaMaxX > epsilon)
                direction |= ResizeDirection.Right;
            if (deltaMinX > epsilon)
                direction |= ResizeDirection.Left;
            if (deltaMaxY > epsilon)
                direction |= ResizeDirection.Top;
            if (deltaMinY > epsilon)
                direction |= ResizeDirection.Bottom;
            
            return direction;
        }

        public static Color32[] ResizeCanvas(Color32[] pixels, int oldWidth, int oldHeight, int width, int height, Color32 defaultColor, ResizeDirection resizeDirection)
        {
            var newPixels = new Color32[width * height];

            var widthDelta  = (resizeDirection & ResizeDirection.Left) == ResizeDirection.Left ? width - oldWidth : 0;
            var heightDelta = (resizeDirection & ResizeDirection.Bottom) == ResizeDirection.Bottom ? height - oldHeight : 0;
            
            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    var shouldCopyPixel = true;
                    
                    var pixelIndex = y * width + x;
                    if (CheckRightDirection(x, oldWidth, resizeDirection) 
                        || CheckTopDirection(y, oldHeight, resizeDirection)
                        || CheckLeftDirection(x, width, oldWidth, resizeDirection) 
                        || CheckBottomDirection(y, height, oldHeight, resizeDirection))
                    {
                        newPixels[pixelIndex] = defaultColor;
                        shouldCopyPixel       = false;
                    }

                    if(!shouldCopyPixel)
                        continue;
                    
                    var oldPixelIndex = (y - heightDelta) * oldWidth + (x - widthDelta);
                    newPixels[pixelIndex] = pixels[oldPixelIndex];
                }
            }

            return newPixels;
        }

        private static bool CheckRightDirection(int x, int oldWidth, ResizeDirection direction)
        {
            return (direction & ResizeDirection.Right) == ResizeDirection.Right && x >= oldWidth;
        }

        private static bool CheckTopDirection(int y, int oldHeight, ResizeDirection direction)
        {
            return (direction & ResizeDirection.Top) == ResizeDirection.Top && y >= oldHeight;
        }

        private static bool CheckLeftDirection(int x, int width, int oldWidth, ResizeDirection direction)
        {
            return (direction & ResizeDirection.Left) == ResizeDirection.Left && x <= width - oldWidth;
        }

        private static bool CheckBottomDirection(int y, int height, int oldHeight, ResizeDirection direction)
        {
            return (direction & ResizeDirection.Bottom) == ResizeDirection.Bottom && y <= height - oldHeight;
        }

        private static bool IsPointInsideTriangle(Vector3 objectPoint, Vector3 a, Vector3 b, Vector3 c)
        {
            var d1 = Sign(objectPoint, a, b);
            var d2 = Sign(objectPoint, b, c);
            var d3 = Sign(objectPoint, c, a);

            var negative = d1 < 0 || d2 < 0 || d3 < 3;
            var positive = d1 > 0 || d2 > 0 || d3 > 0;

            return !(negative && positive);
        }

        private static float Sign(Vector3 a, Vector3 b, Vector3 c)
        {
            return (a.x - c.x) * (b.y - c.y) - (b.x - c.x) * (a.y - c.y);
        }
    }

    [Flags]
    public enum ResizeDirection
    {
        None = 0x0,
        Left = 0x1,
        Right = 0x2,
        Bottom = 0x4,
        Top = 0x8
    }
}
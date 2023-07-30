namespace Taktika.Rendering.Runtime.Water.Triangulator
{
    using System.Collections.Generic;
    using UnityEngine;

    public static class EarClippingTriangulator
    {
        public static int[] Triangulate(Shape shape)
        {
            var triangleIndex                = 0;
            var numHoleToHullConnectionVerts = 2 * shape.HoleCount; // 2 verts are added when connecting a hole to the hull.
            var totalNumVerts                = shape.PointCount + numHoleToHullConnectionVerts;
            
            var triangles                = new int[(totalNumVerts - 2) * 3];
            var verticesInClippedPolygon = GenerateVertexList(shape);
            
            while (verticesInClippedPolygon.Count >= 3)
            {
                var hasRemovedEarThisIteration = false;
                var vertexNode = verticesInClippedPolygon.First;
                for (var i = 0; i < verticesInClippedPolygon.Count; i++)
                {
                    var prevVertexNode = vertexNode.Previous ?? verticesInClippedPolygon.Last;
                    var nextVertexNode = vertexNode.Next ?? verticesInClippedPolygon.First;

                    if (vertexNode.Value.IsConvex)
                    {
                        if (!TriangleContainsVertex(prevVertexNode.Value, vertexNode.Value, nextVertexNode.Value, verticesInClippedPolygon))
                        {
                            // check if removal of ear makes prev/next vertex convex (if was previously reflex)
                            if (!prevVertexNode.Value.IsConvex)
                            {
                                var prevOfPrev = prevVertexNode.Previous ?? verticesInClippedPolygon.Last;

                                prevVertexNode.Value.IsConvex = IsConvex(prevOfPrev.Value.Position, prevVertexNode.Value.Position, nextVertexNode.Value.Position);
                            }
                            if (!nextVertexNode.Value.IsConvex)
                            {
                                var nextOfNext = nextVertexNode.Next ?? verticesInClippedPolygon.First;
                                nextVertexNode.Value.IsConvex = IsConvex(prevVertexNode.Value.Position, nextVertexNode.Value.Position, nextOfNext.Value.Position);
                            }

                            // add triangle to tri array
                            triangles[triangleIndex * 3 + 2] = prevVertexNode.Value.Index;
                            triangles[triangleIndex * 3 + 1] = vertexNode.Value.Index;
                            triangles[triangleIndex * 3]     = nextVertexNode.Value.Index;
                            triangleIndex++;

                            hasRemovedEarThisIteration = true;
                            verticesInClippedPolygon.Remove(vertexNode);
                            break;
                        }
                    }
                    
                    vertexNode = nextVertexNode;
                }

                if (!hasRemovedEarThisIteration)
                {
                    Debug.LogError("Error triangulating mesh. Aborted.");
                    return null;
                }
            }
            
            return triangles;
        }

        // Creates a linked list of all vertices in the polygon, with the hole vertices joined to the hull at optimal points.
        private static LinkedList<Vertex> GenerateVertexList(Shape shape)
        {
            var vertexList = new LinkedList<Vertex>();
            LinkedListNode<Vertex> currentNode = null;

            // Add all hull points to the linked list
            for (var i = 0; i < shape.VertexCount; i++)
            {
                var prevPointIndex = (i - 1 + shape.VertexCount) % shape.VertexCount;
                var nextPointIndex = (i + 1) % shape.VertexCount;

                var vertexIsConvex    = IsConvex(shape.Points[prevPointIndex], shape.Points[i], shape.Points[nextPointIndex]);
                var currentHullVertex = new Vertex(shape.Points[i], i, vertexIsConvex);

                currentNode = currentNode == null ? vertexList.AddFirst(currentHullVertex) : vertexList.AddAfter(currentNode, currentHullVertex);
            }

            // Process holes:
            var sortedHoleData = new List<HoleData>();
            for (var holeIndex = 0; holeIndex < shape.HoleCount; holeIndex++)
            {
                // Find index of rightmost point in hole. This 'bridge' point is where the hole will be connected to the hull.
                var holeBridgePoint = new Vector2(float.MinValue, 0);
                var holeBridgeIndex = 0;
                for (var i = 0; i < shape.PointPerHoleCount[holeIndex]; i++)
                {
                    if (shape.GetHolePoint(i, holeIndex).x <= holeBridgePoint.x)
                        continue;
                    
                    holeBridgePoint = shape.GetHolePoint(i, holeIndex);
                    holeBridgeIndex = i;
                }
                sortedHoleData.Add(new HoleData(holeIndex, holeBridgeIndex, holeBridgePoint));
            }
            // Sort hole data so that holes furthest to the right are first
            sortedHoleData.Sort((x, y) => x.BridgePoint.x > y.BridgePoint.x ? -1 : 1);

            foreach (var holeData in sortedHoleData)
            {
                // Find first edge which intersects with rightwards ray originating at the hole bridge point.
                var rayIntersectPoint = new Vector2(float.MaxValue, holeData.BridgePoint.y);
                var hullNodesPotentiallyInBridgeTriangle = new List<LinkedListNode<Vertex>>();
                
                LinkedListNode<Vertex> initialBridgeNodeOnHull = null;
                currentNode = vertexList.First;
                while (currentNode != null)
                {
                    var nextNode = currentNode.Next ?? vertexList.First;

                    var p0 = currentNode.Value.Position;
                    var p1 = nextNode.Value.Position;

                    // at least one point must be to right of holeData.bridgePoint for intersection with ray to be possible
                    if (p0.x > holeData.BridgePoint.x || p1.x > holeData.BridgePoint.x)
                    {
                        // one point is above, one point is below
                        if (p0.y > holeData.BridgePoint.y != p1.y > holeData.BridgePoint.y)
                        {
                            var rayIntersectX = p1.x; // only true if line p0,p1 is vertical
                            if (!Mathf.Approximately(p0.x, p1.x))
                            {
                                var intersectY = holeData.BridgePoint.y;
                                var gradient = (p0.y - p1.y) / (p0.x - p1.x);
                                var c = p1.y - gradient * p1.x;
                                rayIntersectX = (intersectY - c) / gradient;
                            }

                            // intersection must be to right of bridge point
                            if (rayIntersectX > holeData.BridgePoint.x)
                            {
                                var potentialNewBridgeNode = p0.x > p1.x ? currentNode : nextNode;
                                // if two intersections occur at same x position this means is duplicate edge
                                // duplicate edges occur where a hole has been joined to the outer polygon
                                var isDuplicateEdge = Mathf.Approximately(rayIntersectX, rayIntersectPoint.x);

								// connect to duplicate edge (the one that leads away from the other, already connected hole, and back to the original hull) if the
								// current hole's bridge point is higher up than the bridge point of the other hole (so that the new bridge connection doesn't intersect).
								var connectToThisDuplicateEdge = holeData.BridgePoint.y > potentialNewBridgeNode.Previous.Value.Position.y;
  
                                if (!isDuplicateEdge || connectToThisDuplicateEdge)
                                {
                                    // if this is the closest ray intersection thus far, set bridge hull node to point in line having greater x pos (since def to right of hole).
                                    if (rayIntersectX < rayIntersectPoint.x || isDuplicateEdge)
                                    {
                                        rayIntersectPoint.x = rayIntersectX;
                                        initialBridgeNodeOnHull = potentialNewBridgeNode;
                                    }
                                }
                            }
                        }
                    }

                    // Determine if current node might lie inside the triangle formed by holeBridgePoint, rayIntersection, and bridgeNodeOnHull
                    // We only need consider those which are reflex, since only these will be candidates for visibility from holeBridgePoint.
                    // A list of these nodes is kept so that in next step it is not necessary to iterate over all nodes again.
                    if (currentNode != initialBridgeNodeOnHull)
                    {
                        if (!currentNode.Value.IsConvex && p0.x > holeData.BridgePoint.x)
                        {
                            hullNodesPotentiallyInBridgeTriangle.Add(currentNode);
                        }
                    }
                    currentNode = currentNode.Next;
                }

                // Check triangle formed by hullBridgePoint, rayIntersection, and bridgeNodeOnHull.
                // If this triangle contains any points, those points compete to become new bridgeNodeOnHull
                var validBridgeNodeOnHull = initialBridgeNodeOnHull;
                foreach (var nodePotentiallyInTriangle in hullNodesPotentiallyInBridgeTriangle)
                {
                    if (nodePotentiallyInTriangle.Value.Index == initialBridgeNodeOnHull.Value.Index)
                    {
                        continue;
                    }
                    // if there is a point inside triangle, this invalidates the current bridge node on hull.
                    if (MathHelper.PointInTriangle(holeData.BridgePoint, rayIntersectPoint, initialBridgeNodeOnHull.Value.Position, nodePotentiallyInTriangle.Value.Position))
                    {
                        // Duplicate points occur at hole and hull bridge points.
                        var isDuplicatePoint = validBridgeNodeOnHull.Value.Position == nodePotentiallyInTriangle.Value.Position;

                        // if multiple nodes inside triangle, we want to choose the one with smallest angle from holeBridgeNode.
                        // if is a duplicate point, then use the one occurring later in the list
                        var currentDstFromHoleBridgeY = Mathf.Abs(holeData.BridgePoint.y - validBridgeNodeOnHull.Value.Position.y);
                        var pointInTriDstFromHoleBridgeY = Mathf.Abs(holeData.BridgePoint.y - nodePotentiallyInTriangle.Value.Position.y);

                        if (pointInTriDstFromHoleBridgeY < currentDstFromHoleBridgeY || isDuplicatePoint)
                        {
                            validBridgeNodeOnHull = nodePotentiallyInTriangle;
                        }
                    }
                }

                // Insert hole points (starting at holeBridgeNode) into vertex list at validBridgeNodeOnHull
                currentNode = validBridgeNodeOnHull;
                for (var i = holeData.BridgeIndex; i <= shape.PointPerHoleCount[holeData.HoleIndex] + holeData.BridgeIndex; i++)
                {
                    var previousIndex = currentNode.Value.Index;
                    var currentIndex  = shape.IndexOfPointInHole(i % shape.PointPerHoleCount[holeData.HoleIndex], holeData.HoleIndex);
                    var nextIndex     = shape.IndexOfPointInHole((i + 1) % shape.PointPerHoleCount[holeData.HoleIndex], holeData.HoleIndex);

                    if (i == shape.PointPerHoleCount[holeData.HoleIndex] + holeData.BridgeIndex) // have come back to starting point
                    {
                        nextIndex = validBridgeNodeOnHull.Value.Index; // next point is back to the point on the hull
                    }

                    var vertexIsConvex = IsConvex(shape.Points[previousIndex], shape.Points[currentIndex], shape.Points[nextIndex]);
                    var holeVertex     = new Vertex(shape.Points[currentIndex], currentIndex, vertexIsConvex);
                    currentNode = vertexList.AddAfter(currentNode, holeVertex);
                }

                // Add duplicate hull bridge vert now that we've come all the way around. Also set its concavity
                var nextVertexPos  = currentNode.Next?.Value.Position ?? vertexList.First.Value.Position;
                var isConvex  = IsConvex(holeData.BridgePoint, validBridgeNodeOnHull.Value.Position, nextVertexPos);
                var repeatStartHullVert = new Vertex(validBridgeNodeOnHull.Value.Position, validBridgeNodeOnHull.Value.Index, isConvex);
                vertexList.AddAfter(currentNode, repeatStartHullVert);

                //Set concavity of initial hull bridge vert, since it may have changed now that it leads to hole vert
                var nodeBeforeStartBridgeNodeOnHull = validBridgeNodeOnHull.Previous ?? vertexList.Last;
                var nodeAfterStartBridgeNodeOnHull  = validBridgeNodeOnHull.Next ?? vertexList.First;
                validBridgeNodeOnHull.Value.IsConvex = IsConvex(nodeBeforeStartBridgeNodeOnHull.Value.Position, validBridgeNodeOnHull.Value.Position, nodeAfterStartBridgeNodeOnHull.Value.Position);
            }
            return vertexList;
        }


        // check if triangle contains any verts (note, only necessary to check reflex verts).
        private static bool TriangleContainsVertex(Vertex v0, Vertex v1, Vertex v2, LinkedList<Vertex> verticesInClippedPolygon)
        {
            var vertexNode = verticesInClippedPolygon.First;
            for (var i = 0; i < verticesInClippedPolygon.Count; i++)
            {
                if (!vertexNode.Value.IsConvex) // convex verts will never be inside triangle
                {
                    var vertexToCheck = vertexNode.Value;
                    if (vertexToCheck.Index != v0.Index && vertexToCheck.Index != v1.Index && vertexToCheck.Index != v2.Index) // dont check verts that make up triangle
                    {
                        if (MathHelper.PointInTriangle(v0.Position, v1.Position, v2.Position, vertexToCheck.Position))
                        {
                            return true;
                        }
                    }
                }
                vertexNode = vertexNode.Next;
            }

            return false;
        }
        
        // v1 is considered a convex vertex if v0-v1-v2 are wound in a counter-clockwise order.
        private static bool IsConvex(Vector2 v0, Vector2 v1, Vector2 v2)
        {
            return MathHelper.SideOfLine(v0, v2, v1) == -1;
        }
    }
}
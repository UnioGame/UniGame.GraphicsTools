namespace Taktika.Rendering.Runtime.Water.Triangulator
{
    using UnityEngine;

    public class Vertex
    {
        public readonly Vector2 Position;
        public readonly int     Index;

        public bool IsConvex;

        public Vertex(Vector2 position, int index, bool isConvex)
        {
            Position = position;
            Index    = index;
            IsConvex = isConvex;
        }
    }
}
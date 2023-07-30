namespace UniGame.Rendering.Runtime.Water.Triangulator
{
    using UnityEngine;

    public readonly struct HoleData
    {
        public readonly int HoleIndex;
        public readonly int BridgeIndex;
        
        public readonly Vector2 BridgePoint;

        public HoleData(int holeIndex, int bridgeIndex, Vector2 bridgePoint)
        {
            HoleIndex   = holeIndex;
            BridgeIndex = bridgeIndex;
            BridgePoint = bridgePoint;
        }
    }
}
namespace UniModules.UniGame.GraphicsTools.Runtime
{
    using UnityEngine;

    public readonly struct LABColor
    {
        public float L { get; }
        public float A { get; }
        public float B { get; }

        public LABColor(float l, float a, float b)
        {
            L = l;
            A = a;
            B = b;
        }
        
        public static LABColor ToLABColor(Color color)
        {
            var xyz   = TristimulusColor.ToTristimulusColor(color);
            var white = TristimulusColor.White;

            var x = PivotXYZ(xyz.X / white.X);
            var y = PivotXYZ(xyz.Y / white.Y);
            var z = PivotXYZ(xyz.Z / white.Z);

            var l = Mathf.Max(0.0f, 116.0f * y - 16.0f);
            var a = 500.0f * (x - y);
            var b = 200.0f * (y - z);
                
            return new LABColor(l, a, b);
        }

        private static float PivotXYZ(float n)
        {
            return n > TristimulusColor.Epsilon ? CubicRoot(n) : (TristimulusColor.Kappa * n + 16.0f) / 116.0f;
        }

        private static float CubicRoot(float n)
        {
            return Mathf.Pow(n, 1.0f / 3.0f);
        }
    }
}
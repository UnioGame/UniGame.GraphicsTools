namespace UniModules.UniGame.GraphicsTools.Runtime
{
    using UnityEngine;

    public readonly struct TristimulusColor
    {
        public static TristimulusColor White => new TristimulusColor(95.047f, 100.000f, 108.883f);
        
        public const float Epsilon = 0.008856f;
        public const float Kappa   = 903.3f;
        
        public float X { get; }
        public float Y { get; }
        public float Z { get; }

        public TristimulusColor(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }
        
        public static TristimulusColor ToTristimulusColor(Color color)
        {
            var r = PivotRgb(color.r);
            var g = PivotRgb(color.g);
            var b = PivotRgb(color.b);

            var x = r * 0.4124f + g * 0.3576f + b * 0.1805f;
            var y = r * 0.2126f + g * 0.7152f + b * 0.0722f;
            var z = r * 0.0193f + g * 0.1192f + b * 0.9505f;
                
            return new TristimulusColor(x, y, z);
        }

        private static float PivotRgb(float n)
        {
            return (n > 0.04045f ? Mathf.Pow((n + 0.055f) / 1.055f, 2.4f) : n / 12.92f) * 100.0f;
        }
    }
}
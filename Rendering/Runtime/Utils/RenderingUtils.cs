namespace UniGame.Rendering.Runtime.Utils
{
    using UnityEngine;

    public static class RenderingUtils
    {
        public static Texture2D CreateGradientTexture(Gradient gradient, int width = 32, int height = 1)
        {
            var gradientTexture = new Texture2D(width, height, TextureFormat.ARGB32, false)
            {
                filterMode = FilterMode.Bilinear,
                wrapMode   = TextureWrapMode.Clamp
            };

            var inv = 1.0f / (width - 1);
            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    var t     = x * inv;
                    var color = gradient.Evaluate(t);
                    gradientTexture.SetPixel(x, y, color);
                }
            }
            
            gradientTexture.Apply();

            return gradientTexture;
        }
    }
}
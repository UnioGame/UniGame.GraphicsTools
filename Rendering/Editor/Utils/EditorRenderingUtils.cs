namespace Taktika.Rendering.Editor.Utils
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public static class EditorRenderingUtils
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Mul(float[] matrix, Vector2 p)
        {
            if(matrix.Length != 4)
                throw new InvalidOperationException();
            
            return new Vector2(matrix[0] * p.x + matrix[1] * p.y, matrix[2] * p.x + matrix[3] * p.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Mul(Vector2 p, float[] matrix)
        {
            if(matrix.Length != 4)
                throw new InvalidOperationException();
            
            return new Vector2(matrix[0] * p.x + matrix[2] * p.y, matrix[1] * p.y + matrix[3] * p.y);
        }

        public static float[] Mul(float[] a, float[] b)
        {
            if(a.Length != 4 || b.Length != 4)
                throw new InvalidOperationException();
            
            var m0 = a[0] * b[0] + a[1] * b[2];
            var m1 = a[0] * b[1] + a[1] * b[3];
            var m2 = a[2] * b[0] + a[3] * b[2];
            var m3 = a[2] * b[1] + a[3] * b[3];

            return new[] {m0, m1, m2, m3};
        }

        public static float SampleNoise(Vector2 uv, float scale)
        {
            var t = 0.0f;

            var freq = Mathf.Pow(2.0f, 0.0f);
            var amp  = Mathf.Pow(0.5f, 3.0f);

            t += ValueNoise(new Vector2(uv.x * scale / freq, uv.y * scale / freq)) * amp;

            freq = Mathf.Pow(2.0f, 1.0f);
            amp  = Mathf.Pow(0.5f, 2.0f);

            t += ValueNoise(new Vector2(uv.x * scale / freq, uv.y * scale / freq)) * amp;

            freq = Mathf.Pow(2.0f, 2.0f);
            amp  = Mathf.Pow(0.5f, 1.0f);

            t += ValueNoise(new Vector2(uv.x * scale / freq, uv.y * scale / freq)) * amp;

            return t;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float NoiseRandomValue(Vector2 uv)
        {
            return Frac(Mathf.Sin(Vector2.Dot(uv, new Vector2(12.9898f, 78.233f))) * 43758.5453f);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float NoiseInterpolate(float a, float b, float t)
        {
            return (1.0f - t) * a + t * b;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float ValueNoise(Vector2 uv)
        {
            var i = Floor(uv);
            var f = Frac(uv);
            f = f * f * Subtract(3.0f,2.0f * f);

            uv = Abs(Subtract(Frac(uv), 0.5f));

            var c0 = i + new Vector2(0.0f, 0.0f);
            var c1 = i + new Vector2(1.0f, 0.0f);
            var c2 = i + new Vector2(0.0f, 1.0f);
            var c3 = i + new Vector2(1.0f, 1.0f);

            var r0 = NoiseRandomValue(c0);
            var r1 = NoiseRandomValue(c1);
            var r2 = NoiseRandomValue(c2);
            var r3 = NoiseRandomValue(c3);

            var bottomOfGrid = NoiseInterpolate(r0, r1, f.x);
            var topOfGrid    = NoiseInterpolate(r2, r3, f.x);
            var t            = NoiseInterpolate(bottomOfGrid, topOfGrid, f.y);

            return t;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float Frac(float value)
        {
            return value - (float) Math.Truncate(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Vector2 Frac(Vector2 value)
        {
            return new Vector2(Frac(value.x), Frac(value.y));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Vector2 Floor(Vector2 value)
        {
            return new Vector2(Mathf.Floor(value.x), Mathf.Floor(value.y));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Vector2 Subtract(float a, Vector2 b)
        {
            return new Vector2(a - b.x, a - b.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Vector2 Subtract(Vector2 a, float b)
        {
            return new Vector2(a.x - b, a.y - b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Vector2 Abs(Vector2 value)
        {
            return new Vector2(Mathf.Abs(value.x), Mathf.Abs(value.y));
        }
    }
}
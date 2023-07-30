inline float unity_noise_randomValue (float2 uv)
{
    return frac(sin(dot(uv, float2(12.9898, 78.233)))*43758.5453);
}

inline float unity_noise_interpolate (float a, float b, float t)
{
    return (1.0-t) * a + t*b;
}

inline float unity_valueNoise (float2 uv)
{
    float2 i = floor(uv);
    float2 f = frac(uv);
    f = f * f * (3.0 - 2.0 * f);

    uv = abs(frac(uv) - 0.5);
    float2 c0 = i + float2(0.0, 0.0);
    float2 c1 = i + float2(1.0, 0.0);
    float2 c2 = i + float2(0.0, 1.0);
    float2 c3 = i + float2(1.0, 1.0);
    float r0 = unity_noise_randomValue(c0);
    float r1 = unity_noise_randomValue(c1);
    float r2 = unity_noise_randomValue(c2);
    float r3 = unity_noise_randomValue(c3);

    float bottomOfGrid = unity_noise_interpolate(r0, r1, f.x);
    float topOfGrid = unity_noise_interpolate(r2, r3, f.x);
    float t = unity_noise_interpolate(bottomOfGrid, topOfGrid, f.y);
    return t;
}

float Unity_SimpleNoise_float(float2 UV, float Scale)
{
    float t = 0.0;

    float freq = pow(2.0, float(0));
    float amp = pow(0.5, float(3-0));
    t += unity_valueNoise(float2(UV.x*Scale/freq, UV.y*Scale/freq))*amp;

    freq = pow(2.0, float(1));
    amp = pow(0.5, float(3-1));
    t += unity_valueNoise(float2(UV.x*Scale/freq, UV.y*Scale/freq))*amp;

    freq = pow(2.0, float(2));
    amp = pow(0.5, float(3-2));
    t += unity_valueNoise(float2(UV.x*Scale/freq, UV.y*Scale/freq))*amp;

    return t;
}

float noise(float2 uv)
{
    return Unity_SimpleNoise_float(uv * (1./256.), 500.0);
}

float Remap(float value, float2 inMinMax, float2 outMinMax)
{
    return outMinMax.x + (value - inMinMax.x) * (outMinMax.y - outMinMax.x) / (inMinMax.y - inMinMax.x);
}

static const float2x2 m = float2x2(0.72, -1.65,  1.65,  0.72);

float SampleLargeWaves(float2 uv, float height, float waveSize, float waveHeight, float2 speed)
{
    float2 uv2 = uv * waveSize;
    float2 shift = speed * float2(380.0, 260.0);

    float f = 0.6 * noise(uv);
    f += 0.25 * noise(mul(m, uv));
    f += 0.1666 * noise(mul(m, mul(m, uv)));

    return sin(uv2.x * 0.622 + uv2.y * 0.622 + shift.x * 4.269) * waveHeight * f * height * height;
}

float SampleSmallWaves(float2 uv, float waveSize, float waveHeight, float2 speed)
{
    uv *= waveSize;
    float2 shift = speed * float2(320.0, 240.0);
    
    float f = 0.0;
    float amp = 1.0, s = 0.5;
    
    for(int i = 0; i < 9; i++)
    {
        uv = mul(uv, m) * 0.947;
        f -= amp * abs(sin((noise(uv + shift * s) - 0.5) * 2.0));
        amp *= 0.59;
        s *= -1.329;
    }

    return f * waveHeight;
}

float4 Bake(float3 normal, float depth)
{
    float r = (normal.x + 1.0) * 0.5;
    float g = (normal.y + 1.0) * 0.5;
    float b = (normal.z + 1.0) * 0.5;
    float a = (depth + 1.0) * 0.5;

    return float4(r, g, b, a);
}
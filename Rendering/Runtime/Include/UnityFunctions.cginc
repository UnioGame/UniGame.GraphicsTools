float2 unity_tiling_and_offset(float2 uv, float2 tiling, float2 offset)
{
    return uv * tiling + offset;
}

float2 unity_rotate_degrees(float2 uv, float2 center, float rotation)
{
    rotation = rotation * (3.1415926/180.0);
    uv-=center;

    float s = sin(rotation);
    float c = cos(rotation);

    float2x2 rMatrix = float2x2(c, -s, s, c);
    rMatrix *= 0.5;
    rMatrix += 0.5;
    rMatrix = rMatrix * 2 - 1;
    
    uv.xy = mul(uv.xy, rMatrix);
    uv += center;

    return uv;
}

float2 unity_rotate_radians(float2 uv, float2 center, float rotation)
{
    uv -= center;
    
    float s = sin(rotation);
    float c = cos(rotation);

    float2x2 rMatrix = float2x2(c, -s, s, c);
    rMatrix *= 0.5;
    rMatrix += 0.5;
    rMatrix = rMatrix * 2 - 1;

    uv.xy = mul(uv.xy, rMatrix);
    uv += center;

    return uv;
}

float clamp01(float value)
{
    return clamp(value, 0.0, 1.0);
}

half clamp01(half value)
{
    return clamp(value, 0.0, 1.0);
}
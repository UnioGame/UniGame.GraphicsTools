#ifndef HELPER_FUNCTIONS_INCLUDED
#define HELPER_FUNCTIONS_INCLUDED

#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Macros.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

float4 object_to_clip_pos(float3 pos)
{
    return mul(UNITY_MATRIX_VP, mul(UNITY_MATRIX_M, float4(pos, 1)));
}

float2 tiling_and_offset(float2 uv, float2 tiling, float2 offset)
{
    return uv * tiling + offset;
}

float2 rotate_degrees(float2 uv, float2 center, float rotation)
{
    rotation = rotation * (PI/180.0);
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

float2 rotate_radians(float2 uv, float2 center, float rotation)
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

float remap(float value, float2 inMinMax, float2 outMinMax)
{
    return outMinMax.x + (value - inMinMax.x) * (outMinMax.y - outMinMax.x) / (inMinMax.y - inMinMax.x);
}

float color_mask(float3 inValue, float3 maskColor, float range, float fuzziness)
{
    float dist = distance(maskColor, inValue);
    return saturate(1.0 - (dist - range) / max(fuzziness, 1e-5));
}

#endif
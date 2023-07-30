#ifndef UI_INCLUDED
#define UI_INCLUDED

inline float Get2DClipping(in float2 position, in float4 clipRect)
{
    float2 inside = step(clipRect.xy, position.xy) * step(position.xy, clipRect.zw);
    return inside.x * inside.y;
}

inline float4 GetUIDiffuseColor(in float2 position, in sampler2D mainTexture, in sampler2D alphaTexture, float4 textureSampleAdd)
{
    return float4(tex2D(mainTexture, position).rgb + textureSampleAdd.rgb, tex2D(alphaTexture, position).r + textureSampleAdd.a);
}
#endif
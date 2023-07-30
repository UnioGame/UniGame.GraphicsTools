float2 hash2D2D (float2 s)
{
    return frac(sin(fmod(float2(dot(s, float2(127.1,311.7)), dot(s, float2(269.5,183.3))), 3.14159))*43758.5453);
}

float4 tex2DStochastic(UnityTexture2D tex, UnitySamplerState ss, float2 uv)
{
    //triangle vertices and blend weights
    //BW_vx[0...2].xyz = triangle verts
    //BW_vx[3].xy = blend weights (z is unused)
    float4x3 BW_vx;
 
    //uv transformed into triangular grid space with UV scaled by approximation of 2*sqrt(3)
    float2 skewUV = mul(float2x2 (1.0, 0.0, -0.57735027, 1.15470054), uv * 3.464);
 
    //vertex IDs and barycentric coords
    float2 vxID = float2(floor(skewUV));
    float3 barry = float3(frac(skewUV), 0);
    
    barry.z = 1.0 - barry.x - barry.y;
 
    BW_vx = barry.z > 0 ? 
        float4x3(float3(vxID, 0), float3(vxID + float2(0, 1), 0), float3(vxID + float2(1, 0), 0), barry.zyx) :
        float4x3(float3(vxID + float2 (1, 1), 0), float3(vxID + float2 (1, 0), 0), float3(vxID + float2 (0, 1), 0), float3(-barry.z, 1.0 - barry.y, 1.0 - barry.x));
 
    //calculate derivatives to avoid triangular grid artifacts
    float2 dx = ddx(uv);
    float2 dy = ddy(uv);
 
    //blend samples with calculated weights
    return mul(SAMPLE_TEXTURE2D_GRAD(tex, ss, uv + hash2D2D(BW_vx[0].xy), dx, dy), BW_vx[3].x) + 
            mul(SAMPLE_TEXTURE2D_GRAD(tex, ss, uv + hash2D2D(BW_vx[1].xy), dx, dy), BW_vx[3].y) + 
            mul(SAMPLE_TEXTURE2D_GRAD(tex, ss, uv + hash2D2D(BW_vx[2].xy), dx, dy), BW_vx[3].z);
}

void tex2DStochastic_half(UnityTexture2D tex, UnitySamplerState ss, float2 uv, out half4 color)
{
    color = tex2DStochastic(tex, ss, uv);
}

void tex2DStochastic_float(UnityTexture2D tex, UnitySamplerState ss, float2 uv, out float4 color)
{
    color = tex2DStochastic(tex, ss, uv);
}
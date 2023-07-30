Shader "Water/BakedWavesShader"
{
    Properties
    {
        _LargeWaves ("Large Waves", 2D) = "black" {}
        _SmallWaves("Small Waves", 2D) = "black" {}

        _WaterLevel("Water Level", float) = 0.94
        _Height("Height", float) = 0.2
        _Coast2WaterFadeDepth("Coast2WaterFadeDepth", float) = 0.1
        _WaterGlossyLight("WaterGlossyLight", float) = 120.0

        _ShallowColor("_ShallowColor", Color) = (0.43, 0.60, 0.66)
        _DeepColor("_DeepColor", Color) = (0.06, 0.07, 0.11)
        _SpecularColor("_SpecularColor", Color) = (1.3, 1.3, 0.9)

        _LargeWavesSpeed("LargeWavesSpeed", float) = 0.1
        _SmallWavesSpeed("SmallWavesSpeed", float) = 0.001

        _Direction("Direction", Vector) = (1.0, 1.0, 0.0, 0.0)

        _LightPos("Light Position", Vector) = (-0.27, 0.9, 5.04, 0)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _LargeWaves;
            float4 _LargeWaves_ST;

            sampler2D _SmallWaves;

            float _WaterLevel;
            float _Height;

            float _Coast2WaterFadeDepth;
            float _WaterGlossyLight;

            float3 _ShallowColor;
            float3 _DeepColor;
            float3 _SpecularColor;

            float _LargeWavesSpeed;
            float _SmallWavesSpeed;

            float2 _Direction;

            float3 _LightPos;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _LargeWaves);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 largeUv = i.uv + _Direction * _Time.y * _LargeWavesSpeed;
                
                float4 largeWaves = tex2D(_LargeWaves, largeUv);
                float largeDepth = largeWaves.w * 2.0 - 1.0;
                float3 largeNormal = normalize(largeWaves.rgb * 2.0 - 1.0);

                float2 smallUv = i.uv + _Direction * _Time.y * _SmallWavesSpeed;
                
                float4 smallWaves = tex2D(_SmallWaves, smallUv);
                float smallDepth = smallWaves.w * 2.0 - 1.0;
                float3 smallNormal = normalize(smallWaves.rgb * 2.0 - 1.0);

                float depth = largeDepth + smallDepth;
                float3 normal = normalize(largeNormal + smallNormal);
                
                float deepWaterFadeDepth = 0.5 + _Coast2WaterFadeDepth;
                float level = _WaterLevel + 0.2 * depth;

                float coastfade = clamp((level - _Height) / _Coast2WaterFadeDepth, 0., 1.);
                float coastfade2 = clamp((level - _Height) / deepWaterFadeDepth, 0., 1.);

                float3 col = _ShallowColor;

                float intensity = col.r * .2126 + col.g * .7152 + col.b * .0722;
                float3 watercolor = lerp(col * intensity, _DeepColor, smoothstep(0., 1., coastfade2));

                float2 pos = i.uv + normal.xy * 0.002 * (level - _Height);

                float3 r0 = float3(pos, _WaterLevel);
                float3 rd = normalize(_LightPos - r0);
                float grad = dot(normal, rd);
                float specular2 = pow(grad, _WaterGlossyLight);
                watercolor += specular2 * _SpecularColor;

                col = lerp(col, watercolor, coastfade);

                return fixed4(col.rgb, 1.0);
            }
            ENDCG
        }
    }
}

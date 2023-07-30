Shader "UniGame/URP/Blur/KawaseBlur"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            HLSLPROGRAM
            
            #pragma vertex vert
            #pragma fragment frag

            #pragma target 2.0

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "./../../../../Rendering/Runtime/Include/Core.hlsl"

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

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
            float4 _MainTex_ST;
            float4 _MainTex_TexelSize;
            CBUFFER_END
            
            float _offset;

            half3 sample_main_tex(const float2 uv)
            {
                return SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv).rgb;
            }

            v2f vert(const appdata v)
            {
                v2f o;
                o.vertex = object_to_clip_pos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            half4 frag (const v2f input) : SV_Target
            {
                const float2 resolution = _MainTex_TexelSize.xy;
                float i = _offset;
                
                half4 col;                
                col.rgb = sample_main_tex(input.uv);
                col.rgb += sample_main_tex(input.uv + float2(i, i) * resolution);
                col.rgb += sample_main_tex(input.uv + float2(i, -i) * resolution);
                col.rgb += sample_main_tex(input.uv + float2(-i, i) * resolution);
                col.rgb += sample_main_tex(input.uv + float2(-i, -i) * resolution);
                col.rgb /= 5.0f;
                
                return col;
            }
            ENDHLSL
        }
    }
    CustomEditor "UniGame.Rendering.Editor.Blur.KawaseBlur.KawaseBlurShaderEditor"
}
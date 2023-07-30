Shader "Taktika/URP/Blur/KawaseBlur"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

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

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            float4 _MainTex_ST;
            
            float _offset;

            half3 sample(const float2 uv)
            {
                return tex2D(_MainTex, uv).rgb;
            }

            v2f vert(const appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (const v2f input) : SV_Target
            {
                const float2 resolution = _MainTex_TexelSize.xy;
                float i = _offset;
                
                fixed4 col;                
                col.rgb = sample(input.uv);
                col.rgb += sample(input.uv + float2(i, i) * resolution);
                col.rgb += sample(input.uv + float2(i, -i) * resolution);
                col.rgb += sample(input.uv + float2(-i, i) * resolution);
                col.rgb += sample(input.uv + float2(-i, -i) * resolution);
                col.rgb /= 5.0f;
                
                return col;
            }
            ENDCG
        }
    }
    CustomEditor "Taktika.Rendering.Editor.Blur.KawaseBlur.KawaseBlurShaderEditor"
}
Shader "Water/Bake/Noise Shader(BAKE)"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        
        _NoiseSize("Noise Size", float) = 500
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
            #include "WavesCore.hlsl"

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
            float4 _MainTex_ST;

            float _NoiseSize;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float noiseValue = Unity_SimpleNoise_float(i.uv, _NoiseSize);
                
                return fixed4(noiseValue, noiseValue, noiseValue, 1);
            }
            ENDCG
        }
    }
}

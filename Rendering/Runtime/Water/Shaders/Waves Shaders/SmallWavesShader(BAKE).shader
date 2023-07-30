Shader "Water/Bake/Small Waves Shader(BAKE)"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        
        _WaveHeight("Wave Height", float) = 0.6
        _WaveSize("Wave Size", float) = 0.6

        _Direction("Direction (xy - dir, z - waves speed, w - water speed)", Vector) = (1, 1, 0.001, 0.1)
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

            float _WaveHeight;
            float _WaveSize;

            float4 _Direction;

            float WaterMap(float2 uv)
            {
                return SampleSmallWaves(uv, _WaveSize, _WaveHeight, _Direction.xy * _Direction.z);
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 pos = i.uv * 15.0 - _Direction.xy * _Direction.w;

                float depth = WaterMap(pos);

                float2 diff = float2(.0, .01);

                float h1 = WaterMap(pos - diff);
                float h2 = WaterMap(pos + diff);
                float h3 = WaterMap(pos - diff.yx);
                float h4 = WaterMap(pos + diff.yx);

                float3 normal = normalize(float3(h3 - h4, h1 - h2, .125));
                
                return Bake(normal, depth);
            }
            ENDCG
        }
    }
}

Shader "Water/Realtime Waves"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}

        coast2water_fadedepth("coast2water_fadedepth", float) = 0.10

        large_waveheight("large_waveheight", float) = 0.10
        large_wavesize("large_wavesize", float) = 4.0
        small_waveheight("small_waveheight", float) = 0.6
        small_wavesize("small_wavesize", float) = 0.5

        water_softlight_fact("water_softlight_fact", float) = 15
        water_glossylight_fact("water_glossylight_fact", float) = 120

        watercolor("watercolor", Color) = (0.43, 0.60, 0.66)
        watercolor2("watercolor2", Color) = (0.06, 0.07, 0.11)
        water_specularcolor("water_specularcolor", Color) = (1.3, 1.3, 0.9)

        _WaterLevel("Water Level", float) = 0.94
        _Height("Height", float) = 0.2

        _LightPos("Light Position", Vector) = (-0.27, 0.9, 5.04, 0)
        _Direction("Direction (xy - dir, z - waves speed, w - water speed)", Vector) = (1, 1, 0.001, 0.1)
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

            float coast2water_fadedepth = 0.10;
            
            float large_waveheight = 0.10;
            float large_wavesize = 4.0;
            float small_waveheight = 0.6;
            float small_wavesize = 0.5;

            float water_softlight_fact  = 15;  // range [1..200] (should be << smaller than glossy-fact)
            float water_glossylight_fact = 120;

            float3 watercolor             = float3(0.43, 0.60, 0.66); // 'transparent' low-water color (RGB)
            float3 watercolor2            = float3(0.06, 0.07, 0.11); // deep-water color (RGB, should be darker than the low-water color)
            float3 water_specularcolor    = float3(1.0, 1.0, 0.9);

            float _WaterLevel;
            float _Height;

            float3 _LightPos;
            float4 _Direction;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }
            
            float water_map(float2 p, float height)
            {
                float largeWave = SampleLargeWaves(p, height, large_wavesize, large_waveheight, _Direction.xy * _Direction.z * _Time.y);
                float smallWave = SampleSmallWaves(p, small_wavesize, small_waveheight, _Direction.xy * _Direction.z * _Time.y);

                return largeWave + smallWave;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv =  i.uv;

                float deepwater_fadedepth = 0.5 + coast2water_fadedepth;

                float3 col = watercolor;

                float waveheight = clamp(_WaterLevel * 3.0 - 1.5, 0., 1.);
                float2 pos = uv * 15.0 - _Direction.xy * _Direction.w * _Time.y;

                float level = _WaterLevel + 0.2 * water_map(pos, waveheight);

                float2 diff = float2(.0, .01);

                float h1 = water_map(pos - diff, waveheight);
                float h2 = water_map(pos + diff, waveheight);
                float h3 = water_map(pos - diff.yx, waveheight);
                float h4 = water_map(pos + diff.yx, waveheight);

                float3 normwater = normalize(float3(h3 - h4, h1 - h2, 0.125));
                uv += normwater.xy * 0.002 * (level - _Height);

                float coastfade = clamp((level - _Height) / coast2water_fadedepth, 0., 1.);
                float coastfade2 = clamp((level - _Height) / deepwater_fadedepth, 0., 1.);

                float intensity = col.r * .2126 + col.g * .7152 + col.b * .0722;
                watercolor = lerp(watercolor * intensity, watercolor2, smoothstep(0., 1., coastfade2));

                float3 r0 = float3(uv, _WaterLevel);
                float3 rd = normalize(_LightPos - r0);
                float grad = dot(normwater, rd);
                float specular2 = pow(grad, water_glossylight_fact);
                watercolor += specular2 * water_specularcolor;

                col = lerp(col, watercolor, coastfade);

                return fixed4(col.rgb, 1.0);
            }
            ENDCG
        }
    }
}

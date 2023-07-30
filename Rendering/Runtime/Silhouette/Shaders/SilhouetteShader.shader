Shader "UniGame/URP/UI/SilhouetteShader"
{
    Properties
    {
        [PerRendererData]_MainTex ("Texture", 2D) = "white" {}
        _OutlineColor("Outline Color", Color) = (1, 1, 1, 1)
        _OutlineThickness("Outline Thickness", Range(0, 100)) = 5
        _OutlineAccuracy("Outline Accuracy", Range(4, 100)) = 20
        
        _Rect("Rect Display", Vector) = (0,0,1,1)

        _StencilComp ("Stencil Comparison", float) = 8
        _Stencil ("Stencil ID", float) = 0
        _StencilOp ("Stencil Operation", float) = 0
        _StencilWriteMask ("Stencil Write Mask", float) = 255
        _StencilReadMask ("Stencil Read Masl", float) = 255

        _ColorMask ("Color Mask", float) = 15
        
        [Toggle(UNITY_UI_ALPHACLIP)]_UseUIAlphaClip ("Use Alpha Clip", float) = 0
    }
    SubShader
    {
        Tags 
        { 
            "RenderType"="Transparent" 
            "IgnoreProjectoe"="True" 
            "Queue"="Transparent" 
            "PreviewType"="Plane" 
            "CanUseSpriteAtlas"="True" 
        }
        Stencil 
        { 
            Ref[_Stencil] 
            Comp[_StencilComp] 
            Pass[_StencilOp] 
            ReadMask[_StencilReadMask]
            WriteMask[_StencilWriteMask] 
        }
        
        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask[_ColorMask]        

        Pass
        {
            Name "Default"

            CGPROGRAM

            #pragma vertex vertex_shader
            #pragma fragment fragment_shader
            #pragma target 2.0

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

            struct Input
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Output
            {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                float2 uv : TEXCOORD0;
                float4 world_position : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;

            fixed4 _TextureSampleAdd;
            float4 _ClipRect;
            float4 _MainTex_ST;

            fixed4 _MainTex_TexelSize;

            CBUFFER_START(UnityPerMaterial)
            float4 _OutlineColor;
            float _OutlineThickness;
            int _OutlineAccuracy;
            fixed4 _Rect;
            CBUFFER_END

            fixed4 sample_texture(const float2 uv)
            {
                if(uv.x < _Rect.x || uv.x > _Rect.z || uv.y < _Rect.y || uv.y > _Rect.w)
                {
                     return fixed4(0,0,0,0);
                }

                return tex2D(_MainTex, uv);
            }
            
            fixed2 get_texture_size()
            {
                return fixed2((_Rect.z - _Rect.x) * _MainTex_TexelSize.z, (_Rect.w - _Rect.y) * _MainTex_TexelSize.w);
            }

            float2 sample_uv(const float2 uv, const fixed2 offset)
            {
                const fixed2 real_size = get_texture_size();
                const fixed2 big_size = fixed2(real_size.x, real_size.y);
                const fixed2 small_size = fixed2(real_size.x - offset.x, real_size.y - offset.y);

                const fixed2 center = fixed2((_Rect.x + _Rect.z) * 0.5, (_Rect.y + _Rect.w) * 0.5);

                return float2
                (
                    uv.x * (big_size.x / small_size.x) - center.x * offset.x / small_size.x,
                    uv.y * (big_size.y / small_size.y) - center.y * offset.y / small_size.y
                );
            }

            Output vertex_shader(const Input input)
            {
                Output output;
                
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                output.world_position = input.vertex;
                output.vertex = UnityObjectToClipPos(input.vertex);
                output.uv = TRANSFORM_TEX(input.uv, _MainTex);
                output.color = input.color;
        
                return output;
            }

            fixed4 fragment_shader(const Output input) : SV_Target
            {
                const float2 uv = sample_uv(input.uv, float2(_OutlineThickness, _OutlineThickness) * 2);
                const fixed color_alpha = sample_texture(uv).a;

                const fixed2 thickness = fixed2(_OutlineThickness * _MainTex_TexelSize.x, _OutlineThickness * _MainTex_TexelSize.y);

                static const float angle_step = 360.0 / _OutlineAccuracy;
                float outline_alpha = 0.0;
                for(int i = 0; i < _OutlineAccuracy; i++)
                {
                    const float angle = i * angle_step * 2.0 * 3.14 / 360.0;
                    const float2 outline_uv = uv + fixed2(thickness.x * cos(angle), thickness.y * sin(angle));

                    outline_alpha += sample_texture(outline_uv).a;
                }

                outline_alpha = clamp(outline_alpha, 0.0, 1.0) - color_alpha;

                const fixed4 outline_color = outline_alpha * _OutlineColor * input.color.a;
                
                fixed4 result_color = outline_color + input.color * color_alpha;

                #ifdef UNITY_UI_CLIP_RECT
                result_color.a *= UnityGet2DClipping(input.world_position.xy, _ClipRect);
                #endif

                #ifdef UNITY_UI_ALPHACLIP
                clip(result_color.a - 0.001);
                #endif

                return result_color;
            }
            
            ENDCG
        }
    }
    CustomEditor "UniGame.Rendering.Editor.Silhouette.SilhouetteShaderEditor"
}

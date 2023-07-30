Shader "UniGame/URP/UI/Blur/UIKawaseBlur"
{
    Properties
    {
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		_Color("Tint", Color) = (1,1,1,1)

		_StencilComp("Stencil Comparison", Float) = 8
		_Stencil("Stencil ID", Float) = 0
		_StencilOp("Stencil Operation", Float) = 0
		_StencilWriteMask("Stencil Write Mask", Float) = 255
		_StencilReadMask("Stencil Read Mask", Float) = 255

		_ColorMask("Color Mask", Float) = 15

		[Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip("Use Alpha Clip", Float) = 0
    }
    SubShader
    {
		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
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
		ZTest[unity_GUIZTestMode]
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			Name "Default"

			HLSLPROGRAM
			
			#pragma vertex vert
			#pragma fragment frag
			
			#pragma target 2.0

			#pragma multi_compile_local _ UNITY_UI_CLIP_RECT
			#pragma multi_compile_local _ UNITY_UI_ALPHACLIP

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "./../../../../Rendering/Runtime/Include/Core.hlsl"
			#include "./../../../../Rendering/Runtime/Include/UI.hlsl"

			struct appdata_t
			{
				float4 vertex    : POSITION;
				float4 color     : COLOR;
				float2 texcoord  : TEXCOORD0;
				float4 screenPos : TEXCOORD2;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				half4 color : COLOR;
				float2 texcoord : TEXCOORD0;
				float4 world_position : TEXCOORD1;
				float4 screen_pos : TEXCOORD2;
				UNITY_VERTEX_OUTPUT_STEREO
			};

			TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
			
			TEXTURE2D(_blurTexture);
            SAMPLER(sampler_blurTexture);

            CBUFFER_START(UnityPerMaterial)
            float4 _MainTex_ST;
			half4 _Color;
			half4 _TextureSampleAdd;
			float4 _ClipRect;
            CBUFFER_END

			v2f vert(const appdata_t v)
			{
				v2f output;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
				output.world_position = v.vertex;
				output.vertex = object_to_clip_pos(output.world_position);
				output.screen_pos = ComputeScreenPos(output.vertex);

				output.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);

				output.color = v.color * _Color;
				return output;
			}

			half4 frag(const v2f input) : SV_Target
			{
				half4 color = (SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.texcoord) + _TextureSampleAdd) * input.color;

				const float2 screenUV = input.screen_pos.xy / input.screen_pos.w;
				color.rgb *= SAMPLE_TEXTURE2D(_blurTexture, sampler_blurTexture, screenUV).rgb;
				
				#ifdef UNITY_UI_CLIP_RECT
				color.a *= Get2DClipping(input.world_position.xy, _ClipRect);
				#endif

				#ifdef UNITY_UI_ALPHACLIP
				clip(color.a - 0.001);
				#endif

				return color;
			}
			ENDHLSL
		}
    }
	CustomEditor "UniGame.Rendering.Editor.Blur.KawaseBlur.UIKawaseBlurShaderEditor"
}
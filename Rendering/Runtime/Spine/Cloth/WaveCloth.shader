Shader "Taktika/Spine/SkeletonGraphics/WaveCloth"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		[Toggle(_STRAIGHT_ALPHA_INPUT)] _StraightAlphaInput("Straight Alpha Texture", Int) = 0
		[Toggle(_CANVAS_GROUP_COMPATIBLE)] _CanvasGroupCompatible("CanvasGroup Compatible", Int) = 0
		_Color ("Tint", Color) = (1,1,1,1)

		_NormalTex ("Normal Map", 2D) = "white" {}
		_DistortStrength ("Distortion Strength", Range(0.0, 1.0)) = 1.0
		_WaveSmallDistortSpeedTile ("Wave small distort speed tile", Vector) = (0.1, 0.1, 1.0, 1.0)
		_Opacity ("Opacity", Float) = 1.0
		_GradMaskAngle ("Grad Mask Angle", Range(-180.0, 180.0)) = 0.0
		_GradMaskWidth ("Grad Mask Width", Range(0.01, 1.0)) = 0.2
		_GradMaskBlend ("Grad Mask Blend", Range(0.0, 1.0)) = 0.5
		_DistortMask ("Distort Mask", 2D) = "white" {}

		[HideInInspector][Enum(UnityEngine.Rendering.CompareFunction)] _StencilComp ("Stencil Comparison", Float) = 8
		[HideInInspector] _Stencil ("Stencil ID", Float) = 0
		[HideInInspector][Enum(UnityEngine.Rendering.StencilOp)] _StencilOp ("Stencil Operation", Float) = 0
		[HideInInspector] _StencilWriteMask ("Stencil Write Mask", Float) = 255
		[HideInInspector] _StencilReadMask ("Stencil Read Mask", Float) = 255

		[HideInInspector] _ColorMask ("Color Mask", Float) = 15

		[Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0

		// Outline properties are drawn via custom editor.
		[HideInInspector] _OutlineWidth("Outline Width", Range(0,8)) = 3.0
		[HideInInspector] _OutlineColor("Outline Color", Color) = (1,1,0,1)
		[HideInInspector] _OutlineReferenceTexWidth("Reference Texture Width", Int) = 1024
		[HideInInspector] _ThresholdEnd("Outline Threshold", Range(0,1)) = 0.25
		[HideInInspector] _OutlineSmoothness("Outline Smoothness", Range(0,1)) = 1.0
		[HideInInspector][MaterialToggle(_USE8NEIGHBOURHOOD_ON)] _Use8Neighbourhood("Sample 8 Neighbours", Float) = 1
		[HideInInspector] _OutlineMipLevel("Outline Mip Level", Range(0,3)) = 0
	}

	SubShader
	{
		Tags
		{
			"Queue"="Transparent"
			"IgnoreProjector"="True"
			"RenderType"="Transparent"
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}

		Stencil
		{
			Ref [_Stencil]
			Comp [_StencilComp]
			Pass [_StencilOp]
			ReadMask [_StencilReadMask]
			WriteMask [_StencilWriteMask]
		}

		Cull Off
		Lighting Off
		ZWrite Off
		ZTest [unity_GUIZTestMode]
		Fog { Mode Off }
		Blend One OneMinusSrcAlpha
		ColorMask [_ColorMask]

		Pass
		{
			Name "Normal"

			CGPROGRAM
			#pragma shader_feature _ _STRAIGHT_ALPHA_INPUT
			#pragma shader_feature _ _CANVAS_GROUP_COMPATIBLE
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0

			#include "UnityCG.cginc"
			#include "UnityUI.cginc"
			#include "../../Include/UnityFunctions.cginc"

			#pragma multi_compile __ UNITY_UI_ALPHACLIP

			struct VertexInput
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 vertex   : SV_POSITION;
				fixed4 color    : COLOR;
				half2 texcoord  : TEXCOORD0;
				float4 worldPosition : TEXCOORD1;
				UNITY_VERTEX_OUTPUT_STEREO
			};

			sampler2D _MainTex;
			fixed4 _Color;
			fixed4 _TextureSampleAdd;
			float4 _ClipRect;

			sampler2D _NormalTex;
			float _DistortStrength;
			float4 _WaveSmallDistortSpeedTile;
			fixed _Opacity;
			float _GradMaskAngle;
			float _GradMaskWidth;
			float _GradMaskBlend;

			sampler2D _DistortMask;

			VertexOutput vert (VertexInput IN)
			{
				VertexOutput OUT;

				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

				OUT.worldPosition = IN.vertex;
				OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);
				OUT.texcoord = IN.texcoord;

				#ifdef UNITY_HALF_TEXEL_OFFSET
				OUT.vertex.xy += (_ScreenParams.zw-1.0) * float2(-1,1);
				#endif

				OUT.color = IN.color * float4(_Color.rgb * _Color.a, _Color.a); // Combine a PMA version of _Color with vertexColor.
				return OUT;
			}

			float2 transform_uv(float2 uv)
			{
				float2 rotatedUv = unity_rotate_degrees(uv, float2(0.5, 0.5), _GradMaskAngle);
				float width = _GradMaskWidth * 0.5;
				float lmask = lerp(width + 1.0, -width, _GradMaskBlend);

				float distortion = tex2D(_DistortMask, uv).r;
				
				float rotMask = clamp01((rotatedUv.x - lmask) * (-2.0 / _GradMaskWidth)) * 2.0 * distortion;

				float2 normalUv = unity_tiling_and_offset(uv, _WaveSmallDistortSpeedTile.zw, _WaveSmallDistortSpeedTile.xy * _Time.y);
				float3 waveNormal = tex2D(_NormalTex, normalUv).rgb;

				float2 waveMask = waveNormal.rg * (_DistortStrength * 0.1) + uv;

				return lerp(uv, waveMask, rotMask);
			}

			half calculate_alpha(half alpha)
			{
				return alpha * _Opacity * _Color.a;
			}
			
			fixed4 frag (VertexOutput IN) : SV_Target
			{
				half4 texColor = tex2D(_MainTex, transform_uv(IN.texcoord));
				texColor.a = calculate_alpha(texColor.a);

				#if defined(_STRAIGHT_ALPHA_INPUT)
				texColor.rgb *= texColor.a;
				#endif

				half4 color = (texColor + _TextureSampleAdd) * IN.color;
				#ifdef _CANVAS_GROUP_COMPATIBLE
				// CanvasGroup alpha sets vertex color alpha, but does not premultiply it to rgb components.
				color.rgb *= IN.color.a;
				#endif

				color *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);

				#ifdef UNITY_UI_ALPHACLIP
				clip (color.a - 0.001);
				#endif

				return color;
			}
			ENDCG
		}
	}
}

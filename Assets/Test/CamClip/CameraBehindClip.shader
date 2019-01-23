﻿Shader "Custom/CameraBehindClip"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
	}
		SubShader
		{
			Tags {"Queue" = "Transparent" "RenderType" = "Transparent" }
			  ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha
			CGPROGRAM
			// Physically based Standard lighting model, and enable shadows on all light types
			#pragma surface surf Standard fullforwardshadows alpha:blend

			// Use shader model 3.0 target, to get nicer looking lighting
			#pragma target 3.0

			sampler2D _MainTex;

			struct Input
			{
				float2 uv_MainTex;
				float4 screenPos;
			};

			half _Glossiness;
			half _Metallic;
			fixed4 _Color;

			void surf(Input IN, inout SurfaceOutputStandard o)
			{

				float2 screenPosition = (IN.screenPos.xy / IN.screenPos.w);
				if (screenPosition.x >= 0.5) {
					discard;
				}
				o.Albedo = float3(screenPosition, 0);
				o.Alpha = _Color.a;
			}
			ENDCG
		}
			FallBack "Diffuse"
}

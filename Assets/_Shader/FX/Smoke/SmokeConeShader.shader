Shader "Custom/SmokeConeShader"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_FadeTex("Fade (RGB)", 2D) = "white" {}
		_MinimumNdotL("Minimum absolute NdotL", Range(0,1)) = 0.0

		_TextureTranslationSpeed("Texture Translation Speed", Float) = 1.0

		_CutoutAlphaSmooth("Cutout Alpha Smooth", Range(0,1)) = 0.0
		_AlphaCutDelta("Alpha Cut Delta", Range(0,1)) = 0.0
		_AlphaCutPower("Alpha Cut Power", Range(0, 6)) = 3.0


		_AppearanceSmooth("Appearanche Smooth", Range(0.0,1.0)) = 0.0
		_StartUV("Start UV", Range(0.0,1.0)) = 0.0
		_EndUV("End UV", Range(0.0,1.0)) = 1.0
	}
		SubShader
		{
			Tags { "RenderType" = "Opaque" }
			Cull Off
			CGPROGRAM
			// Physically based Standard lighting model, and enable shadows on all light types
			#pragma surface surf Wrapped fullforwardshadows alpha:blend

			// Use shader model 3.0 target, to get nicer looking lighting
			#pragma target 3.0

			#include "wrappedLight.cginc"

			sampler2D _MainTex, _FadeTex;
			half _MinimumNdotL;

			half _TextureTranslationSpeed;
			half _CutoutAlphaSmooth, _AlphaCutDelta, _AlphaCutPower;

			float _StartUV;
			half _EndUV;
			half _AppearanceSmooth;

			//light model
			half4 LightingWrapped(SurfaceOutput s, half3 lightDir, half3 viewDir, half atten)
			{
				return LightingWrappedCalculation(s.Normal, s.Alpha, lightDir, _LightColor0, _MinimumNdotL, atten);
			}


			struct Input
			{
				float2 uv_MainTex;
				float2 uv_FadeTex;
			};

			fixed4 _Color;

			void surf(Input IN, inout SurfaceOutput o)
			{
				// Albedo comes from a texture tinted by color
				fixed4 texColor = tex2D(_MainTex, IN.uv_MainTex + half2(0, _TextureTranslationSpeed * _Time.x));
				fixed4 c = texColor * _Color;
				o.Albedo = c.rgb;

				half fadeAlpha = 1 - tex2D(_FadeTex, IN.uv_FadeTex).r;
				half smootherCutout = pow(1 - fadeAlpha + _AlphaCutDelta, _AlphaCutPower);
				half computedAlpha = (texColor.r);
				computedAlpha = smoothstep(smootherCutout - _CutoutAlphaSmooth, smootherCutout + _CutoutAlphaSmooth, computedAlpha);

				half finalAlpha = min(fadeAlpha, computedAlpha);
				finalAlpha *= smoothstep(_StartUV - _AppearanceSmooth, _StartUV + _AppearanceSmooth, IN.uv_FadeTex.y);
				finalAlpha *= smoothstep(1 - _EndUV - _AppearanceSmooth, 1 - _EndUV + _AppearanceSmooth, 1 - IN.uv_FadeTex.y);

					o.Alpha = finalAlpha;
				}
				ENDCG
		}
			FallBack "Diffuse"
}

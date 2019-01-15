Shader "Custom/SmokeConeShader"
{
	Properties
	{

		_MinimumNdotL("Minimum absolute NdotL", Range(0,1)) = 0.0


	}
		SubShader
	{
		Tags { "RenderType" = "Transparent" }

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Wrapped vertex:vert fullforwardshadows alpha:blend

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		#include "wrappedLight.cginc"

		half _MinimumNdotL;

		half _AlphaCutSmooth;
		//light model
		half4 LightingWrapped(SurfaceOutput s, half3 lightDir, half3 viewDir, half atten)
		{
			return LightingWrappedCalculation(s.Normal, s.Alpha, lightDir, _LightColor0, _MinimumNdotL, atten);
		}

		struct appdata
		{
			float4 vertex : POSITION;
			float3 normal : NORMAL;
			float4 color : COLOR;
			fixed4 custom : TEXCOORD0;
		};

		struct Input
		{
			float4 vertexColor : COLOR;
			fixed4 custom;
		};


		void vert(inout appdata v, out Input o) {
			UNITY_INITIALIZE_OUTPUT(Input, o);
			o.custom = v.custom;
		}

		void surf(Input IN, inout SurfaceOutput o)
		{
			fixed4 c = IN.vertexColor;
			o.Albedo = c.rgb;

			half cutoutAlpha = IN.custom.x;

			//noise
			o.Alpha = IN.vertexColor.a;
		}
		ENDCG
	}
		FallBack "Diffuse"
}

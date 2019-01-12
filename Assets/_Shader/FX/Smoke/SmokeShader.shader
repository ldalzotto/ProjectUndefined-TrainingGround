Shader "Custom/SmokeShader"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Main Texture", 2D) = "white" {}
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0

		_MinimumNdotL("Minimum absolute NdotL", Range(0,1)) = 0.0

		_RimIntensity("Rim Intensity", float) = 1.0

		_AlphaCutout("Alpha Cutout", Range(0.0,1.0)) = 0.0
			_AlphaCutSmooth("Alpha Cut Smooth", Range(0.0,1.0)) = 0.05

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
			#include "noiseSimplex.cginc"

			sampler2D _MainTex;

			half _MinimumNdotL;

			//rim effect
			half _RimIntensity;

			UNITY_INSTANCING_BUFFER_START(Props)
				fixed4 _Color;
				half _AlphaCutout;
			UNITY_INSTANCING_BUFFER_END(Props)

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
				fixed4 randomNbparticle : TEXCOORD1;
			};

			struct Input
			{
				float2 uv_MainTex;
				float3 viewDir;
				float3 worldNormal;
				float3 worldPos;
				float4 vertexColor : COLOR;
				fixed samplexNoiseValue;
				fixed4 custom;
			};


			void vert(inout appdata v, out Input o) {
				UNITY_INITIALIZE_OUTPUT(Input, o);
				float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
				fixed3 randomNbparticleVector = fixed3(v.randomNbparticle.x, v.randomNbparticle.x, v.randomNbparticle.x);
				o.samplexNoiseValue = (snoise(worldPos * randomNbparticleVector / 2) + 1)*0.5;
				o.custom = v.custom;

				v.vertex.xyz += (o.samplexNoiseValue / 2.3);
			}

			void surf(Input IN, inout SurfaceOutput o)
			{
				fixed4 c = IN.vertexColor;
				o.Albedo = c.rgb;


				//rim effect
				half nv = saturate(dot(IN.worldNormal, IN.viewDir));
				nv = pow(nv, _RimIntensity);

				half cutoutAlpha = IN.custom.x;
				half maskA = smoothstep(cutoutAlpha - _AlphaCutSmooth, cutoutAlpha + _AlphaCutSmooth, IN.samplexNoiseValue);

				//noise
				o.Alpha = nv * maskA;
			}
			ENDCG
		}
			FallBack "Diffuse"
}

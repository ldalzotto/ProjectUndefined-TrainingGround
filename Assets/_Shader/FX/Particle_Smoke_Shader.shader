Shader "Custom/SmokeShader"
{
	Properties
	{
		_MainTex("Main Texture", 2D) = "white" {}
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0

		_AlphaCutSmooth("Alpha Cut Smooth", Range(0.0,1.0)) = 0.05

		_DeformationFactor("Deformation Factor", Range(0.0,5.0)) = 1.0

	}
		SubShader
		{
			Tags { "RenderType" = "Transparent" }

			CGPROGRAM
			// Physically based Standard lighting model, and enable shadows on all light types
			#pragma surface surf Standard vertex:vert fullforwardshadows alpha:blend

			// Use shader model 3.0 target, to get nicer looking lighting
			#pragma target 3.0

			#include "wrappedLight.cginc"
			#include "noiseSimplex.cginc"

			sampler2D _MainTex;

		half _Glossiness;
		half _Metallic;
			half _DeformationFactor;
			half _AlphaCutSmooth;

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
				float4 color : COLOR;
				fixed samplexNoiseValue;
				fixed4 custom;
			};


			void vert(inout appdata v, out Input o) {
				UNITY_INITIALIZE_OUTPUT(Input, o);
				float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
				fixed3 randomNbparticleVector = fixed3(v.randomNbparticle.x, v.randomNbparticle.x, v.randomNbparticle.x) ;
				o.samplexNoiseValue = (snoise(worldPos * randomNbparticleVector / 2) + 1)*0.5;
				o.custom = v.custom;
				o.color = v.color;

				v.vertex.xyz += (o.samplexNoiseValue   * _DeformationFactor) ;
			}

			void surf(Input IN, inout SurfaceOutputStandard o)
			{
				fixed4 c = IN.color;
				o.Albedo = c.rgb; 
				o.Metallic = _Metallic;
				o.Smoothness = _Glossiness;
				//noise
				//rim effect
				half cutoutAlpha = IN.custom.x;
				half maskA = smoothstep(cutoutAlpha - _AlphaCutSmooth, cutoutAlpha + _AlphaCutSmooth, IN.samplexNoiseValue);
				half nv = saturate(dot(IN.worldNormal, IN.viewDir));
				o.Alpha = nv * maskA * IN.color.a;
			}
			ENDCG
		}
			FallBack "Diffuse"
}

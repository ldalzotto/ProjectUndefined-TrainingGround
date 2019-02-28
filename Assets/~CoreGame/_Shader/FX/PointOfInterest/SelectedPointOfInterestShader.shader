Shader "Custom/FX/PointOfInterest/SelectedPointOfInterestShader"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
		_RimMaxInfluenceFactor("Rim Max Influence Factor", Float) = 1.0
		_RimInfluenceSpeed("Rim Influence Speed", Float) = 1.0
			_RimColor("Rim Color", Color) = (1,1,1,1)
	}
		SubShader
		{
			Tags { "RenderType" = "Opaque" }
			LOD 200

			CGPROGRAM
			// Physically based Standard lighting model, and enable shadows on all light types
			#pragma surface surf Standard fullforwardshadows

			// Use shader model 3.0 target, to get nicer looking lighting
			#pragma target 3.0

			sampler2D _MainTex;

			struct Input
			{
				float2 uv_MainTex;
				float3 viewDir;
				float3 worldNormal;
			};

			half _Glossiness;
			half _Metallic;
			fixed4 _Color;
			half _RimMaxInfluenceFactor;
			half _RimInfluenceSpeed;
			half4 _RimColor;


			void surf(Input IN, inout SurfaceOutputStandard o)
			{
				// Albedo comes from a texture tinted by color
				fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
				half rim = 1 - saturate(dot(IN.viewDir , IN.worldNormal));

				rim *= abs(sin(_Time * _RimInfluenceSpeed)*_RimMaxInfluenceFactor);

				o.Albedo = c.rgb + (rim * _RimColor);
				// Metallic and smoothness come from slider variables
				o.Metallic = _Metallic;
				o.Smoothness = _Glossiness;
				o.Alpha = c.a;
			}
			ENDCG
		}
			FallBack "Diffuse"
}

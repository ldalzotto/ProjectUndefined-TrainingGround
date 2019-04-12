Shader "Custom/VertexSurface"
{
	Properties
	{
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
		[Toggle(VERTEX_OCCLUSION_MAP)] _OcclusionEnabled("Occlusion enabled ?", Int) = 0
		_Occlusion("Occlusion map", 2D) = "black" {}
	}
		SubShader
	{
		Tags { "RenderType" = "Opaque" }

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows
		#pragma shader_feature VERTEX_OCCLUSION_MAP
		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input
		{
			float2 uv_MainTex;
			float4 color: COLOR;
		};

		half _Glossiness;
		half _Metallic;
		sampler2D _Occlusion;

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			// Albedo comes from a texture tinted by color
			o.Albedo = IN.color.rgb;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
#if VERTEX_OCCLUSION_MAP
			o.Occlusion = tex2D(_Occlusion, IN.uv_MainTex).r;
#endif
			o.Alpha = IN.color.a;
		}
		ENDCG
	}
		FallBack "Diffuse"
}

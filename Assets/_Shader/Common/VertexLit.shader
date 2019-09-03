Shader "Lit/VertexLit"
{
    Properties
    {
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
		[Toggle(WITH_EMISSION)] _WithEmission("Emission?", Float) = 0.0
		[ShowOnKeywordDrawer(WITH_EMISSION)]_EmissionIntensity("Emission Intensity", Float) = 0.0
		[Toggle(BIPLANAR)] _WithBiPlanar("Biplanar?", Float) = 0.0
		[ShowOnKeywordDrawer(BIPLANAR)] _BiPlanarNormaltexture("_BiPlanarNormaltexture", 2D) = "bump" {}
		[ShowOnKeywordDrawer(BIPLANAR)] _BiPlanarNormaltextureTiling("_BiPlanarNormaltextureTiling", Vector) = (0,0,0,0)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows
		#pragma shader_feature WITH_EMISSION
		#pragma shader_feature BIPLANAR

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        struct Input
        {
			float4 vertexColor : COLOR;
#if BIPLANAR
			float3 worldPos;
#endif
        };

	    half _Glossiness;
		half _Metallic;

#if WITH_EMISSION
		float _EmissionIntensity;
#endif
#if BIPLANAR
		sampler2D _BiPlanarNormaltexture;
		float2 _BiPlanarNormaltextureTiling;
#endif

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
			fixed4 c = IN.vertexColor;

            o.Albedo = c.rgb;

            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
#if BIPLANAR
			o.Normal = UnpackNormal(tex2D(_BiPlanarNormaltexture, float2(IN.worldPos.x * _BiPlanarNormaltextureTiling.x, IN.worldPos.z * _BiPlanarNormaltextureTiling.y)));
#endif
			o.Emission =
#if WITH_EMISSION
				o.Albedo * _EmissionIntensity
#else
				0
#endif
			;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
	CustomEditor "CustomVertexLitGUI"
}

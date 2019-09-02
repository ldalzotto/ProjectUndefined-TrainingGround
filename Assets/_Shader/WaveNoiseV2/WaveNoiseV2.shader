// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/WaveNoiseV2"
{
    Properties
    {
		_DisplacementFactorMap("Displacement factor map", 2D) = "white" {}
		_WorldSpaceDirection("World space direction", Vector) = (1,1,1,1)
		_MaxIntensity("Max Intensity", Float) = 1.0
		_NoiseSpeed("Noise Speed", Float) = 1.0
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows vertex:vert
        #pragma target 3.0
		#include "Assets/_Shader/Noise/noiseSimplex.cginc"

        struct Input
        {
			float4 vertexColor : COLOR;
        };

		sampler2D _DisplacementFactorMap;
		float4 _DisplacementFactorMap_ST;
		float4 _WorldSpaceDirection;
		float _MaxIntensity;
		float _NoiseSpeed;

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

		void vert(inout appdata_full v) {
			float3 worldPosition = mul(unity_ObjectToWorld, v.vertex);
			float noiseIntensity = snoise(worldPosition + (_Time *_NoiseSpeed));
			v.vertex.xyz += (tex2Dlod(_DisplacementFactorMap, float4(v.texcoord.xy, 0, 0)).x * mul(unity_WorldToObject, _WorldSpaceDirection) * _MaxIntensity * noiseIntensity);
		}

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            o.Albedo = IN.vertexColor;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = IN.vertexColor.w;
        }
        ENDCG
    }
    FallBack "Diffuse"
}

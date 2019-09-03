// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/WaveNoiseV2"
{
	Properties
	{
		_MaxIntensity("Max Intensity", Float) = 1.0
		_MinIntensity("Min Intensity", Float) = -1.0
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
		[Toggle(IS_NOISE)] _IsNoise("Is Noise?", Float) = 1.0
		[ShowOnKeywordDrawer(IS_NOISE)] _DisplacementFactorMap("Displacement factor map", 2D) = "white" {}
		[ShowOnKeywordDrawer(IS_NOISE)] _WorldSpaceDirection("World space direction", Vector) = (1,1,1,1)
		[ShowOnKeywordDrawer(IS_NOISE)] _NoiseSpeed("Noise Speed", Float) = 1.0
		[ShowOnKeywordDrawer(IS_NOISE)] _NoiseFrequency("Noise frequency", Float) = 1.0

		[Toggle(IS_WAVE)] _IsWave("Is Wave?", Float) = 0.0
		[ShowOnKeywordDrawer(IS_WAVE)] _WaveMap("(R) Amplitude texture, (G) Speed texture, (B) Frequency texture", 2D) = "white" {}
		[ShowOnKeywordDrawer(IS_WAVE)] _MaxSpeed("Max speed", Float) = 1.0
		[ShowOnKeywordDrawer(IS_WAVE)] _MaxFrequency("Max frequency", Float) = 1.0

		[Toggle(DIRECTION_TEXTURE)] _IsDirectionTexture("Direction texture?", Float) = 0.0
		[ShowOnKeywordDrawer(DIRECTION_TEXTURE)] _DirectionTexture("Direction texture", 2D) = "grey" {}

		[Toggle(FLAT_SHADE)] _IsFlatShade("Flat shade?", Float) = 0.0
	}
		SubShader
		{
			Tags { "RenderType" = "Opaque" }

			CGPROGRAM
			#pragma surface WaveNoiseSurf Standard addshadow vertex:WaveNoiseVert
			#pragma target 3.0
			#pragma shader_feature IS_NOISE
			#pragma shader_feature IS_WAVE
			#pragma shader_feature DIRECTION_TEXTURE
			#pragma shader_feature FLAT_SHADE

			#include "Assets/_Shader/WaveNoiseV2/WaveNoiseV2Include.cginc"
			ENDCG
		}
		FallBack "Diffuse"
}

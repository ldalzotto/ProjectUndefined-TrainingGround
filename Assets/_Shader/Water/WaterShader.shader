// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "Custom/Environment/Water"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo", 2D) = "white" {}

		_Cutoff("Alpha Cutoff", Range(0.0, 1.0)) = 0.5

		_Glossiness("Smoothness", Range(0.0, 1.0)) = 0.5
		_GlossMapScale("Smoothness Factor", Range(0.0, 1.0)) = 1.0
		[Enum(Specular Alpha,0,Albedo Alpha,1)] _SmoothnessTextureChannel("Smoothness texture channel", Float) = 0

		_SpecColor("Specular", Color) = (0.2,0.2,0.2)
		_SpecGlossMap("Specular", 2D) = "white" {}
		[ToggleOff] _SpecularHighlights("Specular Highlights", Float) = 1.0
		[ToggleOff] _GlossyReflections("Glossy Reflections", Float) = 1.0

		_BumpScale("Scale", Float) = 1.0
		_BumpMap("Normal Map", 2D) = "bump" {}

		_Parallax("Height Scale", Range(0.005, 0.08)) = 0.02
		_ParallaxMap("Height Map", 2D) = "black" {}

		_OcclusionStrength("Strength", Range(0.0, 1.0)) = 1.0
		_OcclusionMap("Occlusion", 2D) = "white" {}

		_EmissionColor("Color", Color) = (0,0,0)
		_EmissionMap("Emission", 2D) = "white" {}

		_DetailMask("Detail Mask", 2D) = "white" {}

		_DetailAlbedoMap("Detail Albedo x2", 2D) = "grey" {}
		_DetailNormalMapScale("Scale", Float) = 1.0
		_DetailNormalMap("Normal Map", 2D) = "bump" {}

		[Enum(UV0,0,UV1,1)] _UVSec("UV Set for secondary textures", Float) = 0


			// Blending state
			[HideInInspector] _Mode("__mode", Float) = 0.0
			[HideInInspector] _SrcBlend("__src", Float) = 1.0
			[HideInInspector] _DstBlend("__dst", Float) = 0.0
			[HideInInspector] _ZWrite("__zw", Float) = 1.0

		_FlatWireFrameInfluence("Triangulation mess", Range(0,4)) = 0

		_DepthColor("Water depth color", Color) = (0,0,0,0)
		_MinDepthDelta("Min depth delta", Range(0,0.1)) = 0
		_MaxDepthDelta("Max depth delta", Range(0,0.1)) = 0

		_foamScale("Foam Scale", Float) = 10
			_FoamColor("Foam Color", Color) = (1,1,1,1)
		_MinFoamDepthDelta("Min depth foam delta", Range(0,0.1)) = 0
		_MaxFoamDepthDelta("Max depth foam delta", Range(0,0.1)) = 0
		_WaterTrailTexture("Water trail texture", 2D) = "black" {}
		_FoamTexture("Foam texture", 2D) = "white" {}
	}

		CGINCLUDE
#define UNITY_SETUP_BRDF_INPUT SpecularSetup
#define UNITY_STANDARD_SIMPLE 0
		//#define _WAVE_DEPTH 1
		ENDCG

		SubShader
	{
		Tags { "RenderType" = "Opaque"  "PerformanceChecks" = "False" }


		// ------------------------------------------------------------------
		//  Base forward pass (directional light, emission, lightmaps, ...)
		Pass
		{
			Name "FORWARD"
			Tags { "LightMode" = "ForwardBase" }

			Blend[_SrcBlend][_DstBlend]
			ZWrite[_ZWrite]

			CGPROGRAM
			#pragma target 4.0

		// -------------------------------------

		#pragma shader_feature _NORMALMAP
		#pragma shader_feature _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
		#pragma shader_feature _EMISSION
		#pragma shader_feature _SPECGLOSSMAP
		#pragma shader_feature ___ _DETAIL_MULX2
		#pragma shader_feature _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
		#pragma shader_feature _ _SPECULARHIGHLIGHTS_OFF
		#pragma shader_feature _ _GLOSSYREFLECTIONS_OFF
		#pragma shader_feature _PARALLAXMAP
		#pragma shader_feature _ _WAVE_MOVEMENT
		#pragma shader_feature _ _FLAT_SHADING
		#pragma shader_feature _ _WAVE_DEPTH
		#pragma shader_feature _ _WAVE_FOAM

		#pragma multi_compile_fwdbase
		#pragma multi_compile_fog
		#pragma multi_compile_instancing
		// Uncomment the following line to enable dithering LOD crossfade. Note: there are more in the file to uncomment for other passes.
		//#pragma multi_compile _ LOD_FADE_CROSSFADE

		#pragma vertex vertBase
		#pragma geometry MyGeometryProgramBase
		#pragma fragment fragBase

		#include "WaterStandardCoreForward.cginc"
		#include "GeometryFlatShade.cginc"

		ENDCG
	}
		// ------------------------------------------------------------------
		//  Additive forward pass (one light per pass)
		Pass
		{
			Name "FORWARD_DELTA"
			Tags { "LightMode" = "ForwardAdd" }
			Blend[_SrcBlend] One
			Fog { Color(0,0,0,0) } // in additive pass fog should be black
			ZWrite Off
			ZTest LEqual

			CGPROGRAM
			#pragma target 4.0

		// -------------------------------------

		#pragma shader_feature _NORMALMAP
		#pragma shader_feature _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
		#pragma shader_feature _SPECGLOSSMAP
		#pragma shader_feature _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
		#pragma shader_feature _ _SPECULARHIGHLIGHTS_OFF
		#pragma shader_feature ___ _DETAIL_MULX2
		#pragma shader_feature _PARALLAXMAP
		#pragma shader_feature _ _WAVE_MOVEMENT
		#pragma shader_feature _FLAT_SHADING

		#pragma multi_compile_fwdadd_fullshadows
		#pragma multi_compile_fog
		// Uncomment the following line to enable dithering LOD crossfade. Note: there are more in the file to uncomment for other passes.
		//#pragma multi_compile _ LOD_FADE_CROSSFADE

		#pragma vertex vertAdd
		#pragma geometry MyGeometryProgramAdd
		#pragma fragment fragAdd
		#include "WaterStandardCoreForward.cginc"
		#include "GeometryFlatShade.cginc"

		ENDCG
	}

		// ------------------------------------------------------------------
		// Extracts information for lightmapping, GI (emission, albedo, ...)
		// This pass it not used during regular rendering.
		Pass
		{
			Name "META"
			Tags { "LightMode" = "Meta" }

			Cull Off

			CGPROGRAM
			#pragma vertex vert_meta
			#pragma fragment frag_meta

			#pragma shader_feature _EMISSION
			#pragma shader_feature _SPECGLOSSMAP
			#pragma shader_feature _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
			#pragma shader_feature ___ _DETAIL_MULX2
			#pragma shader_feature EDITOR_VISUALIZATION

			#include "UnityStandardMeta.cginc"
			ENDCG
		}
	}
		FallBack "VertexLit"
		CustomEditor "WaterShaderGUI"
}

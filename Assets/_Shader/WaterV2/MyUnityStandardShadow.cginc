// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

#ifndef MY_STANDARD_SHADOW_CORE_FORWARD_INCLUDED
#define MY_STANDARD_SHADOW_CORE_FORWARD_INCLUDED

#include "UnityStandardShadow.cginc"
#include "WaterWaveMovement.cginc"

void MyVertShadowCaster(VertexInput v
	, out float4 opos : SV_POSITION
#ifdef UNITY_STANDARD_USE_SHADOW_OUTPUT_STRUCT
	, out VertexOutputShadowCaster o
#endif
#ifdef UNITY_STANDARD_USE_STEREO_SHADOW_OUTPUT_STRUCT
	, out VertexOutputStereoShadowCaster os
#endif
)
{
	Displace(v);
	vertShadowCaster(v, opos
#ifdef UNITY_STANDARD_USE_SHADOW_OUTPUT_STRUCT
		, o
#endif
#ifdef UNITY_STANDARD_USE_STEREO_SHADOW_OUTPUT_STRUCT
		, os
#endif
	);
}
#endif // MY_STANDARD_SHADOW_CORE_FORWARD_INCLUDED

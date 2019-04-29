#ifndef WATER_FORWARD_PASS
#define WATER_FORWARD_PASS

#if defined(UNITY_NO_FULL_STANDARD_SHADER)
#   define UNITY_STANDARD_SIMPLE 1
#endif

#include "UnityStandardConfig.cginc"
#include "UnityStandardCore.cginc"
#include "WaterWaveMovement.cginc"
#include "FragmentFlatShade.cginc"

VertexOutputForwardBase waterVertBase (VertexInput v) {
	Displace(v);
	return vertForwardBase(v);
}
VertexOutputForwardAdd waterVertAdd (VertexInput v) {
	Displace(v);
	return vertForwardAdd(v); 
}

half4 waterFragBase (VertexOutputForwardBase i) : SV_Target { 
	FlatShadeFragBase(i);
	return fragForwardBaseInternal(i); 
}
half4 waterFragAdd (VertexOutputForwardAdd i) : SV_Target {
	FlatShadeFragAdd(i);
	return fragForwardAddInternal(i);
}

#endif // WATER_FORWARD_PASS

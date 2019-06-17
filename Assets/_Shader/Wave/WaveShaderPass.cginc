#ifndef WAVE_FORWARD_PASS
#define WAVE_FORWARD_PASS

#if defined(UNITY_NO_FULL_STANDARD_SHADER)
#   define UNITY_STANDARD_SIMPLE 1
#endif

#include "UnityStandardConfig.cginc"
#include "UnityStandardCore.cginc"
#include "Assets/_Shader/Common/FragmentFlatShade.cginc"
#include "Assets/_Shader/Wave/WaveMovement.cginc"

sampler2D _WaveDisplacementTexture;
sampler2D _WaveMap;

float _MaxAmplitude;
float _MaxSpeed;
float _MaxFrequency;


float3 Displace(VertexInput v) {
	WaveMovementDefinition waveDefinition;

	waveDefinition.waveDisplacementTexture = _WaveDisplacementTexture;
	waveDefinition.waveMap = _WaveMap;
	waveDefinition.maxAmplitude = _MaxAmplitude;
	waveDefinition.maxSpeed = _MaxSpeed;
	waveDefinition.maxFrequency = _MaxFrequency;
	return WaveMovement(v, waveDefinition);
}

VertexOutputForwardBase waveVertBase(VertexInput v) {
	v.vertex.xyz = Displace(v);
	return vertForwardBase(v);
}
VertexOutputForwardAdd waveVertAdd(VertexInput v) {
	v.vertex.xyz = Displace(v);
	return vertForwardAdd(v);
}

half4 waveFragBase(VertexOutputForwardBase i) : SV_Target{
	FlatShadeFragBase(i);
	return fragForwardBaseInternal(i);
}
half4 waveFragAdd(VertexOutputForwardAdd i) : SV_Target{
	FlatShadeFragAdd(i);
	return fragForwardAddInternal(i);
}

#endif // WAVE_FORWARD_PASS

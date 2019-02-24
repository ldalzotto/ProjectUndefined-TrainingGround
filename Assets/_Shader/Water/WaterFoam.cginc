#ifndef WATER_FOAM
#define WATER_FOAM

#include "DepthColoring.cginc"

float4 _FoamColor;
float _foamScale;
float _MinFoamDepthDelta;
float _MaxFoamDepthDelta;
sampler2D _FoamTexture;
float4 _FoamTexture_ST;
sampler2D _WaterTrailTexture;

float4 GetInterpolatedFoamWeight(VertexOutputForwardBase i) {
#if _WAVE_FOAM
	//float foamWeight = DepthDeltaSample01(i, _MinFoamDepthDelta, _MaxFoamDepthDelta) * _foamScale;
	// float foamWeight = i.foamWeight;
	float foamWeight = tex2D(_WaterTrailTexture, i.tex.xy).r;
	float4 foamSample = tex2D(_FoamTexture, i.tex.xy  * _FoamTexture_ST.xy + _FoamTexture_ST.zw);
	return float4(foamSample.xyz * _FoamColor.xyz, foamSample.r) * foamWeight * _foamScale;// float4(foamWeight, foamWeight, foamWeight, 0);
#else
	return float4(0, 0, 0, 0);
#endif
}

#endif // WATER_FOAM

#ifndef WAVE_DEPTH_COLORING
#define WAVE_DEPTH_COLORING

sampler2D _CameraDepthTexture;

float4 _DepthColor;
float _MinDepthDelta;
float _MaxDepthDelta;


float DepthDeltaSample(VertexOutputForwardBase i) {
	float depth = tex2Dlod(_CameraDepthTexture, UNITY_PROJ_COORD(i.screenUV)).r;
	return abs(depth - i.screenUV.z);
}

float DepthDeltaSample01(VertexOutputForwardBase i, float minDepthDelta, float maxDepthDelta) {
	float depthDelta = DepthDeltaSample(i);
	return (1 - smoothstep(minDepthDelta, maxDepthDelta, depthDelta));
}

half4 DepthColor(VertexOutputForwardBase i) {
#if _WAVE_DEPTH
	return _DepthColor * DepthDeltaSample01(i, _MinDepthDelta, _MaxDepthDelta);
#else
	return half4(0, 0, 0, 0);
#endif
}

#endif // WAVE_DEPTH_COLORING

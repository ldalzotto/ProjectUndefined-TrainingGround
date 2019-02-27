#ifndef WAVE_MOVEMENT
#define WAVE_MOVEMENT

sampler2D _WaveDisplacementTexture;
float3 _DisplacementFactor;

void Displace(inout VertexInput v) {
#if _WAVE_MOVEMENT
	float2 tex = TexCoords(v).xy;
	float4 displacementSample = tex2Dlod(_WaveDisplacementTexture, float4(tex.xy, 0, 0));

	displacementSample -= 0.5; // [-0.5,0.5] range
	displacementSample *= 2;//[-1,1] range

	float theta = dot(displacementSample.xy, v.vertex.xy);
	v.vertex.z += _DisplacementFactor.x * sin(theta * (2/ _DisplacementFactor.y) + (_Time.x* _DisplacementFactor.z / 2* _DisplacementFactor.y));
#endif
}

#endif // WAVE_MOVEMENT

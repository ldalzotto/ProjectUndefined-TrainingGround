#ifndef WAVE_MOVEMENT
#define WAVE_MOVEMENT

sampler2D _WaveDisplacementTexture, _WaterWaveMap;
float  _MaxAmplitude, _MaxSpeed, _MaxFrequency;

void Displace(inout VertexInput v) {
	float2 tex = TRANSFORM_TEX(v.uv0, _MainTex);
	float4 displacementSample = tex2Dlod(_WaveDisplacementTexture, float4(tex.xy, 0, 0));	
	displacementSample -= 0.5; // [-0.5,0.5] range
	displacementSample *= 2;//[-1,1] range

	float4 waterMapSample = tex2Dlod(_WaterWaveMap, float4(tex.xy, 0,0));

	float theta = dot(displacementSample.xyz, v.vertex.xyz);
	float amplitude = waterMapSample.r * _MaxAmplitude;
	float speed  = waterMapSample.g * _MaxSpeed;
	float frequency = waterMapSample.b * _MaxFrequency;

	v.vertex.xyz += v.normal *  amplitude * sin(theta * (2/ frequency) + (_Time.x* speed));
}



#endif // WAVE_MOVEMENT

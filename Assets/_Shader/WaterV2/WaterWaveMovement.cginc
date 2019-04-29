#ifndef WAVE_MOVEMENT
#define WAVE_MOVEMENT

sampler2D _WaveDisplacementTexture, _AmplitudeTexture, _SpeedTexture, _FrequencyTexture;
float  _MaxAmplitude, _MaxSpeed, _MaxFrequency;

void Displace(inout VertexInput v) {
	float2 tex = TRANSFORM_TEX(v.uv0, _MainTex);
	float4 displacementSample = tex2Dlod(_WaveDisplacementTexture, float4(tex.xy, 0, 0));	
	displacementSample -= 0.5; // [-0.5,0.5] range
	displacementSample *= 2;//[-1,1] range

	float4 amplitudeSample = tex2Dlod(_AmplitudeTexture, float4(tex.xy, 0,0));
	float4 speedSample = tex2Dlod(_SpeedTexture, float4(tex.xy, 0, 0));
	float4 frequencySample = tex2Dlod(_FrequencyTexture, float4(tex.xy,0,0));


	float theta = dot(displacementSample.xyz, v.vertex.xyz);
	float amplitude = amplitudeSample.r * _MaxAmplitude;
	float speed  = speedSample.r * _MaxSpeed;
	float frequency = frequencySample.r * _MaxFrequency;

	v.vertex.xyz += v.normal *  amplitude * sin(theta * (2/ frequency) + (_Time.x* speed));
}



#endif // WAVE_MOVEMENT

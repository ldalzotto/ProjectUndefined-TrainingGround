// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

#ifndef WAVE_MOVEMENT
#define WAVE_MOVEMENT

struct WaveMovementDefinition {
	sampler2D waveDisplacementTexture;
	sampler2D waveMap;
	float maxAmplitude;
	float maxSpeed;
	float maxFrequency;
};

#if PARTICLE_DEFORMATION
struct ParticleDeformationBufferData {
	float3 WolrdPosition;
	float DeformationRadius;
	float DeformationStrength;
};

StructuredBuffer<ParticleDeformationBufferData> _ParticleDeformationBuffer;
float _ParticleDeformationBufferCount;

float3 ApplyParticleDeformation(float4 vertex, float amplitude) {
	float3 delta = float3(0,0,0);
	float4 worldPos = mul(unity_ObjectToWorld, vertex);
	for (int index = 0; index < _ParticleDeformationBufferCount; index++) {

		float dist = distance(worldPos, _ParticleDeformationBuffer[index].WolrdPosition);
		float3 deformationDirection = normalize(_ParticleDeformationBuffer[index].WolrdPosition - worldPos);
		delta += (deformationDirection * smoothstep(0, _ParticleDeformationBuffer[index].DeformationRadius, _ParticleDeformationBuffer[index].DeformationRadius - dist) * _ParticleDeformationBuffer[index].DeformationStrength);
	}
	return delta;
}

#endif

//v.vertex.xyz, v.normal, v.uv0
float3 WaveMovement(VertexInput v, WaveMovementDefinition waveDefinition) {
	float2 tex = TRANSFORM_TEX(v.uv0, _MainTex);
	float4 displacementSample = tex2Dlod(waveDefinition.waveDisplacementTexture, float4(tex.xy, 0, 0));
	displacementSample -= 0.5; // [-0.5,0.5] range
	displacementSample *= 2;//[-1,1] range

	float4 waveMapSample = tex2Dlod(waveDefinition.waveMap, float4(tex.xy, 0, 0));

	float theta = dot(displacementSample.xyz, v.vertex.xyz);
	float amplitude = waveMapSample.r * waveDefinition.maxAmplitude;
	float speed = waveMapSample.g * waveDefinition.maxSpeed;
	float frequency = waveMapSample.b * waveDefinition.maxFrequency;

	float sinusWave = sin(theta * (2 / frequency) + (_Time.x* speed));

	float3 finalLocalPosition = float3(0, 0, 0);
#if PARTICLE_DEFORMATION
	finalLocalPosition += ApplyParticleDeformation(v.vertex, amplitude);
#endif
	finalLocalPosition += v.vertex.xyz + (v.normal *  amplitude * sinusWave);

	return finalLocalPosition;
}

#endif // WAVE_MOVEMENT



#include "Assets/_Shader/Noise/noiseSimplex.cginc"

#if IS_WAVE
#include "Assets/_Shader/Wave/WaveMovementV2.cginc"
#endif

struct Input
{
	float4 vertexColor : COLOR;
#if FLAT_SHADE
	float3 worldPos;
#endif
};

float _MaxIntensity;
float _MinIntensity;

#if IS_NOISE
sampler2D _DisplacementFactorMap;
float4 _DisplacementFactorMap_ST;
float _NoiseSpeed;
float _NoiseFrequency;
#endif

#if IS_WAVE
sampler2D _WaveMap;
float _MaxSpeed;
float _MaxFrequency;
#endif

#if DIRECTION_TEXTURE
sampler2D _DirectionTexture;
#else
float4 _WorldSpaceDirection;
#endif


half _Glossiness;
half _Metallic;
fixed4 _Color;

void WaveNoiseVert(inout appdata_full v) {
#if IS_NOISE
	float3 worldPosition = mul(unity_ObjectToWorld, v.vertex);
	float noiseIntensity = snoise(worldPosition * (1 / _NoiseFrequency) + (_Time.x *_NoiseSpeed));
	noiseIntensity = _MinIntensity + ((abs(_MinIntensity) + abs(_MaxIntensity)) * ((noiseIntensity + 1) / 2));
#if DIRECTION_TEXTURE
	float4 displacementSample = tex2Dlod(_DirectionTexture, float4(v.texcoord.xy, 0, 0));
	displacementSample -= 0.5; // [-0.5,0.5] range
	displacementSample *= 2;//[-1,1] range
	float3 localDirection = normalize(displacementSample * v.normal);
#else
	float3 localDirection = normalize(mul(unity_WorldToObject, _WorldSpaceDirection));
#endif
	v.vertex.xyz += (tex2Dlod(_DisplacementFactorMap, float4(v.texcoord.xy, 0, 0)).x * localDirection * _MaxIntensity * noiseIntensity);
#endif

#if IS_WAVE
	WaveMovementDefinition waveDefinition;
#if DIRECTION_TEXTURE
	waveDefinition.waveDisplacementTexture = _DirectionTexture;
#endif
	waveDefinition.waveMap = _WaveMap;
	waveDefinition.maxAmplitude = _MaxIntensity;
	waveDefinition.maxSpeed = _MaxSpeed;
	waveDefinition.maxFrequency = _MaxFrequency;
	v.vertex.xyz = WaveMovement(v.texcoord.xy, v.vertex.xyz, v.normal.xyz, waveDefinition);
#endif
}

#if FLAT_SHADE
float3 FlatShadeNormal(float3 worldPos) {
	float3 dpdx = ddx(worldPos);
	float3 dpdy = ddy(worldPos);
	return normalize(cross(dpdy, dpdx));
}
#endif
void WaveNoiseSurf(Input IN, inout SurfaceOutputStandard o)
{
	o.Albedo = IN.vertexColor;
	o.Metallic = _Metallic;
	o.Smoothness = _Glossiness;
	o.Alpha = IN.vertexColor.w;
#if FLAT_SHADE
	o.Normal = FlatShadeNormal(IN.worldPos.xyz);
#endif
}
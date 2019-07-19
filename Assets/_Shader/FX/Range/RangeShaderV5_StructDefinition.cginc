#ifndef RANGE_STRUCT_DEFINITION
#define RANGE_STRUCT_DEFINITION

struct CircleRangeBufferData {
	float3 CenterWorldPosition;
	float3 WorldRangeForward;
	float Radius;
	float4 AuraColor;
	float AuraTextureAlbedoBoost;
	float AuraAnimationSpeed;
	int OccludedByFrustums;
};

struct RangeExecutionOrderBufferData
{
	int IsSphere;
	int IsCube;
	int IsFrustum;
	int Index;
};

struct BoxRangeBufferData
{
	float3 Forward;
	float3 Up;
	float3 Right;
	float3 Center;
	float3 LocalSize;
	float4 AuraColor;
	float AuraTextureAlbedoBoost;
	float AuraAnimationSpeed;
};

struct FrustumRangeBufferData
{
	float3 FC1;
	float3 FC2;
	float3 FC3;
	float3 FC4;
	float3 FC5;
	float3 FC6;
	float3 FC7;
	float3 FC8;

	int OccludedByFrustums;
	float4 AuraColor;
	float AuraTextureAlbedoBoost;
	float AuraAnimationSpeed;
};

struct FrustumBufferData
{
	float3 FC1;
	float3 FC2;
	float3 FC3;
	float3 FC4;
	float3 FC5;
	float3 FC6;
	float3 FC7;
	float3 FC8;
};

struct RangeToFrustumBufferLink {
	int RangeIndex;
	int FrustumIndex;
};
#endif
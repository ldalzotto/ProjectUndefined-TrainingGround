#ifndef FRAGMENT_FLAT_SHADER
#define FRAGMENT_FLAT_SHADER

void SetNormalWorld(inout VertexOutputForwardBase i, float3 normalWorld) {
#ifdef _TANGENT_TO_WORLD
	float4 tangentWorld = float4(UnityObjectToWorldDir(i.tangent.xyz), i.tangent.w);

	float3x3 tangentToW
	orld = CreateTangentToWorldPerVertex(normalWorld, tangentWorld.xyz, tangentWorld.w);
	i.tangentToWorldAndPackedData[0].xyz = tangentToWorld[0];
	i.tangentToWorldAndPackedData[1].xyz = tangentToWorld[1];
	i.tangentToWorldAndPackedData[2].xyz = tangentToWorld[2];
#else
	i.tangentToWorldAndPackedData[0].xyz = 0;
	i.tangentToWorldAndPackedData[1].xyz = 0;
	i.tangentToWorldAndPackedData[2].xyz = normalWorld;
#endif
}

void SetNormalWorld(inout VertexOutputForwardAdd i, float3 normalWorld) {
#ifdef _TANGENT_TO_WORLD
	float4 tangentWorld = float4(UnityObjectToWorldDir(i.tangent.xyz), i.tangent.w);

	float3x3 tangentToWorld = CreateTangentToWorldPerVertex(normalWorld, tangentWorld.xyz, tangentWorld.w);
	i.tangentToWorldAndLightDir[0].xyz = tangentToWorld[0];
	i.tangentToWorldAndLightDir[1].xyz = tangentToWorld[1];
	i.tangentToWorldAndLightDir[2].xyz = tangentToWorld[2];
#else
	i.tangentToWorldAndLightDir[0].xyz = 0;
	i.tangentToWorldAndLightDir[1].xyz = 0;
	i.tangentToWorldAndLightDir[2].xyz = normalWorld;
#endif
}

float rand(float3 myVector, float maxValue) {
	return (frac(sin(dot(myVector, float3(12.9898, 78.233, 45.5432))) * 43758.5453)) / maxValue;
}

void FlatShadeFragBase(inout VertexOutputForwardBase i){
	float3 worldPos = IN_WORLDPOS(i);
	float3 dpdx = ddx(worldPos);
	float3 dpdy = ddy(worldPos);
	SetNormalWorld(i,  normalize(cross(dpdy, dpdx)));
}

void FlatShadeFragAdd(inout VertexOutputForwardAdd i) {
	float3 worldPos = IN_WORLDPOS_FWDADD(i);
	float3 dpdx = ddx(worldPos);
	float3 dpdy = ddy(worldPos);
	SetNormalWorld(i,  normalize(cross(dpdy, dpdx)));
}

#endif // FRAGMENT_FLAT_SHADER

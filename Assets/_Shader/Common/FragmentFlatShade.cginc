#ifndef FRAGMENT_FLAT_SHADE
#define FRAGMENT_FLAT_SHADE

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

float3 FlatShadeNormal(float3 worldPos) {
	float3 dpdx = ddx(worldPos);
	float3 dpdy = ddy(worldPos);
	return normalize(cross(dpdy, dpdx));
}

void FlatShadeFragBase(inout VertexOutputForwardBase i) {
	float3 worldPos = IN_WORLDPOS(i);
	SetNormalWorld(i, FlatShadeNormal(worldPos));
}

void FlatShadeFragAdd(inout VertexOutputForwardAdd i) {
	float3 worldPos = IN_WORLDPOS_FWDADD(i);
	SetNormalWorld(i, FlatShadeNormal(worldPos));
}


#endif // FRAGMENT_FLAT_SHADE

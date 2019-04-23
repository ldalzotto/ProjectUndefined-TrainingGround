
float _FlatWireFrameInfluence;

void SetNormalWorld(inout VertexOutputForwardBase i, float3 normalWorld) {
#ifdef _TANGENT_TO_WORLD
	float4 tangentWorld = float4(UnityObjectToWorldDir(i.tangent.xyz), i.tangent.w);

	float3x3 tangentToWorld = CreateTangentToWorldPerVertex(normalWorld, tangentWorld.xyz, tangentWorld.w);
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


[maxvertexcount(3)]
void MyGeometryProgramBase(
	triangle VertexOutputForwardBase i[3],
	inout TriangleStream<VertexOutputForwardBase> stream
) {

	float3 p0 = IN_WORLDPOS(i[0]);
	float3 p1 = IN_WORLDPOS(i[1]);
	float3 p2 = IN_WORLDPOS(i[2]);

	float3 triangleNormal = normalize(cross(p1 - p0, p2 - p0));

	SetNormalWorld(i[0], triangleNormal);
	SetNormalWorld(i[1], triangleNormal);
	SetNormalWorld(i[2], triangleNormal);

	stream.Append(i[0]);
	stream.Append(i[1]);
	stream.Append(i[2]);

}

[maxvertexcount(3)]
void MyGeometryProgramAdd(
	triangle VertexOutputForwardAdd i[3],
	inout TriangleStream<VertexOutputForwardAdd> stream
) {

	float3 p0 = IN_WORLDPOS_FWDADD(i[0]);
	float3 p1 = IN_WORLDPOS_FWDADD(i[1]);
	float3 p2 = IN_WORLDPOS_FWDADD(i[2]);

	float3 triangleNormal = normalize(cross(p1 - p0, p2 - p0));

	SetNormalWorld(i[0], triangleNormal);
	SetNormalWorld(i[1], triangleNormal);
	SetNormalWorld(i[2], triangleNormal);

	stream.Append(i[0]);
	stream.Append(i[1]);
	stream.Append(i[2]);

}

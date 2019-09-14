#ifndef RANGE_CALCULATIONS
#define RANGE_CALCULATIONS

uniform StructuredBuffer<CircleRangeBufferData> CircleRangeBuffer;
uniform StructuredBuffer<BoxRangeBufferData> BoxRangeBuffer;
uniform StructuredBuffer<FrustumRangeBufferData> FrustumRangeBuffer;
uniform StructuredBuffer<RoundedFrustumRangeBufferData> RoundedFrustumRangeBuffer;

uniform StructuredBuffer<FrustumBufferData> FrustumBufferDataBuffer;
int _FrustumBufferDataBufferCount;

int BoxDirectionIntersectsPoint(float3 WorldBoxDirection, float3 WorldBoxCenterPosition, float LocalDirectionSize, float3 pointWorldPosition) {
	float3 startPoint = WorldBoxCenterPosition - (WorldBoxDirection * (LocalDirectionSize*0.5));
	float3 endPoint = WorldBoxCenterPosition + (WorldBoxDirection * (LocalDirectionSize*0.5));
	return (dot(endPoint - startPoint, endPoint - startPoint) > dot(pointWorldPosition - startPoint, endPoint - startPoint)) && dot(pointWorldPosition - startPoint, endPoint - startPoint) > 0;
}

int BoxIntersectsPoint(BoxRangeBufferData boxRangeBufferData, float3 pointWorldPosition) {
	return (BoxDirectionIntersectsPoint(boxRangeBufferData.Forward, boxRangeBufferData.Center, boxRangeBufferData.LocalSize.z, pointWorldPosition)
		+ BoxDirectionIntersectsPoint(boxRangeBufferData.Up, boxRangeBufferData.Center, boxRangeBufferData.LocalSize.y, pointWorldPosition)
		+ BoxDirectionIntersectsPoint(boxRangeBufferData.Right, boxRangeBufferData.Center, boxRangeBufferData.LocalSize.x, pointWorldPosition))
		== 3;
}

/*
 *     C5----C6
 *    / |    /|
 *   C1----C2 |
 *   |  C8  | C7
 *   | /    |/     C3->C7  Forward
 *   C4----C3
 */

int PointInsideFrustumV2(float3 comparisonPoint, float3 FC1, float3 FC2, float3 FC3, float3 FC4, float3 FC5,
				float3 normal1, float3 normal2, float3 normal3, float3 normal4, float3 normal5, float3 normal6) {

	return ((dot(normal1, comparisonPoint - FC1) > 0) *
			(dot(normal2, comparisonPoint - FC1) > 0) * 
			(dot(normal3, comparisonPoint - FC2) > 0) * 
			(dot(normal4, comparisonPoint - FC3) > 0) *
			(dot(normal5, comparisonPoint - FC4) > 0) * 
			(dot(normal6, comparisonPoint - FC5) > 0) 
		);
}

int PointInsideFrustumV2(float3 comparisonPoint, FrustumRangeBufferData frustumRangeBufferData) {
	return PointInsideFrustumV2(comparisonPoint, frustumRangeBufferData.FC1, frustumRangeBufferData.FC2, frustumRangeBufferData.FC3, frustumRangeBufferData.FC4,
		frustumRangeBufferData.FC5, frustumRangeBufferData.normal1, frustumRangeBufferData.normal2, frustumRangeBufferData.normal3, frustumRangeBufferData.normal4, frustumRangeBufferData.normal5, frustumRangeBufferData.normal6);
}

int PointInsideFrustumV2(float3 comparisonPoint, RoundedFrustumRangeBufferData frustumBufferData) {
	return PointInsideFrustumV2(comparisonPoint, frustumBufferData.FC1, frustumBufferData.FC2, frustumBufferData.FC3, frustumBufferData.FC4,
		frustumBufferData.FC5, frustumBufferData.normal1, frustumBufferData.normal2, frustumBufferData.normal3, frustumBufferData.normal4, frustumBufferData.normal5, frustumBufferData.normal6);
}

int PointInsideFrustumV2(float3 comparisonPoint, FrustumBufferData frustumBufferData) {
	return PointInsideFrustumV2(comparisonPoint, frustumBufferData.FC1, frustumBufferData.FC2, frustumBufferData.FC3, frustumBufferData.FC4,
		frustumBufferData.FC5, frustumBufferData.normal1, frustumBufferData.normal2, frustumBufferData.normal3, frustumBufferData.normal4, frustumBufferData.normal5, frustumBufferData.normal6);
}

int PointIsOccludedByFrustumV2(float3 comparisonPoint) {
	int isInsideFrustum = 0;
	for (int index = 0; (index < _FrustumBufferDataBufferCount); index++) {
		if (!isInsideFrustum) {
			isInsideFrustum = PointInsideFrustumV2(comparisonPoint, FrustumBufferDataBuffer[index]);
		}

	}
	return isInsideFrustum;
}


#endif
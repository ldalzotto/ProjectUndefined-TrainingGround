#ifndef RANGE_CALCULATIONS
#define RANGE_CALCULATIONS

uniform StructuredBuffer<RangeExecutionOrderBufferData> RangeExecutionOrderBuffer;
uniform StructuredBuffer<CircleRangeBufferData> CircleRangeBuffer;
uniform StructuredBuffer<BoxRangeBufferData> BoxRangeBuffer;
uniform StructuredBuffer<FrustumRangeBufferData> FrustumRangeBuffer;

uniform StructuredBuffer<FrustumBufferData> FrustumBufferDataBuffer;

uniform StructuredBuffer<RangeToFrustumBufferLink> RangeToFrustumBufferLinkBuffer;
int _RangeToFrustumBufferLinkCount;



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

int PointInsideFrustumV2(float3 comparisonPoint, float3 FC1, float3 FC2, float3 FC3, float3 FC4, float3 FC5, float3 FC6, float3 FC7, float3 FC8) {

	float crossSign = sign(dot(FC5 - FC1, cross(FC2 - FC1, FC4 - FC1)));

	float3 normal1 = crossSign * cross(FC2 - FC1, FC3 - FC1);
	float3 normal2 = crossSign * cross(FC5 - FC1, FC2 - FC1);
	float3 normal3 = crossSign * cross(FC6 - FC2, FC3 - FC2);
	float3 normal4 = crossSign * cross(FC7 - FC3, FC4 - FC3);
	float3 normal5 = crossSign * cross(FC8 - FC4, FC1 - FC4);
	float3 normal6 = crossSign * cross(FC8 - FC5, FC6 - FC5);

	return ((dot(normal1, comparisonPoint - FC1) >= 0) * (dot(normal1, FC5 - FC1) > 0) *
		(dot(normal2, comparisonPoint - FC1) >= 0) * (dot(normal2, FC4 - FC1) > 0) *
		(dot(normal3, comparisonPoint - FC2) >= 0) * (dot(normal3, FC1 - FC2) > 0) *
		(dot(normal4, comparisonPoint - FC3) >= 0) * (dot(normal4, FC2 - FC3) > 0) *
		(dot(normal5, comparisonPoint - FC4) >= 0) * (dot(normal5, FC3 - FC4) > 0) *
		(dot(normal6, comparisonPoint - FC5) >= 0) * (dot(normal6, FC1 - FC5) > 0)
		);
}

int PointInsideFrustumV2(float3 comparisonPoint, FrustumRangeBufferData frustumRangeBufferData) {
	return PointInsideFrustumV2(comparisonPoint, frustumRangeBufferData.FC1, frustumRangeBufferData.FC2, frustumRangeBufferData.FC3, frustumRangeBufferData.FC4,
		frustumRangeBufferData.FC5, frustumRangeBufferData.FC6, frustumRangeBufferData.FC7, frustumRangeBufferData.FC8);
}

int PointInsideFrustumV2(float3 comparisonPoint, FrustumBufferData frustumBufferData) {
	return PointInsideFrustumV2(comparisonPoint, frustumBufferData.FC1, frustumBufferData.FC2, frustumBufferData.FC3, frustumBufferData.FC4,
		frustumBufferData.FC5, frustumBufferData.FC6, frustumBufferData.FC7, frustumBufferData.FC8);
}

int PointIsOccludedByFrustum(float3 comparisonPoint, RangeExecutionOrderBufferData rangeExecutionOrderBufferData) {
	int isInsideFrustum = 0;
	for (int index = 0; (index < _RangeToFrustumBufferLinkCount) && (isInsideFrustum == 0); index++) {
		isInsideFrustum = (RangeToFrustumBufferLinkBuffer[index].RangeIndex == rangeExecutionOrderBufferData.Index) * (RangeToFrustumBufferLinkBuffer[index].RangeType == rangeExecutionOrderBufferData.RangeType)
			* PointInsideFrustumV2(comparisonPoint, FrustumBufferDataBuffer[RangeToFrustumBufferLinkBuffer[index].FrustumIndex]);
	}
	return isInsideFrustum;
}

int PointInsideAngleRestrictionV2(float3 rangeCenterPoint, float3 worldRangeForward, float3 comparisonPoint, float maxAngleLimitationRad) {
	float3 from = normalize(worldRangeForward);
	float3 to = normalize(comparisonPoint - rangeCenterPoint);
	float angleValue = (2 - (dot(from, to) + 1)) * 0.5 * 3.141592;
	return angleValue <= maxAngleLimitationRad;
}
#endif
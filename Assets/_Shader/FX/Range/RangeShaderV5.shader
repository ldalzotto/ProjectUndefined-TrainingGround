Shader "Custom/FX/Range/RangeShaderV5_Pass"
{
	Properties
	{
		_AuraTexture("Aura Texture", 2D) = "white" {}
		_CountSize("Count Size", int) = 0
		_FrustumBufferDataBufferCount("_FrustumBufferDataBufferCount", Int) = 0
		_RangeToFrustumBufferLinkCount("_RangeToFrustumBufferLinkCount", Int) = 0
		_AlbedoBoost("Albedo Boost", Float) = 1.0
	}
		SubShader
	{

		Tags{ "RenderType" = "Transparent" }
		Pass
		{
			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha 
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag


			#include "UnityCG.cginc"
			#include "RangeShaderV5_StructDefinition.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float4 worldPos: TEXCOORD1;
			};

			uniform StructuredBuffer<RangeExecutionOrderBufferData> RangeExecutionOrderBuffer;
			int _CountSize;

			uniform StructuredBuffer<CircleRangeBufferData> CircleRangeBuffer;
			uniform StructuredBuffer<BoxRangeBufferData> BoxRangeBuffer;
			uniform StructuredBuffer<FrustumRangeBufferData> FrustumRangeBuffer;
			
			uniform StructuredBuffer<FrustumBufferData> FrustumBufferDataBuffer;
			int _FrustumBufferDataBufferCount;

			uniform StructuredBuffer<RangeToFrustumBufferLink> RangeToFrustumBufferLinkBuffer;
			int _RangeToFrustumBufferLinkCount;

			sampler2D _AuraTexture;
			float4 _AuraTexture_ST;
			float _AlbedoBoost;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
				return o;
			}

			int BoxDirectionIntersectsPoint(float3 WorldBoxDirection, float3 WorldBoxCenterPosition, float LocalDirectionSize, float3 pointWorldPosition) {
				float3 startPoint = WorldBoxCenterPosition - (WorldBoxDirection * (LocalDirectionSize*0.5));
				float3 endPoint = WorldBoxCenterPosition + (WorldBoxDirection * (LocalDirectionSize*0.5));
				if (dot(endPoint - startPoint, endPoint - startPoint) > dot(pointWorldPosition - startPoint, endPoint - startPoint) && dot(pointWorldPosition - startPoint, endPoint - startPoint) > 0) {
					return 1;
				}
				return 0;
			}
			
			int BoxIntersectsPoint(BoxRangeBufferData boxRangeBufferData, float3 pointWorldPosition) {
		
				if (BoxDirectionIntersectsPoint(boxRangeBufferData.Forward, boxRangeBufferData.Center, boxRangeBufferData.LocalSize.z, pointWorldPosition) == 1
					&& BoxDirectionIntersectsPoint(boxRangeBufferData.Up, boxRangeBufferData.Center, boxRangeBufferData.LocalSize.y, pointWorldPosition) == 1
					&& BoxDirectionIntersectsPoint(boxRangeBufferData.Right, boxRangeBufferData.Center, boxRangeBufferData.LocalSize.x, pointWorldPosition) == 1) {
					return 1;
				}
				return 0;
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
				float3 normal = crossSign * cross(FC2 - FC1, FC3 - FC1);
				int pointInsideFrustum = dot(normal, comparisonPoint - FC1) >= 0 && dot(normal, FC5 - FC1) > 0;

				if (pointInsideFrustum) {
					normal = crossSign * cross(FC5 - FC1, FC2 - FC1);
					pointInsideFrustum = dot(normal, comparisonPoint - FC1) >= 0 && dot(normal, FC4 - FC1) > 0;

					if (pointInsideFrustum) {
						normal = crossSign * cross(FC6 - FC2, FC3 - FC2);
						pointInsideFrustum = dot(normal, comparisonPoint - FC2) >= 0 && dot(normal, FC1 - FC2) > 0;

						if (pointInsideFrustum) {
							normal = crossSign * cross(FC7 - FC3, FC4 - FC3);
							pointInsideFrustum = dot(normal, comparisonPoint - FC3) >= 0 && dot(normal, FC2 - FC3) > 0;

							if (pointInsideFrustum) {
								normal = crossSign * cross(FC8 - FC4, FC1 - FC4);
								pointInsideFrustum = dot(normal, comparisonPoint - FC4) >= 0 && dot(normal, FC3 - FC4) > 0;

								if (pointInsideFrustum) {
									normal = crossSign * cross(FC8 - FC5, FC6 - FC5);
									pointInsideFrustum = dot(normal, comparisonPoint - FC5) >= 0 && dot(normal, FC1 - FC5) > 0;
								}
							}
						}

					}
				}

				return pointInsideFrustum;
			}


			int PointInsideFrustumV2(float3 comparisonPoint, FrustumRangeBufferData frustumRangeBufferData) {
				return PointInsideFrustumV2(comparisonPoint, frustumRangeBufferData.FC1, frustumRangeBufferData.FC2, frustumRangeBufferData.FC3, frustumRangeBufferData.FC4,
					frustumRangeBufferData.FC5, frustumRangeBufferData.FC6, frustumRangeBufferData.FC7, frustumRangeBufferData.FC8);
			}

			int PointInsideFrustumV2(float3 comparisonPoint, FrustumBufferData frustumBufferData) {
				return PointInsideFrustumV2(comparisonPoint, frustumBufferData.FC1, frustumBufferData.FC2, frustumBufferData.FC3, frustumBufferData.FC4,
					frustumBufferData.FC5, frustumBufferData.FC6, frustumBufferData.FC7, frustumBufferData.FC8);
			}

			int PointIsOccludedByFrustum(float3 comparisonPoint, int circleBufferDataIndex) {
				int isInsideFrustum = 0;
				for (int index = 0; index < _RangeToFrustumBufferLinkCount; index++) {
					if (RangeToFrustumBufferLinkBuffer[index].RangeIndex == circleBufferDataIndex) {

						isInsideFrustum = PointInsideFrustumV2(comparisonPoint, FrustumBufferDataBuffer[RangeToFrustumBufferLinkBuffer[index].FrustumIndex]);
						if (isInsideFrustum) {
							break;
						}
					}
				}
				return isInsideFrustum;
			}

			int PointInsideAngleRestrictionV2(float3 rangeCenterPoint, float3 worldRangeForward, float3 comparisonPoint, float maxAngleLimitationRad) {
				float3 from = normalize(worldRangeForward);
				float3 to = normalize(comparisonPoint - rangeCenterPoint);
				float angleValue = (2 - (dot(from, to) + 1)) * 0.5 * 3.141592;

				return angleValue <= maxAngleLimitationRad;
			}

			fixed4 frag(v2f i) : SV_Target
			{

			fixed4 returnCol = fixed4(0,0,0,0);
			fixed4 computeCol = fixed4(0,0,0,0);

			for (int index = 0; index < _CountSize; index++) {
				RangeExecutionOrderBufferData executionOrder = RangeExecutionOrderBuffer[index];

					if (executionOrder.IsSphere == 1) {
						CircleRangeBufferData rangeBuffer = CircleRangeBuffer[executionOrder.Index];
						float calcDistance = abs(distance(i.worldPos, rangeBuffer.CenterWorldPosition));
						if (calcDistance <= rangeBuffer.Radius) {

								if ((rangeBuffer.OccludedByFrustums == 1 && !PointIsOccludedByFrustum(i.worldPos, executionOrder.Index)) || (rangeBuffer.OccludedByFrustums == 0)) {
									fixed4 newCol = rangeBuffer.AuraColor * (1 - step(rangeBuffer.Radius, calcDistance));
									fixed4 patternColor = tex2D(_AuraTexture, float2(i.worldPos.x, i.worldPos.z) * 2 * _AuraTexture_ST.xy + float2(_AuraTexture_ST.z + rangeBuffer.AuraAnimationSpeed * _Time.x, _AuraTexture_ST.w));
									newCol = saturate(newCol + patternColor * rangeBuffer.AuraTextureAlbedoBoost) * _AlbedoBoost;
									computeCol = saturate((computeCol + newCol)*0.5);
									returnCol = computeCol;
								}

						}
					}
					else if (executionOrder.IsFrustum) {
						FrustumRangeBufferData rangeBuffer = FrustumRangeBuffer[executionOrder.Index];
						if (PointInsideFrustumV2(i.worldPos, rangeBuffer)) {

							if ((rangeBuffer.OccludedByFrustums == 1 && !PointIsOccludedByFrustum(i.worldPos, executionOrder.Index)) || (rangeBuffer.OccludedByFrustums == 0)) {
								fixed4 newCol = rangeBuffer.AuraColor;
								fixed4 patternColor = tex2D(_AuraTexture, float2(i.worldPos.x, i.worldPos.z) * 2 * _AuraTexture_ST.xy + float2(_AuraTexture_ST.z + rangeBuffer.AuraAnimationSpeed * _Time.x, _AuraTexture_ST.w));
								newCol = saturate(newCol + patternColor * rangeBuffer.AuraTextureAlbedoBoost) * _AlbedoBoost;
								computeCol = saturate((computeCol + newCol)*0.5);
								returnCol = computeCol;
							}

						}
					}
					else {
						BoxRangeBufferData rangeBuffer = BoxRangeBuffer[executionOrder.Index];
						if (BoxIntersectsPoint(rangeBuffer, i.worldPos) == 1) {
							fixed4 newCol = rangeBuffer.AuraColor;
							fixed4 patternColor = tex2D(_AuraTexture, float2(i.worldPos.x, i.worldPos.z)*2 * _AuraTexture_ST.xy + float2(_AuraTexture_ST.z + rangeBuffer.AuraAnimationSpeed * _Time.x, _AuraTexture_ST.w));
							newCol = saturate(newCol + patternColor * rangeBuffer.AuraTextureAlbedoBoost) * _AlbedoBoost;
							computeCol = saturate((computeCol + newCol)*0.5);
							returnCol = computeCol;
						}
					}
			}

			if (returnCol.a == 0) {
				discard;
			}

			return returnCol;
		}
	ENDCG
	}
	}
		FallBack "Diffuse"
}

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

			int PointInsideFrustumV2(float3 comparisonPoint, FrustumBufferData frustumBufferData) {
				float crossSign = sign(dot(frustumBufferData.FC5 - frustumBufferData.FC1, cross(frustumBufferData.FC2 - frustumBufferData.FC1, frustumBufferData.FC4 - frustumBufferData.FC1)));
				float3 normal = crossSign * cross(frustumBufferData.FC2 - frustumBufferData.FC1, frustumBufferData.FC3 - frustumBufferData.FC1);
				int pointInsideFrustum = dot(normal, comparisonPoint - frustumBufferData.FC1) >= 0 && dot(normal, frustumBufferData.FC5 - frustumBufferData.FC1) > 0;

				if (pointInsideFrustum) {
					normal = crossSign * cross(frustumBufferData.FC5 - frustumBufferData.FC1, frustumBufferData.FC2 - frustumBufferData.FC1);
					pointInsideFrustum = dot(normal, comparisonPoint - frustumBufferData.FC1) >= 0 && dot(normal, frustumBufferData.FC4 - frustumBufferData.FC1) > 0;
					
					if (pointInsideFrustum) {
						normal = crossSign * cross(frustumBufferData.FC6 - frustumBufferData.FC2, frustumBufferData.FC3 - frustumBufferData.FC2);
						pointInsideFrustum = dot(normal, comparisonPoint - frustumBufferData.FC2) >= 0 && dot(normal, frustumBufferData.FC1 - frustumBufferData.FC2) > 0;

						if (pointInsideFrustum) {
							normal = crossSign * cross(frustumBufferData.FC7 - frustumBufferData.FC3, frustumBufferData.FC4 - frustumBufferData.FC3);
							pointInsideFrustum = dot(normal, comparisonPoint - frustumBufferData.FC3) >= 0 && dot(normal, frustumBufferData.FC2 - frustumBufferData.FC3) > 0;

							if (pointInsideFrustum) {
								normal = crossSign * cross(frustumBufferData.FC8 - frustumBufferData.FC4, frustumBufferData.FC1 - frustumBufferData.FC4);
								pointInsideFrustum = dot(normal, comparisonPoint - frustumBufferData.FC4) >= 0 && dot(normal, frustumBufferData.FC3 - frustumBufferData.FC4) > 0;

								if (pointInsideFrustum) {
									normal = crossSign * cross(frustumBufferData.FC8 - frustumBufferData.FC5, frustumBufferData.FC6 - frustumBufferData.FC5);
									pointInsideFrustum = dot(normal, comparisonPoint - frustumBufferData.FC5) >= 0 && dot(normal, frustumBufferData.FC1 - frustumBufferData.FC5) > 0;
								}
							}
						}

					}
				}

				return pointInsideFrustum;
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
				
							if ( (rangeBuffer.OccludedByFrustums == 1 && !PointIsOccludedByFrustum(i.worldPos, executionOrder.Index)) || (rangeBuffer.OccludedByFrustums == 0) ) {
								fixed4 newCol = rangeBuffer.AuraColor * (1 - step(rangeBuffer.Radius, calcDistance));
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

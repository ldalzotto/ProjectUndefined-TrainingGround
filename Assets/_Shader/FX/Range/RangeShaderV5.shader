Shader "Custom/FX/Range/RangeShaderV5_Pass"
{
	Properties
	{
		_AuraTexture("Aura Texture", 2D) = "white" {}
		_CountSize("Count Size", int) = 0
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
			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float4 worldPos: TEXCOORD1;
				float4 screenPos : TEXCOORD2;
			};

			struct CircleRangeBufferData {
				float3 CenterWorldPosition;
				float Radius;
				float4 AuraColor;
				float AuraTextureAlbedoBoost;
				float AuraAnimationSpeed;
			};

			struct RangeExecutionOrderBufferData
			{
				int IsSphere;
				int IsCube;
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

			uniform StructuredBuffer<RangeExecutionOrderBufferData> RangeExecutionOrderBuffer;
			int _CountSize;

			uniform StructuredBuffer<CircleRangeBufferData> CircleRangeBuffer;
			uniform StructuredBuffer<BoxRangeBufferData> BoxRangeBuffer;

			sampler2D _AuraTexture;
			float4 _AuraTexture_ST;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
				o.screenPos = ComputeScreenPos(o.vertex);
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
							fixed4 newCol = rangeBuffer.AuraColor * (1 - step(rangeBuffer.Radius, calcDistance));
							fixed4 patternColor = tex2D(_AuraTexture, float2(i.worldPos.x, i.worldPos.z)*2 * _AuraTexture_ST.xy + float2(_AuraTexture_ST.z + rangeBuffer.AuraAnimationSpeed * _Time.x, _AuraTexture_ST.w));
							newCol = saturate(newCol + patternColor * rangeBuffer.AuraTextureAlbedoBoost);
							computeCol = saturate((computeCol + newCol)*0.5);
							returnCol = computeCol;
						}
					}
					else {
						BoxRangeBufferData rangeBuffer = BoxRangeBuffer[executionOrder.Index];
						if (BoxIntersectsPoint(rangeBuffer, i.worldPos) == 1) {
							fixed4 newCol = rangeBuffer.AuraColor;
							fixed4 patternColor = tex2D(_AuraTexture, float2(i.worldPos.x, i.worldPos.z)*2 * _AuraTexture_ST.xy + float2(_AuraTexture_ST.z + rangeBuffer.AuraAnimationSpeed * _Time.x, _AuraTexture_ST.w));
							newCol = saturate(newCol + patternColor * rangeBuffer.AuraTextureAlbedoBoost);
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

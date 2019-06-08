﻿Shader "Custom/FX/Range/RangeShaderV5_Pass"
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

			uniform StructuredBuffer<RangeExecutionOrderBufferData> RangeExecutionOrderBuffer;
			int _CountSize;

			uniform StructuredBuffer<CircleRangeBufferData> CircleRangeBuffer;
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
							float aspect = _ScreenParams.x / _ScreenParams.y;
							float2 screenTextureCoordinate = i.screenPos.xy;
							screenTextureCoordinate.x *= aspect;
							screenTextureCoordinate.xy = screenTextureCoordinate.xy / i.screenPos.w;
							newCol = saturate(newCol + tex2D(_AuraTexture, screenTextureCoordinate * _AuraTexture_ST.xy + float2(_AuraTexture_ST.z + rangeBuffer.AuraAnimationSpeed * _Time.x, _AuraTexture_ST.w))
								* rangeBuffer.AuraTextureAlbedoBoost);
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

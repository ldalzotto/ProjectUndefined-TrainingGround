﻿Shader "Custom/Test/SphereFrustumV2"
{
	Properties
	{
		_AuraTexture("Aura Texture", 2D) = "white" {}

		_FrustumBufferDataBufferCount("_FrustumBufferDataBufferCount", Int) = 0
		_FrustumProjectionPointBufferCount("_FrustumProjectionPointBufferCount", Int) = 0

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
				int FrustumProjectionPointBufferDataIndex;
			};

			struct FrustumProjectionPointBufferData
			{
				float3 WorldPosition;
				float Radius;
				float4 BaseColor;
			};

			sampler2D _AuraTexture;
			float4 _AuraTexture_ST;
			float4 _BaseColor;
			

			uniform StructuredBuffer<FrustumBufferData> FrustumBufferDataBuffer;
			uniform StructuredBuffer<FrustumProjectionPointBufferData> FrustumProjectionPointBufferDataBuffer;

			int _FrustumBufferDataBufferCount;
			int _FrustumProjectionPointBufferCount;

			int PointInsideFrustum(float3 comparisonPoint, float3 FC1, float3 FC2, float3 FC3, float3 FC4, float3 FC5, float3 FC6, float3 FC7, float3 FC8) {
				float crossSign = sign(dot(FC5 - FC1, cross(FC2 - FC1, FC4 - FC1)));
				return ((dot(crossSign*cross(FC2 - FC1, FC3 - FC1), comparisonPoint - FC1) >= 0)
						&& dot(crossSign*cross(FC5 - FC1, FC2 - FC1), comparisonPoint - FC1) >= 0
					    && dot(crossSign*cross(FC6 - FC2, FC3 - FC2), comparisonPoint - FC2) >= 0
					    && dot(crossSign*cross(FC7 - FC3, FC4 - FC3), comparisonPoint - FC3) >= 0
					    && dot(crossSign*cross(FC8 - FC4, FC1 - FC4), comparisonPoint - FC4) >= 0
					    && dot(crossSign*cross(FC8 - FC5, FC6 - FC5), comparisonPoint - FC5) >= 0
					);
			}

			int PointInsideFrustumV2(float3 comparisonPoint, FrustumBufferData frustumBufferData) {
				float crossSign = sign(dot(frustumBufferData.FC5 - frustumBufferData.FC1, cross(frustumBufferData.FC2 - frustumBufferData.FC1, frustumBufferData.FC4 - frustumBufferData.FC1)));
				return ((dot(crossSign*cross(frustumBufferData.FC2 - frustumBufferData.FC1, frustumBufferData.FC3 - frustumBufferData.FC1), comparisonPoint - frustumBufferData.FC1) >= 0)
					&& dot(crossSign*cross(frustumBufferData.FC5 - frustumBufferData.FC1, frustumBufferData.FC2 - frustumBufferData.FC1), comparisonPoint - frustumBufferData.FC1) >= 0
					&& dot(crossSign*cross(frustumBufferData.FC6 - frustumBufferData.FC2, frustumBufferData.FC3 - frustumBufferData.FC2), comparisonPoint - frustumBufferData. FC2) >= 0
					&& dot(crossSign*cross(frustumBufferData.FC7 - frustumBufferData.FC3, frustumBufferData.FC4 - frustumBufferData.FC3), comparisonPoint - frustumBufferData.FC3) >= 0
					&& dot(crossSign*cross(frustumBufferData.FC8 - frustumBufferData.FC4, frustumBufferData.FC1 - frustumBufferData.FC4), comparisonPoint - frustumBufferData.FC4) >= 0
					&& dot(crossSign*cross(frustumBufferData.FC8 - frustumBufferData.FC5, frustumBufferData.FC6 - frustumBufferData.FC5), comparisonPoint - frustumBufferData.FC5) >= 0
					);
			}

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
				return o;
			}

		
			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 returnColor = fixed4(0,0,0,0);
				for (int projectionPointIndex = 0; projectionPointIndex < _FrustumProjectionPointBufferCount; projectionPointIndex++) {

					int isInside = (distance(i.worldPos, FrustumProjectionPointBufferDataBuffer[projectionPointIndex].WorldPosition) <= FrustumProjectionPointBufferDataBuffer[projectionPointIndex].Radius);
					int isInsideFrustum = 0;
					for (int index = 0; index < _FrustumBufferDataBufferCount; index++) {
						if (FrustumBufferDataBuffer[index].FrustumProjectionPointBufferDataIndex == projectionPointIndex) {
							isInsideFrustum = isInsideFrustum || PointInsideFrustumV2(i.worldPos, FrustumBufferDataBuffer[index]);
						}
					}

					returnColor = saturate((returnColor + (FrustumProjectionPointBufferDataBuffer[projectionPointIndex].BaseColor * (isInside && !isInsideFrustum)))*0.5);
				}
				return returnColor;
				if (returnColor.a == 0) {
					discard;
				}
		    }
	ENDCG
	}
	}
		FallBack "Diffuse"
}

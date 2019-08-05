Shader "Custom/FX/Range/RangeShaderV5_Pass"
{
	Properties
	{
		_AuraTexture("Aura Texture", 2D) = "white" {}
		_RangeToFrustumBufferLinkCount("_RangeToFrustumBufferLinkCount", Int) = 0

		_AlbedoBoost("Albedo Boost", Float) = 1.0
		_RangeMixFactor("Range Mix Factor", Range(0.0, 1.0)) = 0.5
	}

	CGINCLUDE

		#include "UnityCG.cginc"
		#include "RangeShaderV5_StructDefinition.cginc"
		#include "RangeShaderV5_Calculations.cginc"
		#include "Assets/Test/RangeEffectRework/WorldPositionConstants.cginc"

	    struct appdata
		{
			float4 vertex : POSITION;
			float2 uv : TEXCOORD0;
		};

		struct v2f
		{
			float4 vertex : SV_POSITION;
			float2 uv : TEXCOORD0;
			float3 worldPos : TEXCOORD1;
		};


		uniform int _ExecutionOrderIndex;

		sampler2D _AuraTexture;
		float4 _AuraTexture_ST;

		sampler2D _WorldPositionBuffer;

		float _AlbedoBoost;
		float _RangeMixFactor;


		v2f vert(appdata v)
		{
			v2f o;
			o.vertex = UnityObjectToClipPos(v.vertex);
			o.uv = v.uv;
			o.worldPos = mul(unity_ObjectToWorld, v.vertex);
			return o;
		}
	ENDCG

	SubShader
	{

		Tags{ "RenderType" = "Transparent" }
		Pass
		{
			Name "SphereBuffer"
			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha 
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			fixed4 frag(v2f i) : SV_Target
			{

				float3 worldPos = i.worldPos;
				fixed4 computeCol = fixed4(0,0,0,0);

				RangeExecutionOrderBufferData executionOrder = RangeExecutionOrderBuffer[_ExecutionOrderIndex];
				CircleRangeBufferData rangeBuffer = CircleRangeBuffer[executionOrder.Index];
				float calcDistance = abs(distance(worldPos, rangeBuffer.CenterWorldPosition));
				fixed4 newCol = rangeBuffer.AuraColor * (1 - step(rangeBuffer.Radius, calcDistance));
				//fixed4 patternColor = tex2D(_AuraTexture, float2(worldPos.x, worldPos.z) * 2 * _AuraTexture_ST.xy + float2(_AuraTexture_ST.z + rangeBuffer.AuraAnimationSpeed * _Time.x, _AuraTexture_ST.w));
				newCol = saturate(newCol /*+ patternColor * rangeBuffer.AuraTextureAlbedoBoost*/) * _AlbedoBoost;
				computeCol = lerp(computeCol, newCol, _RangeMixFactor * (calcDistance <= rangeBuffer.Radius) * ((rangeBuffer.OccludedByFrustums == 1 && !PointIsOccludedByFrustum(worldPos, executionOrder)) || (rangeBuffer.OccludedByFrustums == 0)));

				if (computeCol.a == 0) {
					discard;
				}

				return saturate(computeCol);
			}
			ENDCG
		}

		Pass
		{
			Name "BoxBuffer"
			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			fixed4 frag(v2f i) : SV_Target
			{
				float3 worldPos = i.worldPos;
				fixed4 computeCol = fixed4(0,0,0,0);

				RangeExecutionOrderBufferData executionOrder = RangeExecutionOrderBuffer[_ExecutionOrderIndex];
				BoxRangeBufferData rangeBuffer = BoxRangeBuffer[executionOrder.Index];
				fixed4 newCol = rangeBuffer.AuraColor;
			//	fixed4 patternColor = tex2D(_AuraTexture, float2(worldPos.x, worldPos.z) * 2 * _AuraTexture_ST.xy + float2(_AuraTexture_ST.z + rangeBuffer.AuraAnimationSpeed * _Time.x, _AuraTexture_ST.w));
				newCol = saturate(newCol /*+ patternColor * rangeBuffer.AuraTextureAlbedoBoost*/) * _AlbedoBoost;
				computeCol = lerp(computeCol, newCol, _RangeMixFactor * BoxIntersectsPoint(rangeBuffer, worldPos));

				if (computeCol.a == 0) {
					discard;
				}

				return saturate(computeCol);
			}
		ENDCG
		}

		Pass
		{
			Name "FrustumBuffer"
			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			fixed4 frag(v2f i) : SV_Target
			{
				float3 worldPos = i.worldPos;
				fixed4 computeCol = fixed4(0,0,0,0);

				RangeExecutionOrderBufferData executionOrder = RangeExecutionOrderBuffer[_ExecutionOrderIndex];
				FrustumRangeBufferData rangeBuffer = FrustumRangeBuffer[executionOrder.Index];
				fixed4 newCol = rangeBuffer.AuraColor;
				//fixed4 patternColor = tex2D(_AuraTexture, float2(worldPos.x, worldPos.z) * 2 * _AuraTexture_ST.xy + float2(_AuraTexture_ST.z + rangeBuffer.AuraAnimationSpeed * _Time.x, _AuraTexture_ST.w));
				newCol = saturate(newCol /*+ patternColor * rangeBuffer.AuraTextureAlbedoBoost*/) * _AlbedoBoost;
				computeCol = lerp(computeCol, newCol, _RangeMixFactor * PointInsideFrustumV2(worldPos, rangeBuffer) * ((rangeBuffer.OccludedByFrustums == 1 && !PointIsOccludedByFrustum(worldPos, executionOrder)) || (rangeBuffer.OccludedByFrustums == 0)));

				if (computeCol.a == 0) {
					discard;
				}

				return saturate(computeCol);
			}
		ENDCG
		}
	}
	
	FallBack "Diffuse"
}



/*
		for (int index = 0; index < _CountSize; index++) {
			RangeExecutionOrderBufferData executionOrder = RangeExecutionOrderBuffer[index];

				if (executionOrder.RangeType == 0) {
					CircleRangeBufferData rangeBuffer = CircleRangeBuffer[executionOrder.Index];
					float calcDistance = abs(distance(i.worldPos, rangeBuffer.CenterWorldPosition));
					fixed4 newCol = rangeBuffer.AuraColor * (1 - step(rangeBuffer.Radius, calcDistance));
					fixed4 patternColor = tex2D(_AuraTexture, float2(i.worldPos.x, i.worldPos.z) * 2 * _AuraTexture_ST.xy + float2(_AuraTexture_ST.z + rangeBuffer.AuraAnimationSpeed * _Time.x, _AuraTexture_ST.w));
					newCol = saturate(newCol + patternColor * rangeBuffer.AuraTextureAlbedoBoost) * _AlbedoBoost;
					computeCol = lerp(computeCol, newCol, _RangeMixFactor * (calcDistance <= rangeBuffer.Radius) * ((rangeBuffer.OccludedByFrustums == 1 && !PointIsOccludedByFrustum(i.worldPos, executionOrder)) || (rangeBuffer.OccludedByFrustums == 0)));
				}
				else if (executionOrder.RangeType == 2) {
					FrustumRangeBufferData rangeBuffer = FrustumRangeBuffer[executionOrder.Index];
					fixed4 newCol = rangeBuffer.AuraColor;
					fixed4 patternColor = tex2D(_AuraTexture, float2(i.worldPos.x, i.worldPos.z) * 2 * _AuraTexture_ST.xy + float2(_AuraTexture_ST.z + rangeBuffer.AuraAnimationSpeed * _Time.x, _AuraTexture_ST.w));
					newCol = saturate(newCol + patternColor * rangeBuffer.AuraTextureAlbedoBoost) * _AlbedoBoost;
					computeCol = lerp(computeCol, newCol, _RangeMixFactor * PointInsideFrustumV2(i.worldPos, rangeBuffer) * ((rangeBuffer.OccludedByFrustums == 1 && !PointIsOccludedByFrustum(i.worldPos, executionOrder)) || (rangeBuffer.OccludedByFrustums == 0)));
				}
				else {
					BoxRangeBufferData rangeBuffer = BoxRangeBuffer[executionOrder.Index];
					fixed4 newCol = rangeBuffer.AuraColor;
					fixed4 patternColor = tex2D(_AuraTexture, float2(i.worldPos.x, i.worldPos.z)*2 * _AuraTexture_ST.xy + float2(_AuraTexture_ST.z + rangeBuffer.AuraAnimationSpeed * _Time.x, _AuraTexture_ST.w));
					newCol = saturate(newCol + patternColor * rangeBuffer.AuraTextureAlbedoBoost) * _AlbedoBoost;
					computeCol = lerp(computeCol, newCol, _RangeMixFactor * BoxIntersectsPoint(rangeBuffer, i.worldPos));
				}
		}
		*/

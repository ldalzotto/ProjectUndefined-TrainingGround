Shader "Custom/FX/Range/RangeShaderV5_Pass"
{
	Properties
	{
		_AlbedoBoost("Albedo Boost", Float) = 1.0
		_RangeMixFactor("Range Mix Factor", Range(0.0, 1.0)) = 0.5
	}

	CGINCLUDE

		#include "UnityCG.cginc"
		#include "RangeShaderV5_StructDefinition.cginc"
		#include "RangeShaderV5_Calculations.cginc"

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
			Blend One One
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			fixed4 frag(v2f i) : SV_Target
			{

				float3 worldPos = i.worldPos;
				fixed4 computeCol = fixed4(0,0,0,0);

				CircleRangeBufferData rangeBuffer = CircleRangeBuffer[0];
				float calcDistance = abs(distance(worldPos, rangeBuffer.CenterWorldPosition));
				fixed4 newCol = rangeBuffer.AuraColor * (1 - step(rangeBuffer.Radius, calcDistance));
				newCol = saturate(newCol) * _AlbedoBoost;
				computeCol = lerp(computeCol, newCol, _RangeMixFactor * (calcDistance <= rangeBuffer.Radius) * ((rangeBuffer.OccludedByFrustums == 1 && !PointIsOccludedByFrustumV2(worldPos)) || (rangeBuffer.OccludedByFrustums == 0)));

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
			Blend One One
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			fixed4 frag(v2f i) : SV_Target
			{
				float3 worldPos = i.worldPos;
				fixed4 computeCol = fixed4(0,0,0,0);

				BoxRangeBufferData rangeBuffer = BoxRangeBuffer[0];
				fixed4 newCol = rangeBuffer.AuraColor;
				newCol = saturate(newCol) * _AlbedoBoost;
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
			Blend One One
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			fixed4 frag(v2f i) : SV_Target
			{
				float3 worldPos = i.worldPos;
				fixed4 computeCol = fixed4(0,0,0,0);

				FrustumRangeBufferData rangeBuffer = FrustumRangeBuffer[0];
				fixed4 newCol = rangeBuffer.AuraColor;
				newCol = saturate(newCol) * _AlbedoBoost;
				computeCol = lerp(computeCol, newCol, _RangeMixFactor * PointInsideFrustumV2(worldPos, rangeBuffer) * ((rangeBuffer.OccludedByFrustums == 1 && !PointIsOccludedByFrustumV2(worldPos)) || (rangeBuffer.OccludedByFrustums == 0)));

				if (computeCol.a == 0) {
					discard;
				}

				return saturate(computeCol);
			}
		ENDCG
		}

		Pass
		{
			Name "RoundedFrustumBuffer"
			ZWrite Off
			Blend One One
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			fixed4 frag(v2f i) : SV_Target
			{
				float3 worldPos = i.worldPos;
				fixed4 computeCol = fixed4(0,0,0,0);

				RoundedFrustumRangeBufferData rangeBuffer = RoundedFrustumRangeBuffer[0];

				float calcDistance = abs(distance(worldPos, rangeBuffer.CenterWorldPosition));
				fixed4 newCol = rangeBuffer.AuraColor;
				newCol = newCol * _AlbedoBoost;
				computeCol = lerp(computeCol, newCol, _RangeMixFactor * (calcDistance <= rangeBuffer.RangeRadius) * PointInsideFrustumV2(worldPos, rangeBuffer) * ((rangeBuffer.OccludedByFrustums == 1 && !PointIsOccludedByFrustumV2(worldPos)) || (rangeBuffer.OccludedByFrustums == 0)));

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
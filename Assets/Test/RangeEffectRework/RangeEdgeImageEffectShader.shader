Shader "Hidden/RangeEdgeImageEffectShader"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_DetectionUVDistance("Detection UV distance", Float) = 0.05
	}
		SubShader
		{
			// No culling or depth
			Cull Off ZWrite Off ZTest Always
			Blend One One

			Pass
			{
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
					float2 uv : TEXCOORD0;
					float4 vertex : SV_POSITION;
				};

				v2f vert(appdata v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.uv = v.uv;
					return o;
				}

				sampler2D _MainTex;
				float _DetectionUVDistance;

				fixed4 frag(v2f i) : SV_Target
				{
					fixed4 col = tex2D(_MainTex, i.uv);
					fixed4 comparisonCol = tex2D(_MainTex, i.uv + float2(_DetectionUVDistance, 0))
									 * tex2D(_MainTex, i.uv + float2(0, _DetectionUVDistance))
									 * tex2D(_MainTex, i.uv + float2(-_DetectionUVDistance, 0))
									 * tex2D(_MainTex, i.uv + float2(0, -_DetectionUVDistance))
									 * tex2D(_MainTex, i.uv + (float2(_DetectionUVDistance, _DetectionUVDistance) * 0.707106))
									 * tex2D(_MainTex, i.uv + (float2(-_DetectionUVDistance, _DetectionUVDistance) * 0.707106))
									 * tex2D(_MainTex, i.uv + (float2(_DetectionUVDistance, -_DetectionUVDistance) * 0.707106))
									 * tex2D(_MainTex, i.uv + (float2(-_DetectionUVDistance, -_DetectionUVDistance) * 0.707106));
				//	if (comparisonCol.x == 0) { discard; }
					return saturate((col * (comparisonCol.x == 0)) + (col * fixed4(0.2, 0.2, 0.2, 0.2)));
				}
				ENDCG
			}
		}
}

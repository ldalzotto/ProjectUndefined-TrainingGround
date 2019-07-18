Shader "Test/VectorAngleTest"
{
	Properties
	{
		_ForwardDirection("_ForwardDirection", Vector) = (0,0,0,0)
		_Center("_Center", Vector) = (0,0,0,0)
		_MainTex("Texture", 2D) = "white" {}
		_MaxAngle("MaxAngle", Float) = 180
	}
		SubShader
		{
			Tags { "RenderType" = "Opaque" }

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
					float3 worldPos : TEXCOORD1;
					float4 vertex : SV_POSITION;
				};

				float4 _ForwardDirection;
				float4 _Center;
				float _MaxAngle;
				sampler2D _MainTex;
				float4 _MainTex_ST;

				v2f vert(appdata v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.uv = TRANSFORM_TEX(v.uv, _MainTex);
					o.worldPos = mul(unity_ObjectToWorld, v.vertex);
					return o;
				}

				int PointInsideAngleRestriction(float3 rangeCenterPoint, float3 worldRangeForward, float3 comparisonPoint, float maxAngleLimitation) {
					float3 from = normalize(worldRangeForward);
					float3 to = normalize(comparisonPoint - rangeCenterPoint);
					float angleDeg = abs(acos(dot(from, to))) * (180.0 / 3.141592);
					return angleDeg <= maxAngleLimitation;
				}

				float PointInsideAngleRestrictionV2(float3 rangeCenterPoint, float3 worldRangeForward, float3 comparisonPoint, float maxAngleLimitation) {
					float3 from = normalize(worldRangeForward);
					float3 to = normalize(comparisonPoint - rangeCenterPoint);
					return abs(dot(from, to));
				}

				fixed4 frag(v2f i) : SV_Target
				{
					float intensity = PointInsideAngleRestrictionV2(_Center, _ForwardDirection, i.worldPos,_MaxAngle);
					return  fixed4(intensity, intensity, intensity, 1);
				}
				ENDCG
			}
		}
}

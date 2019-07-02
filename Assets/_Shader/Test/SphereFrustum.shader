Shader "Custom/Test/SphereFrustum"
{
	Properties
	{
		_AuraTexture("Aura Texture", 2D) = "white" {}
		_BaseColor("Color", Color) = (1,1,1,1)

		_FC1("FC1", Vector) = (0,0,0,0)
		_FC2("FC2", Vector) = (0,0,0,0)
		_FC3("FC3", Vector) = (0,0,0,0)
		_FC4("FC4", Vector) = (0,0,0,0)
		_FC5("FC5", Vector) = (0,0,0,0)
		_FC6("FC6", Vector) = (0,0,0,0)
		_FC7("FC7", Vector) = (0,0,0,0)
		_FC8("FC8", Vector) = (0,0,0,0)
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

			sampler2D _AuraTexture;
			float4 _AuraTexture_ST;
			float4 _BaseColor;
			
			float4 _FC1;
			float4 _FC2;
			float4	_FC3;
			float4	_FC4;
			float4	_FC5;
			float4	_FC6;
			float4	_FC7;
			float4	_FC8;

			int PointInsideFrustum(float3 comparisonPoint, float3 FC1, float3 FC2, float3 FC3, float3 FC4, float3 FC5, float3 FC6, float3 FC7, float3 FC8) {
				int pointInsideFrustum = 0;
				float3 normal = (0,0,0);

				normal = -cross(FC2 - FC1, FC3 - FC1);
				if (dot(normal, comparisonPoint - FC1) >= 0) {

					normal = -cross(FC5 - FC1, FC2 - FC1);
					if (dot(normal, comparisonPoint - FC1) >= 0) {

						normal = -cross(FC6 - FC2, FC3 - FC2);
						if (dot(normal, comparisonPoint - FC2) >= 0) {

							normal = -cross(FC7 - FC3, FC4 - FC3);
							if (dot(normal, comparisonPoint - FC3) >= 0) {

								normal = -cross(FC8 - FC4, FC1 - FC4);
								if (dot(normal, comparisonPoint - FC4) >= 0) {

									normal = -cross(FC8 - FC5, FC6 - FC5);
									if (dot(normal, comparisonPoint - FC5) >= 0) {
										pointInsideFrustum = 1;
									}
								}

							}
						}
					}
				}
				return pointInsideFrustum;
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

			if (PointInsideFrustum(i.worldPos, _FC1, _FC2, _FC3, _FC4, _FC5, _FC6, _FC7, _FC8) == 1) {
				return _BaseColor;
			}
			return fixed4(0, 0, 0, 0);
		}
	ENDCG
	}
	}
		FallBack "Diffuse"
}

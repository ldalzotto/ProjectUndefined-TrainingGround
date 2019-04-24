Shader "Custom/FX/Range/RangeShaderV4_Pass"
{
	Properties
	{
		_CenterWorldPosition("Center World Position", Vector) = (0,0,0,0)
		_Radius("Radius", float) = 0
		[HDR] _AuraColor("Aura Color", Color) = (1,1,1,1)
		_AuraTexture("Aura Texture", 2D) = "white" {}
		_AuraAnimationSpeed("Aura Animation Speed", Float) = 0
	}
		SubShader
	{

		Tags{ "RenderType" = "Transparent" }
		Pass
		{
			Blend SrcAlpha SrcAlpha
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
			
			sampler2D _CameraDepthTexture;

			float4 _CenterWorldPosition;
			float _Radius;
			float4 _AuraColor;
			sampler2D _AuraTexture;
			float4 _AuraTexture_ST;
			float _AuraAnimationSpeed;

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
			float calcDistance = abs(distance(i.worldPos,_CenterWorldPosition));
			clip(_Radius - calcDistance);
			fixed4 col = _AuraColor * (1 - step(_Radius, calcDistance));

			float aspect = _ScreenParams.x / _ScreenParams.y;
			float2 screenTextureCoordinate = i.screenPos.xy;
			screenTextureCoordinate.x *= aspect;
			screenTextureCoordinate.xy = screenTextureCoordinate.xy / i.screenPos.w;
			_AuraTexture_ST.z += _AuraAnimationSpeed * _Time.x;
			//col = saturate(col + tex2D(_AuraTexture, screenTextureCoordinate * _AuraTexture_ST.xy + _AuraTexture_ST.zw)/15);

			/*
 // sample the texture in the orthographic way
			float sceneDepth = tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.screenPos)).r;
			float objectZ = i.screenPos.z;
			float diff = saturate(abs(sceneDepth - objectZ));
			if (diff > 0.1) {
				discard;
            }
			*/

			return col;
		}
	ENDCG
	}
	}
		FallBack "Diffuse"
}

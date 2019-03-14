Shader "Custom/FX/Range/RangeShaderV3_Pass"
{
	Properties
	{
		_CenterWorldPosition("Center World Position", Vector) = (0,0,0,0)
		_Radius("Radius", float) = 0
		[HDR] _AuraColor("Aura Color", Color) = (1,1,1,1)
		_SmoothedRadius("Smoothed Radius Factor", Range(0.0,1.0)) = 0.8
		_AuraTexture("Aura Texture", 2D) = "white" {}
		_AuraAnimationSpeed("Aura Animation Speed", Float) = 0
	}
		SubShader
	{

		Tags{ "RenderType" = "Transparent" }
		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#pragma shader_feature AURA_TEXTURED

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
				float4 worldPos: TEXCOORD1;
#if AURA_TEXTURED
				float4 screenPos : TEXCOORD2;
#endif
			};

			float4 _CenterWorldPosition;
			float _Radius;
			float4 _AuraColor;
			float _SmoothedRadius;
			sampler2D _AuraTexture;
			float4 _AuraTexture_ST;
			float _AuraAnimationSpeed;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
#if AURA_TEXTURED
				o.screenPos = ComputeScreenPos(o.vertex);
#endif
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
			float calcDistance = abs(distance(i.worldPos,_CenterWorldPosition));
			clip(_Radius - calcDistance);
			fixed4 col = smoothstep(0, _Radius *_SmoothedRadius, calcDistance) *_AuraColor * (1 - step(_Radius, calcDistance));

#if AURA_TEXTURED
			float aspect = _ScreenParams.x / _ScreenParams.y;
			float2 screenTextureCoordinate = i.screenPos.xy;
			screenTextureCoordinate.x *= aspect;
			screenTextureCoordinate.xy = screenTextureCoordinate.xy / i.screenPos.w;
			_AuraTexture_ST.z += _AuraAnimationSpeed * _Time.x;
			col = saturate(col + tex2D(_AuraTexture, screenTextureCoordinate * _AuraTexture_ST.xy + _AuraTexture_ST.zw)/15);
#endif

			return col;
		}
	ENDCG
	}
	}
		FallBack "Diffuse"
		CustomEditor "RangeShaderV3_PassEditor"
}

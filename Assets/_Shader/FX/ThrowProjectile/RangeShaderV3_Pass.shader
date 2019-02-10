Shader "Custom/FX/Range/RangeShaderV3_Pass"
{
	Properties
	{
		_CenterWorldPosition("Center World Position", Vector) = (0,0,0,0)
		_Radius("Radius", float) = 0
		_AuraColor("Aura Color", Color) = (1,1,1,1)
		_AlphaAnimationSpeed("Alpha animation speed", float) = 10
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
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _CenterWorldPosition;
			float _Radius;
			float4 _AuraColor;
			half _AlphaAnimationSpeed;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				float calcDistance = abs(distance(i.worldPos,_CenterWorldPosition));

			//float computedRadius = _Radius * min(abs(sin(frac(_Time.x * _AlphaAnimationSpeed)) * 2), 1);
			fixed4 col = smoothstep(0, _Radius *0.8, calcDistance) *_AuraColor * (1 - step(_Radius, calcDistance));
			return col;
		}
	ENDCG
	}
	}
		FallBack "Diffuse"
}

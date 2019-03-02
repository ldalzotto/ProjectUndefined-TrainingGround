Shader "Custom/Test/WallShatterShader"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
		_WorldLimit("World max height/smooth height", Vector) = (2,1,0,0)
	}
		SubShader
		{
			Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }

			CGPROGRAM
			#pragma surface surf Standard noshadow exclude_path:deferred alpha

			#pragma target 3.0

			sampler2D _MainTex;

			struct Input
			{
				float2 uv_MainTex;
				float3 worldPos;
			};

			half _Glossiness;
			half _Metallic;
			fixed4 _Color;
			float2 _WorldLimit;

			UNITY_INSTANCING_BUFFER_START(Props)
				// put more per-instance properties here
			UNITY_INSTANCING_BUFFER_END(Props)

			void surf(Input IN, inout SurfaceOutputStandard o)
			{

				//float deltaWorldPos = IN.worldPos.y - _WorldLimit.x;
				//clip(_WorldLimit.x - IN.worldPos.y);
			//	o.Alpha = 1 - step(_WorldLimit.y, IN.worldPos.y - _WorldLimit.x);
				o.Alpha = 1 - smoothstep(_WorldLimit.y - _WorldLimit.y, _WorldLimit.y + _WorldLimit.y, IN.worldPos.y - _WorldLimit.x);
				//o.Alpha = smoothstep(_WorldLimit.x - _WorldLimit.y, _WorldLimit.x + _WorldLimit.y, IN.worldPos.y);


				o.Albedo = _Color;
				// Metallic and smoothness come from slider variables
				o.Metallic = _Metallic;
				o.Smoothness = _Glossiness;
			}
			ENDCG
		}
			FallBack "Diffuse"
}

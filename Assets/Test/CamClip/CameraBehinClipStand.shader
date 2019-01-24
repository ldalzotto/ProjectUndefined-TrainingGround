Shader "Custom/CameraBehinClipStand"
{
	Properties
	{
		_CircleRadius("Circle Radius", Range(0.0,0.5)) = 0.5

		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
	}
		SubShader
		{

			Tags { "RenderType" = "Opaque" }

			CGPROGRAM
			// Physically based Standard lighting model, and enable shadows on all light types
			#pragma surface surf Standard addshadow

			// Use shader model 3.0 target, to get nicer looking lighting
			#pragma target 3.0

			sampler2D _MainTex;

			struct Input
			{
				float2 uv_MainTex;
				float4 screenPos;
			};

			half _Glossiness;
			half _Metallic;
			half _CircleRadius;
			fixed4 _Color;

			void surf(Input IN, inout SurfaceOutputStandard o)
			{
				half ratio = _ScreenParams.x / _ScreenParams.y;
				float2 screenPosition = (IN.screenPos.xy);
				screenPosition.x *= ratio;

				half distanceFromCenter = distance(half2(0.5*ratio, 0.5), screenPosition);

				if (distanceFromCenter <= _CircleRadius) {
					discard;
				}
				else {
					// Albedo comes from a texture tinted by color
					fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
					o.Albedo = c.rgb;
					// Metallic and smoothness come from slider variables
					o.Metallic = _Metallic;
					o.Smoothness = _Glossiness;
					o.Alpha = c.a;
				}


			}
			ENDCG
		}
			FallBack "Diffuse"
}

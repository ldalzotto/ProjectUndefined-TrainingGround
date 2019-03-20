Shader "Custom/VertexSurfaceTransparentUnlit"
{
	Properties
	{
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
	}
		SubShader
	{
		Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma vertex vert
        #pragma fragment frag

		struct appdata
		{
			float4 vertex : POSITION;
			float2 uv : TEXCOORD0;
			float4 color : COLOR;
		};
		ENDCG
	}
		FallBack "Diffuse"
}

Shader "Unlit/InstacniatedBufferTest"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
	}
		SubShader
	{
		Tags { "RenderType" = "Opaque" }
		ZWrite Off
		ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma multi_compile_instancing
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

			struct InstanceBufferData {
				float4x4 LocalToWorld;
			};

			uniform StructuredBuffer<InstanceBufferData> InstanceBufferDataComputeBuffer;

			sampler2D _MainTex;
			float4 _MainTex_ST;

			v2f vert(appdata v, uint instanceID : SV_InstanceID)
			{
				v2f o;
				o.vertex = mul(mul(unity_ObjectToWorld, UNITY_MATRIX_VP), v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				return col;
			}
			ENDCG
		}
	}
}

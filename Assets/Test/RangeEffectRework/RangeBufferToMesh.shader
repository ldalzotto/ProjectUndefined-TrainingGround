﻿Shader "Hidden/RangeBufferToMesh"
{
	Properties
	{
	}
		SubShader
	{
		Tags { "RenderType" = "Opaque" }
		Blend One One
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog

			#include "UnityCG.cginc"
			#include "WorldPositionConstants.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float4 screenPosition : TEXCOORD0;
			};

			sampler2D _RangeRenderBuffer;
			float4 _RangeRenderBuffer_ST;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.screenPosition = ComputeScreenPos(o.vertex);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				float2 textureCoordinate = i.screenPosition.xy / i.screenPosition.w;
				return tex2D(_RangeRenderBuffer, textureCoordinate);
			}
			ENDCG
		}
	}
}

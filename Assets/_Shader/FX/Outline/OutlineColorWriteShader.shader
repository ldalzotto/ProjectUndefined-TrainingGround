Shader "Unlit/OutlineColorWriteShader"
{
	Properties
	{
		[HDR] _Color("Color", Color) = (0,0,0,0)
		_ColorSpeed("Color speed", float) = 1
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

			float4 _Color;
			float _ColorSpeed;

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return _Color * abs(cos(_Time.x * _ColorSpeed));
            }
            ENDCG
        }
    }
}

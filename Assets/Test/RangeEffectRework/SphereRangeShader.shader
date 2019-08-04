Shader "Hidden/SphereRangeShader"
{
    Properties
    {
		_Color ("Color", Color) = (1,1,1,1)
		_SpherePosition ("Sphere position", Vector) = (0,0,0,0)
		_SphereRadius ("Sphere radius", Float) = 0
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
			#include "WorldPositionConstants.cginc"

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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

			sampler2D _WorldPositionBuffer;
			float3 _SpherePosition;
			float _SphereRadius;
			float4 _Color;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed3 worldPos = WorldBufferDeNormalize(tex2D(_WorldPositionBuffer, i.uv).xyz);
                return (distance(worldPos, _SpherePosition) <= _SphereRadius) * fixed4(1,1,1,1) * _Color;
            }
            ENDCG
        }


    }
}

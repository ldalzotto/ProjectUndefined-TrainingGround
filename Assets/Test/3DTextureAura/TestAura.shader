// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Unlit/TestAura"
{
    Properties
    {
		_SphereWorldCenter("_SphereWorldCenter", Vector) = (0,0,0,0)
        _MainTex ("Texture", 2D) = "white" {}
		_MainColor("Main Color", Color) = (1,1,1,1)
		_BaseColor("Bas Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
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
				float3 worldPos : TEXCOORD1;
				float4 screenPos : TEXCOORD3;
            };

			float4 _SphereWorldCenter;
            sampler2D _MainTex;
            float4 _MainTex_ST;
			float4 _MainColor;
			float4 _BaseColor;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				o.screenPos = ComputeScreenPos(o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				
				float dist = abs(distance(i.worldPos, _SphereWorldCenter.xyz));
				clip(_SphereWorldCenter.w - dist);
				float aspect = _ScreenParams.x / _ScreenParams.y;
				float2 textureCoordinate = i.screenPos.xy;
				textureCoordinate.x *= aspect;
				textureCoordinate.xy = textureCoordinate.xy / i.screenPos.w;
				fixed4 col = tex2D(_MainTex, textureCoordinate * _MainTex_ST.xy + _MainTex_ST.zw) * _MainColor;
				col = saturate(col + _BaseColor);
				//col += tex2D(_MainTex, i.worldPos.xy + float2(i.worldPos.y, 0));
				//col *= 0.5;
             //   fixed4 col = tex2D(_MainTex, i.uv);
                return col;
            }
            ENDCG
        }
    }
}

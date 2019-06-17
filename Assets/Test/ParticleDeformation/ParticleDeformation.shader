// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Unlit/ParticleDeformation"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_ParticlePos("Particle Pos", Vector) = (0,0,0,0)
			_RadiusEffect("Radius Effect", Float) = 1
			_DeformationStrength("Deformation Strength", Float) = 1
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
				float3 normal : NORMAL;
            };

            struct v2f
            {
				float4 vertex : SV_POSITION;
            };

			float3 _ParticlePos;
			float _RadiusEffect;
			float _DeformationStrength;

            v2f vert (appdata v)
            {
                v2f o;
				float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
				float dist = distance(worldPos, _ParticlePos);
				float3 deformationDirection = normalize(_ParticlePos - worldPos);
				v.vertex.xyz += (deformationDirection * smoothstep(0, _RadiusEffect, _RadiusEffect - dist) * _DeformationStrength);
		
                o.vertex = UnityObjectToClipPos(v.vertex);
				
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return fixed4(1,1,1,1);
            }
            ENDCG
        }
    }
}

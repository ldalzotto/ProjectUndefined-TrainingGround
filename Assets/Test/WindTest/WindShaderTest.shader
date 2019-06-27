Shader "Custom/WindShaderTest"
{
    Properties
    {
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0

		 [Toggle(WIND_ENABLED)]
		_EnableWind("Enable Wind", Float) = 0
		_WindMap("Wind Map (R)", 2D) = "white" {}
		_WindWorldDirection("Wind World Direction", Vector) = (0,0,0,0)
		_MaxVertexDisplacement("Max vertex displacement", Float) = 0
		_WindSpeed("Wind speed", Float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows vertex:vert

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

		#pragma shader_feature WIND_ENABLED

        struct Input
        {
            float2 uv_MainTex;
			float4 color : COLOR;
        };

#if WIND_ENABLED
		sampler2D _WindMap;
		float4 _WindMap_ST;
		float4 _WindWorldDirection;
		float _MaxVertexDisplacement;
		float _WindSpeed;
#endif
        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

#if WIND_ENABLED
		float WaveSine(float speed) {
			float time = _Time.x * speed;
			return  (((sin(time) + sin(2 * time) + sin(0.5*time)) / 3)*0.65) + 0.35;
		}
#endif

		void vert(inout appdata_full v) {
#if WIND_ENABLED
		float2 windTex = TRANSFORM_TEX(v.texcoord.xy, _WindMap);
		float4 localWindDirection = mul(unity_WorldToObject, _WindWorldDirection);

		v.vertex.xyz += (localWindDirection * tex2Dlod(_WindMap, float4(windTex.xy, 0,0) ).r * _MaxVertexDisplacement * WaveSine(_WindSpeed));
#endif
		}

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
			o.Albedo = IN.color.rgb; 
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = IN.color.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}

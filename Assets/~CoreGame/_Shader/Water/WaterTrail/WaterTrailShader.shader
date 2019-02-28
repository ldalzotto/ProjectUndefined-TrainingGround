Shader "Unlit/WaterTrailShader"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_DrawPositionRadiusAlpha("Draw position / Radius z / Smooth distance w", Vector) = (0,0,0,0)
		_DepleteRate("Deplete rate", Float) = 0.1
		_TrailRemovalTexture("Trail removal texture", 2D) = "black" {}
		_TrailDistortionFactor("Trail distorsion factor", Float) = 0
		_TrailDistortionMode("Trail distorsion move", Vector) = (0,0,0,0)
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

				sampler2D _MainTex, _TrailRemovalTexture, _FoamMask;
				float4 _MainTex_ST, _TrailRemovalTexture_ST;
				float4 _DrawPositionRadiusAlpha;
				float _DepleteRate;
				float _DistortRate;
				float _TrailDistortionFactor;
				float4 _TrailDistortionMode;

				v2f vert(appdata v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.uv = TRANSFORM_TEX(v.uv, _MainTex);
					return o;
				}

				void DistordTrailRemoval(v2f i) {
					_TrailRemovalTexture_ST.zw += _TrailDistortionMode.xy *_Time.x;
				}

				float2 SampleDistortedTrailRemovalTexture(v2f i) {
					float2 texDistSample = tex2D(_TrailRemovalTexture, i.uv.xy * _TrailRemovalTexture_ST.xy + _TrailRemovalTexture_ST.zw).yz;
					texDistSample -= 0.5;
					return texDistSample;
				}

				fixed4 frag(v2f i) : SV_Target
				{
					DistordTrailRemoval(i);

					float2 texDistSample = SampleDistortedTrailRemovalTexture(i);
					float trailRemovalSample = tex2D(_TrailRemovalTexture, i.uv * _TrailRemovalTexture_ST.xy + _TrailRemovalTexture_ST.zw).x;


					//float3 textureColor = float3(1, 1, 1);
					float3 textureColor = tex2D(_MainTex, i.uv *_MainTex_ST.xy + (_MainTex_ST.zw + texDistSample * _TrailDistortionFactor)).xyz;
					textureColor -= unity_DeltaTime * (_DepleteRate * trailRemovalSample);
					fixed3 drawnColor = saturate((1 -
						smoothstep(_DrawPositionRadiusAlpha.z - _DrawPositionRadiusAlpha.w, _DrawPositionRadiusAlpha.z, distance(_DrawPositionRadiusAlpha.xy, i.uv.xy))
							) * fixed3(1, 0, 0)
								+ saturate(textureColor));
					return fixed4(drawnColor, 1);
				}
				ENDCG
			}
		}
}

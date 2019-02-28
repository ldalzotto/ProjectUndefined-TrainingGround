

half4 LightingWrappedCalculation(float3 normal, float inputAlpha, half3 lightDir, float4 lightColor, fixed minimumNdotL, half atten) {
	half PositiveNdotL = max(minimumNdotL, (dot(normal, lightDir) + 1)*0.5);
	half4 col = lightColor;
	col.rgb *= PositiveNdotL;
	return half4(col.rgb * (atten / 3), inputAlpha);
}
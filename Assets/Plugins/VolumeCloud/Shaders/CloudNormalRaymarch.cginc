#pragma once
#include "./CloudShaderHelper.cginc"
 
float GetDensity(float3 startPos, float3 dir, float maxSampleDistance, float raymarchOffset, out float3 intensity,out float depth) {
	float raymarchDistance = 0;

	RaymarchStatus result;
	InitRaymarchStatus(result);

	[loop]
	for (int j = 0; j < SAMPLE_COUNT; j++) {
		float prevRayDist = raymarchDistance;
		raymarchDistance = pow((j+raymarchOffset)/SAMPLE_COUNT,2) * _ProjectionParams.z;
		if(raymarchDistance > maxSampleDistance) break;
		float step = raymarchDistance - prevRayDist;
		float3 rayPos = startPos + dir * raymarchDistance;
		float fade = smoothstep(_ProjectionParams.z*.95,_ProjectionParams.z, raymarchDistance);
		IntegrateRaymarch(startPos, rayPos, fade, step, result);
		if (result.intTransmittance < 0.01f) {
			result.intTransmittance = 0;
			break;
		}
	}

	depth = result.depth / result.depthweightsum / _ProjectionParams.z;
	if (depth == 0.0f) {
		depth = maxSampleDistance;
	}
	intensity = result.intensity;
	return (1.0f - result.intTransmittance);	
}

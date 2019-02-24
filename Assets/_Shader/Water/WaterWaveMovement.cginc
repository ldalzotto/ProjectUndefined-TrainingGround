#ifndef WAVE_MOVEMENT
#define WAVE_MOVEMENT

void Displace(inout VertexInput v) {
#if _WAVE_MOVEMENT
	v.vertex.z +=
		(
		(
			(sin(_Time.x * 30 - v.vertex.x * 100) / 2)
			+ (cos(_Time.x * 10 - v.vertex.x * 50) / 2)
			)

			);
#endif
}

#endif // WAVE_MOVEMENT

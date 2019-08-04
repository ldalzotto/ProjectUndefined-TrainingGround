#ifndef WORLD_POSITION_CONSTANTS
#define WORLD_POSITION_CONSTANTS

#define WORLD_ABSOLUDE_SIZE 100000.0

float3 WorldBufferNormalize(float3 source) {
	return (source + WORLD_ABSOLUDE_SIZE) / (2 * WORLD_ABSOLUDE_SIZE);
}

float3 WorldBufferDeNormalize(float3 source) {
	return (source * 2 * WORLD_ABSOLUDE_SIZE) - WORLD_ABSOLUDE_SIZE;
}

#endif
#include "TArray2.h"
#include "mtrand.h"

typedef unsigned int uint;

class PerlinNoise
{
public:
	PerlinNoise(unsigned long int seed);
	TArray2<uint>* GetIntMap(uint width, uint height, uint minY, uint maxY, uint octaveCount);
//private:
	TArray2<float>* GenerateWhiteNoise(uint width, uint height);
	uint Interpolate(uint minY, uint maxY, float t);
	float Interpolate(float minY, float maxY, float t);
	TArray2<uint>* MapInts(uint minY, uint maxY, TArray2<float>* perlinNoise);
	TArray2<float>* GenerateSmoothNoise(int octave, TArray2<float>* baseNoise);
	TArray2<float>* GeneratePerlinNoise(int octaveCount, TArray2<float>* baseNoise);

private:
	MTRand mRand;
};

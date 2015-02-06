#ifndef PERLINNOISE_H_
#define PERLINNOISE_H_

#include "PerlinNoise.h"

enum eBlock
{
  Empty = 0,
  Rock = 1,
  Tree = 2,
  Wall = 3
};


class MapServer
{
public:
	static TArray2<uint>* GetMap(uint x, uint y);

protected:
	MapServer();
	~MapServer();

private:
	TArray2<uint>* GetMapInternal(uint x, uint y);
	void GenerateMap();
	void DumpMap();

	static MapServer* instance;

	uint _MapWidth;
	uint _MapHeight;
	uint _MapMinY;
	uint _MapMaxY;
	uint _OctaveCount;

	TArray2<uint>* map; // TODO - more than 1 map

	char sprites[4];
};


#endif

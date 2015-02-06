#include "MapServer.h"
#include "SimplexNoise.h"
#include <iostream>
#include <stdlib.h>


/*static*/ MapServer* MapServer::instance = new MapServer();

MapServer::MapServer() :
	_MapWidth(40),
	_MapHeight(40),
	_MapMinY(0),
	_MapMaxY(1),
	_OctaveCount(3),
	map(NULL)
{
	sprites[0] = ' ';
	sprites[1] = '#';
	sprites[2] = 'o';
	sprites[3] = '+';
}

MapServer::~MapServer()
{
}

/*static*/ TArray2<uint>* MapServer::GetMap(uint x, uint y)
{
	return instance->GetMapInternal(x,y);
}

TArray2<uint>* MapServer::GetMapInternal(uint x, uint y)
{
	if (map == NULL)
		GenerateMap();
	return map;
}

void MapServer::GenerateMap()
{
  PerlinNoise perlin(1234567);
  map = perlin.GetIntMap(_MapWidth, _MapHeight, _MapMinY, _MapMaxY, _OctaveCount);
  
}

void MapServer::DumpMap()
{
  for (uint i=0; i<_MapHeight; i++)
  {
    for (uint j=0; j<_MapWidth; j++)
    {
      std::cout << sprites[(*map)(i,j)];
    }
    std::cout << std::endl;
  }
}

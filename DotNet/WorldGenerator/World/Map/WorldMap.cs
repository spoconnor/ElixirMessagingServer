using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Sean.World
{
    internal class WorldMap
    {
        public WorldMap(int chunkSize, int initialSize)
        {
            MapScale = chunkSize;
            MaxXBlock = initialSize;
            MaxZBlock = initialSize;
            mapChunks = new MapChunk[MaxXBlock, MaxZBlock];
        }
            
        public void Generate()
        {
            var size = new ArraySize(){
                minX=0, maxX=MaxXPosition, minZ=0, maxZ=MaxZPosition, 
                scale=MapScale};
            heightMap = PerlinNoise.GetIntMap(size, 3);
        }

        /// <summary>Get a chunk from the array. Based on the x,z of the chunk in the world. Note these are chunk coords not block coords.</summary>
        public Chunk Chunk(int x, int z)
        {
            return GetOrCreate(x, z); 
        }

        public Chunk Chunk(ChunkCoords chunkCoords)
        {
            return GetOrCreate (chunkCoords.X, chunkCoords.Z);
        }

        /// <summary>Get a chunk from the array. Based on world block coords.</summary>
        public Chunk Chunk(Position position)
        {
            int x = (position.X / MapScale);
            int z = (position.Z / MapScale);
            return GetOrCreate(x, z);
        }

        /// <summary>Get a chunk from the array. Based on more accurate world object coords.</summary>
        public Chunk Chunk(Coords coords)
        {
            int x = (coords.Xblock / MapScale);
            int z = (coords.Zblock / MapScale);
            return GetOrCreate(x, z);
        }
        private Chunk GetOrCreate(int x, int z)
        {
            Console.WriteLine ($"Getting {x},{z}");
            var mapChunk = mapChunks[x, z];
            if (mapChunk == null)
            {
                Console.WriteLine ($"Generating {x},{z}");
                mapChunk = new MapChunk();
                var chunkCoords = new ChunkCoords(x, z);
                mapChunk.Chunk = new Chunk(chunkCoords);
                Generator.Generate(mapChunk.Chunk);
                mapChunks[x, z] = mapChunk;
            }
            return mapChunk.Chunk;
        }

        /*
        public void Render ()
        {
            for (int x = 0; x < MapX; x++) {
                System.Text.StringBuilder builder = new System.Text.StringBuilder ();
                for (int z = 0; z < MapZ; z++) {
                    if (Cursor.WorldX == x && Cursor.WorldZ == z)
                        builder.Append ("*");
                    else
                        builder.Append (chunks [x,z].Render ());
                }
                Console.WriteLine (builder.ToString ());
            }
        }
        */

        public int MaxXBlock { get; set; }
        public int MaxZBlock { get; set; }
        public int MaxXPosition { get { return MaxXBlock * MapScale; } }
        public int MaxZPosition { get { return MaxZBlock * MapScale; } }
        public int MapScale { get; set; }
        private MapChunk[,] mapChunks;

        private Array<int> heightMap;
    }
}


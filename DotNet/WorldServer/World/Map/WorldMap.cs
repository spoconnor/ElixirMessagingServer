using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Sean.World
{
    internal class WorldMap
    {
        public WorldMap(int chunkSize, int initialSize)
        {
            MinX = 10000;
            MinZ = 10000;
            MapSize = initialSize;
            MapScale = chunkSize;
            MaxX = MinX + (MapScale * MapSize);
            MaxZ = MinZ + (MapScale * MapSize);
            mapChunks = new MapChunk[MapSize, MapSize];
        }
            
        public void Generate()
        {
            var size = new ArraySize(){
                minX=MinX, maxX=MaxX, minZ=MinZ, maxZ=MaxZ, 
                scale=MapScale};
            heightMap = PerlinNoise.GetIntMap(size, 3);
        }



        /// <summary>Get a chunk from the array. Based on world block coords.</summary>
        public Chunk this[Position position]
        {
            get {
                int i = (position.X / Chunk.CHUNK_SIZE) - MinX;
                int j = (position.Z / Chunk.CHUNK_SIZE) - MinZ;
                return GetOrCreate(i, j);
            }
        }

        /// <summary>Get a chunk from the array. Based on more accurate world object coords.</summary>
        public Chunk this[Coords coords]
        {
            get {
                int i = (coords.Xblock / Chunk.CHUNK_SIZE) - MinX;
                int j = (coords.Zblock / Chunk.CHUNK_SIZE) - MinZ;
                return GetOrCreate(i, j);
            }
        }
        private Chunk GetOrCreate(int i, int j)
        {
            var mapChunk = mapChunks[i, j];
            if (mapChunk == null)
            {
                mapChunk = new MapChunk();
                var chunkCoords = new ChunkCoords(i + MinX, j + MinZ);
                mapChunk.Chunk = new Chunk(chunkCoords);
                Generator.Generate(mapChunk.Chunk);
                mapChunks[i, j] = mapChunk;
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


        public int MinX { get; set; }
        public int MaxX { get; set; }
        public int MinZ { get; set; }
        public int MaxZ { get; set; }
        public int MapScale { get; set; }
        public int MapSize { get; set; }
        private MapChunk[,] mapChunks;

        private Array<int> heightMap;
    }
}


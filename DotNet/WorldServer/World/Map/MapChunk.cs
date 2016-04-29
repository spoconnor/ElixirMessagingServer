using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sean.World
{
    public class MapChunk
    {
        public bool ChunkGenerated { get; set; }
        bool Loaded { get; set; }
        bool IsWater { get; set; }
        public Chunk Chunk { get; set; }
    }
}

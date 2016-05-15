using System;
using Hexpoint.Blox.Hosts.World;
using System.Collections.Generic;
using AiKnowledgeEngine;

namespace AiKnowledgeEngine
{
    public class Construction
    {
        private Construction ()
        {
        }

        private static Construction instance = new Construction ();
        private Dictionary<Position, Block.BlockType> blocks = new Dictionary<Position, Block.BlockType>();
        private Dictionary<string, Dictionary<Position, Block.BlockType>> construction = new Dictionary<string, Dictionary<Position, Block.BlockType>>();

        public static Construction Instance {
            get { return instance; }
        }

        public void AddOrRemoveBlock(Position position, Block.BlockType blockType)
        {
            Console.WriteLine ("AddOrRemoveBlock {0} at {1}", blockType, position);
            blocks[position] = blockType;
        }

        public void Save(string name)
        {
            construction[name] = blocks;
            blocks = new Dictionary<Position, Block.BlockType>();
        }

        public Dictionary<Position, Block.BlockType> GetConstruction(string name)
        {
            if (!construction.ContainsKey(name))
                return null;
            return construction[name];
        }

        public Position FindFlatishArea(Position start, int width, int length, int height)
        {
            Coords coord = start.ToCoords();
            int y = WorldData.GetHeightMapLevel(coord.Xblock, coord.Zblock); //start on block above the surface

            Position test;
            int needsRemoving = 0;
            int needsAdding = 0;
            for (test.X=1; test.X<width; test.X++)
            {
                for (test.Z=1; test.Z<length; test.Z++)
                {
                    for (test.Y=y; test.Y<(y+height); test.Y++)
                    {
                        Block block = test.GetBlock();
                        if (block.IsSolid && !Block.IsBlockTypeTree(block.Type))
                        {
                            needsRemoving++;
                        }
                    }
                }
            }
            return start;
        }
    }
}


using System;
using System.Collections.Generic;
using Hexpoint.Blox.Hosts.World;

namespace AiKnowledgeEngine
{
    public class Time
    {
        public int time;
        private static int now;

        public static int Now 
		{
            get { return now; }
        }
    }
    
    public class MemoryItem
    {
        public string who;
        public string what;
        public Time when;
        public Coords where;
    }
    
    public class Knowledge
    {
		public Knowledge ()
		{
			knownBlockLocations = new Dictionary<Block.BlockType, List<KnownBlockLocation>>();
		}

        public void Add (Position position, Position accessFrom, int distance)
		{
			Block.BlockType blockType = position.GetBlock ().Type;
			if (!knownBlockLocations.ContainsKey (blockType))
				knownBlockLocations.Add (blockType, new List<KnownBlockLocation> ());

			KnownBlockLocation knownBlockLocation = null;
			foreach (KnownBlockLocation i in knownBlockLocations[blockType]) {
				if (i.accessFrom == accessFrom) {
					knownBlockLocation = i;
					break;
				}   
			}   
			if (knownBlockLocation == null) {
				knownBlockLocation = new KnownBlockLocation () 
				{ accessFrom = accessFrom, blockCount = 0, closestDistance = 9999 };
				knownBlockLocations [blockType].Add (knownBlockLocation);
			}

			knownBlockLocation.blockCount++;
			if (distance < knownBlockLocation.closestDistance) {
				knownBlockLocation.closestDistance = distance;
				knownBlockLocation.closestBlock = position;
			}
		}

		/// <summary>
		/// Return the block positions in order from closest to furtherest.
		/// </summary>
		public IEnumerable<Position> GetNearestBlocks (Block.BlockType blockType, Position start)
		{
			if (knownBlockLocations.ContainsKey (blockType)) 
			{
	    		List<Position> returned = new List<Position> ();
				while (knownBlockLocations[blockType].Count > returned.Count) 
				{
					foreach (KnownBlockLocation loc in knownBlockLocations[blockType]) 
					{
						yield return loc.closestBlock;
					}
				}
			}
	     }

		class KnownBlockLocation
		{
			public Position accessFrom;
			public int blockCount;
			public int closestDistance;
			public Position closestBlock;
		}

		private const int MaxBlockPositions = 100;
		private const int MinBlockPositions = 100;
		private Dictionary<Block.BlockType, List<KnownBlockLocation>> knownBlockLocations;
    }
}


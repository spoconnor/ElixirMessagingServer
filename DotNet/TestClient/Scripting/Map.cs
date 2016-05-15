using System;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Collections.Generic;
using Hexpoint.Blox.Hosts.World;
using Hexpoint.Blox.GameObjects.Units;

namespace AiKnowledgeEngine
{
    public class WorldObject
    {
        public bool passable = false;
    }
    
    public class WorldItem : WorldObject
    {
    }
    
    public class Food : WorldItem
    {
        public bool passable = true;
    }

    public class MapObject
    {
    }

    public class MapLocation
    {
        public MapLocation ()
        {
            mapObjects = new List<MapObject> ();
            IsWall = false;
        }

        public void AddObject (MapObject obj)
        {
            mapObjects.Add (obj);
        }

        public void AddWall ()
        {
            IsWall = true;
        }
      
        public void AddFood ()
        {
            IsFood = true;
        }

        private List<MapObject> mapObjects;
        public bool IsWall;
        public bool IsFood;
    }

    public static class MapManager
    {
        private static Map instance = new Map ();

        public static Map Instance {
            get { return instance; }
        }
    }

    public class Map
    {
        public int MapXSize = 0;
        public int MapYSize = 0;

        public Map ()
        {
            MapXSize = 60;
            MapYSize = 30;
            gameMap = new Dictionary<Position, Block> ();
            characters = new List<Character> ();
        }
        
//        public MapLocation GetLocation (Position pos)
//        {
//            return WorldData.GetBlock(coords);
//            if (!IsOnMap (pos))
//                return null;
//
//            return GetLocation (pos.X, pos.Y);
//        }

        public void UpdateLocation (Position pos, Block block)
        {
            if (!pos.IsValidBlockLocation)
                return;

            gameMap [pos] = block;
        }

//        private MapLocation GetLocation (int x, int y)
//        {
//            int key = y * MapXSize + x;
//            if (!gameMap.ContainsKey (key))
//                return null;
//            else
//                return gameMap [key];
//        }

//        private MapLocation GetOrCreateLocation (int x, int y)
//        {
//            int key = y * MapXSize + x;
//            if (!gameMap.ContainsKey (key))
//                gameMap.Add (key, new MapLocation ());
//
//            return gameMap [key];
//        }

        public bool IsOnMap (Position pos)
        {
            return pos.X >= 0 && pos.X < MapXSize && pos.Y >= 0 && pos.Y < MapYSize;
        }

        public IEnumerable<Position> GetSurrounding (Position origin, int range)
        {
            for (int z = origin.Z - range; z < origin.Z + range; z++)
            {
                for (int y = origin.Y - range; y < origin.Y + range; y++)
                {
                    for (int x = origin.X - range; x < origin.X + range; x++)
                    {
                        Position pt = new Position (x, y, z);
                        if (pt.IsValidBlockLocation)
                            yield return pt;
                    }
                }
            }
        }

        public IEnumerable<Position> Get2dPlaneQuadrantCells (Position origin, int heading, int fieldOfView, int viewDepth)
        {
            List<Position> cells = new List<Position> ();
            Position left = origin + GraphicsAlgorithms.FastTan (heading - (fieldOfView / 2), viewDepth);
            Position right = origin + GraphicsAlgorithms.FastTan (heading + (fieldOfView / 2), viewDepth);
            foreach (Position farCell in GraphicsAlgorithms.DrawLineOn2dPlane(left, right))
            {
                foreach (Position cell in GraphicsAlgorithms.DrawLineOn2dPlane(origin, farCell))
                {
                    if (!cell.IsValidBlockLocation)
                        continue;

                    if (cells.Contains (cell))
                        continue;

                    cells.Add (cell);
                    yield return cell;

                    Block block = cell.GetBlock();
                    if (block.IsSolid)
                        break;
                }
            }
        }

        public void Dump (List<Position> route)
        {
            int z = 0;
            for (int y = 0; y < MapYSize; y++)
            {
                StringBuilder str = new StringBuilder ();
                for (int x = 0; x < MapXSize; x++)
                {
                    if (route.Contains (new Position (x, y, 0)))
                    {
                        str.Append ("+");
                    }
                    else
                    {
                        Block block = WorldData.GetBlock (x, y, z);
                        if (block.IsSolid)
                            str.Append ("#");
                        //else if (block.IsFood)
                        //    str.Append (".");
                        else
                            str.Append (" ");
                    }
                }
                Console.WriteLine ("{0}", str.ToString ());
            }
        }

        private Dictionary<Position, Block> gameMap;
        private List<Character> characters;
    }
}


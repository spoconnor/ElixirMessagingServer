using System;
using System.Diagnostics;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.CSharp;
using OpenTK;
using Block = Hexpoint.Blox.Hosts.World.Block;
using Hexpoint.Blox.GameObjects.GameItems;
using Hexpoint.Blox.GameActions;
using Hexpoint.Blox.Hosts.World;
using System.IO;
using AiKnowledgeEngine;

namespace Hexpoint.Blox.Scripting
{
    // A class to share context between our ScriptDriver and Mono's Evaluator
    public static class ScriptHost// : IScriptHost
    {
        public static void AddBlock (int characterId, Position position, Block.BlockType blockType)
        {
            Console.WriteLine ("AddBlock");
        }

        public static void AddBlockItem (int characterId, Coords coords, Vector3 velocity, Block.BlockType blockType, int gameObjectId)
        {
            Console.WriteLine ("AddBlockItem");
        }

        public static void AddProjectile (int characterId, Coords coords, Vector3 velocity, Block.BlockType blockType, bool allowBounce, int gameObjectId)
        {
            Console.WriteLine ("AddProjectile");
        }

        public static void AddStaticItem (int characterId, Coords coords, StaticItemType staticItemType, ushort subType, Face attachedToFace, int gameObjectId)
        {
            Console.WriteLine ("AddStaticItem");
        }

        public static void AddStructure (int characterId, Position position, StructureType structureType, Facing frontFace)
        {
            Console.WriteLine ("AddStructure");
        }

        public static void ChatMsg (int characterId, string message)
        {
            Console.WriteLine ("ChatMsg");
        }

        public static void PickupBlockItem (int characterId, int gameObjectId)
        {
            Console.WriteLine ("PickupBlockItem");
        }

        public static void CharacterMove (int characterId, Coords coords)
        {
            Console.WriteLine ("CharacterMove");
        }

        public static void RemoveBlock (int characterId, Position position)
        {
            Console.WriteLine ("RemoveBlock");
        }

        public static void RemoveBlockItem (int characterId, int gameObjectId, bool isDecayed)
        {
            Console.WriteLine ("RemoveBlockItem");
        }
    }

    //Main Program
//    class ScriptDriver
//    {
//        private object mutex = new object ();
//
//        public void Init ()
//        {
//            //MapManager.Instance.LoadMap (System.IO.Path.Combine ("Resources", "Map1.bmp"));
//            path = new PathFinder (MapManager.Instance);
//        }
//   
//        private PathFinder path;
        //private Character chr1, chr2, chr3;
            
//        private Position GetRandomLocation ()
//        {
//            Random rnd = new Random ();
//            Position pt = new Position ();
//            do
//            {
//                pt.X = rnd.Next (MapManager.Instance.MapXSize);
//                pt.Y = rnd.Next (MapManager.Instance.MapYSize);
//            } while (MapManager.Instance.GetLocation(pt).IsWall);
//            return pt;
//        }
            
        //public void Run ()
        //{
        //    while (true)
        //    {
        //        Step ();
        //        Console.ReadKey ();
        //    }
        //}
            
//        public void Step ()
//        {
//            //List<Location> sight = map.CanSee(chr1.GetLocation(), Direction.NorthWest);
//            chr1.UpdateKnownMap ();
//            chr1.DoTasks ();
//            chr1.Dump ();
//            //map.Dump(route);
//        }
//
//        public void Execute (Hexpoint.Blox.GameObjects.Units.Character character)
//        {
//            lock (mutex)
//            {
//                try
//                {
//                    Console.WriteLine ("Executing script for {0}", character.Id);
//
//                }
//                catch (Exception ex)
//                {
//                    Console.WriteLine ("Script error...");
//                    Console.WriteLine (ex.Message);
//                }
//            }
//        }
//    }


} // namespace

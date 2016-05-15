using System;
using System.Collections.Generic;
using Hexpoint.Blox.Hosts.World;
using Hexpoint.Blox.Scripting;

namespace AiKnowledgeEngine
{
//    public class Character : ICharacter
//    {
//        public string Execute(int characterId)
//        {
//            Console.WriteLine("Executing for Character {0}", characterId);
//            Hexpoint.Blox.Scripting.ScriptHost.ChatMsg(1,"Hi");
//            return "Ok";
//        }
//  
//
//        public Character (string name, Position position)
//        {
//            Name = name;
//            this.Coords = position.ToCoords();
//            KnownMap = new Map ();
//            KnownMap.Add (this);
//            path = new PathFinder (KnownMap);
//            hunger = new Hunger ();
//            knowledge = new Knowledge ();
//            tasks = new Tasks ();
//        }
//
//        public void Dump ()
//        {
//            Console.WriteLine (string.Format ("Name:{0}", Name));
//            Console.WriteLine (string.Format ("Health:{0}", hunger));
//            KnownMap.Dump (new List<Position> ());
//        }
//
//        public void DoTasks ()
//        {
//            tasks.DoTask (this);
//        }
//        
//        public void MoveTo (Position destination)
//        {
//            Coords = destination.ToCoords();
//        }
//        
//        public void UpdateKnownMap ()
//        {
//            //foreach (Location location in MapManager.Instance.GetQuadrantCells(Location, Heading, 90, 5)) 
//            //{
//            //    MapLocation mapLoc = MapManager.Instance.GetLocation(location);
//            //  KnownMap.UpdateLocation(location, mapLoc);
//            //}
//            foreach (Position position in MapManager.Instance.GetSurrounding(Coords.ToPosition(), 5))
//            {
//                MapLocation mapLoc = MapManager.Instance.GetLocation (position);
//                KnownMap.UpdateLocation (position, mapLoc);
//            }
//        }
//
//        public IScriptHost Parent { get; set; }
//
//        public string Name { get; private set;}
//        public Coords Coords { get; private set;}
//        public Position Position { get { return Coords.ToPosition(); } }
//        public Map KnownMap;
//        public PathFinder path;
//        public int Heading;
//        public Hunger hunger;
//        public Knowledge knowledge;
//        public Tasks tasks;
//    }
    
//    public class Hunger
//    {
//        public Hunger ()
//        {
//            Value = 100;
//            Increment = -5;
//            Events = new Dictionary<int, MemoryItem> ();
//            Events [25] = new MemoryItem ()
//            {
//                what = "Hungry"
//            };
//            Events [0] = new MemoryItem ()
//            {
//            };
//        }
//
//        public string ToString ()
//        {
//            if (Value < 25)
//                return "Feeling Hungry";
//            else
//                return "Feeling ok";
//        }
//        
//        public int Value;
//        public int Increment;
//        public Dictionary<int, MemoryItem> Events;
//    }

}

